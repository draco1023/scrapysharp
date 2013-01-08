using NUnit.Framework;
using ScrapySharp.Core;
using System.Linq;

namespace ScrapySharp.Tests
{
    [TestFixture]
    public class FastHtmlParserTests
    {
        [Test]
        public void When_readTag()
        {
            var source = @"petit test: 1 < 2 <html><body>
  <input name=""man-news"" />

  <input name=""milk man"" />
  <input name=""letterman2"" />
  <input name=""newmilk"" />

  <input type=""checkbox"" checked />
  <input type=""checkbox"" />
  <input type=""checkbox"" checked=""checked"" />
  <input type=""file"" />
  <input type=""hidden"" />
</body>
</html>";
            var fastHtmlParser = new FastHtmlParser(source);
            var tag1 = fastHtmlParser.ReadTag();
            Assert.AreEqual("petit test: 1 < 2 ", tag1.InnerText);

            var tag2 = fastHtmlParser.ReadTag();
            Assert.AreEqual("html", tag2.Name);

            var tag3 = fastHtmlParser.ReadTag();
            Assert.AreEqual("body", tag3.Name);

            var tag4 = fastHtmlParser.ReadTag();
            Assert.AreEqual("\r\n  ", tag4.InnerText);

            var tag5 = fastHtmlParser.ReadTag();
            Assert.AreEqual("input", tag5.Name);
            Assert.AreEqual(1, tag5.Attributes.Length);
            Assert.AreEqual("man-news", tag5.Attributes.First(a => a.Name == "name").Value);

            var tag6 = fastHtmlParser.ReadTag();
            Assert.AreEqual("\r\n  ", tag6.InnerText);
        }

        [Test]
        public void When_readString()
        {
            var source = @"petit test: 1 < 2 <html><body>
  <input name=""man-news"" />

  <input name=""milk man"" />
  <input name=""letterman2"" />
  <input name=""newmilk"" />

  <input type=""checkbox"" checked />
  <input type=""checkbox"" />
  <input type=""checkbox"" checked=""checked"" />
  <input type=""file"" />
  <input type=""hidden"" />
</body>
</html>";

            var fastHtmlParser = new FastHtmlParser(source);
            
            var element1 = fastHtmlParser.ReadElement();
            Assert.AreEqual("petit test: 1 < 2 ", element1);

            var element2 = fastHtmlParser.ReadElement();
            Assert.AreEqual("<html>", element2);

            var element3 = fastHtmlParser.ReadElement();
            Assert.AreEqual("<body>\r\n  ", element3);

            var element4 = fastHtmlParser.ReadElement();
            Assert.AreEqual("<input name=\"man-news\" />\r\n\r\n  ", element4);

        }
    }
}