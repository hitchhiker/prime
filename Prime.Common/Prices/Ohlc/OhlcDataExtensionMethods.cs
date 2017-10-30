namespace Prime.Common
{
    public static class OhlcDataExtensionMethods
    {
        public static bool IsEmpty(this OhlcData data)
        {
            return data == null || data.Count == 0;
        }

        public static bool IsNotEmpty(this OhlcData data)
        {
            return data != null && data.Count > 0;
        }
    }
}