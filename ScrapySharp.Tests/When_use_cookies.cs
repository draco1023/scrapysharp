using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ScrapySharp.Tests
{
    [TestClass]
    public class When_use_cookies
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
    }
}