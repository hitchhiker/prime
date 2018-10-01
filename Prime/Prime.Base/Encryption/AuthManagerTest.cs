using System;
using System.IO;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Prime.Core;

namespace Prime.Base.Encryption
{
    public static class AuthManagerTest
    {
        public static void Key1(PrimeContext context)
        {
            var keys = EncryptionHelper.GenerateEcKeys();

            File.WriteAllText(@"D:\hh\scratch\tmp\pub.key", keys.publicKey);
            File.WriteAllText(@"D:\hh\scratch\tmp\prv.key", keys.privateKey);

            var fi = new FileInfo(@"D:\hh\scratch\tmp\signthis.txt");
            File.WriteAllText(fi.FullName, "Testing signature");

            fi.CreateSignedFile(keys.privateKey);

            Console.WriteLine("Verified: " + fi.VerifySignedFile(keys.publicKey));
        }

        public static void Key2(PrimeContext context)
        {
            var identity = "prime-user";

            var krgen = EncryptionHelper.GenerateKeyRingGenerator(new KeyRingParams(identity, EncryptionHelper.TmpPassword) { Length = 2048 });

            var pkr = krgen.GeneratePublicKeyRing();
            var pubout = new BufferedStream(new FileStream(@"D:\hh\scratch\tmp\dummy.pkr", FileMode.Create));
            pkr.Encode(pubout);
            pubout.Close();

            // Generate private key, dump to file.
            var skr = krgen.GenerateSecretKeyRing();
            var secout = new BufferedStream(new FileStream(@"D:\hh\scratch\tmp\dummy.skr", FileMode.Create));
            skr.Encode(secout);
            secout.Close();

            // Generate public key ring.
            //var pubKey = EncryptionHelper.GetKeyString(pkr);

            // Generate private key.
            //var privateKey = EncryptionHelper.GetKeyString(skr);

            var publicKey = pkr.ToPublicKey();
            var privateKey = skr.ToPrivateKey(EncryptionHelper.TmpPassword);

            File.WriteAllText(@"D:\hh\scratch\tmp\pub.key", publicKey);
            File.WriteAllText(@"D:\hh\scratch\tmp\prv.key", privateKey);

            //logger.Log("Pub: " + pubKey);
            //logger.Log("Priv: " + privateKey);

            var fi = new FileInfo(@"D:\hh\scratch\tmp\signthis.txt");

            File.WriteAllText(fi.FullName, "Testing signature");
            fi.CreateSignedFile(privateKey);
            Console.WriteLine("Verified: " + fi.VerifySignedFile(publicKey));
        }
       

        public static void EcdsaKeyTest(PrimeContext context)
        {
            var logger = context.L;
            var size = AsymmetricKeySize.S256;
            var s = "Hello World!";

            logger.Log("======= Key Size: {0} =======", size);
            try
            {
                var key = EncryptionHelper.GenerateEcKeys(size);
                var signature = EncryptionHelper.GetSignature(s, key.prv);
                var signatureOk = EncryptionHelper.VerifySignature(key.pub, s, signature);

                var pubicKey = (ECPublicKeyParameters)(key.pub);
                var privateKey = (ECPrivateKeyParameters)(key.prv);

                logger.Log("Input Text: " +s);
                logger.Log("Pri key ({0} bytes): {1}", privateKey.D.BitLength, privateKey.D);
                logger.Log("Signature ({0} bytes): {1}", signature.Length, signature.ToX2String());
                logger.Log("Signature verified: {0}", signatureOk);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
    }
}