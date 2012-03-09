using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScrapySharp.Utilities
{
    public static class UrlUtility
    {
        private const string SafeChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!()*-._";
        private const string Hexa = "0123456789ABCDEFabcdef";

        public static bool IsEncoded(string value)
        {
            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (c == '%')
                {
                    if (i + 2 >= value.Length || !Hexa.Contains(value[i + 1]) || !Hexa.Contains(value[i + 2]))
                        return false;
                    i += 2;
                    continue;
                }
                if (!SafeChars.Contains(c))
                    return false;
            }
            return true;
        }

        public static string Encode(string value)
        {
            if (IsEncoded(value))
                return value;

            var bytes = Encoding.UTF8.GetBytes(value);

            return
                bytes.Aggregate(string.Empty,
                                (acc, cur) =>
                                acc +
                                (SafeChars.Contains((char)cur)
                                     ? ((char)cur).ToString()
                                     : string.Format("%{0:x2}", cur).ToUpper()));
        }

        public static string Decode(string value)
        {
            var decoded = string.Empty;

            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (c == '%' && (i + 2 < value.Length && Hexa.Contains(value[i + 1]) && Hexa.Contains(value[i + 2])))
                {
                    var left = Hexa.IndexOf(value[i + 1].ToString().ToUpper());
                    var right = Hexa.IndexOf(value[i + 2].ToString().ToUpper());

                    decoded += (char)((left << 4) | right);
                    i += 2;
                    continue;
                }
                decoded += c;
            }

            return Encoding.UTF8.GetString(decoded.Select(c => (byte)c).ToArray());
        }

        public static Uri UrlEncodeQueryStringValues(Uri uri)
        {
            if (uri.Query.Length == 0)
                return uri;

            var values =
                DeserializeQuery(uri.Query[0] == '?' ? uri.Query.Substring(1) : uri.Query).Select(
                    kvp => IsEncoded(kvp.Value) ? kvp : new KeyValuePair<string, string>(kvp.Key, Encode(kvp.Value)));


            return
                new Uri(string.Format("{0}://{1}{2}{3}?{4}", uri.Scheme, uri.Host,
                                      uri.Scheme == "http" && uri.Port == 80 || uri.Scheme == "https" && uri.Port == 443
                                          ? ""
                                          : ":" + uri.Port, uri.AbsolutePath, SerializeQuery(values)));
        }

        public static string SerializeQuery(IEnumerable<KeyValuePair<string, string>> values)
        {
            return values.Aggregate(string.Empty,
                                    (acc, cur) => acc + (acc.Length == 0 ? "" : "&") + cur.Key + '=' + cur.Value);
        }

        public static IEnumerable<KeyValuePair<string, string>> DeserializeQuery(string value)
        {
            return (from split in value.Split(new[] {'&'}, StringSplitOptions.RemoveEmptyEntries)
                    let kvp = split.Split(new[] {'='}, 2, StringSplitOptions.RemoveEmptyEntries).ToArray()
                    select new KeyValuePair<string, string>(kvp[0], kvp[1]));
        }
    }
}
