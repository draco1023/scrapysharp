using System;
using System.IO;

namespace ScrapySharp.Network
{
    public class WebResource : IDisposable
    {
        private readonly MemoryStream content;
        private readonly string lastModified;
        private readonly Uri absoluteUrl;
        private readonly bool forceDownload;

        public WebResource(MemoryStream content, string lastModified, Uri absoluteUrl, bool forceDownload)
        {
            this.content = content;
            this.lastModified = lastModified;
            this.absoluteUrl = absoluteUrl;
            this.forceDownload = forceDownload;
        }

        public void Dispose()
        {
            content.Dispose();
        }

        public MemoryStream Content
        {
            get { return content; }
        }

        public string LastModified
        {
            get { return lastModified; }
        }

        public Uri AbsoluteUrl
        {
            get { return absoluteUrl; }
        }

        public bool ForceDownload
        {
            get { return forceDownload; }
        }

        public bool IsScript { get; set; }
    }
}