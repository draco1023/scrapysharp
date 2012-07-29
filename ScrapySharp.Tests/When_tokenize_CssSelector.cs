// ReSharper disable InconsistentNaming

using NUnit.Framework;
using ScrapySharp.Core;

namespace ScrapySharp.Tests
{
    [TestFixture]
    public class When_tokenize_CssSelector
    {
        [Test]
        public void When_css_class_contains_no_alpha_numerics()
        {
            var tokenizer = new CssSelectorTokenizer();
            var tokens = tokenizer.Tokenize("span.loginbox");

            Assert.AreEqual(3, tokens.Length);

            tokens = tokenizer.Tokenize("span.login-box");

            Assert.AreEqual(3, tokens.Length);
        }
    }
}

// ReSharper restore InconsistentNaming