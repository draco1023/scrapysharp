using System.Collections.Generic;
using System.Linq;
using ScrapySharp.Html.Parsing;

namespace ScrapySharp.Html.Dom
{
    public class HDocument : HContainer
    {
        public HDocument()
        {
            Children = new List<HElement>();
        }

        public static HDocument Parse(string source)
        {
            var codeReader = new CodeReader(source);
            var declarationReader = new HtmlDeclarationReader(codeReader);
            var domBuilder = new HtmlDomBuilder(declarationReader);

            return new HDocument
                       {
                           Children = domBuilder.BuildDom().ToList()
                       };
        }

    }
}