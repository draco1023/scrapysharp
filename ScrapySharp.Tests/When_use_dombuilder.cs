// ReSharper disable InconsistentNaming

using NUnit.Framework;
using ScrapySharp.Html.Parsing;

namespace ScrapySharp.Tests
{
    [TestFixture]
    public class When_use_dombuilder
    {
        [Test]
        public void When_read_a_simple_tag()
        {
            var sourceCode = " dada\n<div class=\"login box1\">login: \n\t romcy</div>";
            var codeReader = new CodeReader(sourceCode);
            var domBuilder = new HtmlDomBuilder(codeReader);

            var element = domBuilder.ReadHtmlElement();
            Assert.AreEqual(" ", element.InnerText);
            
            element = domBuilder.ReadHtmlElement();
            Assert.AreEqual("dada", element.InnerText);

            element = domBuilder.ReadHtmlElement();
            Assert.AreEqual("\n", element.InnerText);

            element = domBuilder.ReadHtmlElement();
            Assert.AreEqual("div", element.Name);


        }
    }
}

// ReSharper restore InconsistentNaming