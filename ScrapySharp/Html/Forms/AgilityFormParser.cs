using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace ScrapySharp.Html.Forms
{
    public class AgilityFormParser
    {
        public static List<FormField> ParseFormFields(HtmlNode html)
        {
            var hidden = from input in html.CssSelect("input")
                         let value = input.GetAttributeValue("value")
                         select new FormField
                             {
                                 Name = input.GetAttributeValue("name"),
                                 Value = string.IsNullOrEmpty(value) ? input.InnerText : value
                             };

            var selects = from @select in html.CssSelect("select")
                          let name = @select.GetAttributeValue("name")
                          let option =
                              @select.CssSelect("option").FirstOrDefault(o => o.Attributes["selected"] != null) ??
                              @select.CssSelect("option").FirstOrDefault()
                          let value = option.GetAttributeValue("value")
                          select new FormField
                              {
                                  Name = name,
                                  Value = string.IsNullOrEmpty(value) ? option.InnerText : value
                              };

            return hidden.Concat(selects).ToList();
        }
    }
}