using System.Collections.Generic;

namespace ScrapySharp.Html.Dom
{
    public class HElement
    {
        public HElement()
        {
            Children = new List<HElement>();
            Attributes = new Dictionary<string, string>();
        }

        public string Name { get; set; }

        public Dictionary<string, string> Attributes { get; set; }

        public string InnerText { get; set; }

        public List<HElement> Children { get; set; }
    }
}