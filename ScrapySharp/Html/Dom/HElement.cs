using System.Collections.Generic;
using System.Text;

namespace ScrapySharp.Html.Dom
{
    public class HElement
    {
        private string innerText;

        public HElement()
        {
            Children = new List<HElement>();
            Attributes = new Dictionary<string, string>();
        }

        public string Name { get; set; }

        public Dictionary<string, string> Attributes { get; set; }

        
        public string InnerText
        {
            get
            {
                if (innerText == null)
                    innerText = string.Empty;

                var builder = new StringBuilder();
                builder.Append(innerText);

                if (Children != null)
                    foreach (var child in Children)
                        builder.Append(child.innerText);

                return builder.ToString();
            }
            set { innerText = value; }
        }

        public List<HElement> Children { get; set; }
    }
}