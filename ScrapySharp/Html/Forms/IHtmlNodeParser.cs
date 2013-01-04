using System.Collections.Generic;
using ScrapySharp.Html.Dom;

namespace ScrapySharp.Html.Forms
{
    internal interface IHtmlNodeParser<TNode>
    {
        IEnumerable<IHtmlNodeParser<TNode>> CssSelect(string selector);
        string GetAttributeValue(string name);
        HAttibutesCollection Attributes { get; }
        string InnerText { get; }
    }
}