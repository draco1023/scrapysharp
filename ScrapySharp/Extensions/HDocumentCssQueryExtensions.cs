using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ScrapySharp.Html.Dom;

namespace ScrapySharp.Extensions
{
    public static class HDocumentCssQueryExtensions
    {
        private const string PatternId = @"[#](?<id>[\w-_]+)";
        private const string PatternAttribute = @"\[(?<name>\w+)(\^|\$)?=(?<value>[\w/-\\+]+)\]";
        private static readonly Regex regexId = new Regex(PatternId, RegexOptions.Compiled);
        private static readonly Regex regexAttribute = new Regex(PatternAttribute, RegexOptions.Compiled);


        public static IEnumerable<HElement> CssSelect(this HDocument doc, string expression)
        {
            return doc.Children.CssSelect(expression);
        }

        public static IEnumerable<HElement> CssSelect(this IEnumerable<HElement> nodes, string expression)
        {
            return nodes.SelectMany(node => CssSelect((HElement)node, expression));
        }

        public static IEnumerable<HElement> CssSelectAncestors(this IEnumerable<HElement> nodes, string expression)
        {
            return nodes.SelectMany(node => CssSelectAncestors(node, expression)).Distinct();
        }

        public static IEnumerable<HElement> CssSelectAncestors(this HElement node, string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return new HElement[] { };

            if (expression.Contains(">"))
            {
                return MatchDirectAncestors(node, expression);
            }

            return CssSelect(node, expression, false, true);
        }

        public static IEnumerable<HElement> CssSelect(this HElement node, string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return new HElement[] { };

            if (expression.Contains(">"))
            {
                return MatchDirectDescendants(node, expression);
            }

            return CssSelect(node, expression, false, false);
        }

        private static IEnumerable<HElement> CssSelect(HElement node, string expression, bool directDescendants, bool matchAncestors)
        {
            var selectors = expression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var selector = selectors.First();
            var htmlNodes = Match(node, selector, directDescendants, matchAncestors);

            if (selectors.Length == 1)
                return htmlNodes;

            var descendantSelector = string.Join(" ", selectors.Skip(selectors.Length - 1).ToArray());

            return htmlNodes.SelectMany(n => CssSelect(n, descendantSelector, false, matchAncestors));
        }

        private static IEnumerable<HElement> MatchDirectAncestors(HElement node, string expression)
        {
            var selectors = expression.Split(new[] { '>' }, StringSplitOptions.RemoveEmptyEntries);
            var parents = CssSelect(node, selectors[0], false, true).ToArray();

            for (int i = 1; i < selectors.Length; i++)
            {
                parents = parents.SelectMany(p => CssSelect(p, selectors[i], true, true)).ToArray();
            }

            return parents;
        }

        private static IEnumerable<HElement> MatchDirectDescendants(HElement node, string expression)
        {
            var selectors = expression.Split(new[] { '>' }, StringSplitOptions.RemoveEmptyEntries);
            var parents = CssSelect(node, selectors[0], false, false).ToArray();

            for (int i = 1; i < selectors.Length; i++)
            {
                parents = parents.SelectMany(p => CssSelect(p, selectors[i], true, false)).ToArray();
            }

            return parents;
        }

        private static IEnumerable<HElement> Match(HElement node, string selector, bool directDescendants = false, bool matchAncestors = false)
        {
            if (selector.Contains('[') && selector.Contains(']'))
                return MatchAttributeSelector(node, selector, directDescendants, matchAncestors);

            if (selector.Contains("#"))
                return MatchIdSelector(node, selector, directDescendants, matchAncestors);

            if (selector.Contains("."))
                return GetElementsByClasses(node, selector, directDescendants, matchAncestors);

            if (matchAncestors)
                return directDescendants ? (node.ParentNode.Name == selector ? new[] { node.ParentNode } : new HElement[] { }) : node.Ancestors(selector);

            return directDescendants ? node.Elements(selector) : node.Descendants(selector);
        }

        private static IEnumerable<HElement> MatchAttributeSelector(HElement node, string selector, bool directDescendants,
                                                                    bool matchAncestors)
        {
            var match = regexAttribute.Match(selector);
            if (!match.Success || !match.Groups["name"].Success || !match.Groups["value"].Success)
                throw new FormatException("Invalid css selector: '" + selector + "'");

            var startsWith = selector.Contains("^");
            var endsWith = selector.Contains("$");

            var name = match.Groups["name"].Value;
            var value = match.Groups["value"].Value;

            var unlessAttributeSelector = regexAttribute.Replace(selector, string.Empty);
            var nodes = Match(node, unlessAttributeSelector, directDescendants, matchAncestors);


            return from n in nodes
                   let attributeValue = n.GetAttributeValue(name, string.Empty)
                   where
                       startsWith
                           ? attributeValue.StartsWith(value)
                           : endsWith ? attributeValue.EndsWith(value) : attributeValue == value
                   select n;
        }

        private static IEnumerable<HElement> MatchIdSelector(HElement node, string selector, bool directDescendants, bool matchAncestors)
        {
            var match = regexId.Match(selector);
            if (!match.Success || !match.Groups["id"].Success)
                throw new FormatException("Invalid css selector: '" + selector + "'");

            var id = match.Groups["id"].Value;

            var unlessIdSelector = regexId.Replace(selector, string.Empty);
            if (string.IsNullOrEmpty(unlessIdSelector.Trim()))
                return GetDescendantsById(node, id);

            IEnumerable<HElement> matched;
            if (!matchAncestors)
                matched = !selector.Contains(".")
                              ? (directDescendants ? node.Elements(unlessIdSelector) : node.Descendants(unlessIdSelector))
                              : GetElementsByClasses(node, unlessIdSelector, directDescendants);
            else
            {
                matched = MatchAncestors(node, selector, directDescendants, unlessIdSelector);
            }

            return matched.Where(m => m.Id == id);
        }

        private static IEnumerable<HElement> MatchAncestors(HElement node, string selector, bool directAncestor, string unlessIdSelector)
        {
            if (!selector.Contains("."))
            {
                if (directAncestor)
                {
                    if (unlessIdSelector == node.ParentNode.Name)
                        return new[] { node.ParentNode };
                    return new HElement[] { };
                }
                return node.Ancestors(unlessIdSelector);
            }

            return GetElementsByClasses(node, unlessIdSelector, directAncestor, true);
        }

        public static IEnumerable<HElement> GetDescendantsById(this HContainer node, string id)
        {
            return from n in node.Descendants()
                   where n.Id == id
                   select n;
        }

        private static IEnumerable<HElement> GetElementsByClasses(HElement node, string selector, bool directDescendants, bool matchAncestors = false)
        {
            if (!selector.Contains("."))
                return new HElement[] { };

            var parts = selector.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            var tag = parts.First();
            var cssClasses = parts.Skip(1);

            IEnumerable<HElement> descendants;
            if (!matchAncestors)
                descendants = directDescendants ? node.Elements(tag) : node.Descendants(tag);
            else
                descendants = directDescendants ? (node.ParentNode.Name == tag ? new[] { node.ParentNode } : new HElement[] { }) : node.Ancestors(tag);

            return from d in descendants
                   let @class = d.GetAttributeValue("class", string.Empty).Trim()
                   where !string.IsNullOrEmpty(@class)
                   let styles = @class.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                   where cssClasses.All(styles.Contains)
                   select d;
        }
    }
}