using System;
using System.Globalization;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScrapySharp.Network;

namespace ScrapySharp.Tests
{
    [TestClass]
    public class When_use_browser
    {
        [TestMethod]
        public void When_parses_cookies()
        {
            var exp1 = @"FBXSID=""8KgAN7h4ZQsvn9OWXy1fvBlrNuRdIr4J0bkguqR5AIdL7clHgA+NQ5URtThL10od""; Max-Age=86400; HTTPOnly";
            var cookieContainer = new CookieContainer();
            cookieContainer.SetCookies(new Uri("http://www.popo.com"), exp1);

            Assert.AreEqual(1, cookieContainer.Count);

            var cookieHeader = cookieContainer.GetCookieHeader(new Uri("http://www.popo.com"));
        }

        [TestMethod]
        public void When_forcing_anguage()
        {
            var browser1 = new ScrapingBrowser();
            var html1 = browser1.DownloadString(new Uri("http://www.google.com"));

            var browser2 = new ScrapingBrowser {Language = CultureInfo.CreateSpecificCulture("fr-FR")};
            var html2 = browser2.DownloadString(new Uri("http://www.google.com"));

            Assert.AreNotEqual(html1, html2);
        }
    }
}