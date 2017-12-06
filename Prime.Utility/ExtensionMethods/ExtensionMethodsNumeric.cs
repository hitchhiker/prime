namespace Prime.Utility
{
    public static class ExtensionMethodsNumeric
    {
        public static string ToString(this decimal source, int decimalPlaces)
        {
            return source.ToString("F" + decimalPlaces);
        }

        public static string ToString(this double source, int decimalPlaces)
        {
            return source.ToString("F" + decimalPlaces);
        }

        public static decimal PercentageDifference(this decimal value1, decimal value2)
        {
            if (value1 == 0 && value2 == 0)
                return 0;

            if (value1 == 0 || value2 == 0)
                return 100;

            return 100m / value1 * (value2 - value1);
        }

        public static decimal PercentageDifference(this int value1, int value2)
        {
            return PercentageDifference((decimal)value1, value2);
        }
    }
}