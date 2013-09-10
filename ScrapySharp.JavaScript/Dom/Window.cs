using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using smnetjs;

namespace ScrapySharp.JavaScript.Dom
{
    [SMEmbedded(Name = "Window", AccessibleName = "Window", AllowInheritedMembers = true, AllowScriptDispose = true)]
    public class Window : DomElement, ISMDynamic
    {
        private readonly SMScript smScript;
        private readonly Dictionary<string, object> dynamicMembers = new Dictionary<string, object>();

        public object OnPropertyGetter(SMScript script, string name)
        {
            if (!dynamicMembers.ContainsKey(name))
                return null;

            return dynamicMembers[name];
        }

        public void OnPropertySetter(SMScript script, string name, object value)
        {
            //var eval = script.Eval<object>("return window." + name + ";");

            if (dynamicMembers.ContainsKey(name))
                dynamicMembers[name] = value;
            else
                dynamicMembers.Add(name, value);

            smScript.SetGlobalProperty(name, value);
        }

        [SMProperty(Name = "document")]
        public Document Document { get; set; }

        public Window(Document document, SMScript smScript) : base(document.HtmlDocument, document)
        {
            this.smScript = smScript;
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