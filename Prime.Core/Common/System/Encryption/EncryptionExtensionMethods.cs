using System.IO;

namespace Prime.Core
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