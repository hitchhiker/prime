using System;
using System.IO;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Prime.Base.Encryption;
using Prime.Core;

namespace Prime.Base
{
    public static class EncryptionExtensionMethods
    {
        public static FileInfo CreateSignedFile(this FileInfo file, string base64PrivateKey)
        {
            return CreateSignedFile(file, base64PrivateKey.ToPrivateKey());
        }

        public static FileInfo CreateSignedFile(this FileInfo file, ICipherParameters privateKey)
        {
            if (!file.Exists)
                return null;

            var s = File.ReadAllText(file.FullName);

            var signature = EncryptionHelper.GetSignature(s, privateKey);

            var fi = new FileInfo(file.FullName + ".sign");
            if (fi.Exists)
                fi.Delete();

            File.WriteAllText(fi.FullName, Convert.ToBase64String(signature));
            fi.Refresh();
            return fi;
        }

        public static bool VerifySignedFile(this FileInfo file, string base64PublicKey)
        {
            return VerifySignedFile(file, base64PublicKey.ToPublicKey());
        }

        public static bool VerifySignedFile(this FileInfo file, ICipherParameters publicKey)
        {
            if (!file.Exists)
                return false;

            var signFi = new FileInfo(file.FullName + ".sign");
            if (!signFi.Exists)
                return false;

            var source = File.ReadAllText(file.FullName);
            var signature = Convert.FromBase64String(File.ReadAllText(signFi.FullName));

            return EncryptionHelper.VerifySignature((AsymmetricKeyParameter)publicKey, source, signature);
        }

        public static string ToPrivateKey(this PgpSecretKeyRing privateKeyRing, string password)
        {
            return privateKeyRing.GetSecretKey().ExtractPrivateKey(password.ToCharArray()).Key.ToPrivateKey();
        }

        public static string ToPublicKey(this PgpPublicKeyRing publicKeyRing)
        {
            return publicKeyRing.GetPublicKey().GetKey().ToPublicKey();
        }

        public static string ToPrivateKey(this AsymmetricKeyParameter privateKey)
        {
            var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey);
            var serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetDerEncoded();
            return Convert.ToBase64String(serializedPrivateBytes);
        }

        public static string ToPublicKey(this AsymmetricKeyParameter publicKey)
        {
            var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
            var serializedPublicBytes = publicKeyInfo.ToAsn1Object().GetDerEncoded();
            return Convert.ToBase64String(serializedPublicBytes);
        }

        public static ICipherParameters ToPrivateKey(this string key)
        {
            return PrivateKeyFactory.CreateKey(Convert.FromBase64String(key));
        }

        public static ICipherParameters ToPublicKey(this string key)
        {
            return PublicKeyFactory.CreateKey(Convert.FromBase64String(key));
        }
       
    }
}