using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using ScrapySharp.Core;

namespace ScrapySharp.Extensions
{
    public static class CssQueryExtensions
    {
        public static IEnumerable<HtmlNode> CssSelect2(this IEnumerable<HtmlNode> nodes, string expression)
        {
            return nodes.SelectMany(node => CssSelect2(node, expression));
        }

        public static IEnumerable<HtmlNode> CssSelect2(this HtmlNode node, string expression)
        {
            var tokenizer = new CssSelectorTokenizer();
            var tokens = tokenizer.Tokenize(expression);
            var executor = new CssSelectorExecutor(node.ChildNodes.ToList(), tokens.ToList());
            
            return executor.GetElements();
        }

        public static IEnumerable<HtmlNode> CssSelectAncestors2(this IEnumerable<HtmlNode> nodes, string expression)
        {
            var htmlNodes = nodes.SelectMany(node => CssSelectAncestors2(node, expression)).ToArray();
            return htmlNodes.Distinct();
        }

        public static IEnumerable<HtmlNode> CssSelectAncestors2(this HtmlNode node, string expression)
        {
            var tokenizer = new CssSelectorTokenizer();
            var tokens = tokenizer.Tokenize(expression);
            var executor = new CssSelectorExecutor(new List<HtmlNode> { node }, tokens.ToList());
            executor.MatchAncestors = true;

            return executor.GetElements();
        }

    }
}