// ReSharper disable InconsistentNaming

using NUnit.Framework;
using ScrapySharp.Html.Parsing;
using System.Linq;

namespace ScrapySharp.Tests
{
    [TestFixture]
    public class When_build_HtmlDom
    {
        [Test]
        public void When_read_a_simple_tag()
        {
            var sourceCode = " dada\n<div class=\"login box1\" id=\"div1\" data-tooltip=\"salut, ça va?\">login: \n\t romcy</div>"
                             + "<img src=\"http://popo.fr/titi.gif\" />";

            var codeReader = new CodeReader(sourceCode);
            var declarationReader = new HtmlDeclarationReader(codeReader);
            var domBuilder = new HtmlDomBuilder(declarationReader);

            var elements = domBuilder.BuildDom().ToList();

            Assert.AreEqual(3, elements.Count);

            Assert.AreEqual(" dada\n", elements[0].InnerText);
            Assert.IsNull(elements[0].Name);

            Assert.AreEqual("div", elements[1].Name);
            Assert.AreEqual(3, elements[1].Attributes.Count);
            Assert.AreEqual("login box1", elements[1].Attributes["class"]);
            Assert.AreEqual("div1", elements[1].Attributes["id"]);
            Assert.AreEqual("salut, ça va?", elements[1].Attributes["data-tooltip"]);
            
            Assert.AreEqual("login: \n\t romcy", elements[1].InnerText);
            
            Assert.AreEqual(1, elements[1].Children.Count);
            Assert.IsNull(elements[1].Children[0].Name);
            Assert.AreEqual("login: \n\t romcy", elements[1].Children[0].InnerText);

            Assert.AreEqual("img", elements[2].Name);
            Assert.AreEqual(1, elements[2].Attributes.Count);
            Assert.AreEqual("http://popo.fr/titi.gif", elements[2].Attributes["src"]);
            Assert.AreEqual(0, elements[2].Children.Count);
        }
    }
}

// ReSharper restore InconsistentNaming