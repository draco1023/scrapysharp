using System.Collections.Generic;
using System.Text;

namespace ScrapySharp.Html.Dom
{
    public class HElement : HContainer
    {
        private string name;
        private string innerText;

        public HElement()
        {
            Children = new List<HElement>();
            Attributes = new Dictionary<string, string>();
        }

        public string Name
        {
            get
            {
                if (name == null)
                    return string.Empty;
                return name;
            }
            set { name = value; }
        }

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

    }
}