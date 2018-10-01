using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace Prime.Base
{
    public class RsaKeyPair
    {
        public RsaPrivateCrtKeyParameters PrivateKey { get; private set; }
        public RsaKeyParameters PublicKey { get; private set; }
        public string Public { get; private set; }
        public string Private { get; private set; }

        public static RsaKeyPair Create()
        {
            var pair = new RsaKeyPair();

            var g = new RsaKeyPairGenerator();
            g.Init(new KeyGenerationParameters(new SecureRandom(), 2048));
            var p = g.GenerateKeyPair();
            pair.PrivateKey = (RsaPrivateCrtKeyParameters)p.Private;
            pair.PublicKey = (RsaKeyParameters)p.Public;

            var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(pair.PrivateKey);
            var serializedPrivate = privateKeyInfo.ToAsn1Object().GetDerEncoded();
            pair.Private = Convert.ToBase64String(serializedPrivate);

            var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pair.PublicKey);
            var serializedPublic = publicKeyInfo.ToAsn1Object().GetDerEncoded();
            pair.Public = Convert.ToBase64String(serializedPublic);

            return pair;
        }

        public static RsaKeyPair CreateFrom(string privateKey, string publicKey)
        {
            return new RsaKeyPair
            {
                Private = privateKey,
                Public = publicKey,
                PrivateKey = (RsaPrivateCrtKeyParameters) PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey)),
                PublicKey = (RsaKeyParameters) PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey))
            };
        }
    }
}