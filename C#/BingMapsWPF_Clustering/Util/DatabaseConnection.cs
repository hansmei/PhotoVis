using System;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Data;
using System.Configuration;

using PhotoVis.Data;

namespace PhotoVis.Util
{

    public class DatabaseConnection
    {
        //private static string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + App.PhotoVisDataRoot + "NoVis.mdb\"";
        private static OleDbConnection dbConnection;
        private int projectId;

        public DatabaseConnection()
        {
            if (dbConnection == null)
            {
                DatabaseInitializer dbinit = new DatabaseInitializer();
                string connectionString = dbinit.GetConnectionString();
                dbConnection = new OleDbConnection(connectionString);
                dbConnection.Open();
            }
        }

        public DataTable GetValues(string table, Dictionary<string, object> where, string[] fields)
        {
            if (fields.Length == 0)
                throw new Exception("Fields must be set");

            List<object> arguments = new List<object>();
            string selectFields = "";
            string tableString = "";

            int counter = 0;

            // Check which fields to fetch
            if (fields[0] != "*")
            {
                int n = 0;
                foreach (string field in fields)
                {
                    arguments.Add(this.escapeString(field));
                    if (n == 0)
                    {
                        selectFields = "{" + counter++ + "}";
                        n++;
                    }
                    else
                    {
                        selectFields += ", {" + counter++ + "}";
                    }
                }

            }
            else if (fields[0] == "*")
            {
                arguments.Add("*");
                selectFields = "{" + counter++ + "}";
            }

            // Insert the table
            arguments.Add(table);
            tableString = "{" + counter++ + "}";

            // Update the where string
            int i = 0;
            string whereString = "";
            if (where != null)
            {
                foreach (KeyValuePair<string, object> pair in where)
                {
                    arguments.Add(this.escapeString(pair.Key));

                    // Handle ints
                    if (typeof(int) == pair.Value.GetType())
                        arguments.Add(pair.Value);
                    else
                        arguments.Add(this.sqlQuoteOrDefault(this.escapeString(pair.Value.ToString())));
                    if (i == 0)
                    {
                        whereString = "WHERE ";
                        i++;
                    }
                    else
                    {
                        whereString += "AND ";
                    }

                    whereString += "{" + counter++ + "} = {" + counter++ + "} ";
                }
            }

            string commandString = "SELECT " + selectFields + " FROM " + tableString + " " + whereString + ";";
            string sqlString = string.Format(commandString, arguments.ToArray());
            DataTable data = this.QueryToArray(sqlString);

            return data;
        }

        public int InsertValue(string table, Dictionary<string, object> rowValues)
        {
            int counter = 1;

            // Prepare the value fields and insert fields
            string numColumns = "({" + counter++ + "}";
            for (int i = 1; i < rowValues.Count; i++)
            {
                numColumns += ", {" + counter++ + "}";
            }
            numColumns += ")";

            string numValues = "({" + counter++ + "}";
            for (int i = 1; i < rowValues.Count; i++)
            {
                numValues += ", {" + counter++ + "}";
            }
            numValues += ")";

            List<string> columns = new List<string>();
            List<string> values = new List<string>();
            foreach (KeyValuePair<string, object> pair in rowValues)
            {
                columns.Add(this.escapeString(pair.Key));
                Type t = pair.Value.GetType();
                if (t == typeof(DateTime))
                {
                    DateTime time = (DateTime)pair.Value;
                    values.Add("#'" + time.ToString("yyyy-MM-dd HH:mm:ss") + "'#");
                }
                else
                {
                    values.Add(this.sqlQuoteOrDefault(this.escapeString(pair.Value.ToString())));
                }
            }

            // Prepare arguments
            List<string> arguments = new List<string>();
            arguments.Add(table);

            foreach (string col in columns)
            {
                arguments.Add(col);
            }
            foreach (string val in values)
            {
                arguments.Add(val);
            }

            string commandString = "INSERT INTO {0} " + numColumns + " VALUES " + numValues + ";";
            string sqlString = string.Format(commandString, arguments.ToArray());
            int numRowsAffected = this.query(sqlString);

            return numRowsAffected;

        }

