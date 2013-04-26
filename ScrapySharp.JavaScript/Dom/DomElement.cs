using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace ScrapySharp.JavaScript.Dom
{
    public class DomElement
    {
        protected readonly HtmlNode node;

        public DomElement(HtmlNode node)
        {
            if (node == null)
                this.node = new HtmlNode(HtmlNodeType.Element, Document.Current.HtmlDocument, 0);
            else
                this.node = node;
        }

        public HtmlNode Node
        {
            get { return node; }
        }

        public void SetAttribute(string name, string value)
        {
            node.SetAttributeValue(name, value);
        }

        public List<DomElement> GetElementsByName(string name)
        {
            return node.Descendants(name).Select(d => new DomElement(d)).ToList();
        }

        public List<DomElement> GetElementsByTagName(string name)
        {
            return node.Descendants(name).Select(d => new DomElement(d)).ToList();
        }
        
        public void AppendChild(DomElement element)
        {
            Node.AppendChild(element.Node);
        }
    }
}