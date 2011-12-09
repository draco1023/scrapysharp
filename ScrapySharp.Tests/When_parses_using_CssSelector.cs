// ReSharper disable InconsistentNaming

using System;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScrapySharp.Extensions;

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

    [TestClass]
    public class When_parses_using_CssSelector
    {
        private readonly HtmlNode html;

        public When_parses_using_CssSelector()
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(@"Html/Page1.htm");
            html = htmlDocument.DocumentNode;
        }

        [TestMethod]
        public void When_uses_simple_tagName()
        {
            var divs = html.CssSelect("div").ToArray();

            Assert.AreEqual(28, divs.Length);
        }

        [TestMethod]
        public void When_uses_tagName_with_css_class()
        {
            Assert.AreEqual(3, html.CssSelect("div.content").Count());

            Assert.AreEqual(1, html.CssSelect("div.widget.monthlist").Count());
        }

        [TestMethod]
        public void When_uses_tagName_with_css_class_using_inheritance()
        {
            Assert.AreEqual(1, html.CssSelect("div.left-corner div.node").Count());

            var nodes = html.CssSelect("span#testSpan span").ToArray();

            Assert.AreEqual(2, nodes.Length);

            Assert.AreEqual("tototata", nodes[0].InnerText);
            Assert.AreEqual("tata", nodes[1].InnerText);

        }

        [TestMethod]
        public void When_uses_id()
        {
            Assert.AreEqual(1, html.CssSelect("#postPaging").Count());

            Assert.AreEqual(1, html.CssSelect("div#postPaging").Count());

            Assert.AreEqual(1, html.CssSelect("div#postPaging.testClass").Count());
        }

        [TestMethod]
        public void When_uses_tagName_with_css_class_using_direct_inheritance()
        {
            Assert.AreEqual(1, html.CssSelect("div.content > p.para").Count());
        }

        [TestMethod]
        public void When_uses_tagName_with_id_class_using_direct_inheritance()
        {
            Assert.AreEqual(1, html.CssSelect("ul#pagelist > li#listItem1").Count());
        }

        [TestMethod]
        public void When_uses_ancestor()
        {
            var ancestors = html.CssSelect("p.para").CssSelectAncestors("div div.menu").ToArray();
            Assert.AreEqual(1, ancestors.Count());
        }

        [TestMethod]
        public void When_uses_direct_ancestor()
        {
            var ancestors1 = html.CssSelect("p.para").CssSelectAncestors("div.content > div.menu").ToArray();
            Assert.AreEqual(0, ancestors1.Count());

            var ancestors2 = html.CssSelect("p.para").CssSelectAncestors("div.content > div.widget").ToArray();
            Assert.AreEqual(1, ancestors2.Count());
        }

        [TestMethod]
        public void When_uses_attribute_selector()
        {
            Assert.AreEqual(1, html.CssSelect("input[type=button]").Count());
            
            Assert.AreEqual(2, html.CssSelect("input[type=text]").Count());
        }

        [TestMethod]
        public void When_uses_attribute_selector_with_css_class()
        {
            Assert.AreEqual(1, html.CssSelect("input[type=text].login").Count());
        }
    }
}

// ReSharper restore InconsistentNaming
