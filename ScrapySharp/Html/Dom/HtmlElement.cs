using System.Collections.Generic;
using ScrapySharp.Html.Parsing;

namespace ScrapySharp.Html.Dom
{
    public class HtmlElement
    {
        public string InnerText { get; set; }

        public string Name { get; set; }

        public Dictionary<string, string> Attributes { get; set; }

        public List<Word> Words { get; set; }
    }
}