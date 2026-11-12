using System.Security.Cryptography;

namespace Cronos.Infrastructure.EncryptionService;

public class Base64EncryptionService : IEncryptionService
{
    private const string Password = "0123456789";
    private static readonly byte[] Salt = [0x26, 0xdc, 0xff, 0x00, 0xad, 0xed, 0x7a, 0xee, 0xc5, 0xfe, 0x07, 0xaf, 0x4d, 0x08, 0x22, 0x3c];

    public string Encrypt(string text)
    {
        if (string.IsNullOrEmpty(text))
            return "";

        using var aes = Aes.Create();
        var key = new Rfc2898DeriveBytes(Password, Salt, 1000, HashAlgorithmName.SHA256);
        aes.Key = key.GetBytes(aes.KeySize / 8);
        aes.IV = key.GetBytes(aes.BlockSize / 8);

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        using (var writer = new StreamWriter(cryptoStream))
        {
            writer.Write(text);
        }

        return Convert.ToBase64String(memoryStream.ToArray());
    }

    public string Decrypt(string encryptedText)
    {
        if (string.IsNullOrEmpty(encryptedText))
            return "";

        using var aes = Aes.Create();
        var key = new Rfc2898DeriveBytes(Password, Salt, 1000, HashAlgorithmName.SHA256);
        aes.Key = key.GetBytes(aes.KeySize / 8);
        aes.IV = key.GetBytes(aes.BlockSize / 8);

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var memoryStream = new MemoryStream(Convert.FromBase64String(encryptedText));
        using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(cryptoStream);
        return reader.ReadToEnd();
    }
}