using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BoxyBot.HttpWeb
{
    public class RequestBuilder
    {
        public static RequestBuilder Empty = new RequestBuilder(RequestMethod.Get);

        public RequestMethod Method { get; set; }

        private readonly List<KeyValuePair<String, String>> _parameters;

        public RequestBuilder() { _parameters = new List<KeyValuePair<string, string>>(); }
        public RequestBuilder(RequestMethod method)
        {
            _parameters = new List<KeyValuePair<string, string>>();
            this.Method = method;
        }

        public void Add(String key, String value) => _parameters.Add(new KeyValuePair<string, string>(key, value));

        public String GetRequestString()
        {
            var output = _parameters.Aggregate("", (current, kv) => current + $"{kv.Key}={HttpUtility.UrlEncode(kv.Value)}&");
            return output.Length > 0 ? output.Remove(output.Length - 1) : String.Empty;
        }

    }
}
