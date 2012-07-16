using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ScrapySharp.Html.Dom
{
    public class HElement : HContainer
    {

        public HElement()
        {
            Children = new List<HElement>();
            Attributes = new NameValueCollection();
        }
        
        public string Id
        {
            get
            {
                if (HasAttributes)
                    return Attributes["id"];
                return string.Empty;
            }
            set
            {
                if (HasAttributes)
                    Attributes["id"] = value;
            }
        }

        public NameValueCollection Attributes { get; internal set; }

        public bool HasAttributes
        {
            get { return Attributes != null && Attributes.Count > 0; }
        }

        public HElement ParentNode { get; set; }
        
        public IEnumerable<HElement> Ancestors()
        {
            for (HElement node = this.ParentNode; node.ParentNode != null; node = node.ParentNode)
                yield return node.ParentNode;
        }

        public IEnumerable<HElement> Ancestors(string name)
        {
            for (HElement n = this.ParentNode; n != null; n = n.ParentNode)
            {
                if (n.Name == name)
                    yield return n;
            }
        }

        public string GetAttributeValue(string name, string def)
        {
            if (!HasAttributes)
                return def;
            var value = Attributes[name];
            if (value == null)
                return def;
            return value;
        }

        public int GetAttributeValue(string name, int def)
        {
            if (!HasAttributes)
                return def;
            var value = Attributes[name];
            if (value == null)
                return def;
            
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return def;
            }
        }

        public bool GetAttributeValue(string name, bool def)
        {
            if (!HasAttributes)
                return def;
            var value = Attributes[name];
            if (value == null)
                return def;
            
            try
            {
                return Convert.ToBoolean(value);
            }
            catch
            {
                return def;
            }
        }
    }
}