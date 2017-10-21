using System.IO;

namespace Prime.Common
{
    public static class EncryptionExtensionMethods
    {
        public static FileInfo Encrypt(this FileInfo fileInfo, UserContext context)
        {
            return context.Crypt.Encrypt(fileInfo);
        }

        public static FileInfo Decrypt(this FileInfo fileInfo, UserContext context)
        {
            return context.Crypt.DeCrypt(fileInfo);
        }
    }
}