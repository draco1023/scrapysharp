using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ScrapySharp.Network
{
    public class ScrapingBrowser
    {
        private CookieContainer cookieContainer;
        private Uri referer;

        private static readonly Regex splitCookiesRegex = new Regex("(?<name>[^=]+)=(?<val>[^;]+)[^,]+,?", RegexOptions.Compiled);

        public ScrapingBrowser()
        {
            InitCookieContainer();
            UserAgent = FakeUserAgents.Chrome;
            AllowAutoRedirect = true;
            Language = CultureInfo.CreateSpecificCulture("EN-US");
            UseDefaultCookiesParser = true;
            IgnoreCookies = false;
            ProtocolVersion = HttpVersion.Version10;
            KeepAlive = false;
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
            var request = (HttpWebRequest)WebRequest.Create(url.AbsoluteUri);
            request.Referer = referer != null ? referer.AbsoluteUri : null;
            request.Method = ToMethod(verb);
            request.CookieContainer = cookieContainer;
            request.UserAgent = UserAgent.UserAgent;
            request.Headers["Accept-Language"] = Language.Name;

            if (Timeout > TimeSpan.Zero)
                request.Timeout = (int) Timeout.TotalMilliseconds;

            request.KeepAlive = KeepAlive;
            request.ProtocolVersion = ProtocolVersion;

            return request;
        }

        private string GetResponse(Uri url, HttpWebRequest request)
        {
            var response = GetWebResponse(url, request);

            var responseStream = response.GetResponseStream();
            if (responseStream == null)
                return string.Empty;
            using (var reader = new StreamReader(responseStream))
                return reader.ReadToEnd();
        }

        private WebResponse GetWebResponse(Uri url, HttpWebRequest request)
        {
            referer = url;
            request.AllowAutoRedirect = AllowAutoRedirect;
            var response = request.GetResponse();
            var headers = response.Headers;

            if (!IgnoreCookies)
            {
                var cookiesExpression = headers["Set-Cookie"];
                if (!string.IsNullOrEmpty(cookiesExpression))
                {
                    var cookieUrl =
                        new Uri(string.Format("{0}://{1}:{2}/", response.ResponseUri.Scheme, response.ResponseUri.Host,
                                              response.ResponseUri.Port));
                    if (UseDefaultCookiesParser)
                        cookieContainer.SetCookies(cookieUrl, cookiesExpression);
                    else
                        SetCookies(cookieUrl, cookiesExpression);
                }
            }
            return response;
        }

        public void SetCookies(Uri cookieUrl, string cookiesExpression)
        {
            var match = splitCookiesRegex.Match(cookiesExpression);
            
            while (match.Success)
            {
                if (match.Groups["name"].Success && match.Groups["val"].Success)
                {
                    try
                    {
                        cookieContainer.Add(new Cookie((match.Groups["name"].Value), (match.Groups["val"].Value), "/", cookieUrl.Host));
                    }
                    catch (CookieException) { }
                }
                match = match.NextMatch();
            }
        }

        public WebResponse ExecuteRequest(Uri url, HttpVerb verb, NameValueCollection data)
        {
            return ExecuteRequest(url, verb, GetHttpPostVars(data));
        }

        public WebResponse ExecuteRequest(Uri url, HttpVerb verb, string data)
        {
            var path = string.IsNullOrEmpty(data)
                              ? url.AbsoluteUri
                              : (verb == HttpVerb.Get ? string.Format("{0}?{1}", url.AbsoluteUri, data) : url.AbsoluteUri);

            var request = CreateRequest(new Uri(path), verb);

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

            return GetWebResponse(url, request);
        }

        public string NavigateTo(Uri url, HttpVerb verb, string data)
        {
            var path = string.IsNullOrEmpty(data)
                              ? url.AbsoluteUri
                              : (verb == HttpVerb.Get ? string.Format("{0}?{1}", url.AbsoluteUri, data) : url.AbsoluteUri);

            var request = CreateRequest(new Uri(path), verb);

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

        public FakeUserAgent UserAgent { get; set; }

        public bool AllowAutoRedirect { get; set; }
        
        public bool UseDefaultCookiesParser { get; set; }
        
        public bool IgnoreCookies { get; set; }

        public TimeSpan Timeout { get; set; }

        public CultureInfo Language { get; set; }

        public Version ProtocolVersion { get; set; }

        public bool KeepAlive { get; set; }

        public Cookie GetCookie(Uri url, string name)
        {
            var collection = cookieContainer.GetCookies(url);

            return collection[name];
        }
    }
}