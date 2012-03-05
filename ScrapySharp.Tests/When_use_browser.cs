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
        public void When_parses_cookies2()
        {
            var exp1 = @"JSESSIONID=8811CD44E7B62E1F29347088E89E4318.p0580; Path=/merchandising,Service=LMN; Domain= www.lastminute.com; Path=/,SID=T000V00000X120305033255169934019722775; Domain= www.lastminute.com; Path=/,Devteam=snappy; Domain= www.lastminute.com; Path=/";
            //var exp1 = @"Path=/merchandising,toto=popeofezfez; Domain= www.lastminute.com; Path=/merchandising,Service=LMN; Domain= www.lastminute.com; Path=/,SID=T000V00000X120305033255169934019722775; Domain= www.lastminute.com; Path=/,Devteam=snappy; Domain= www.lastminute.com; Path=/";
            //var exp1 = @"Path=/merchandising,toto=popeofezfez; Domain= www.lastminute.com; Path=/merchandising,Service=LMN; Domain= www.lastminute.com;";
            //var cookieContainer = new CookieContainer();
            //cookieContainer.SetCookies(new Uri("http://www.lastminute.com"), exp1);

            //Assert.AreEqual(1, cookieContainer.Count);

            //var cookieHeader = cookieContainer.GetCookieHeader(new Uri("http://www.lastminute.com"));

            var browser = new ScrapingBrowser();
            var url = new Uri("http://www.lastminute.com");
            browser.SetCookies(url, exp1);

            Assert.AreEqual("LMN", browser.GetCookie(url, "Service").Value);
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

        [TestMethod]
        public void When_downloading_page_with_a_different_cookie_domain()
        {
            var url = new Uri("http://www.lastminute.com/hotels-d110-united-kingdom-hotels");
            var browser = new ScrapingBrowser();
            browser.UseDefaultCookiesParser = false;
            browser.DownloadString(url);
            
        }
    }
}