        public int UpdateValue(string table, Dictionary<string, object> where, Dictionary<string, object> rowValues)
        {
            int counter = 1;

            // Prepare the value fields and insert fields
            string updates = "";
            for (int i = 0; i < rowValues.Count; i++)
            {
                updates += "{" + counter++ + "} = {" + counter++ + "}";
            }

            string condition = "";
            for (int i = 0; i < where.Count; i++)
            {
                condition += "{" + counter++ + "} = {" + counter++ + "}";
                if (i != where.Count - 1)
                    condition += " AND ";
            }

            // Prepare arguments
            List<string> arguments = new List<string>();
            arguments.Add(table);
            foreach (KeyValuePair<string, object> pair in rowValues)
            {
                arguments.Add(this.escapeString(pair.Key));

                if (pair.Value.GetType() == typeof(DateTime))
                {
                    DateTime time = (DateTime)pair.Value;
                    arguments.Add("#'" + time.ToString("yyyy-MM-dd HH:mm:ss") + "'#");
                }
                else
                {
                    arguments.Add(this.sqlQuoteOrDefault(this.escapeString(pair.Value.ToString())));
                }
            }

            foreach (KeyValuePair<string, object> pair in where)
            {
                arguments.Add(this.escapeString(pair.Key));
                if (pair.Value.GetType() == typeof(DateTime))
                {
                    DateTime time = (DateTime)pair.Value;
                    arguments.Add("#'" + time.ToString("yyyy-MM-dd HH:mm:ss") + "'#");
                }
                else
                {
                    arguments.Add(this.sqlQuoteOrDefault(this.escapeString(pair.Value.ToString())));
                }
            }

            string commandString = "UPDATE {0} SET " + updates + " WHERE " + condition + ";";
            string sqlString = string.Format(commandString, arguments.ToArray());
            int numRowsAffected = this.query(sqlString);

            return numRowsAffected;
        }


        public int DeleteValue(string table, Dictionary<string, object> where)
        {
            int counter = 1;
            
            string condition = "";
            for (int i = 0; i < where.Count; i++)
            {
                condition += "{" + counter++ + "} = {" + counter++ + "}";
                if (i != where.Count - 1)
                    condition += " AND ";
            }

            // Prepare arguments
            List<string> arguments = new List<string>();
            arguments.Add(table);
            foreach (KeyValuePair<string, object> pair in where)
            {
                arguments.Add(this.escapeString(pair.Key));
                if (pair.Value.GetType() == typeof(DateTime))
                {
                    DateTime time = (DateTime)pair.Value;
                    arguments.Add("#'" + time.ToString("yyyy-MM-dd HH:mm:ss") + "'#");
                }
                else
                {
                    arguments.Add(this.sqlQuoteOrDefault(this.escapeString(pair.Value.ToString())));
                }
            }

            string commandString = "DELETE FROM {0} WHERE " + condition + ";";
            string sqlString = string.Format(commandString, arguments.ToArray());
            int numRowsAffected = this.query(sqlString);

            return numRowsAffected;
        }

        public DataTable QueryToArray(string sql)
        {
            try
            {
                OleDbCommand cmd = new OleDbCommand(sql, dbConnection);

                OleDbDataReader reader = cmd.ExecuteReader();

                DataTable data = new DataTable();
                data.Load(reader);

                return data;
            }
            catch (Exception ex)
            {
                //BimControl.BimConsole.WriteLine(string.Format("Database error: {0}", ex.Message), BimConsoleControl.LogCategory.Error);
                return new DataTable();
            }

        }

        public int GetLastInsertId()
        {
            string sql = "SELECT @@IDENTITY;";
            int lastId = 0;

            try
            {
                using (OleDbCommand command = new OleDbCommand(sql, dbConnection))
                {
                    lastId = (int)command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                //BimControl.BimConsole.WriteLine(string.Format("Database error: {0}", ex.Message), BimConsoleControl.LogCategory.Error);
            }

            return lastId;
        }

        private int query(string sql)
        {
            try
            {
                OleDbCommand cmd = new OleDbCommand(sql, dbConnection);
                int numRowsAffected = cmd.ExecuteNonQuery();

                return numRowsAffected;
            }
            catch (Exception ex)
            {
                //BimControl.BimConsole.WriteLine(string.Format("Database error: {0}", ex.Message), BimConsoleControl.LogCategory.Error);
                return 0;
            }
        }

        private string escapeString(string str)
        {
            return str.Replace("'", "\'");
        }

        private string sqlQuoteOrDefault(string field)
        {
            if (field == "NULL")
            {
                field = "NULL";
            }
            else if (field == "NOT NULL")
            {
                field = "NOT NULL";
            }
            else if (field.ToLower() == "true")
            {
                field = "1";
            }
            else if (field.ToLower() == "false")
            {
                field = "0";
            }
            else
            {
                field = "'" + field + "'";
            }
            return field;
        }

    }
}
