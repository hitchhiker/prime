using System.IO;
using System.Security.Cryptography;
using System.Text;


public class Alice
{
    public static byte[] AlicePublicKey;

    public static void Go()
    {
        using (var alice = new ECDiffieHellmanCng())
        {
            alice.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            alice.HashAlgorithm = CngAlgorithm.Sha256;
            AlicePublicKey = alice.PublicKey.ToByteArray();
            var bob = new Bob();
            var k = CngKey.Import(bob.BobPublicKey, CngKeyBlobFormat.EccPublicBlob);
            var aliceKey = alice.DeriveKeyMaterial(CngKey.Import(bob.BobPublicKey, CngKeyBlobFormat.EccPublicBlob));
            Send(aliceKey, "Secret message", out var encryptedMessage, out var iv);
            bob.Receive(encryptedMessage, iv);
        }
    }

    private static void Send(byte[] key, string secretMessage, out byte[] encryptedMessage, out byte[] iv)
    {
        using (Aes aes = new AesCryptoServiceProvider())
        {
            aes.Key = key;
            iv = aes.IV;

            // Encrypt the message
            using (var ciphertext = new MemoryStream())
            using (var cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                byte[] plaintextMessage = Encoding.UTF8.GetBytes(secretMessage);
                cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                cs.Close();
                encryptedMessage = ciphertext.ToArray();
            }
        }
    }

}