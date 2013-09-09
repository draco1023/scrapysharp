using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using smnetjs;

namespace ScrapySharp.JavaScript.Dom
{
    [SMEmbedded(Name = "Window", AccessibleName = "Window", AllowInheritedMembers = true, AllowScriptDispose = true)]
    public class Window : DomElement
    {
        [SMProperty(Name = "document")]
        public Document Document { get; set; }

        public Window(Document document) : base(document.HtmlDocument, document)
        {
            Document = document;
        }

        [SMMethod(Name = "createElement")]
        public DomElement CreateElement(string tagName)
        {
            return new DomElement(Document.CreateElement(tagName).Node, Document);
        }

        [SMMethod(Name = "createDocumentFragment")]
        public DomElement CreateDocumentFragment()
        {
            return new DomElement(HtmlNode.CreateNode(""), Document);
        }

        [SMMethod(Name = "getElementsByName")]
        public List<DomElement> GetElementsByName(string name)
        {
            return Document.HtmlDocument.Descendants(name).Select(d => new DomElement(d, Document)).ToList();
        }

        [SMMethod(Name = "getElementsByTagName")]
        public List<DomElement> GetElementsByTagName(string name)
        {
            return Document.HtmlDocument.Descendants(name).Select(d => new DomElement(d, Document)).ToList();
        }

        [SMMethod(Name = "appendChild")]
        public void AppendChild(DomElement element)
        {
            Document.HtmlDocument.AppendChild(element.Node);
        }
    }
}