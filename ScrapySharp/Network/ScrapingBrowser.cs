﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using ScrapySharp.Extensions;

namespace ScrapySharp.Network
{
    public class ScrapingBrowser
    {
        private CookieContainer cookieContainer;
        private Uri referer;

        private static readonly Regex parseMetaRefreshRegex = new Regex(@"((?<seconds>[0-9]+);)?\s*URL=(?<url>(.+))", RegexOptions.Compiled);

        public ScrapingBrowser()
        {
            InitCookieContainer();
            UserAgent = FakeUserAgents.Chrome24;
            AllowAutoRedirect = true;
            Language = CultureInfo.CreateSpecificCulture("EN-US");
            UseDefaultCookiesParser = true;
            IgnoreCookies = false;
            ProtocolVersion = HttpVersion.Version10;
            KeepAlive = false;
            Proxy = WebRequest.DefaultWebProxy;
            Headers = new Dictionary<string, string>();
        }

        public void ClearCookies()
        {
            InitCookieContainer();
        }

        private void InitCookieContainer()
        {
            cookieContainer = new CookieContainer();
        }

        public WebResource DownloadWebResource(Uri url)
        {
            var response = ExecuteRequest(url, HttpVerb.Get, new NameValueCollection());
            var memoryStream = new MemoryStream();
            var responseStream = response.GetResponseStream();

            if (responseStream != null)
                responseStream.CopyTo(memoryStream);

            return new WebResource(memoryStream, response.Headers["Last-Modified"], url, !IsCached(response.Headers["Cache-Control"]));
        }

        private bool IsCached(string header)
        {
            if (string.IsNullOrEmpty(header))
                return false;

            var noCacheHeaders = new []
                {
                    "no-cache",
                    "no-store",
                    "max-age=0",
                    "pragma: no-cache",
                };

            return !noCacheHeaders.Contains(header.ToLowerInvariant());
        }

        public string AjaxDownloadString(Uri url)
        {
            var request = CreateRequest(url, HttpVerb.Get);
            request.Headers["X-Prototype-Version"] = "1.6.1";
            request.Headers["X-Requested-With"] = "XMLHttpRequest";

            return GetResponse(url, request, 0);
        }

        public string DownloadString(Uri url)
        {
            var request = CreateRequest(url, HttpVerb.Get);
            return GetResponse(url, request, 0);
        }

        public Dictionary<string, string> Headers { get; private set; }

        private HttpWebRequest CreateRequest(Uri url, HttpVerb verb)
        {
            var request = (HttpWebRequest)WebRequest.Create(url.AbsoluteUri);
            request.Referer = referer != null ? referer.AbsoluteUri : null;
            request.Method = ToMethod(verb);
            request.CookieContainer = cookieContainer;
            request.UserAgent = UserAgent.UserAgent;

            request.Headers["Accept-Language"] = Language.Name;

            if (Headers != null)
            {
                foreach (var header in Headers)
                    request.Headers[header.Key] = header.Value;
                Headers.Clear();
            }

            request.CachePolicy = CachePolicy;

            if (Timeout > TimeSpan.Zero)
                request.Timeout = (int) Timeout.TotalMilliseconds;

            request.KeepAlive = KeepAlive;
            request.ProtocolVersion = ProtocolVersion;

            return request;
        }

        public IWebProxy Proxy { get; set; }

        public RequestCachePolicy CachePolicy { get; set; }

        public bool AllowMetaRedirect { get; set; }
        
        public bool AutoDownloadPagesResources { get; set; }

        private WebPage GetResponse(Uri url, HttpWebRequest request, int iteration)
        {
            var content = string.Empty;
            var response = GetWebResponse(url, request);
            var responseStream = response.GetResponseStream();
            
            if (responseStream == null)
                return new WebPage(this, url, content, AutoDownloadPagesResources);

            using (var reader = new StreamReader(responseStream))
                content = reader.ReadToEnd();

            var webPage = new WebPage(this, url, content, AutoDownloadPagesResources);

            if (AllowMetaRedirect && !string.IsNullOrEmpty(response.ContentType) && response.ContentType.Contains("html") && iteration < 10)
            {
                var html = content.ToHtmlNode();
                var meta = html.CssSelect("meta")
                    .FirstOrDefault(p => p.Attributes != null && p.Attributes.HasKeyIgnoreCase("HTTP-EQUIV")
                                         && p.Attributes.GetIgnoreCase("HTTP-EQUIV").Equals("refresh", StringComparison.InvariantCultureIgnoreCase));
                
                if (meta != null)
                {
                    var attr= meta.Attributes.GetIgnoreCase("content");
                    var match = parseMetaRefreshRegex.Match(attr);
                    if (!match.Success)
                        return webPage;
                    
                    var seconds = 0;
                    if (match.Groups["seconds"].Success)
                        seconds = int.Parse(match.Groups["seconds"].Value);
                    if (!match.Groups["url"].Success)
                        return webPage;

                    var redirect = Unquote(match.Groups["url"].Value);
                    
                    Uri redirectUrl;
                    if (!Uri.TryCreate(redirect, UriKind.RelativeOrAbsolute, out redirectUrl))
                        return webPage;

                    if (!redirectUrl.IsAbsoluteUri)
                    {
                        var baseUrl = string.Format("{0}://{1}", url.Scheme, url.Host);
                        if (!url.IsDefaultPort)
                            baseUrl += ":" + url.Port;

                        if (redirect.StartsWith("/"))
                            redirectUrl = baseUrl.CombineUrl(redirect);
                        else
                        {
                            var path = string.Join("/", url.Segments.Take(url.Segments.Length - 1).Skip(1));
                            redirectUrl = baseUrl.CombineUrl(path).Combine(redirect);
                        }
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(seconds));

                    return DownloadRedirect(redirectUrl, iteration + 1);
                }
            }

            return webPage;
        }

        private string Unquote(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            if (value.StartsWith("'") || value.StartsWith("\""))
                value = value.Substring(1);

            if (value.EndsWith("'") || value.EndsWith("\"") && value.Length > 1)
                value = value.Substring(0, value.Length - 1);

            return value;
        }

        private WebPage DownloadRedirect(Uri url, int iteration)
        {
            var request = CreateRequest(url, HttpVerb.Get);
            return GetResponse(url, request, iteration);
        }

        public string TransferEncoding { get; set; }

        private WebResponse GetWebResponse(Uri url, HttpWebRequest request)
        {
            referer = url;
            request.AllowAutoRedirect = AllowAutoRedirect;
            var response = request.GetResponse();
            var headers = response.Headers;

            if (!string.IsNullOrWhiteSpace(TransferEncoding))
                request.TransferEncoding = TransferEncoding;

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
                        SetCookies(url, cookiesExpression);
                }
            }
            return response;
        }

        public void SetCookies(Uri cookieUrl, string cookiesExpression)
        {
            var parser = new CookiesParser(cookieUrl.Host);
            var cookies = parser.ParseCookies(cookiesExpression);
            var previousCookies = cookieContainer.GetCookies(cookieUrl);

            foreach (var cookie in cookies)
            {
                var c = previousCookies[cookie.Name];
                if (c != null)
                    c.Value = cookie.Value;
                else
                    cookieContainer.Add(cookie);
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
            return NavigateToPage(url, verb, data);
        }

        public WebPage NavigateToPage(Uri url, HttpVerb verb, string data)
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

            return GetResponse(url, request, 0);
        }

        public WebPage NavigateToPage(Uri url, HttpVerb verb, NameValueCollection data)
        {
            return NavigateToPage(url, verb, GetHttpPostVars(data));
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