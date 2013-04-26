namespace ScrapySharp.JavaScript.Dom
{
    public class Window
    {
        private Document document;

        public Window(Document document)
        {
            this.document = document;
        }

        public DomElement CreateElement(string tagName)
        {
            return document.createElement(tagName);
        }
    }
}