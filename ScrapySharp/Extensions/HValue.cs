using System;
using System.Globalization;
using ScrapySharp.Html.Dom;

namespace ScrapySharp.Extensions
{
    public class HValue : IEquatable<HValue>
    {
        private readonly object value;

        public HValue(object value)
        {
            this.value = value;
        }

        public object Value
        {
            get { return value; }
        }

        public static T ConvertValue<T>(HValue hValue)
        {
            var type = typeof (T);
            return (T) Convert.ChangeType(hValue, type);
        }

        #region Equality members

        public bool Equals(HValue other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(value, other.value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((HValue) obj);
        }

        public override int GetHashCode()
        {
            return (value != null ? value.GetHashCode() : 0);
        }

        #endregion

        public override string ToString()
        {
            return value == null ? string.Empty : value.ToString();
        }

        public static implicit operator string(HValue htmlValue)
        {
            if (htmlValue == null)
                return null;
            return htmlValue.ToString();
        }

        public static implicit operator HValue(string value)
        {
            return new HValue(value);
        }

        public static implicit operator HElement(HValue htmlValue)
        {
            if (htmlValue == null)
                return null;
            return (HElement) htmlValue.value;
        }

        public static implicit operator HValue(HElement element)
        {
            return new HValue(element);
        }

        public static implicit operator HContainer(HValue htmlValue)
        {
            if (htmlValue == null)
                return null;
            return (HContainer)htmlValue.value;
        }

        public static implicit operator HValue(HContainer element)
        {
            return new HValue(element);
        }
        
        public static explicit operator bool(HValue htmlValue)
        {
            if (htmlValue == null)
                return false;
            return Convert.ToBoolean(htmlValue.value);
        }

        public static explicit operator bool?(HValue htmlValue)
        {
            bool result;
            if (bool.TryParse(htmlValue, out result))
                return result;
            return null;
        }

        public static explicit operator int(HValue htmlValue)
        {
            return int.Parse(htmlValue, NumberStyles.AllowLeadingWhite
                | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
        }

        public static explicit operator int?(HValue htmlValue)
        {
            int result;
            if (int.TryParse(htmlValue, out result))
                return result;
            return null;
        }

        public static explicit operator uint(HValue htmlValue)
        {
            return uint.Parse(htmlValue, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite
                | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent,
                NumberFormatInfo.InvariantInfo);
        }

        public static explicit operator uint?(HValue htmlValue)
        {
            uint result;
            if (uint.TryParse(htmlValue, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite
                    | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent,
                    NumberFormatInfo.InvariantInfo, out result))
                return result;
            return null;
        }

        public static explicit operator long(HValue htmlValue)
        {
            return long.Parse(htmlValue, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign
                | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo);
        }

        public static explicit operator long?(HValue htmlValue)
        {
            long result;
            if (long.TryParse(htmlValue, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign
                | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out result))
                return result;
            return null;
        }

        public static explicit operator ulong(HValue htmlValue)
        {
            return ulong.Parse(htmlValue, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign
                | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo);
        }

        public static explicit operator ulong?(HValue htmlValue)
        {
            ulong result;
            if (ulong.TryParse(htmlValue, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign
                | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out result))
                return result;
            return null;
        }

        public static explicit operator float(HValue htmlValue)
        {
            return float.Parse(htmlValue, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign
                | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo);
        }

        public static explicit operator float?(HValue htmlValue)
        {
            float result;
            if (float.TryParse(htmlValue, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign
                | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out result))
                return result;
            return null;
        }

        public static explicit operator double(HValue htmlValue)
        {
            return double.Parse(htmlValue, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign
                | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo);
        }

        public static explicit operator double?(HValue htmlValue)
        {
            double result;
            if (double.TryParse(htmlValue, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign
                | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out result))
                return result;
            return null;
        }

        public static explicit operator decimal(HValue htmlValue)
        {
            return Convert.ToDecimal(htmlValue);
        }

        public static explicit operator decimal?(HValue htmlValue)
        {
            decimal result;
            if (decimal.TryParse(htmlValue, out result))
                return result;
            return null;
        }

        public static explicit operator DateTime(HValue htmlValue)
        {
            if (htmlValue == null)
                return DateTime.MinValue;
            return htmlValue.ToString().ToDate();
        }

        public static explicit operator DateTime?(HValue htmlValue)
        {
            if (htmlValue == null)
                return null;
            return htmlValue.ToString().ToDate();
        }

        public static explicit operator TimeSpan(HValue htmlValue)
        {
            if (htmlValue == null)
                return TimeSpan.Zero;
            return TimeSpan.Parse(htmlValue);
        }

        public static explicit operator TimeSpan?(HValue htmlValue)
        {
            TimeSpan result;
            if (TimeSpan.TryParse(htmlValue, out result))
                return result;
            return null;
        }

        public static explicit operator Guid(HValue htmlValue)
        {
            if (htmlValue == null)
                return Guid.Empty;
            return new Guid(htmlValue);
        }

        public static explicit operator Guid?(HValue htmlValue)
        {
            if (htmlValue == null)
                return null;
            return new Guid(htmlValue);
        }
    }
}