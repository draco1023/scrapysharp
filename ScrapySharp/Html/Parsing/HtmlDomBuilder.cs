using System.Collections.Generic;
using System.Globalization;
using ScrapySharp.Html.Dom;
using System.Linq;

namespace ScrapySharp.Html.Parsing
{
    public class HtmlDomBuilder
    {
        private CodeReadingContext context;
        private List<Word> words;
        private int position = 0;

        public HtmlDomBuilder(CodeReader reader)
        {
            context = CodeReadingContext.None;
            words = new List<Word>();

            while (!reader.End)
            {
                var w = reader.ReadWord();
                words.Add(w);
            }
        }

        public bool End
        {
            get { return position >= words.Count - 1; }
        }

        public HtmlElement ReadHtmlElement()
        {
            var w = ReadWord();

            if (!Istoken(w))
                return new HtmlElement
                           {
                               InnerText = w.Value,
                               Words = new []{w}.ToList()
                           };

            if (w == Tokens.TagBegin && !GetNextWord().IsWhiteSpace)
            {
                var expression = new List<Word>();
                
                do
                {
                    expression.Add(w);
                    w = ReadWord();
                } while (!End && w != Tokens.TagBegin && w != Tokens.TagEnd);

                expression.Add(w);

                return new HtmlElement
                           {
                               Words = expression
                           };
            }

            return null;
        }

        private bool Istoken(Word word)
        {
            return word.Value == Tokens.CloseTag ||
                   word.Value == Tokens.CommentBegin ||
                   word.Value == Tokens.CommentEnd ||
                   word.Value == Tokens.Quote.ToString(CultureInfo.InvariantCulture) ||
                   word.Value == Tokens.SimpleQuote.ToString(CultureInfo.InvariantCulture) ||
                   word.Value == Tokens.TagBegin.ToString(CultureInfo.InvariantCulture) ||
                   word.Value == Tokens.TagEnd.ToString(CultureInfo.InvariantCulture);
        }

        public Word GetNextWord()
        {
            if (position >= words.Count)
                return null;
            return words[position];
        }

        public Word GetPreviousChar()
        {
            if (position <= 1)
                return null;
            return words[position - 2];
        }

        public Word ReadWord()
        {
            return End ? null : words[position++];
        }
    }
}