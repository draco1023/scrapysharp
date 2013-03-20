using System.IO;
using ScrapySharp.Network;

namespace ScrapySharp.Cache
{
    public class WebResourceStorage
    {
        private readonly string basePath = "_WebResourcesCache";

        public WebResourceStorage()
        {
            Initialize();

        }

        private void Initialize()
        {
            if (!Directory.Exists(basePath))
                Directory.Exists(basePath);
        }

        public void Save(WebResource webResource)
        {
            var domain = webResource.AbsoluteUrl.Host;


        }
    }
}