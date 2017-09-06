namespace Prime.Core
{
    public static class OhclDataExtensionMethods
    {
        public static bool IsEmpty(this OhclData data)
        {
            return data == null || data.Count == 0;
        }

        public static bool IsNotEmpty(this OhclData data)
        {
            return data != null && data.Count > 0;
        }
    }
}