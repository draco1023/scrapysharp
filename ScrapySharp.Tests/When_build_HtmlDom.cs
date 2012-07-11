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
        }
    }
}

// ReSharper restore InconsistentNaming