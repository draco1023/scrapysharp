using System;
using System.Globalization;
using System.Linq;
using System.Net;
using NUnit.Framework;
using ScrapySharp.Network;
using ScrapySharp.Extensions;

namespace ScrapySharp.Tests
{
    [TestFixture]
    public class When_use_browser
    {
        [Test]
        public void When_combine_url()
        {
            var baseUrl = "http://toto.dada.com/izi/";

            var relative1 = "../general/images/izi/logo.gif";

            var abs1 = baseUrl.CombineUrl(relative1);
        }

        [Test]
        [Category("Integration")]
        public void When_parses_cookies()
        {
            var exp1 = @"FBXSID=""8KgAN7h4ZQsvn9OWXy1fvBlrNuRdIr4J0bkguqR5AIdL7clHgA+NQ5URtThL10od""; Max-Age=86400; HTTPOnly";
            var cookieContainer = new CookieContainer();
            cookieContainer.SetCookies(new Uri("http://www.popo.com"), exp1);

            Assert.AreEqual(1, cookieContainer.Count);
        }

        [Test]
        [Category("Integration")]
        public void When_parses_cookies2()
        {
            var exp1 = @"JSESSIONID=8811CD44E7B62E1F29347088E89E4318.p0580; Path=/merchandising,Service=LMN; Domain= www.lastminute.com; Path=/,SID=T000V00000X120305033255169934019722775; Domain= www.lastminute.com; Path=/,Devteam=snappy; Domain= www.lastminute.com; Path=/";
            var browser = new ScrapingBrowser();
            var url = new Uri("http://www.lastminute.com");
            browser.SetCookies(url, exp1);

            Assert.AreEqual("LMN", browser.GetCookie(url, "Service").Value);
        }

        [Test, Ignore]
        [Category("Integration")]
        public void When_forcing_anguage()
        {
            var browser1 = new ScrapingBrowser();
            var html1 = browser1.DownloadString(new Uri("http://www.google.com"));

            var browser2 = new ScrapingBrowser {Language = CultureInfo.CreateSpecificCulture("fr-FR")};
            var html2 = browser2.DownloadString(new Uri("http://www.google.com"));

            Assert.AreNotEqual(html1, html2);
        }

        [Test, Ignore]
        [Category("Integration")]
        public void When_downloading_page_with_a_different_cookie_domain()
        {
            var url = new Uri("http://www.lastminute.com/hotels-d110-united-kingdom-hotels");
            var browser = new ScrapingBrowser();
            browser.UseDefaultCookiesParser = false;
            browser.DownloadString(url);
        }

        [Test, Ignore]
        [Category("Integration")]
        public void When_downloading_page_without_referer()
        {
            var url = new Uri("http://www.hotel.info/Booking.aspx?cpn=1&h_step=3&h_sfi=1&lng=EN&h_rooms=1&h_arrival=25/04/2012%2000:00:00&h_chpn=-1&h_nri=1&h_departure=26/04/2012%2000:00:00&h_persons=2&h_hmid=193791&h_sar=1&h_chp=");
            var browser = new ScrapingBrowser();

            //browser.DownloadString(new Uri("http://www.hotel.info/"));

            browser.SetCookies(url, "seo=sl=1; path=/");
            browser.SetCookies(url, "SHOPPERMANAGER%2FAT=lng=en; expires=Thu, 03-Apr-2014 13:57:37 GMT; path=/");
            browser.SetCookies(url, "hdesession=cpn=1; path=/");
            browser.SetCookies(url, "Webtrekk=hotelinfo_Hoteldetail_Rate; expires=Thu, 03-May-2012 13:57:37 GMT; path=/");

            var html = browser.DownloadString(url);
        }

        
    }
}