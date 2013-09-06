using System;
using System.IO;
using NUnit.Framework;
using ScrapySharp.Html;
using ScrapySharp.Html.Dom;
using ScrapySharp.Html.Forms;
using ScrapySharp.Extensions;
using System.Linq;
using ScrapySharp.Network;

namespace ScrapySharp.Tests
{
    [TestFixture]
    public class When_use_web_forms
    {
        [Test]
        public void When_browsing_using_helpers()
        {
            var browser = new ScrapingBrowser();
            var homePage = browser.NavigateToPage(new Uri("http://www.bing.com/"));

            var form = homePage.FindFormById("sb_form");
            form["q"] = "test";
            WebPage resultsPage = form.Submit();

            var links = resultsPage.Html.CssSelect("div.sb_tlst h3 a").ToArray();

            var webPage = resultsPage.FindLinks(By.Text("rooms")).Single().Click();
        }

        [Test]
        public void When_parsing_form()
        {
            var source = File.ReadAllText("Html/WebFormPage.htm");
            var html = HDocument.Parse(source);
            
            var webForm = new WebForm(html.CssSelect("form[name=TestForm]").Single());

            Assert.AreEqual(5, webForm.FormFields.Count);
        }

        [Test]
        public void When_parsing_form_with_agility_pack()
        {
            var source = File.ReadAllText("Html/WebFormPage.htm");
            var html = source.ToHtmlNode();

            var webForm = new WebForm(html.CssSelect("form[name=TestForm]").Single());

            //Because HtmlAgilityPack fails the form parsing !
            Assert.AreNotEqual(5, webForm.FormFields.Count);
        }

        [Test]
        public void When_parsing_partial_view()
        {
            var source = File.ReadAllText("Html/Form1.htm");
            var html = HDocument.Parse(source);

            var form = html.CssSelect("form").SingleOrDefault();

            Assert.IsNotNull(form);


        }
    }
}