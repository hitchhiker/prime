using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace Prime.Core.Encryption
{
    public class KeyRingParams
    {
        public KeyRingParams(string identity, string password)
        {
            Identity = identity;
            Password = password;
        }

        public uint Length { get; set; } = 4096;

        public string Identity { get; }

        public string Password { get; }

        public SymmetricKeyAlgorithmTag? PrivateKeyEncryptionAlgorithm { get; set; } = SymmetricKeyAlgorithmTag.Aes256;

        public SymmetricKeyAlgorithmTag[] SymmetricAlgorithms { get; set; } =
        {
            SymmetricKeyAlgorithmTag.Aes256,
            SymmetricKeyAlgorithmTag.Aes192,
            SymmetricKeyAlgorithmTag.Aes128
        };

        public HashAlgorithmTag[] HashAlgorithms { get; set; } =
        {
            HashAlgorithmTag.Sha256,
            HashAlgorithmTag.Sha1,
            HashAlgorithmTag.Sha384,
            HashAlgorithmTag.Sha512,
            HashAlgorithmTag.Sha224
        };

        public RsaKeyGenerationParameters RsaParams()
        {
            return new RsaKeyGenerationParameters(BigInteger.ValueOf(0x10001), new SecureRandom(), (int) Length, 12);
        }
        
        public char[] GetPassword()
        {
            return Password.ToCharArray();
        }
    }
}