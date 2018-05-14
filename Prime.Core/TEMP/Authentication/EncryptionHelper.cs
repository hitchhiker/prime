#region

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

#endregion

namespace Prime.Core.Encryption
{
    public class EncryptionHelper
    {
        public static AsymmetricCipherKeyPair GenerateKeys(AsymmetricKeySize keySize)
        {
            var gen = new ECKeyPairGenerator();
            var secureRandom = new SecureRandom();
            var keyGenParam = new KeyGenerationParameters(secureRandom, (int)keySize);
            gen.Init(keyGenParam);
            return gen.GenerateKeyPair();
        }

        public static bool VerifySignature(AsymmetricCipherKeyPair key, string plainText, byte[] signature)
        {
            var encoder = new ASCIIEncoding();
            var inputData = encoder.GetBytes(plainText);
            var signer = SignerUtilities.GetSigner("ECDSA");
            signer.Init(false, key.Public);
            signer.BlockUpdate(inputData, 0, inputData.Length);
            return signer.VerifySignature(signature);
        }

        public static byte[] GetSignature(string plainText, AsymmetricCipherKeyPair key)
        {
            var encoder = new ASCIIEncoding();
            var inputData = encoder.GetBytes(plainText);

            var signer = SignerUtilities.GetSigner("ECDSA");
            signer.Init(true, key.Private);
            signer.BlockUpdate(inputData, 0, inputData.Length);

            return signer.GenerateSignature();
        }

        public static string GetKeyString(PgpKeyRing kr)
        {
            using (var ms = new MemoryStream())
            {
                using (var pubout = new BufferedStream(ms))
                {
                    switch (kr)
                    {
                        case PgpSecretKeyRing skr:
                            skr.Encode(pubout);
                            break;
                        case PgpPublicKeyRing pkr:
                            pkr.Encode(pubout);
                            break;
                    }

                    pubout.Flush();

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            using (var ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public static PgpKeyRingGenerator GenerateKeyRingGenerator(KeyRingParams keyRingParams)
        {
            var generator = GeneratorUtilities.GetKeyPairGenerator("RSA");
            generator.Init(keyRingParams.RsaParams());

            /* Create the master (signing-only) key. */
            var masterKeyPair = new PgpKeyPair(PublicKeyAlgorithmTag.RsaSign, generator.GenerateKeyPair(), DateTime.UtcNow);

            Debug.WriteLine("Generated master key with ID " + masterKeyPair.KeyId.ToString("X"));

            var masterSubpckGen = new PgpSignatureSubpacketGenerator();
            masterSubpckGen.SetKeyFlags(false, PgpKeyFlags.CanSign | PgpKeyFlags.CanCertify);
            masterSubpckGen.SetPreferredSymmetricAlgorithms(false, keyRingParams.SymmetricAlgorithms.Select(a => (int) a).ToArray());
            masterSubpckGen.SetPreferredHashAlgorithms(false, keyRingParams.HashAlgorithms.Select(a => (int) a).ToArray());

            /* Create a signing and encryption key for daily use. */
            var encKeyPair = new PgpKeyPair(PublicKeyAlgorithmTag.RsaGeneral, generator.GenerateKeyPair(), DateTime.UtcNow);

            Debug.WriteLine("Generated encryption key with ID "+ encKeyPair.KeyId.ToString("X"));

            var encSubpckGen = new PgpSignatureSubpacketGenerator();
            encSubpckGen.SetKeyFlags(false, PgpKeyFlags.CanEncryptCommunications | PgpKeyFlags.CanEncryptStorage);

            masterSubpckGen.SetPreferredSymmetricAlgorithms(false, (keyRingParams.SymmetricAlgorithms.Select(a => (int) a)).ToArray());
            masterSubpckGen.SetPreferredHashAlgorithms(false, (keyRingParams.HashAlgorithms.Select(a => (int) a)).ToArray());

            /* Create the key ring. */

            var keyRingGen = new PgpKeyRingGenerator(
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
    }
}