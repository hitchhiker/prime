#region

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

#endregion

namespace Prime.Core.Authentication
{
    public class AuthUtilities
    {
        public static string GetKeyString(PgpKeyRing kr)
        {
            using (var ms = new MemoryStream())
            {
                using (var pubout = new BufferedStream(ms))
                {
                    if (kr is PgpSecretKeyRing skr)
                        skr.Encode(pubout);
                    else if (kr is PgpSecretKeyRing pkr)
                        pkr.Encode(pubout);
                    ms.Seek(0, SeekOrigin.Begin);
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static PgpKeyRingGenerator GenerateKeyRingGenerator(string identity, string password)
        {
            var keyRingParams = new KeyRingParams
            {
                Password = password,
                Identity = identity,
                PrivateKeyEncryptionAlgorithm = SymmetricKeyAlgorithmTag.Aes256,
                SymmetricAlgorithms = new[]
                {
                    SymmetricKeyAlgorithmTag.Aes256,
                    SymmetricKeyAlgorithmTag.Aes192,
                    SymmetricKeyAlgorithmTag.Aes128
                },
                HashAlgorithms = new[]
                {
                    HashAlgorithmTag.Sha256,
                    HashAlgorithmTag.Sha1,
                    HashAlgorithmTag.Sha384,
                    HashAlgorithmTag.Sha512,
                    HashAlgorithmTag.Sha224
                }
            };

            var generator = GeneratorUtilities.GetKeyPairGenerator("RSA");
            generator.Init(keyRingParams.RsaParams);

            /* Create the master (signing-only) key. */
            var masterKeyPair = new PgpKeyPair(PublicKeyAlgorithmTag.RsaSign, generator.GenerateKeyPair(), DateTime.UtcNow);

            Debug.WriteLine("Generated master key with ID " + masterKeyPair.KeyId.ToString("X"));

            PgpSignatureSubpacketGenerator masterSubpckGen = new PgpSignatureSubpacketGenerator();
            masterSubpckGen.SetKeyFlags(false, PgpKeyFlags.CanSign | PgpKeyFlags.CanCertify);
            masterSubpckGen.SetPreferredSymmetricAlgorithms(false, keyRingParams.SymmetricAlgorithms.Select(a => (int) a).ToArray());
            masterSubpckGen.SetPreferredHashAlgorithms(false, keyRingParams.HashAlgorithms.Select(a => (int) a).ToArray());

            /* Create a signing and encryption key for daily use. */
            var encKeyPair = new PgpKeyPair(PublicKeyAlgorithmTag.RsaGeneral,generator.GenerateKeyPair(),DateTime.UtcNow);

            Debug.WriteLine("Generated encryption key with ID "+ encKeyPair.KeyId.ToString("X"));

            PgpSignatureSubpacketGenerator encSubpckGen = new PgpSignatureSubpacketGenerator();
            encSubpckGen.SetKeyFlags(false, PgpKeyFlags.CanEncryptCommunications | PgpKeyFlags.CanEncryptStorage);

            masterSubpckGen.SetPreferredSymmetricAlgorithms(false, (keyRingParams.SymmetricAlgorithms.Select(a => (int) a)).ToArray());
            masterSubpckGen.SetPreferredHashAlgorithms(false, (keyRingParams.HashAlgorithms.Select(a => (int) a)).ToArray());

            /* Create the key ring. */
            PgpKeyRingGenerator keyRingGen = new PgpKeyRingGenerator(
                PgpSignature.DefaultCertification,
                masterKeyPair,
                keyRingParams.Identity,
                keyRingParams.PrivateKeyEncryptionAlgorithm.Value,
                keyRingParams.GetPassword(),
                true,
                masterSubpckGen.Generate(),
                null,
                new SecureRandom());

            /* Add encryption subkey. */
            keyRingGen.AddSubKey(encKeyPair, encSubpckGen.Generate(), null);

            return keyRingGen;
        }

        // Define other methods and classes here
        class KeyRingParams
        {
            public KeyRingParams()
            {
                //Org.BouncyCastle.Crypto.Tls.EncryptionAlgorithm
                RsaParams = new RsaKeyGenerationParameters(BigInteger.ValueOf(0x10001), new SecureRandom(), 1024, 12);
            }

            public SymmetricKeyAlgorithmTag? PrivateKeyEncryptionAlgorithm { get; set; }
            public SymmetricKeyAlgorithmTag[] SymmetricAlgorithms { get; set; }
            public HashAlgorithmTag[] HashAlgorithms { get; set; }
            public RsaKeyGenerationParameters RsaParams { get; }
            public string Identity { get; set; }

            public string Password { get; set; }
            //= EncryptionAlgorithm.NULL;

            public char[] GetPassword()
            {
                return Password.ToCharArray();
            }
        }
    }
}