using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Linq;

namespace ScrapySharp.JavaScript.Dom
{
    public class Document
    {
        [ThreadStatic]
        private static Document current;

        public static Document Current
        {
            get
            {
                if (current == null)
                    current = new Document();
                return current;
            }
        }

        protected readonly HtmlDocument htmlDocument;

        public Document()
        {
            htmlDocument = new HtmlDocument();
        }

        public HtmlDocument HtmlDocument
        {
            get { return htmlDocument; }
        }

        public string InnerHtml
        {
            get { return htmlDocument.DocumentNode.InnerHtml; }
            set { htmlDocument.DocumentNode.InnerHtml = value; }
        }
        
        public void Write(string text)
        {
            Console.WriteLine(text);
        }

        public void LoadHtml(string html)
        {
            InnerHtml = html;
        }

        public DomElement createElement(string tagName)
        {
            return new DomElement(htmlDocument.CreateElement(tagName));
        }

        public DomElement CreateDocumentFragment()
        {
            return new DomElement(HtmlNode.CreateNode(""));
        }

        public List<DomElement> GetElementsByName(string name)
        {
            return htmlDocument.DocumentNode.Descendants(name).Select(d => new DomElement(d)).ToList();
        }

        public List<DomElement> GetElementsByTagName(string name)
        {
            return htmlDocument.DocumentNode.Descendants(name).Select(d => new DomElement(d)).ToList();
        }

        public void AppendChild(DomElement element)
        {
            htmlDocument.DocumentNode.AppendChild(element.Node);
        }
        
        public DomElement DocumentElement
        {
            get
            {
                return new DomElement(htmlDocument.DocumentNode);
            }
            set
            {
                htmlDocument.LoadHtml(value.Node.OuterHtml);
            }
        }
    }
}