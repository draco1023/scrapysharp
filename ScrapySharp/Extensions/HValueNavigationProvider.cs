using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using ScrapySharp.Core;
using ScrapySharp.Html.Dom;

namespace ScrapySharp.Extensions
{
    public class HValueNavigationProvider : INavigationProvider<HValue>
    {
        public List<HValue> ChildNodes(List<HValue> nodes)
        {
            return nodes.AsHElements().SelectMany(n => n.Children).AsHValues().ToList();
        }

        public List<HValue> Descendants(List<HValue> nodes)
        {
            return nodes.AsHElements().SelectMany(n => n.Descendants()).AsHValues().ToList();
        }

        public List<HValue> ParentNodes(List<HValue> nodes)
        {
            return nodes.AsHElements().Select(n => n.ParentNode).AsHValues().ToList();
        }

        public List<HValue> AncestorsAndSelf(List<HValue> nodes)
        {
            return nodes.AsHElements().SelectMany(n => n.Ancestors()).Concat(nodes.AsHElements()).AsHValues().ToList();
        }

        public string GetName(HValue node)
        {
            return ((HElement)node).Name;
        }

        public string GetAttributeValue(HValue node, string name, string defaultValue)
        {
            return ((HElement)node).GetAttributeValue(name, defaultValue);
        }

        public string GetId(HValue hValue)
        {
            if (hValue == null)
                return null;
            return ((HElement)hValue).Id;
        }

        public NameValueCollection Attributes(HValue hValue)
        {
            if (hValue == null)
                return null;
            
            var node = (HElement)hValue;

            if (node.Attributes == null)
                return new NameValueCollection();
            return node.Attributes;
        }
    }
}