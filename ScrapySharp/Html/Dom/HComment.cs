namespace ScrapySharp.Html.Dom
{
    public class HComment : HElement
    {
        public override string OuterHtml
        {
            get { return string.Format("<!--{0}-->", innerText); }
        }
    }
}