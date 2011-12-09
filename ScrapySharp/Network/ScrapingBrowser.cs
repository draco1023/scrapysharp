using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace ScrapySharp.Network
{
    public class ScrapingBrowser
    {
        private CookieContainer cookieContainer;
        private Uri referer;
        private FakeUserAgent userAgent;

        public ScrapingBrowser()
        {
            InitCookieContainer();
            userAgent = FakeUserAgents.Chrome;
            AllowAutoRedirect = true;
        }

        public void ClearCookies()
        {
            InitCookieContainer();
        }

        private void InitCookieContainer()
        {
            cookieContainer = new CookieContainer();
        }

        public string DownloadString(Uri url)
        {
            HttpWebRequest request = CreateRequest(url, HttpVerb.Get);
            
            return GetResponse(url, request);
        }

        private HttpWebRequest CreateRequest(Uri url, HttpVerb verb)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Referer = referer != null ? referer.ToString() : url.ToString();
            request.Method = ToMethod(verb);
            request.CookieContainer = cookieContainer;
            request.UserAgent = UserAgent.UserAgent;

            return request;
        }

        private string GetResponse(Uri url, HttpWebRequest request)
        {
            referer = url;
            request.AllowAutoRedirect = AllowAutoRedirect;
            var response = request.GetResponse();
            var headers = response.Headers;

            var cookiesExpression = headers["Set-Cookie"];

            if (!string.IsNullOrEmpty(cookiesExpression))
                cookieContainer.SetCookies(new Uri(string.Format("{0}://{1}:{2}/", url.Scheme, url.Host, url.Port)), cookiesExpression);

            var responseStream = response.GetResponseStream();
            if (responseStream == null)
                return string.Empty;
            using (var reader = new StreamReader(responseStream))
                return reader.ReadToEnd();
        }
        
        public string NavigateTo(Uri url, HttpVerb verb, string data)
        {
            var path = verb == HttpVerb.Get ? string.Format("{0}?{1}", url, data) : url.ToString();

            HttpWebRequest request = CreateRequest(new Uri(path), verb);

            if (verb == HttpVerb.Post)
                request.ContentType = "application/x-www-form-urlencoded";

            request.CookieContainer = cookieContainer;

            if (verb == HttpVerb.Post)
            {
                var stream = request.GetRequestStream();
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(data);
                    writer.Flush();
                }
            }

            return GetResponse(url, request);
        }

        public string NavigateTo(Uri url, HttpVerb verb, NameValueCollection data)
        {
            return NavigateTo(url, verb, GetHttpPostVars(data));
        }

        private static string ToMethod(HttpVerb verb)
        {
            switch (verb)
            {
                case HttpVerb.Get:
                    return "GET";
                case HttpVerb.Post:
                    return "POST";
                default:
                    throw new ArgumentOutOfRangeException("verb");
            }
        }

        public static string GetHttpPostVars(NameValueCollection variables)
        {
            var builder = new StringBuilder();

            for (int i = 0; i < variables.Count; i++)
            {
                var key = variables.GetKey(i);
                var values = variables.GetValues(i);
                if (values != null)
                    foreach (var value in values)
                        builder.AppendFormat("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value));
                if(i < variables.Count - 1)
                    builder.Append("&");
            }
            
            return builder.ToString();
        }

        public FakeUserAgent UserAgent
        {
            get { return userAgent; }
            set { userAgent = value; }
        }

        public bool AllowAutoRedirect { get; set; }
    }
}