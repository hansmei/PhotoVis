using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpikeAccountManager
{
    [Serializable]
    public partial class ResponseBase
    {
        /// <summary>Besides the http status code, this tells you whether the call succeeded or not.</summary>
        [Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? Success { get; set; }

        /// <summary>Either an error or a confirmation.</summary>
        [Newtonsoft.Json.JsonProperty("message", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Message { get; set; }

        /// <summary>Returned resource (if querying by id)</summary>
        [Newtonsoft.Json.JsonProperty("response", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public object Resource { get; set; }

        /// <summary>Returned resources array (if it's a bulk query)</summary>
        [Newtonsoft.Json.JsonProperty("responses", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public List<object> Resources { get; set; }

        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public static ResponseBase FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseBase>(data);
        }

    }
    
    [Serializable]
    public partial class ResponseUser : ResponseBase
    {
        [Newtonsoft.Json.JsonProperty("response", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public User Resource { get; set; }

        [Newtonsoft.Json.JsonProperty("responses", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public List<User> Resources { get; set; }

        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public static ResponseUser FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseUser>(data);
        }

    }
}
