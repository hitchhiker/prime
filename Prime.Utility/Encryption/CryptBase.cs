using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Prime.Utility.Encrypt
{
    public abstract class CryptBase
    {
        protected Logger L => Logging.I.Common;

        protected byte[] ReadFile(string filename)
        {
            if (!File.Exists(filename))
            {
                L.Error("Couldn't find file {0}", filename);
                return null;
            }

            byte[] content;

            using (var fs = File.OpenRead(filename))
            {
                content = new byte[fs.Length];
                fs.Read(content, 0, content.Length);
                fs.Close();
            }

            return content;
        }

        protected string ReadTextFile(string filename)
        {
            return Encoding.UTF8.GetString(ReadFile(filename));
        }

        private RSA ReadKeypair(string filename)
        {
            // find the keyfile
            var keyname = filename;

            if (!File.Exists(keyname))
            {
                keyname = filename + ".key";
                if (!File.Exists(keyname))
                {
                    keyname = filename + ".pub";
                }
            }

            if (!File.Exists(keyname))
            {
                L.Error("Couldn't find key: {0}, {0}.pub or {0}.key", filename);
                return null;
            }

            var key = ReadTextFile(keyname);

            var rsa = RSA.Create();
            rsa.FromXmlString(key);
            return rsa;
        }

        protected void WriteFile(string filename, byte[] content)
        {
            if (File.Exists(filename))
                File.Delete(filename);

            using (var fs = File.OpenWrite(filename))
            {
                fs.Write(content, 0, content.Length);
                fs.Close();
            }
        }

        protected void WriteFile(string filename, string content)
        {
            WriteFile(filename, Encoding.UTF8.GetBytes(content));
        }

        private byte[] Encrypt(RSA rsa, byte[] input)
        {
            // by default this will create a 128 bits AES (Rijndael) object
            byte[] result;
            using (var sa = SymmetricAlgorithm.Create())
            {
                using (var ct = sa.CreateEncryptor())
                {
                    var encrypt = ct.TransformFinalBlock(input, 0, input.Length);

                    var fmt = new RSAPKCS1KeyExchangeFormatter(rsa);
                    var keyex = fmt.CreateKeyExchange(sa.Key);

                    // return the key exchange, the IV (public) and encrypted data
                    result = new byte[keyex.Length + sa.IV.Length + encrypt.Length];
                    Buffer.BlockCopy(keyex, 0, result, 0, keyex.Length);
                    Buffer.BlockCopy(sa.IV, 0, result, keyex.Length, sa.IV.Length);
                    Buffer.BlockCopy(encrypt, 0, result, keyex.Length + sa.IV.Length, encrypt.Length);
                }
            }
            return result;
        }

        private static byte[] Decrypt(RSA rsa, byte[] input)
        {
            // by default this will create a 128 bits AES (Rijndael) object
            byte[] decrypt;
            using (var sa = SymmetricAlgorithm.Create())
            {
                var keyex = new byte[rsa.KeySize >> 3];
                Buffer.BlockCopy(input, 0, keyex, 0, keyex.Length);

                var def = new RSAPKCS1KeyExchangeDeformatter(rsa);
                var key = def.DecryptKeyExchange(keyex);

                var iv = new byte[sa.IV.Length];
                Buffer.BlockCopy(input, keyex.Length, iv, 0, iv.Length);

                using (var ct = sa.CreateDecryptor(key, iv))
                {
                    decrypt = ct.TransformFinalBlock(input, keyex.Length + iv.Length, input.Length - (keyex.Length + iv.Length));
                }
            }
            return decrypt;
        }

        protected void Error(string error)
        {
            L.Error("{0}ERROR: {1}{0}", Environment.NewLine, error);
        }

        public (FileInfo privateKey, FileInfo publicKey) KeyPairLocations(string filePath)
        {
            var prv = filePath + ".key";
            var pub = filePath + ".pub";

            return (new FileInfo(prv), new FileInfo(pub));
        }

        public (FileInfo privateKey, FileInfo publicKey) CreateKeypair(string filePath)
        {
            var fis = KeyPairLocations(filePath);

            using (var rsa = RSA.Create())
            {
                WriteFile(fis.privateKey.FullName, rsa.ToXmlString(true));
                L.Info("Created protected file {0}.key", filePath);
                WriteFile(fis.publicKey.FullName, rsa.ToXmlString(false));

                fis.privateKey.Refresh();
                fis.publicKey.Refresh();
            }

            L.Info("Created public file {0}.pub", filePath);
            return fis;
        }

        protected string EncryptFile(string filename, string keyfile)
        {
            byte[] encrypted;
            using (var rsa = ReadKeypair(keyfile))
            {
                if (rsa == null)
                    return null;

                var input = ReadFile(filename);
                if (input == null)
                    return null;

                encrypted = Encrypt(rsa, input);
            }
            var fn = filename + ".enc";
            WriteFile(fn, encrypted);
            return fn;
        }

        protected string Encrypt(string source, string keyfile)
        {
            throw new Exception("TODO");
            byte[] encrypted;
            using (var rsa = ReadKeypair(keyfile))
            {
                if (rsa == null)
                    return null;

                var b = Encoding.UTF8.GetBytes(source);
                encrypted = Encrypt(rsa, b);
            }
            return null;
        }

        protected string DecryptFile(string filename, string keyfile)
        {
            byte[] decrypted;
            using (var rsa = ReadKeypair(keyfile))
            {
                if (rsa == null)
                    return null;

                var input = ReadFile(filename);
                if (input == null)
                    return null;

                decrypted = Decrypt(rsa, input);
            }
            if (filename.EndsWith(".enc"))
                filename = filename.Substring(0, filename.Length - 4);
            else
                filename += ".dec";

            WriteFile(filename, decrypted);
            return filename;
        }
    }
}