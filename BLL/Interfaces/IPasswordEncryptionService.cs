namespace BLL.Interfaces;

public interface IPasswordEncryptionService
{
    Task<string> EncryptAsync(string clearText);
    public string HashPassword(string password);
    Task<string> DecryptAsync(string encryptedText);
}
