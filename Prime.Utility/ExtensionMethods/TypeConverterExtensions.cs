using System;


using LiteDB;

namespace Prime.Utility
{
    public static class TypeConverterExtensions
    {
        public static DateTime ToDate(this string str, DateTime? defaultValue = null)
        {
            return str.ToDateTime().Date;
        }

        public static DateTime ToDateTime(this string str, DateTime? defaultValue = null)
        {
            var defaultv = defaultValue ?? DateTime.MinValue;

            if (string.IsNullOrWhiteSpace(str))
                return defaultv;

            str = str.Trim();
            if (string.IsNullOrWhiteSpace(str))
                return defaultv;

            if (str.IsNumeric())
                return new DateTime(Int64.Parse(str), DateTimeKind.Utc);

            DateTime dt;
            var r = ((!str.TryParseDateTime(DateTimeRoutines.DateTimeFormat.UK_DATE, out dt) ? (DateTime?)null : dt) ??
                     (!str.TryParseDateTime(DateTimeRoutines.DateTimeFormat.USA_DATE, out dt) ? (DateTime?)null : dt)) ??
                    (DateTime.TryParse(str, out dt) ? dt : (DateTime?)null);

            return DateTime.SpecifyKind(r ?? defaultv, DateTimeKind.Utc);
        }

        public static ObjectId ToObjectId(this string str, ObjectId defaultValue = null)
        {
            var def = defaultValue ?? ObjectId.Empty;

            if (string.IsNullOrWhiteSpace(str))
                return def;

            return new ObjectId(str);
        }
        
        public static bool ToBool(this string str, bool defaultValue = false)
        {
            if (string.IsNullOrWhiteSpace(str))
                return defaultValue;

            var nb = ToBoolN(str);
            return nb != null ? (bool) nb : defaultValue;
        }

        public static string ToBoolShort(this bool value)
        {
            return value ? "1" : "0";
        }

        public static bool? ToBoolN(this string str, bool? defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(str))
                return defaultValue;

            if (str.Equals("1", StringComparison.OrdinalIgnoreCase))
                return true;
            if (str.Equals("0", StringComparison.OrdinalIgnoreCase))
                return false;
            if (str.Equals("yes", StringComparison.OrdinalIgnoreCase))
                return true;
            if (str.Equals("no", StringComparison.OrdinalIgnoreCase))
                return false;
            if (str.Equals("on", StringComparison.OrdinalIgnoreCase))
                return true;
            if (str.Equals("off", StringComparison.OrdinalIgnoreCase))
                return false;

            bool ret;
            return Boolean.TryParse(str, out ret) ? ret : defaultValue;
        }

        public static Guid? ToGuid(this string str, Guid? defaultValue)
        {
            var defv = defaultValue ?? Guid.Empty;
            if (string.IsNullOrWhiteSpace(str))
                return defv;
            Guid ret;
            return Guid.TryParse(str, out ret) ? ret : defv;
        }

        public static Guid ToGuid(this string str)
        {
            var defv = Guid.Empty;
            if (string.IsNullOrWhiteSpace(str))
                return defv;
            Guid ret;
            return Guid.TryParse(str, out ret) ? ret : defv;
        }

        public static int ToIntPositive(this string str, int defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(str))
                return defaultValue;
            int ret;
            ret = Int32.TryParse(str, out ret) ? ret : defaultValue;
            return ret < 0 ? defaultValue : ret;
        }

        public static int ToInt(this string str, int defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(str))
                return defaultValue;
            int ret;
            return Int32.TryParse(str, out ret) ? ret : defaultValue;
        }

        public static int? ToIntN(this string str, int? defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(str))
                return defaultValue;
            int ret;
            return Int32.TryParse(str, out ret) ? ret : defaultValue;
        }

        public static decimal ToDecimal(this string str, decimal defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(str))
                return defaultValue;
            decimal ret;
            return Decimal.TryParse(str, out ret) ? ret : defaultValue;
        }

        public static decimal? ToDecimalN(this string str, decimal? defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(str))
                return defaultValue;
            decimal ret;
            return Decimal.TryParse(str, out ret) ? ret : defaultValue;
        }
        public static double ToDouble(this string str, double defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(str))
                return defaultValue;
            double ret;
            return Double.TryParse(str, out ret) ? ret : defaultValue;
        }

        public static double? ToDoubleN(this string str, double? defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(str))
                return defaultValue;
            double ret;
            return Double.TryParse(str, out ret) ? ret : defaultValue;
        }
        public static long ToLong(this string str, long defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(str))
                return defaultValue;
            long ret;
            return Int64.TryParse(str, out ret) ? ret : defaultValue;
        }

        public static long? ToLongN(this string str, long? defaultValue)
        {
            if (string.IsNullOrWhiteSpace(str))
                return defaultValue;
            long ret;
            return Int64.TryParse(str, out ret) ? ret : defaultValue;
        }

        public static T ToEnum<T>(this int input) where T : struct
        {
            return ToEnum<T>(input.ToString());
        }

        public static T ToEnum<T>(this char input) where T : struct
        {
            return ToEnum<T>(input.ToString());
        }

        public static Enum ToEnum(this string input, Type type)
        {
            if (string.IsNullOrWhiteSpace(input))
                return (Enum)Enum.ToObject(type, 0);

            return (Enum)Enum.Parse(type, input);
        }

        public static T ToEnum<T>(this string input, T defaultValue = default(T)) where T : struct
        {
            if (string.IsNullOrWhiteSpace(input))
                return defaultValue;

            T ret;
            return Enum.TryParse(input, true, out ret) ? ret : defaultValue;
        }
        
    }
}