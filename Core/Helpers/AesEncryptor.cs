using Core.Helpers;
using System.Security.Cryptography;
using System.Text;

namespace Core.GeneralHelpers;

public static class AesEncryptor
{
    public static string Encrypt(string plainText)
    {
        byte[] encrypted;

        using (var aes = new AesManaged())
        {
            var keyBytes = new Rfc2898DeriveBytes(GeneralStaticHelper.DataProtectionKey, GeneralStaticHelper.DataProtectionSalt).GetBytes(32);
            aes.Mode = CipherMode.CBC;

            aes.GenerateIV();

            ICryptoTransform encryptor = aes.CreateEncryptor(keyBytes, aes.IV);

            using (var ms = new MemoryStream())
            {
                ms.Write(aes.IV, 0, aes.IV.Length);

                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    var data = Encoding.UTF8.GetBytes(plainText);
                    cs.Write(data, 0, data.Length);
                }

                encrypted = ms.ToArray();
            }
        }

        return Convert.ToBase64String(encrypted);
    }

    public static string Decrypt(string encryptedText)
    {
        var cipherText = Convert.FromBase64String(encryptedText);

        using (var aes = new AesManaged())
        {
            var keyBytes = new Rfc2898DeriveBytes(GeneralStaticHelper.DataProtectionKey, GeneralStaticHelper.DataProtectionSalt).GetBytes(32);
            aes.Mode = CipherMode.CBC;

            var iv = new byte[aes.IV.Length];
            Array.Copy(cipherText, iv, iv.Length);

            aes.IV = iv;

            using var ms = new MemoryStream(cipherText, iv.Length, cipherText.Length - iv.Length);
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(keyBytes, aes.IV), CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}