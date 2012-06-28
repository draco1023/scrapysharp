namespace ScrapySharp.Html
{
    public enum CodeReadingContext
    {
        None,
        SearchingTag,
        InBeginTag,
        InTagContent,
        InTagEnd,
        InAttributeName,
        InAttributeValue,
        InQuotes
    }
}