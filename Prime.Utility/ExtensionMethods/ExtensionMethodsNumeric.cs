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
    }
}