using System;
using System.Text;

public static class EncryptionUtility
{
    private static readonly string key = "Match3SecretKey";

    public static string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var keyBytes = Encoding.UTF8.GetBytes(key);

        for (int i = 0; i < plainBytes.Length; i++)
        {
            plainBytes[i] ^= keyBytes[i % keyBytes.Length];
        }

        return Convert.ToBase64String(plainBytes);
    }

    public static string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;

        var cipherBytes = Convert.FromBase64String(cipherText);
        var keyBytes = Encoding.UTF8.GetBytes(key);

        for (int i = 0; i < cipherBytes.Length; i++)
        {
            cipherBytes[i] ^= keyBytes[i % keyBytes.Length];
        }

        return Encoding.UTF8.GetString(cipherBytes);
    }
}