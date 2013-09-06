using System;
using System.Collections.Generic;
using System.Net;
using HtmlAgilityPack;
using ScrapySharp.Cache;
using ScrapySharp.Extensions;
using System.Linq;
using ScrapySharp.Html;
using ScrapySharp.Html.Forms;

namespace ScrapySharp.Network
{
    public class WebPage
    {
        private readonly ScrapingBrowser browser;
        private readonly Uri absoluteUrl;
        private readonly string content;
        private readonly List<WebResource> resources;
        private readonly HtmlNode html;
        private string baseUrl;

        private static readonly Dictionary<string, string> resourceTags = new Dictionary<string, string> 
            {
                {"img", "src"},
                {"script", "src"},
                {"link", "href"},
            };

        public WebPage(ScrapingBrowser browser, Uri absoluteUrl, string content, bool autoDownloadPagesResources)
        {
            this.browser = browser;
            this.absoluteUrl = absoluteUrl;
            this.content = content;
            resources = new List<WebResource>();

            html = content.ToHtmlNode();

            if (autoDownloadPagesResources)
            {
                LoadBaseUrl();
                DownloadResources();
            }
        }

        public IEnumerable<HtmlNode> Find(string tag, By by)
        {
            return @by.CreateElementFinder(html, tag).FindElements();
        }

        public IEnumerable<HyperLink> FindLinks(By by)
        {
            return Find("a", by).Select(a => new HyperLink(this, a));
        }

        public PageWebForm FindForm(string name)
        {
            var node = (from n in Html.Descendants("form")
                        let formName = n.GetAttributeValue("name", string.Empty)
                        where formName == name
                        select n).FirstOrDefault();

            return node == null ? null : new PageWebForm(node, browser);
        }

        public PageWebForm FindFormById(string id)
        {
            var node = Html.Descendants("form").FirstOrDefault(f => f.Id == id);
            return node == null ? null : new PageWebForm(node, browser);
        }

        private void LoadBaseUrl()
        {
            var baseAttr = html.Descendants("base").Where(e => e.Attributes.Any(a => a.Name == "href"))
                .Select(e => e.Attributes["href"].Value).FirstOrDefault();

            if (baseAttr != null)
            {
                baseUrl = baseAttr;
                return;
            }

            baseUrl = string.Format("{0}://{1}", absoluteUrl.Scheme, absoluteUrl.Host);
            if (!absoluteUrl.IsDefaultPort)
                baseUrl += ":" + absoluteUrl.Port;
        }

        public override string ToString()
        {
            return content;
        }

        public static implicit operator string(WebPage page)
        {
            return page.content;
        }

        private void DownloadResources()
        {
            var resourceUrls = GetResourceUrls();

            foreach (var resourceUrl in resourceUrls)
            {
                Uri result;
                Uri.TryCreate(resourceUrl, UriKind.RelativeOrAbsolute, out result);
                Uri url;
                
                if (!result.IsAbsoluteUri)
                {
                    if (resourceUrl.StartsWith("/") || resourceUrl.StartsWith("./") || resourceUrl.StartsWith("../"))
                        //TODO: / is a base path
                        url = baseUrl.CombineUrl(resourceUrl);
                    else
                    {
                        var path = string.Join("/", absoluteUrl.Segments.Take(absoluteUrl.Segments.Length - 1).Skip(1));
                        url = baseUrl.CombineUrl(path).Combine(resourceUrl);
                    }
                }
                else
                    url = new Uri(resourceUrl);

                if (WebResourceStorage.Current.Exists(url.ToString()))
                    continue;

                try
                {
                    WebResource resource = browser.DownloadWebResource(url);
                    resources.Add(resource);
                    if (!resource.ForceDownload || !string.IsNullOrEmpty(resource.LastModified))
                        WebResourceStorage.Current.Save(resource);
                }
                catch
                {
                    
                }
            }
        }

        public List<string> GetResourceUrls()
        {
            var resourceUrls = new List<string>();

            foreach (var resourceTag in resourceTags)
            {
                var sources = html.Descendants(resourceTag.Key)
                    .Where(e => e.Attributes.Any(a => a.Name == resourceTag.Value))
                    .Select(e => e.Attributes[resourceTag.Value].Value).ToArray();
                resourceUrls.AddRange(sources);
            }
            return resourceUrls;
        }

        public ScrapingBrowser Browser
        {
            get { return browser; }
        }

        public Uri AbsoluteUrl
        {
            get { return absoluteUrl; }
        }

        public string Content
        {
            get { return content; }
        }

        public List<WebResource> Resources
        {
            get { return resources; }
        }

        public HtmlNode Html
        {
            get { return html; }
        }

        public string BaseUrl
        {
            get { return baseUrl; }
        }
    }
}