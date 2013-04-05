using System.Collections.Generic;
using System.Linq;
using ScrapySharp.Html.Dom;

namespace ScrapySharp.Extensions
{
    public static class CastHelper
    {
        public static IEnumerable<HElement> AsHElements(this IEnumerable<HValue> hValues)
        {
            return hValues.Select(v => (HElement) v);
        }

        public static IEnumerable<HValue> AsHValues(this IEnumerable<HElement> elements)
        {
            return elements.Select(e => (HValue) e);
        }
    }
}