using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;
using System.Web;
using HtmlAgilityPack;

namespace ScrapySharp.Extensions
{
    public static class HtmlParsingHelper
    {
        /// <summary>
        /// Convert a string to a date.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static DateTime ToDate(this string value)
        {
            return Convert.ToDateTime(value);
        }

        /// <summary>
        /// Convert a string to a date.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static DateTime ToDate(this string value, string format)
        {
            return ToDate(value, format, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convert a string to a date.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="format">The format.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns></returns>
        public static DateTime ToDate(this string value, string format, CultureInfo cultureInfo)
        {
            DateTime result;
            if (DateTime.TryParseExact(value, format, cultureInfo, DateTimeStyles.None, out result))
                return result;

            return DateTime.MinValue;
        }

        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string GetAttributeValue(this HtmlNode node, string name)
        {
            return node.GetAttributeValue(name, string.Empty);
        }

        /// <summary>
        /// Convert string value to HTML node.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static HtmlNode ToHtmlNode(this string content)
        {
            var document = new HtmlDocument();
            document.LoadHtml(content);

            return document.DocumentNode;
        }

        /// <summary>
        /// Gets the next sibling with specified tag name.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static HtmlNode GetNextSibling(this HtmlNode node, string name)
        {
            var currentNode = node.NextSibling;

            while (currentNode.NextSibling != null && currentNode.Name != name)
                currentNode = currentNode.NextSibling;

            return currentNode.Name == name ? currentNode : null;
        }

        /// <summary>
        /// Gets the next table cell value.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="name">The name.</param>
        /// <param name="comparison">The comparison type.</param>
        /// <returns></returns>
        public static HtmlValue GetNextTableCellValue(this HtmlNode node, string name)
        {
            var results = GetNodesFollowedByValue(node, "td", name, NodeValueComparison.Equals);
            if (!results.Any())
                return null;

            var innerText = results.LastOrDefault().InnerText.CleanInnerHtmlAscii().CleanInnerText();
            if (innerText.StartsWith(":"))
                innerText = innerText.Substring(1).CleanInnerHtmlAscii().CleanInnerText();

            return innerText;
        }

        /// <summary>
        /// Gets the next table cell value.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="name">The name.</param>
        /// <param name="comparison">The comparison type.</param>
        /// <returns></returns>
        public static HtmlValue GetNextTableCellValue(this HtmlNode node, string name, NodeValueComparison comparison/* = NodeValueComparison.Equals*/)
        {
            var results = GetNodesFollowedByValue(node, "td", name, comparison);
            if (!results.Any())
                return null;

            var innerText = results.LastOrDefault().InnerText.CleanInnerHtmlAscii().CleanInnerText();
            if (innerText.StartsWith(":"))
                innerText = innerText.Substring(1).CleanInnerHtmlAscii().CleanInnerText();

            return innerText;
        }

        /// <summary>
        /// Gets the nodes followed by value.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="comparison">The comparison.</param>
        /// <returns></returns>
        public static IEnumerable<HtmlNode> GetNodesFollowedByValue(this HtmlNode node, string name, string value, NodeValueComparison comparison = NodeValueComparison.Equals)
        {
            var comparer = new NodeValueComparer(comparison);
            var cleanName = value.CleanInnerText();
            return (from d in node.Descendants(name)
                    where comparer.Compare(d.InnerText.CleanInnerHtmlAscii().CleanInnerText(), cleanName)
                    select d.GetNextSibling(name)).ToArray();
        }

        /// <summary>
        /// Gets the next table line value.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="name">The name.</param>
        /// <param name="comparison">The comparison type.</param>
        /// <returns></returns>
        public static HtmlValue GetNextTableLineValue(this HtmlNode node, string name, NodeValueComparison comparison = NodeValueComparison.Equals)
        {
            var results = GetNodesFollowedByValue(node, "tr", name, comparison);
            if (!results.Any())
                return null;

            var innerText = results.FirstOrDefault().InnerText.CleanInnerHtmlAscii().CleanInnerText();
            if (innerText.StartsWith(":"))
                innerText = innerText.Substring(1).CleanInnerHtmlAscii().CleanInnerText();

            return innerText;
        }

        /// <summary>
        /// Cleans the inner HTML ASCII.
        /// </summary>
        /// <example>
        /// "text =09".CleanInnerHtmlAscii() returns "text "
        /// </example>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static string CleanInnerHtmlAscii(this string expression)
        {
            var cleaned = expression.Replace("=C3=B4", "ô");

            var regex = new Regex("(([=][0-9A-F]{0,2})+)|([ ]+)");

            cleaned = regex.Replace(cleaned, " ");

            return cleaned;
        }

        /// <summary>
        /// Cleans the inner text from excessive spaces characters.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static string CleanInnerText(this string expression)
        {
            var cleaned = expression.Replace('\t', ' ').Replace('\r', ' ')
                .Replace('\n', ' ');

            cleaned = HttpUtility.HtmlDecode(cleaned);
            
            var regex = new Regex("[ ]+");

            return regex.Replace(cleaned, " ").Trim();
        }

    }
}