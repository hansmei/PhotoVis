using System;
using System.Data.OleDb;
using System.IO;

using ADOX;
using ADODB;
using PhotoVis.Data.DatabaseTables;

namespace PhotoVis.Util
{
    class DatabaseInitializer
    {

        public DatabaseInitializer()
        {
        }
        
        public string GetDatabaseFilename()
        {
            return "PhotoVis.mdb";
        }

        public string GetDatabaseFullPath()
        {
            string dbPath = Path.Combine(App.PhotoVisDataRoot, this.GetDatabaseFilename());
            return dbPath;
        }

        public string GetConnectionString()
        {
            string dbPath = this.GetDatabaseFullPath();
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=\"" + dbPath + "\"; Jet OLEDB:Engine Type=5";
            return connectionString;
        }

        public bool CreateDatabase()
        {
            bool result = false;

            ADOX.Catalog cat = new ADOX.Catalog();
            try
            {
                string connection = this.GetConnectionString();
                cat.Create(connection);

                Table assignment = this.CreateAssignmentTable(cat);
                cat.Tables.Append(assignment);
                Table folders = this.CreateFoldersTable(cat);
                cat.Tables.Append(folders);
                Table image = this.CreateImageTable(cat);
                cat.Tables.Append(image);

                //Now Close the database
                ADODB.Connection con = cat.ActiveConnection as ADODB.Connection;
                if (con != null)
                    con.Close();

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            cat = null;
            return result;
        }

        private ADOX.Table CreateAssignmentTable(Catalog cat)
        {
            ADOX.Table table = new ADOX.Table();

            //Create the table and it's fields. 
            table.Name = DTables.Assignments;
            table.Columns.Append(DAssignment.ProjectId, ADOX.DataTypeEnum.adInteger);
            table.Columns.Append(DAssignment.ProjectName);
            table.Columns.Append(DAssignment.Latitude, ADOX.DataTypeEnum.adDouble);
            table.Columns.Append(DAssignment.Longitude, ADOX.DataTypeEnum.adDouble);
            table.Columns.Append(DAssignment.Thumbnail, ADOX.DataTypeEnum.adLongVarWChar);
            table.Columns.Append(DAssignment.TimeCreated);
            table.Columns.Append(DAssignment.TimeLastIndexed);

            table.Keys.Append("PrimaryKey", KeyTypeEnum.adKeyPrimary, DAssignment.ProjectId);
            //table.Columns[DAssignment.ProjectId].ParentCatalog = cat;
            //table.Columns[DAssignment.ProjectId].Properties["AutoIncrement"].Value = true;

            // Allow null
            table.Columns[DAssignment.Thumbnail].Attributes = ColumnAttributesEnum.adColNullable;
            table.Columns[DAssignment.TimeLastIndexed].Attributes = ColumnAttributesEnum.adColNullable;

            return table;
        }

        private ADOX.Table CreateFoldersTable(Catalog cat)
        {
            ADOX.Table table = new ADOX.Table();

            //Create the table and it's fields. 
            table.Name = DTables.Folders;
            table.Columns.Append(DFolders.Id, ADOX.DataTypeEnum.adInteger);
            table.Columns.Append(DFolders.ProjectId, ADOX.DataTypeEnum.adInteger);
            table.Columns.Append(DFolders.FolderPath);
            table.Columns.Append(DFolders.UseSubfolders, ADOX.DataTypeEnum.adBoolean);

            // Add primary and auto-increment
            table.Keys.Append("PrimaryKey", KeyTypeEnum.adKeyPrimary, DFolders.Id);
            table.Columns[DFolders.Id].ParentCatalog = cat;
            table.Columns[DFolders.Id].Properties["AutoIncrement"].Value = true;
            
            table.Keys.Append("ForeignKeyFolders", ADOX.KeyTypeEnum.adKeyForeign, DFolders.ProjectId, DTables.Assignments, DAssignment.ProjectId);
            //table.Keys["ForeignKeyFolders"].UpdateRule = RuleEnum.adRICascade;

            return table;
        }

        private ADOX.Table CreateImageTable(Catalog cat)
        {
            ADOX.Table table = new ADOX.Table();

            //Create the table and it's fields. 
            table.Name = DTables.Images;
            table.Columns.Append(DImageAtLocation.Id, ADOX.DataTypeEnum.adInteger);
            table.Columns.Append(DImageAtLocation.ProjectId, ADOX.DataTypeEnum.adInteger);
            table.Columns.Append(DImageAtLocation.ImagePath);
            table.Columns.Append(DImageAtLocation.Thumbnail, ADOX.DataTypeEnum.adLongVarWChar);
            table.Columns.Append(DImageAtLocation.Latitude, ADOX.DataTypeEnum.adDouble);
            table.Columns.Append(DImageAtLocation.Longitude, ADOX.DataTypeEnum.adDouble);
            table.Columns.Append(DImageAtLocation.Altitude, ADOX.DataTypeEnum.adDouble);
            table.Columns.Append(DImageAtLocation.Heading, ADOX.DataTypeEnum.adInteger);
            table.Columns.Append(DImageAtLocation.Rotation, ADOX.DataTypeEnum.adInteger);
            table.Columns.Append(DImageAtLocation.TimeImageTaken);
            table.Columns.Append(DImageAtLocation.TimeIndexed);

            // Add primary and auto-increment
            table.Keys.Append("PrimaryKey", KeyTypeEnum.adKeyPrimary, DImageAtLocation.Id);
            table.Columns[DImageAtLocation.Id].ParentCatalog = cat;
            table.Columns[DImageAtLocation.Id].Properties["AutoIncrement"].Value = true;

            table.Keys.Append("ForeignKeyImages", ADOX.KeyTypeEnum.adKeyForeign, DImageAtLocation.ProjectId, DTables.Assignments, DAssignment.ProjectId);
            //table.Keys["ForeignKeyImages"].UpdateRule = RuleEnum.;

            // Allow null
            table.Columns[DImageAtLocation.Thumbnail].Attributes = ColumnAttributesEnum.adColNullable;
            table.Columns[DImageAtLocation.Latitude].Attributes = ColumnAttributesEnum.adColNullable;
            table.Columns[DImageAtLocation.Longitude].Attributes = ColumnAttributesEnum.adColNullable;
            table.Columns[DImageAtLocation.Altitude].Attributes = ColumnAttributesEnum.adColNullable;
            table.Columns[DImageAtLocation.Heading].Attributes = ColumnAttributesEnum.adColNullable;

            return table;
        }
    }
}
