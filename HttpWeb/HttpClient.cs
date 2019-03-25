using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.HttpWeb
{
    public class HttpClient
    {
        public String UserAgent { get; set; }
        public CookieContainer Cookies { get; set; }

        public HttpClient()
        {
            Cookies = new CookieContainer();
            UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36";
        }

        public String Request(RequestBuilder request, String url)
        {
            if (url.StartsWith("http://.", StringComparison.CurrentCulture)) return String.Empty;

            var requestString = request.GetRequestString();
            var requestBytes = Encoding.UTF8.GetBytes(requestString);
            url = url + (request.Method == RequestMethod.Get ? "?" + requestString : "");
            WebRequest webRequest = null;
            webRequest = WebRequest.Create(url);
            webRequest.Credentials = CredentialCache.DefaultCredentials;
            webRequest.Proxy = null;
            ((HttpWebRequest)webRequest).UserAgent = this.UserAgent;
            ((HttpWebRequest)webRequest).CookieContainer = Cookies;
            webRequest.Method = request.Method == RequestMethod.Get ? "GET" : "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            Stream dataStream = null;
            if (request.Method == RequestMethod.Post)
            {
                webRequest.ContentLength = requestBytes.Length;
                dataStream = webRequest.GetRequestStream();
                dataStream.Write(requestBytes, 0, requestBytes.Length);
                dataStream.Close();
            }
            var response = webRequest.GetResponse();
            dataStream = response.GetResponseStream();
            if (dataStream == null) return null;
            var reader = new StreamReader(dataStream);
            var responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }
    }
}
