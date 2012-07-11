using System.Collections.Generic;
using ScrapySharp.Html.Parsing;

namespace ScrapySharp.Html.Dom
{
    public class TagDeclaration
    {
        public string InnerText { get; set; }

        public string Name { get; set; }

        public Dictionary<string, string> Attributes { get; set; }

        public List<Word> Words { get; set; }

        public DeclarationType Type { get; set; }
    }

    public enum DeclarationType
    {
        TextElement,
        OpenTag,
        CloseTag,
        SelfClosedTag,
    }
}