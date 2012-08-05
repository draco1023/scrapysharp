// ReSharper disable InconsistentNaming

using System.Linq;
using HtmlAgilityPack;
using NUnit.Framework;
using ScrapySharp.Core;
using ScrapySharp.Extensions;
using ScrapySharp.Core;

namespace ScrapySharp.Tests
{
    [TestFixture]
    public class When_parses_using_CssSelector_with_fsharp_tokenizer
    {
        private readonly HtmlNode html;

        public When_parses_using_CssSelector_with_fsharp_tokenizer()
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(@"Html/Page1.htm");
            html = htmlDocument.DocumentNode;
        }

        [Test]
        public void When_css_class_contains_no_alpha_numerics()
        {
            var tokenizer = new CssSelectorTokenizer();
            var tokens = tokenizer.Tokenize("span.loginbox");

            Assert.AreEqual(3, tokens.Length);

            tokens = tokenizer.Tokenize("span.login-box");

            Assert.AreEqual(3, tokens.Length);
        }

        [Test]
        public void When_execute_css_selector1()
        {
            var tokenizer = new CssSelectorTokenizer();
            var tokens = tokenizer.Tokenize("span.login-box");
            Assert.AreEqual(3, tokens.Length);

            var executor = new CssSelectorExecutor(html.ChildNodes.ToList(), tokens.ToList());
            HtmlNode[] htmlNodes = executor.GetElements();

            Assert.AreEqual(1, htmlNodes.Length);
        }

        [Test]
        public void When_execute_css_selector2()
        {
            var tokenizer = new CssSelectorTokenizer();
            var tokens = tokenizer.Tokenize("div.widget.monthlist");
            Assert.AreEqual(5, tokens.Length);

            var executor = new CssSelectorExecutor(html.ChildNodes.ToList(), tokens.ToList());
            HtmlNode[] htmlNodes = executor.GetElements();

            Assert.AreEqual(1, htmlNodes.Length);
        }
    }
}

// ReSharper restore InconsistentNaming