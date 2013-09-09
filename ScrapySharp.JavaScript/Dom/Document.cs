using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Linq;
using smnetjs;

namespace ScrapySharp.JavaScript.Dom
{
    [SMEmbedded(Name = "Document", AccessibleName = "Document", AllowInheritedMembers = true, AllowScriptDispose = true)]
    public class Document : DomElement
    {
        
        protected readonly HtmlDocument htmlDocument;

        public Document() : base(null, null)
        {
            htmlDocument = new HtmlDocument();
            document = this;
            //node = new HtmlNode(HtmlNodeType.Element, HtmlDocument.OwnerDocument, 0);
            node = htmlDocument.DocumentNode;
        }

        public Document(HtmlNode node, Document document) : base(node, document)
        {
            htmlDocument = document.htmlDocument;
        }
        
        [SMIgnore]
        public HtmlNode HtmlDocument
        {
            get { return htmlDocument.DocumentNode; }
        }

        [SMProperty(Name = "body")]
        public DomElement Body
        {
            get
            {
                var node = htmlDocument.DocumentNode.Descendants("body").FirstOrDefault();
                if (node == null)
                    return null;
                return new DomElement(node, this);
            }
        }
        
        [SMMethod(Name = "write")]
        public void Write(string text)
        {
            Console.WriteLine(text);
        }
        
        [SMProperty(Name = "documentElement")]
        public DomElement DocumentElement
        {
            get
            {
                return new DomElement(htmlDocument.DocumentNode, this);
            }
            set
            {
                htmlDocument.LoadHtml(value.Node.OuterHtml);
            }
        }

        [SMMethod(Name = "LoadHtml")]
        public void LoadHtml(string html)
        {
            htmlDocument.LoadHtml(html);
            node = htmlDocument.DocumentNode;
            //InnerHtml = html;
        }

        public void ExecuteScripts(SMScript smScript)
        {
            var scripts = DocumentElement.Node.Descendants("script").Where(s => s.GetAttributeValue("type", string.Empty).Contains("javascript")).Select(s => s.InnerText);

            foreach (var script in scripts)
            {
                smScript.Eval(script);
            }

            var onloads = DocumentElement.Node.Descendants().Where(e => !string.IsNullOrEmpty(e.GetAttributeValue("onload", string.Empty))).Select(e => e.GetAttributeValue("onload", string.Empty)).ToArray();
            foreach (var onload in onloads)
            {
                smScript.Eval(onload);
            }
        }
    }
}