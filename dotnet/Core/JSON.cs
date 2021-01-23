using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flow
{
    public static class JSON
    {
        public static T Parse<T>(string jsonText) {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonText);
        }

        public static string Stringify(Object obj) {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        public static Dictionary<string, string> ToDict(Object obj, params string[] fields) {
            Dictionary<string, string> result = new Dictionary<string, string>();
            JToken jObj = JToken.FromObject(obj);
            if (fields != null && fields.Length > 0)
            {
                foreach (var fname in fields)
                {
                    var val = jObj[fname];
                    result.Add(fname, val?.ToString());
                }
            }
            else {
                foreach (var fname in jObj) {
                    var val = jObj[fname];
                    result.Add(fname.ToString(), val?.ToString());
                }
            }
            return result;
        }
    }
}
