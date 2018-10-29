using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SpikeAccountManager
{
    [Serializable]
    public class SpikeServerResponse
    {
        /// <summary>Password.</summary>
        [Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Status { get; set; }

        /// <summary>User's role. Defaults to "user".</summary>
        [Newtonsoft.Json.JsonProperty("message", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Message { get; set; }

        /// <summary>We will need profile pics at one point.</summary>
        [Newtonsoft.Json.JsonProperty("response", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Dictionary<string, object> Response { get; set; }

        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public static SpikeServerResponse FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<SpikeServerResponse>(data);
        }
    }
}
