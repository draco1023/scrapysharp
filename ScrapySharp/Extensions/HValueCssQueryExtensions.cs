using System.Collections.Generic;
using System.Linq;
using ScrapySharp.Core;
using ScrapySharp.Html.Dom;

namespace ScrapySharp.Extensions
{
    public static class HValueCssQueryExtensions
    {
        public static IEnumerable<T> CssSelectValue<T>(this HDocument doc, string expression)
        {
            return doc.Children.CssSelectValue<T>(expression);
        }

        public static IEnumerable<T> CssSelectValue<T>(this IEnumerable<HElement> nodes, string expression)
        {
            return nodes.SelectMany(node => CssSelectValue<T>(node, expression));
        }

        public static IEnumerable<T> CssSelectAncestorsValues<T>(this IEnumerable<HElement> nodes, string expression)
        {
            return nodes.SelectMany(node => CssSelectAncestorsValues<T>(node, expression)).Distinct();
        }

        public static IEnumerable<T> CssSelectAncestorsValues<T>(this HElement node, string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return new T[0];

            var tokenizer = new CssSelectorTokenizer();
            var tokens = tokenizer.Tokenize(expression);
            var executor = new CssSelectorExecutor<HValue>(new List<HValue> { node }, tokens.ToList(), new HValueNavigationProvider());
            executor.MatchAncestors = true;

            return executor.GetElements().Select(v => HValue.ConvertValue<T>(v));
        }

        public static IEnumerable<T> CssSelectValue<T>(this HElement node, string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return new T[0];

            var tokenizer = new CssSelectorTokenizer();
            var tokens = tokenizer.Tokenize(expression);
            var executor = new CssSelectorExecutor<HValue>(new List<HValue> { node }, tokens.ToList(), new HValueNavigationProvider());

            return executor.GetElements().Select(v => HValue.ConvertValue<T>(v));
        }
    }
}