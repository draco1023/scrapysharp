using System;
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

            SkipSpaces = false;

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

        public TagDeclaration ReadTagDeclaration()
        {
            var w = ReadWord();

            if (Istoken(w) && w == Tokens.TagBegin && !GetNextWord().IsWhiteSpace)
            {
                var element = new TagDeclaration
                {
                    Words = new List<Word> {w},
                    Name = ReadWord(),
                    Attributes = new Dictionary<string, string>()
                };

                do
                {
                    SkipSpaces = true;
                    element.Words.Add(w);
                    
                    w = ReadWord();
                    if (IsTagDeclarationEnd(w))
                        break;
                    var attributeName = w.Value;
                    w = ReadWord();
                    if (IsTagDeclarationEnd(w))
                        break;
                    if (w.Value == Tokens.Assign)
                    {
                        w = ReadWord();
                        if (IsTagDeclarationEnd(w))
                            break;
                        element.Attributes.Add(attributeName, w.Value);
                    }

                } while (!End && w != Tokens.TagBegin && w != Tokens.TagEnd);

                SkipSpaces = false;

                element.Words.Add(w);

                element.Type = GetDeclarationType(element.Words);

                return element;
            }
            
            return ReadTextElement(w);
        }

        private DeclarationType GetDeclarationType(List<Word> wordList)
        {
            if (wordList.Count < 3)
                return DeclarationType.TextElement;

            if (wordList.Last() != Tokens.TagEnd)
                return DeclarationType.TextElement;

            if (wordList[0] == Tokens.TagBegin)
            {
                if (wordList[1] == Tokens.CloseTag)
                    return DeclarationType.CloseTag;

                if (wordList[wordList.Count - 2] == Tokens.CloseTag)
                    return DeclarationType.SelfClosedTag;

                return DeclarationType.OpenTag;
            }

            return DeclarationType.TextElement;
        }

        private TagDeclaration ReadTextElement(Word word)
        {
            var wordList = new List<Word>();
            var w = word;

            do
            {
                wordList.Add(w);
                w = ReadWord();
            } while (!End && GetNextWord() != Tokens.TagBegin && GetNextWord() != Tokens.TagEnd);

            wordList.Add(w);


            return new TagDeclaration
                       {
                           InnerText = string.Join(string.Empty, wordList.Select(i => i.Value)),
                           Words = wordList,
                           Type = DeclarationType.TextElement
                       };
        }
        
        private bool IsTagDeclarationEnd(Word w)
        {
            return End || w == Tokens.TagBegin || w == Tokens.TagEnd;
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
            if (SkipSpaces)
            {
                while (!End)
                {
                    var w = words[position++];
                    if (!w.IsWhiteSpace)
                        return w;
                }
            }

            return End ? null : words[position++];
        }

        public bool SkipSpaces { get; set; }
    }
}