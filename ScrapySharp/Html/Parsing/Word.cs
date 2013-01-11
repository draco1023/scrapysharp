namespace ScrapySharp.Html.Parsing
{
    public class Word
    {
        private readonly string value;
        private readonly int lineNumber;
        private readonly int linePositionEnd;
        private readonly bool isQuoted;
        private readonly bool isWhiteSpace;
        private readonly char letter;

        private static readonly char[] whiteChars = new [] {' ', '\r', '\n', '\t'};

        public Word(string value, int lineNumber, int linePositionEnd, bool isQuoted)
        {
            this.value = value;
            this.lineNumber = lineNumber;
            this.linePositionEnd = linePositionEnd;
            this.isQuoted = isQuoted;

            isWhiteSpace = string.IsNullOrEmpty(value.Trim());
            //isWhiteSpace = value.IndexOfAny(whiteChars) > 0;
            
            letter = value.Length > 0 ? value[0] : '\0';
        }

        public string Value
        {
            get
            {
                return value;
            }
        }

        public string QuotedValue
        {
            get
            {
                if (IsQuoted)
                    return '"' + value + '"';

                return value;
            }
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

        public static implicit operator string(Word word)
        {
            return word.Value;
        }

        public static implicit operator char(Word word)
        {
            return word.letter;

            //if (string.IsNullOrEmpty(word.Value))
            //    return default(char);

            //return word.Value[0];
        }

        //public bool Equals(Word other)
        //{
        //    return string.Equals(value, other.value) && lineNumber == other.lineNumber && linePositionEnd == other.linePositionEnd && isQuoted.Equals(other.isQuoted) && isWhiteSpace.Equals(other.isWhiteSpace) && letter == other.letter;
        //}

        //public override bool Equals(object obj)
        //{
        //    if (ReferenceEquals(null, obj)) return false;
        //    return obj is Word && Equals((Word) obj);
        //}

        //public override int GetHashCode()
        //{
        //    unchecked
        //    {
        //        int hashCode = (value != null ? value.GetHashCode() : 0);
        //        hashCode = (hashCode*397) ^ lineNumber;
        //        hashCode = (hashCode*397) ^ linePositionEnd;
        //        hashCode = (hashCode*397) ^ isQuoted.GetHashCode();
        //        hashCode = (hashCode*397) ^ isWhiteSpace.GetHashCode();
        //        hashCode = (hashCode*397) ^ letter.GetHashCode();
        //        return hashCode;
        //    }
        //}

        //public static bool operator ==(Word w1, Word w2)
        //{
        //    return w1.Equals(w2);
        //}

        //public static bool operator !=(Word w1, Word w2)
        //{
        //    return !(w1 == w2);
        //}

        //public static bool IsNull(Word word)
        //{
        //    var word1 = default(Word);
        //    return word == word1;
        //}
    }
}