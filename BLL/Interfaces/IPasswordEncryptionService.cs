namespace BLL.Interfaces;

public interface IPasswordEncryptionService
{
    Task<string> EncryptAsync(string clearText);
    string HashPassword(byte[] password);
}
