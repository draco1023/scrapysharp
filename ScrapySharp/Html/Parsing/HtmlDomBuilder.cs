using System.Collections.Generic;
using ScrapySharp.Html.Dom;
using System.Linq;

namespace ScrapySharp.Html.Parsing
{
    public class HtmlDomBuilder
    {
        private readonly List<TagDeclaration> tags;

        public HtmlDomBuilder(HtmlDeclarationReader reader)
        {
            tags = new List<TagDeclaration>();

            while (!reader.End)
            {
                var d = reader.ReadTagDeclaration();
                tags.Add(d);
            }
        }

        public IEnumerable<HElement> BuildDom(List<TagDeclaration> declarations)
        {
            int openning = 0;
            int closing = 0;

            for (int i = 0; i < declarations.Count; i++)
            {
                var declaration = declarations[i];

                if (declaration.Type == DeclarationType.OpenTag)
                {
                    openning = 1;
                    closing = 0;
                    var start = i++;

                    while (closing < openning && i < declarations.Count)
                    {
                        var current = declarations[i++];
                        if (current.Type == DeclarationType.CloseTag && current.Name == declaration.Name)
                            closing++;
                        if (current.Type == DeclarationType.OpenTag && current.Name == declaration.Name)
                            openning++;

                        if (openning == closing)
                        {
                            var childrenTags = declarations.Skip(start).Take(i - start).ToList();

                            yield return new HElement
                                             {
                                                 Name = declaration.Name,
                                                 Attributes = declaration.Attributes,
                                                 InnerText = declaration.InnerText,
                                                 //Children = declarations.Count > childrenTags.Count ? BuildDom(childrenTags).ToList() : new List<HElement>()
                                             };
                        }
                    }
                }

                if (declaration.Type == DeclarationType.TextElement || declaration.Type == DeclarationType.SelfClosedTag)
                    yield return new HElement
                    {
                        InnerText = declaration.InnerText,
                        Name = declaration.Name,
                        Attributes = declaration.Attributes
                    };
            }
        }

        public IEnumerable<HElement> BuildDom()
        {
            return BuildDom(tags);
        }
    }
}