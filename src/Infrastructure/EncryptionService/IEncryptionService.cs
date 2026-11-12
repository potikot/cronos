namespace Cronos.Infrastructure.EncryptionService;

public interface IEncryptionService
{
    string Encrypt(string text);
    string Decrypt(string encryptedText);
}