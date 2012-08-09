namespace ScrapySharp.Html.Dom
{
    public class HComment : HElement
    {
        public override string OuterHtml
        {
            get { return "<--" + InnerText + "-->"; }
        }
    }
}