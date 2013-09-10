using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using smnetjs;

/*
 http://www.w3schools.com/jsref/dom_obj_all.asp
 */
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
                this.node = node ?? new HtmlNode(HtmlNodeType.Element, document.HtmlDocument.OwnerDocument, 0);
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

        [SMProperty(Name = "namespaceURI")]
        public string NamespaceUri
        {
            get { return document.Body.OwnerDocument.NamespaceUri; }
            set { document.Body.OwnerDocument.NamespaceUri = value; }
        }

        [SMProperty(Name = "lang")]
        public string Lang
        {
            get
            {
                var body = document.Body;
                if (body == null)
                    return string.Empty;
                return body.Node.GetAttributeValue("lang", string.Empty);
            }
            set
            {
                var body = document.Body;
                if (body == null)
                    return;
                body.Node.SetAttributeValue("lang", value);
            }
        }

        [SMProperty(Name = "dir")]
        public string Dir
        {
            get
            {
                var body = document.Body;
                if (body == null)
                    return string.Empty;
                return body.Node.GetAttributeValue("dir", string.Empty);
            }
            set
            {
                var body = document.Body;
                if (body == null)
                    return;
                body.Node.SetAttributeValue("dir", value);
            }
        }

        [SMProperty(Name = "nodeType")]
        public int NodeType
        {
            get { return 1; }
        }

        [SMProperty(Name = "nodeValue")]
        public string NodeValue
        {
            get { return Node.InnerText; }
        }

        [SMProperty(Name = "tagName")]
        public string TagName
        {
            get { return Node.Name; }
            set { Node.Name = value; }
        }

        [SMProperty(Name = "nodeName")]
        public string NodeName
        {
            get { return Node.Name; }
            set { Node.Name = value; }
        }

        [SMProperty(Name = "parentNode")]
        public DomElement ParentNode
        {
            get
            {
                if (node.ParentNode == null)
                    return null;
                return new DomElement(node.ParentNode, document);
            }
        }

        [SMProperty(Name = "previousSibling")]
        public DomElement PreviousSibling
        {
            get
            {
                if (node.PreviousSibling == null)
                    return null;
                return new DomElement(node.PreviousSibling, document);
            }
        }

        [SMProperty(Name = "nextSibling")]
        public DomElement NextSibling
        {
            get
            {
                if (node.NextSibling == null)
                    return null;
                return new DomElement(node.NextSibling, document);
            }
        }

        [SMProperty(Name = "lastChild")]
        public DomElement LastChild
        {
            get
            {
                if (node.LastChild == null)
                    return null;
                return new DomElement(node.LastChild, document);
            }
        }

        [SMProperty(Name = "firstChild")]
        public DomElement FirstChild
        {
            get
            {
                if (node.FirstChild == null)
                    return null;
                return new DomElement(node.FirstChild, document);
            }
        }

        [SMMethod(Name = "replaceChild")]
        public void ReplaceChild(DomElement newnode, DomElement oldnode)
        {
            Node.ReplaceChild(newnode.Node, oldnode.Node);
        }

        [SMMethod(Name = "removeChild")]
        public void RemoveChild(DomElement element)
        {
            Node.RemoveChild(element.Node);
        }

        [SMMethod(Name = "removeAttribute")]
        public void RemoveAttribute(string name)
        {
            Node.Attributes.Remove(name);
        }

        [SMMethod(Name = "normalize")]
        public void Normalize()
        {
            throw new NotImplementedException();
        }

        [SMMethod(Name = "getUserData")]
        public DomElement GetUserData()
        {
            throw new NotImplementedException();
        }

        [SMMethod(Name = "getFeature")]
        public DomElement GetFeature()
        {
            throw new NotImplementedException();
        }

        [SMMethod(Name = "hasChildNodes")]
        public bool HasChildNodes()
        {
            return node.HasChildNodes;
        }

        [SMMethod(Name = "hasAttributes")]
        public bool HasAttributes()
        {
            return node.HasAttributes;
        }

        [SMMethod(Name = "isSupported")]
        public bool IsSupported(string feature, string version)
        {
            return true;
        }

        [SMMethod(Name = "isSameNode")]
        public bool IsSameNode(DomElement other)
        {
            return node.OuterHtml == other.Node.OuterHtml && node.XPath == other.Node.XPath;
        }

        [SMMethod(Name = "isEqualNode")]
        public bool IsEqualNode(DomElement other)
        {
            return node.OuterHtml == other.Node.OuterHtml;
        }

        [SMMethod(Name = "isDefaultNamespace")]
        public bool IsDefaultNamespace(string name)
        {
            return document.Body.IsDefaultNamespace(name);
        }

        [SMMethod(Name = "hasAttribute")]
        public bool HasAttribute(string name)
        {
            return node.Attributes[name] != null;
        }

        [SMMethod(Name = "getAttribute")]
        public DomElement GetAttributeNode(string name)
        {
            throw new NotImplementedException();
        }

        [SMMethod(Name = "getAttribute")]
        public string GetAttribute(string name)
        {
            return node.GetAttributeValue(name, string.Empty);
        }

        [SMMethod(Name = "setAttribute")]
        public void SetAttribute(string name, string value)
        {
            node.SetAttributeValue(name, value);
        }
        
        [SMProperty(Name = "accessKey")]
        public string AccessKey { get; set; }

        [SMProperty(Name = "attributes")]
        public Dictionary<string, string> Attributes
        {
            get { return node.Attributes.ToDictionary(kv => kv.Name, kv => kv.Value); }
            set
            {
                if(value == null)
                    return;
                foreach (var key in value.Keys)
                    node.SetAttributeValue(key, value[key]);
            }
        }

        [SMProperty(Name = "childNodes")]
        public List<DomElement> ChildNodes
        {
            get
            {
                return node.ChildNodes.Select(n => new DomElement(n, document)).ToList();
            }
            set
            {
                node.ChildNodes.Clear();
                foreach (var domElement in value)
                    node.ChildNodes.Add(domElement.Node);
            }
        }

        [SMProperty(Name = "className")]
        public string ClassName
        {
            get { return node.GetAttributeValue("class", string.Empty); }
            set { node.SetAttributeValue("class", value); }
        }

        [SMProperty(Name = "name")]
        public string Name
        {
            get { return node.GetAttributeValue("name", string.Empty); }
            set { node.SetAttributeValue("name", value); }
        }

        [SMProperty(Name = "scrollWidth")]
        public int ScrollWidth { get; set; }

        [SMProperty(Name = "scrollTop")]
        public int ScrollTop { get; set; }

        [SMProperty(Name = "scrollLeft")]
        public int ScrollLeft { get; set; }

        [SMProperty(Name = "scrollHeight")]
        public int ScrollHeight { get; set; }

        [SMProperty(Name = "offsetTop")]
        public int OffsetTop { get; set; }

        [SMProperty(Name = "offsetParent")]
        public int OffsetParent { get; set; }

        [SMProperty(Name = "offsetLeft")]
        public int OffsetLeft { get; set; }

        [SMProperty(Name = "offsetWidth")]
        public int OffsetWidth { get; set; }

        [SMProperty(Name = "offsetHeight")]
        public int OffsetHeight { get; set; }

        [SMProperty(Name = "clientHeight")]
        public int ClientHeight { get; set; }

        [SMProperty(Name = "clientWidth")]
        public int ClientWidth { get; set; }

        [SMProperty(Name = "ownerDocument")]
        public Document OwnerDocument
        {
            get { return document; }
            set { document = value; }
        }

        [SMProperty(Name = "id")]
        public string Id
        {
            get { return node.Id; }
            set { node.Id = value; }
        }

        [SMProperty(Name = "textContent")]
        public string TextContent
        {
            get { return node.InnerText; }
            set { node.InnerHtml = value; }
        }

        [SMProperty(Name = "innerText")]
        public string InnerText
        {
            get { return node.InnerText; }
            set { node.InnerHtml = value; }
        }

        [SMProperty(Name = "innerHTML")]
        public string InnerHtml
        {
            get { return node.InnerHtml; }
            set { node.InnerHtml = value; }
        }

        [SMMethod(Name = "compareDocumentPosition")]
        public int CompareDocumentPosition(DomElement other)
        {
            return 0;
        }

        [SMMethod(Name = "cloneNode")]
        public DomElement CloneNode()
        {
            return new DomElement(Node.Clone(), document);
        }

        [SMMethod(Name = "createElement")]
        public DomElement CreateElement(string tagName)
        {
            return new DomElement(node.OwnerDocument.CreateElement(tagName), this.document);
        }

        [SMMethod(Name = "createTextNode")]
        public DomElement CreateTextNode(string text)
        {
            return new DomElement(node.OwnerDocument.CreateTextNode(text), this.document);
        }

        [SMMethod(Name = "createDocumentFragment")]
        public DomElement CreateDocumentFragment()
        {
            return new DomElement(document.Node.OwnerDocument.CreateElement(""), document);
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

        [SMMethod(Name = "attachEvent")]
        public void AttachEvent(string name, SMFunction func)
        {

        }
    }
}