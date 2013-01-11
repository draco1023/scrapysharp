using System.Globalization;
using System.Text;
using ScrapySharp.Extensions;

namespace ScrapySharp.Html.Parsing
{
    public class CodeReader
    {
        private readonly string sourceCode;
        private readonly StringBuilder buffer;
        private int currentPosition;
        private CodeReadingContext context;

        private int lineNumber = 1;
        private int linePosition = 1;
        private readonly int sourceCodeLength = 0;
        public bool end = false;

        private char currentChar;

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

            currentChar = ReadChar();
            
            if (char.IsWhiteSpace(currentChar))
                return new Word(currentChar.ToString(CultureInfo.InvariantCulture), lineNumber, linePosition, false);

            if (context != CodeReadingContext.InQuotes && (currentChar == Tokens.Quote || currentChar == Tokens.SimpleQuote))
            {
                context = CodeReadingContext.InQuotes;
                return ReadQuotedString(currentChar);
            }

            buffer.Append(currentChar);

            var letterOrDigit = IsLetterOrDigit(currentChar);

            while (IsLetterOrDigit(GetNextChar()) == letterOrDigit && !char.IsWhiteSpace(GetNextChar()) && !GetNextChar().IsToken())
            {
                currentChar = ReadChar();
                if (currentChar == Tokens.Quote)
                {
                    currentPosition--;
                    end = false;
                    break;
                }

                buffer.Append(currentChar);
            }

            return new Word(buffer.ToString(), lineNumber, linePosition, false);
        }

        private Word ReadQuotedString(char quoteChar)
        {
            currentChar = ReadChar();
            
            while (!end && context == CodeReadingContext.InQuotes)
            {
                if (currentChar == quoteChar)
                    break;

                var nextChar = GetNextChar();
                if (nextChar == Tokens.TagBegin || nextChar == Tokens.TagEnd)
                    break;

                buffer.Append(currentChar);

                if (currentChar == Tokens.TagBegin || currentChar == Tokens.TagEnd)
                    break;

                currentChar = ReadChar();
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
            return char.IsDigit(c) || char.IsLetter(c) || c == '-' || c == '_'
                || c == ':' || c == ';' || c == '+';
        }
    }
}