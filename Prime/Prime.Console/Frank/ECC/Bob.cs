using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class Bob
{
    public byte[] BobPublicKey;
    private readonly byte[] bobKey;

    public Bob()
    {
        using (ECDiffieHellmanCng bob = new ECDiffieHellmanCng())
        {
            bob.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            bob.HashAlgorithm = CngAlgorithm.Sha256;
            BobPublicKey = bob.PublicKey.ToByteArray();
            bobKey = bob.DeriveKeyMaterial(CngKey.Import(Alice.AlicePublicKey, CngKeyBlobFormat.EccPublicBlob));

        }
    }

    public void Receive(byte[] encryptedMessage, byte[] iv)
    {
        using (Aes aes = new AesCryptoServiceProvider())
        {
            aes.Key = bobKey;
            aes.IV = iv;

            // Decrypt the message
            using (var plaintext = new MemoryStream())
            {
                using (var cs = new CryptoStream(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(encryptedMessage, 0, encryptedMessage.Length);
                    cs.Close();
                    var message = Encoding.UTF8.GetString(plaintext.ToArray());
                    Console.WriteLine(message);
                }
            }
        }
    }
}