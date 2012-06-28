using System.Linq;

namespace ScrapySharp.Html
{
    public class Word
    {
        private readonly string value;
        private readonly int lineNumber;
        private readonly int linePositionEnd;
        private readonly bool isQuoted;
        private readonly bool isWhiteSpace;

        public Word(string value, int lineNumber, int linePositionEnd, bool isQuoted)
        {
            this.value = value;
            this.lineNumber = lineNumber;
            this.linePositionEnd = linePositionEnd;
            this.isQuoted = isQuoted;

            isWhiteSpace = !string.IsNullOrEmpty(value) && value.All(char.IsWhiteSpace);
        }

        public string Value
        {
            get { return value; }
        }

        public int LineNumber
        {
            get { return lineNumber; }
        }

        public int LinePositionEnd
        {
            get { return linePositionEnd; }
        }

        public int LinePositionBegin
        {
            get { return linePositionEnd - value.Length; }
        }

        public bool IsQuoted
        {
            get { return isQuoted; }
        }

        public bool IsWhiteSpace
        {
            get { return isWhiteSpace; }
        }
    }
}