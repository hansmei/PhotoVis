using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpikeAccountManager
{
    public class Error
    {
        /// <summary>User's given name</summary>
        [Newtonsoft.Json.JsonProperty("link", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Link { get; set; }

        /// <summary>User's given name</summary>
        [Newtonsoft.Json.JsonProperty("linktext", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string LinkText { get; set; }
    }
}
