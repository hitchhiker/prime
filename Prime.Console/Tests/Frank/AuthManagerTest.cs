using System.IO;
using System.Text;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Prime.Core.Authentication;

namespace Prime.Console.Tests.Frank
{
    public static class AuthManagerTest
    {
        public static void Go()
        {
            var password = "hello world!";
            var identity = "prime-user";

            var krgen = AuthUtilities.GenerateKeyRingGenerator(identity, password);

            UnicodeEncoding uniEncoding = new UnicodeEncoding();

            PgpPublicKeyRing pkr = krgen.GeneratePublicKeyRing();
            BufferedStream pubout = new BufferedStream(new FileStream(@"c:\tmp\dummy.pkr", System.IO.FileMode.Create));
            pkr.Encode(pubout);
            pubout.Close();

            // Generate private key, dump to file.
            PgpSecretKeyRing skr = krgen.GenerateSecretKeyRing();
            BufferedStream secout = new BufferedStream(new FileStream(@"c:\tmp\dummy.skr", System.IO.FileMode.Create));
            skr.Encode(secout);
            secout.Close();

            // Generate public key ring.
            var pubKey = AuthUtilities.GetKeyString(krgen.GeneratePublicKeyRing());

            // Generate private key.
            var secKey = AuthUtilities.GetKeyString(krgen.GenerateSecretKeyRing());
        }
    }
}