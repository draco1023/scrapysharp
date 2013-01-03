using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using HtmlAgilityPack;
using ScrapySharp.Html.Dom;
using ScrapySharp.Network;

namespace ScrapySharp.Html.Forms
{
    public class WebForm
    {
        public WebForm()
        {
            FormFields = new List<FormField>();
        }

        public WebForm(HtmlNode html)
        {
            FormFields = AgilityFormParser.ParseFormFields(html);
        }

        public WebForm(HDocument html)
        {
            FormFields = HDocumentFormParser.ParseFormFields(html);
        }

        public WebForm(HElement html)
        {
            FormFields = HElementFormParser.ParseFormFields(html);
        }

        public List<FormField> FormFields { get; set; }

        public string SerializeFormFields()
        {
            var builder = new StringBuilder();
            var fields = FormFields.ToArray();

            for (int i = 0; i < fields.Length; i++)
            {
                if (i > 0)
                    builder.Append('&');
                builder.AppendFormat("{0}={1}", HttpUtility.UrlEncode(fields[i].Name), HttpUtility.UrlEncode(fields[i].Value));
            }

            return builder.ToString();
        }

        public string this[string key]
        {
            get
            {
                var field = FormFields.FirstOrDefault(f => f.Name == key);
                return field != null ? field.Value : null;
            }
            set
            {
                var field = FormFields.FirstOrDefault(f => f.Name == key);
                if (field != null)
                    FormFields.Remove(field);

                FormFields.Add(new FormField { Name = key, Value = value });
            }
        }

        public void Submit(ScrapingBrowser browser, Uri url, HttpVerb verb = HttpVerb.Post)
        {
            browser.NavigateTo(url, verb, SerializeFormFields());
        }
    }
}