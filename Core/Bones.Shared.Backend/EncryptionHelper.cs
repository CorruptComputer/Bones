using System.Security.Cryptography;
using System.Text;

namespace Bones.Shared.Backend;

/// <summary>
///   Helper class for encryption related tasks
/// </summary>
public static class EncryptionHelper
{
    /// <summary>
    ///   Generates a new AES-256 encryption key 
    /// </summary>
    /// <returns></returns>
    public static string GenerateAESKey()
    {
        byte[] key = new byte[32]; // 256 bits = 32 bytes

        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(key);
        }

        return Convert.ToBase64String(key);
    }

    /// <summary>
    ///   Encrypts the given plain text using the given key
    /// </summary>
    /// <param name="plainText"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string Encrypt(string plainText, string key)
    {
        using Aes aes = Aes.Create();
        aes.Key = Convert.FromBase64String(key);
        aes.GenerateIV();
        byte[] iv = aes.IV;

        using ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, iv);
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        byte[] combined = new byte[iv.Length + encryptedBytes.Length];
        Buffer.BlockCopy(iv, 0, combined, 0, iv.Length);
        Buffer.BlockCopy(encryptedBytes, 0, combined, iv.Length, encryptedBytes.Length);

        return Convert.ToBase64String(combined);
    }

    /// <summary>
    ///   Decrypts the given cipher text using the given key
    /// </summary>
    /// <param name="cipherText"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string Decrypt(string cipherText, string key)
    {
        byte[] combined = Convert.FromBase64String(cipherText);

        using Aes aes = Aes.Create();
        aes.Key = Convert.FromBase64String(key);

        byte[] iv = new byte[aes.BlockSize / 8];
        byte[] cipherBytes = new byte[combined.Length - iv.Length];

        Buffer.BlockCopy(combined, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(combined, iv.Length, cipherBytes, 0, cipherBytes.Length);

        using ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, iv);
        byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(decryptedBytes);
    }
}
