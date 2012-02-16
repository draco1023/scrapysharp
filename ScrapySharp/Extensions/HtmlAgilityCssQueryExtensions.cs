using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace ScrapySharp.Extensions
{
    public static class HtmlAgilityCssQueryExtensions
    {
        private const string PatternId = @"[#](?<id>\w+)";
        private const string PatternAttribute = @"\[(?<name>\w+)(\^|\$)?=(?<value>[\w/-\\+]+)\]";
        private static readonly Regex regexId = new Regex(PatternId, RegexOptions.Compiled);
        private static readonly Regex regexAttribute = new Regex(PatternAttribute, RegexOptions.Compiled);

        public static IEnumerable<HtmlNode> CssSelect(this IEnumerable<HtmlNode> nodes, string expression)
        {
            return nodes.SelectMany(node => CssSelect(node, expression));
        }

        public static IEnumerable<HtmlNode> CssSelectAncestors(this IEnumerable<HtmlNode> nodes, string expression)
        {
            return nodes.SelectMany(node => CssSelectAncestors(node, expression)).Distinct();
        }

        public static IEnumerable<HtmlNode> CssSelectAncestors(this HtmlNode node, string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return new HtmlNode[] { };

            if (expression.Contains(">"))
            {
                return MatchDirectAncestors(node, expression);
            }

            return CssSelect(node, expression, false, true);
        }

        public static IEnumerable<HtmlNode> CssSelect(this HtmlNode node, string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return new HtmlNode[] { };

            if (expression.Contains(">"))
            {
                return MatchDirectDescendants(node, expression);
            }

            return CssSelect(node, expression, false, false);
        }

        private static IEnumerable<HtmlNode> CssSelect(HtmlNode node, string expression, bool directDescendants, bool matchAncestors)
        {
            var selectors = expression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var selector = selectors.First();
            var htmlNodes = Match(node, selector, directDescendants, matchAncestors);

            if (selectors.Length == 1)
                return htmlNodes;

            var descendantSelector = string.Join(" ", selectors.Skip(selectors.Length - 1).ToArray());

            return htmlNodes.SelectMany(n => CssSelect(n, descendantSelector, false, matchAncestors));
        }

        private static IEnumerable<HtmlNode> MatchDirectAncestors(HtmlNode node, string expression)
        {
            var selectors = expression.Split(new[] { '>' }, StringSplitOptions.RemoveEmptyEntries);
            var parents = CssSelect(node, selectors[0], false, true).ToArray();

            for (int i = 1; i < selectors.Length; i++)
            {
                parents = parents.SelectMany(p => CssSelect(p, selectors[i], true, true)).ToArray();
            }

            return parents;
        }

        private static IEnumerable<HtmlNode> MatchDirectDescendants(HtmlNode node, string expression)
        {
            var selectors = expression.Split(new[] { '>' }, StringSplitOptions.RemoveEmptyEntries);
            var parents = CssSelect(node, selectors[0], false, false).ToArray();

            for (int i = 1; i < selectors.Length; i++)
            {
                parents = parents.SelectMany(p => CssSelect(p, selectors[i], true, false)).ToArray();
            }

            return parents;
        }

        private static IEnumerable<HtmlNode> Match(HtmlNode node, string selector, bool directDescendants = false, bool matchAncestors = false)
        {
            if (selector.Contains('[') && selector.Contains(']'))
                return MatchAttributeSelector(node, selector, directDescendants, matchAncestors);

            if (selector.Contains("#"))
                return MatchIdSelector(node, selector, directDescendants, matchAncestors);

            if (selector.Contains("."))
                return GetElementsByClasses(node, selector, directDescendants, matchAncestors);

            if (matchAncestors)
                return directDescendants ? (node.ParentNode.Name == selector ? new[] { node.ParentNode } : new HtmlNode[] { }) : node.Ancestors(selector);

            return directDescendants ? node.Elements(selector) : node.Descendants(selector);
        }

        private static IEnumerable<HtmlNode> MatchAttributeSelector(HtmlNode node, string selector, bool directDescendants,
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

        private static IEnumerable<HtmlNode> MatchIdSelector(HtmlNode node, string selector, bool directDescendants, bool matchAncestors)
        {
            var match = regexId.Match(selector);
            if (!match.Success || !match.Groups["id"].Success)
                throw new FormatException("Invalid css selector: '" + selector + "'");

            var id = match.Groups["id"].Value;

            var unlessIdSelector = regexId.Replace(selector, string.Empty);
            if (string.IsNullOrEmpty(unlessIdSelector.Trim()))
                return GetDescendantsById(node, id);

            IEnumerable<HtmlNode> matched;
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

        private static IEnumerable<HtmlNode> MatchAncestors(HtmlNode node, string selector, bool directAncestor, string unlessIdSelector)
        {
            if (!selector.Contains("."))
            {
                if (directAncestor)
                {
                    if (unlessIdSelector == node.ParentNode.Name)
                        return new[] { node.ParentNode };
                    return new HtmlNode[] { };
                }
                return node.Ancestors(unlessIdSelector);
            }

            return GetElementsByClasses(node, unlessIdSelector, directAncestor, true);
        }

        public static IEnumerable<HtmlNode> GetDescendantsById(this HtmlNode node, string id)
        {
            return from n in node.Descendants()
                   where n.Id == id
                   select n;
        }

        private static IEnumerable<HtmlNode> GetElementsByClasses(HtmlNode node, string selector, bool directDescendants, bool matchAncestors = false)
        {
            if (!selector.Contains("."))
                return new HtmlNode[] { };

            var parts = selector.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            var tag = parts.First();
            var cssClasses = parts.Skip(1);

            IEnumerable<HtmlNode> descendants;
            if (!matchAncestors)
                descendants = directDescendants ? node.Elements(tag) : node.Descendants(tag);
            else
                descendants = directDescendants ? (node.ParentNode.Name == tag ? new[] { node.ParentNode } : new HtmlNode[] { }) : node.Ancestors(tag);

            return from d in descendants
                   let @class = d.GetAttributeValue("class", string.Empty).Trim()
                   where !string.IsNullOrEmpty(@class)
                   let styles = @class.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                   where cssClasses.All(styles.Contains)
                   select d;
        }
    }
}