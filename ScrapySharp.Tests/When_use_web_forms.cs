using System.IO;
using NUnit.Framework;
using ScrapySharp.Html.Dom;
using ScrapySharp.Html.Forms;
using ScrapySharp.Extensions;
using System.Linq;

namespace ScrapySharp.Tests
{
    [TestFixture]
    public class When_use_web_forms
    {
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
    }
}