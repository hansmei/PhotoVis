using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SpikeAccountManager
{/// <summary>Describes a user.</summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.10.41.0 (Newtonsoft.Json v9.0.0.0)")]
    [Serializable]
    public partial class User
    {
        /// <summary>Database uuid.</summary>
        [Newtonsoft.Json.JsonProperty("_id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string _id { get; set; }

        /// <summary>Password.</summary>
        [Newtonsoft.Json.JsonProperty("password", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Password { get; set; }

        /// <summary>User's role. Defaults to "user".</summary>
        [Newtonsoft.Json.JsonProperty("role", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Role { get; set; }

        /// <summary>We will need profile pics at one point.</summary>
        [Newtonsoft.Json.JsonProperty("avatar", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Avatar { get; set; }

        /// <summary>a signed jwt token that expires in 1 year.</summary>
        [Newtonsoft.Json.JsonProperty("apitoken", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Apitoken { get; set; }

        /// <summary>a signed jwt token that expires in 1 day.</summary>
        [Newtonsoft.Json.JsonProperty("token", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Token { get; set; }

        /// <summary>user's email</summary>
        [Newtonsoft.Json.JsonProperty("email", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Email { get; set; }

        /// <summary>User's given name</summary>
        [Newtonsoft.Json.JsonProperty("firstname", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string FirstName { get; set; }

        /// <summary>User's family name</summary>
        [Newtonsoft.Json.JsonProperty("lastname", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string LastName { get; set; }

        /// <summary>Users's company</summary>
        [Newtonsoft.Json.JsonProperty("company", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Company { get; set; }
        
        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public static User FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<User>(data);
        }

    }
}
