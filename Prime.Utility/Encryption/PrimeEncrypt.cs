using System;
using System.IO;
using System.Security.Cryptography;

namespace Prime.Utility.Encrypt
{
    public class PrimeEncrypt : CryptBase
    {
        public PrimeEncrypt(FileInfo privateKey, FileInfo publicKey)
        {
            PrivateKey = privateKey;
            PublicKey = publicKey;
        }

        public PrimeEncrypt(DirectoryInfo directory, string name)
        {
            var path = Path.Combine(directory.FullName, name);
            var pubprv = KeyPairLocations(path);

            if (!pubprv.privateKey.Exists || !pubprv.publicKey.Exists)
                CreateKeypair(path);

            PrivateKey = pubprv.privateKey;
            PublicKey = pubprv.publicKey;
        }

        public readonly FileInfo PrivateKey;

        public readonly FileInfo PublicKey;

        public string Encrypt(string unEncrypted)
        {
            return unEncrypted;
        }

        public string Decrypt(string encrypted)
        {
            return encrypted;
        }

        public FileInfo Encrypt(FileInfo file)
        {
            if (!file.Exists)
                throw new Exception("Cannot find the file:" + file.FullName);

            var enc = EncryptFile(file.FullName, PrivateKey.FullName);
            if (enc==null)
                throw new Exception("Cannot encrypt the file:" + file.FullName);

            return new FileInfo(enc);
        }

        public FileInfo DeCrypt(FileInfo file)
        {
            if (!file.Exists)
                throw new Exception("Cannot find the file:" + file.FullName);

            var enc = DecryptFile(file.FullName, PrivateKey.FullName);
            if (enc == null)
                throw new Exception("Cannot decrypt the file:" + file.FullName);

            return new FileInfo(enc);
        }
    }
}