namespace Prime.Core
{
    public static class BytesEncoderExtensions
    {
        public static byte[] GetBytes(this string str)
        {
            return System.Text.Encoding.Default.GetBytes(str);
        }

        public static string GetString(this byte[] bytes)
        {
            return System.Text.Encoding.Default.GetString(bytes);
        }
    }
}