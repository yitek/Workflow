using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace WFlow.Utils
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
            if (obj == null) return null;
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

        public static bool SetPathValue(JToken target, string expr, JToken value)
        {
            if (target == null) return false;
            var texts = expr.Split('.');
            for (var i = 0; i < texts.Length - 1; i++)
            {
                var pname = texts[i];
                var nextTargget = target[pname];
                if (nextTargget == null || nextTargget.Type != JTokenType.Object) {
                    target[pname] = nextTargget = new JObject();
                }
                target = nextTargget;
            }
            target[texts.Last()] = value;
            return true;
        }

        public static bool SetPathValue(JToken target, string expr, object value) {
            return SetPathValue(target,expr,JToken.FromObject(value));
        }

        public static T GetPathValue<T>(JToken value,string expr) {
            value = GetPathValue(value,expr);
            if (value == null) return default;
            return value.ToObject<T>();
        }
        
        public static JToken GetPathValue(JToken value,string expr)
        {
            if (value == null || value.Type == JTokenType.Undefined || value.Type == JTokenType.Null) return null;
            var exprs = JPathResolvers.GetOrAdd(expr, (text) => ParseExprText(text));
            foreach (var fn in exprs)
            {
                value = fn(value);
                if (value == null || value.Type == JTokenType.Undefined || value.Type == JTokenType.Null) break;
            }
            return value;
        }

        static List<Func<JToken,  JToken>> ParseExprText(string text) {
            // profile.interests|index:3|resolve:name|count
            var exprs = new List<Func<JToken,  JToken>>();
            var at = text.IndexOf('|');
            var propPath = text;
            string funcExpr = null;
            if (at >= 0)
            {
                propPath = text.Substring(0, at);
                funcExpr = text.Substring(at + 1);
            }
            propPath = propPath.Trim();
            if (propPath == string.Empty) throw new Exception("不正确的表达式:" + text);
            var props = propPath.Split('.');
            exprs.Add((input)=> {
                if (input == null || input.Type == JTokenType.Undefined || input.Type == JTokenType.Undefined) return input;
                foreach (var prop in props) {
                    input = input[prop];
                    if (input == null || input.Type == JTokenType.Undefined || input.Type == JTokenType.Undefined) return input;
                }
                return input;
            });

            
            if (funcExpr != null)
            {
                var fnExprs = funcExpr.Split('|');
                foreach (var fnExp in fnExprs)
                {
                    var fnCmps = fnExp.Split(':');
                    var fnName = fnCmps[0].Trim();
                    if (fnName == string.Empty) throw new Exception("表达式中有函数，但没有指定函数名:" + text);
                    if (!funcs.TryGetValue(fnName, out Func<JToken, string[], JToken> fn)) throw new Exception("无法找到函数:" + fnName + "," + text);
                    string[] args;
                    if (fnCmps.Length == 1) args = new string[0];
                    else {
                        args = fnCmps[1].Split(',');
                    }
                    AddFunc(exprs,fn,args);
                    
                }
            }
            return exprs;
        }
        
        static void AddFunc(List<Func<JToken, JToken>> exprs, Func<JToken, string[], JToken> func, string[] args) {
            exprs.Add((input)=>func(input,args));
        }

        readonly static ConcurrentDictionary<string, Func<JToken, string[], JToken>> funcs = new ConcurrentDictionary<string, Func<JToken, string[], JToken>>();
        readonly static ConcurrentDictionary<string, List<Func<JToken, JToken>>> JPathResolvers = new ConcurrentDictionary<string, List<Func<JToken, JToken>>>() ;
    }
}
