using System.Collections.Generic;
using System.Linq;
using ScrapySharp.Core;
using ScrapySharp.Html.Dom;

namespace ScrapySharp.Extensions
{
    public class HElementNavigationProvider : INavigationProvider<HElement>
    {
        public List<HElement> ChildNodes(List<HElement> nodes)
        {
            return nodes.SelectMany(n => n.Children).ToList();
        }

        public List<HElement> Descendants(List<HElement> nodes)
        {
            return nodes.SelectMany(n => n.Descendants()).ToList();
        }

        public List<HElement> ParentNodes(List<HElement> nodes)
        {
            return nodes.Select(n => n.ParentNode).ToList();
        }

        public List<HElement> AncestorsAndSelf(List<HElement> nodes)
        {
            return nodes.SelectMany(n => n.Ancestors()).Concat(nodes).ToList();
        }
    }
}