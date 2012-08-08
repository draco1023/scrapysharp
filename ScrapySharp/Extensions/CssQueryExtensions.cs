using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using ScrapySharp.Core;

namespace ScrapySharp.Extensions
{
    public static class CssQueryExtensions
    {
        public static IEnumerable<HtmlNode> CssSelect2(this HtmlNode node, string expression)
        {
            var tokenizer = new CssSelectorTokenizer();
            var tokens = tokenizer.Tokenize(expression);
            var executor = new CssSelectorExecutor(node.ChildNodes.ToList(), tokens.ToList());
            
            return executor.GetElements();
        }
    }
}