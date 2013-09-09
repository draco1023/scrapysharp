using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using smnetjs;

namespace ScrapySharp.JavaScript.Dom
{
    [SMEmbedded(Name = "DomElement", AccessibleName = "DomElement", AllowInheritedMembers = true, AllowScriptDispose = true)]
    public class DomElement
    {
        protected Document document;
        protected HtmlNode node;

        public DomElement(HtmlNode node, Document document)
        {
            this.document = document;
            if (document != null)
            {
                if (node == null)
                    this.node = new HtmlNode(HtmlNodeType.Element, document.HtmlDocument.OwnerDocument, 0);
                else
                    this.node = node;
            }
            else
            {
                
            }
        }

        public string GetOuterHtml()
        {
            return node == null ? string.Empty : node.OuterHtml;
        }

        [SMIgnore]
        public HtmlNode Node
        {
            get { return node; }
        }

        [SMMethod(Name = "setAttribute")]
        public void SetAttribute(string name, string value)
        {
            node.SetAttributeValue(name, value);
        }
        

        [SMProperty(Name = "innerHTML")]
        public string InnerHtml
        {
            get { return node.InnerHtml; }
            set { node.InnerHtml = value; }
        }


        [SMMethod(Name = "createElement")]
        public DomElement CreateElement(string tagName)
        {
            return new DomElement(node.OwnerDocument.CreateElement(tagName), this.document);
        }

        [SMMethod(Name = "createDocumentFragment")]
        public DomElement CreateDocumentFragment()
        {
            return new DomElement(HtmlNode.CreateNode(""), this.document);
        }

        [SMMethod(Name = "getElementById")]
        public DomElement GetElementById(string id)
        {
            var e = node.Descendants().FirstOrDefault(d => d.Id == id);
            if (e == null)
                return null;

            return new DomElement(e, document);
        }

        [SMMethod(Name = "getElementsById")]
        public List<DomElement> GetElementsById(string id)
        {
            return node.Descendants()
                .Where(d => d.Id == id)
                .Select(d => new DomElement(d, this.document)).ToList();
        }

        [SMMethod(Name = "getElementsByName")]
        public List<DomElement> GetElementsByName(string name)
        {
            return node.Descendants(name).Select(d => new DomElement(d, this.document)).ToList();
        }

        [SMMethod(Name = "getElementsByTagName")]
        public List<DomElement> GetElementsByTagName(string name)
        {
            return node.Descendants(name).Select(d => new DomElement(d, this.document)).ToList();
        }

        [SMMethod(Name = "insertBefore")]
        public void InsertBefore(DomElement element, DomElement previous)
        {
            node.InsertBefore(element.Node, previous.Node);
        }

        [SMMethod(Name = "appendChild")]
        public void AppendChild(DomElement element)
        {
            node.AppendChild(element.Node);
        }
    }
}