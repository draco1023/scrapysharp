using System.Collections.Generic;
using System.Linq;
using ScrapySharp.Core;
using ScrapySharp.Html.Dom;

namespace ScrapySharp.Extensions
{
    public static class HDocumentCssQueryExtensions
    {
        

        public static IEnumerable<HElement> CssSelect(this HDocument doc, string expression)
        {
            return doc.Children.CssSelect(expression);
        }

        public static IEnumerable<HElement> CssSelect(this IEnumerable<HElement> nodes, string expression)
        {
            return nodes.SelectMany(node => CssSelect(node, expression));
        }

        public static IEnumerable<HElement> CssSelectAncestors(this IEnumerable<HElement> nodes, string expression)
        {
            return nodes.SelectMany(node => CssSelectAncestors(node, expression)).Distinct();
        }

        public static IEnumerable<HElement> CssSelectAncestors(this HElement node, string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return new HElement[0];

            var tokenizer = new CssSelectorTokenizer();
            var tokens = tokenizer.Tokenize(expression);
            var executor = new CssSelectorExecutor<HValue>(new List<HValue> { node }, tokens.ToList(), new HValueNavigationProvider());
            executor.MatchAncestors = true;

            return executor.GetElements().AsHElements();
        }

        public static IEnumerable<HElement> CssSelect(this HElement node, string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return new HElement[0];

            var tokenizer = new CssSelectorTokenizer();
            var tokens = tokenizer.Tokenize(expression);
            var executor = new CssSelectorExecutor<HValue>(new List<HValue> { node }, tokens.ToList(), new HValueNavigationProvider());

            return executor.GetElements().AsHElements();
        }
    }
}