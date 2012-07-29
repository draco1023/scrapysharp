using System;

namespace ScrapySharp.Extensions
{
    public static class UrlHelper
    {
        public static Uri Combine(this Uri uri, string path)
        {
            var url = uri.ToString();
            string combined;
            if (url.EndsWith("/") && path.StartsWith("/"))
                combined = url + path.Substring(1);
            else
                combined = url + path;

            return new Uri(combined);
        }
    }
}