// ReSharper disable InconsistentNaming

using System.IO;
using NUnit.Framework;
using ScrapySharp.Html.Dom;
using System.Linq;
using ScrapySharp.Extensions;

namespace ScrapySharp.Tests
{
    [TestFixture]
    public class When_parse_real_html_pages
    {
        [Test]
        public void When_parsing_InvalidPage1()
        {
            var source = File.ReadAllText("Html/Page1.htm");
            var document = HDocument.Parse(source);
            
        }

        [Test]
        public void When_parsing_ValidPage2()
        {
            var source = File.ReadAllText("Html/ValidPage2.htm");
            var document = HDocument.Parse(source);

            var js = document.Descendants("script").Single().InnerText;
        }

        [Test]
        public void When_parsing_InvalidPage2()
        {
            var source = File.ReadAllText("Html/InvalidPage2.htm");
            var document = HDocument.Parse(source);

            Assert.AreEqual(1, document.CssSelect("div.login").Count());
            
            Assert.AreEqual(3, document.CssSelect("div").Count());

            Assert.AreEqual(1, document.CssSelect("div#footer").Count());

            var outerHtml = document.OuterHtml;
        }
    }
}

// ReSharper restore InconsistentNaming