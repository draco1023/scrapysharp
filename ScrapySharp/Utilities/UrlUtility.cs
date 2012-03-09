using System.Linq;

namespace ScrapySharp.Utilities
{
    public static class UrlUtility
    {
        private const string UrlEncodeUnreservedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
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
                if (!UrlEncodeUnreservedChars.Contains(c))
                    return false;
            }
            return true;
        }

        public static string Encode(string value)
        {
            return
                value.Aggregate(string.Empty,
                                (acc, cur) =>
                                acc +
                                (UrlEncodeUnreservedChars.Contains(cur)
                                     ? cur.ToString()
                                     : string.Format("%{0:x2}", (byte) cur).ToUpper()));
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

            return decoded;
        }
    }
}
