using System.Globalization;
using System.Text;
using ScrapySharp.Extensions;

namespace ScrapySharp.Html.Parsing
{
    public class CodeReader
    {
        private readonly string sourceCode;
        private readonly StringBuilder buffer;
        //private int position;
        private int currentPosition;
        private CodeReadingContext context;

        private int lineNumber = 1;
        private int linePosition = 1;
        private readonly int sourceCodeLength = 0;
        public bool end = false;

        public CodeReader(string sourceCode)
        {
            if (sourceCode.EndsWith("\n"))
                this.sourceCode = sourceCode;
            else
                this.sourceCode = sourceCode + "\n";

            buffer = new StringBuilder();
            context = CodeReadingContext.None;
            sourceCodeLength = this.sourceCode.Length;
        }

        //public int Position
        //{
        //    get { return currentPosition; }
        //    set
        //    {
        //        currentPosition = value;
        //        end = currentPosition >= sourceCode.Length;
        //    }
        //}

        public void SetPosition(int value)
        {
            currentPosition = value;
            end = currentPosition >= sourceCodeLength;
        }

        public int MaxWordCount
        {
            get { return sourceCodeLength; }
        }

        public Word ReadWord()
        {
            buffer.Remove(0, buffer.Length);
            var c = ReadChar();
            
            if (char.IsWhiteSpace(c))
                return new Word(c.ToString(CultureInfo.InvariantCulture), lineNumber, linePosition, false);

            if (context != CodeReadingContext.InQuotes && (c == Tokens.Quote || c == Tokens.SimpleQuote))
            {
                context = CodeReadingContext.InQuotes;
                return ReadQuotedString(c);
            }

            buffer.Append(c);

            var letterOrDigit = IsLetterOrDigit(c);

            while (IsLetterOrDigit(GetNextChar()) == letterOrDigit && !char.IsWhiteSpace(GetNextChar()) && !GetNextChar().IsToken())
            {
                c = ReadChar();
                if (c == Tokens.Quote)
                {
                    //Position--;
                    currentPosition--;
                    end = false;
                    break;
                }

                buffer.Append(c);
            }

            return new Word(buffer.ToString(), lineNumber, linePosition, false);
        }

        private Word ReadQuotedString(char quoteChar)
        {
            var c = ReadChar();
            
            while (!end && context == CodeReadingContext.InQuotes)
            {
                if (c == quoteChar)
                    break;

                var nextChar = GetNextChar();
                if (nextChar == Tokens.TagBegin || nextChar == Tokens.TagEnd)
                    break;

                buffer.Append(c);

                if (c == Tokens.TagBegin || c == Tokens.TagEnd)
                    break;

                c = ReadChar();
            }

            context = CodeReadingContext.None;

            return new Word(buffer.ToString(), lineNumber, linePosition, true);
        }


        public char GetNextChar()
        {
            if (end)
                return (char)0;
            return sourceCode[currentPosition];
        }

        public char GetPreviousChar()
        {
            if (currentPosition <= 1)
                return (char)0;
            return sourceCode[currentPosition - 2];
        }

        public char ReadChar()
        {
            if (end)
                return (char)0;
            var c = sourceCode[currentPosition];
            SetPosition(currentPosition+1);
            linePosition++;

            if (c == '\n')
            {
                lineNumber++;
                linePosition = 1;
            }

            return c;
        }
        
        //public bool End
        //{
        //    get { return position >= sourceCode.Length; }
        //}

        public int LineNumber
        {
            get { return lineNumber; }
        }

        public int LinePosition
        {
            get { return linePosition; }
        }

        public bool IsLetterOrDigit(char c)
        {
            if (c >= 'a' && c <= 'z')
                return true;
            if (c >= 'A' && c <= 'Z')
                return true;
            if (c >= '0' && c <= '9')
                return true;

            return char.IsLetterOrDigit(c) || c == '-' || c == '_'
                || c == ':' || c == ';' || c == '+';
        }
    }
}