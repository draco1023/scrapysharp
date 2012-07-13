// ReSharper disable InconsistentNaming

using System.IO;
using NUnit.Framework;
using ScrapySharp.Html.Dom;

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
    }
}

// ReSharper restore InconsistentNaming