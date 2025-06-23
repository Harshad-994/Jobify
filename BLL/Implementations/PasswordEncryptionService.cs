using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using BLL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;

namespace BLL.Implementations;

public class PasswordEncryptionService : IPasswordEncryptionService
{

    private readonly IConfiguration _configuration;
    private readonly ILogger<PasswordEncryptionService> _logger;
    private readonly string _salt;
    public PasswordEncryptionService(IConfiguration configuration, ILogger<PasswordEncryptionService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        _salt = _configuration["AESAlgorithm:Salt"]!;

        if (string.IsNullOrWhiteSpace(_salt))
        {
            _logger.LogWarning("AESAlgorithm:Salt is invalid.");
            throw new PasswordEncryptionConfigurationException("AESAlgorithm:Salt");
        }
    }
    private readonly byte[] IV =
   {
    0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
    0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
    };

    private byte[] DeriveKeyFromSalt(string password)
    {
        try
        {
            var emptySalt = Array.Empty<byte>();
            int iterations = 1000;
            int desiredKeyLength = 16;
            HashAlgorithmName hashMethod = HashAlgorithmName.SHA384;
            return Rfc2898DeriveBytes.Pbkdf2(Encoding.Unicode.GetBytes(password),
                                             emptySalt,
                                             iterations,
                                             hashMethod,
                                             desiredKeyLength);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Key derivation failed.");
            throw new KeyDerivationException(ex);
        }

    }

    public async Task<string> EncryptAsync(string clearText)
    {
        try
        {
            using Aes aes = Aes.Create();
            aes.Key = DeriveKeyFromSalt(_salt);
            aes.IV = IV;
            using MemoryStream output = new();
            using CryptoStream cryptoStream = new(output, aes.CreateEncryptor(), CryptoStreamMode.Write);
            await cryptoStream.WriteAsync(Encoding.Unicode.GetBytes(clearText));
            await cryptoStream.FlushFinalBlockAsync();
            return Convert.ToBase64String(output.ToArray());
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Password Encryption failed.", ex);
            throw new PasswordEncryptionFailedException(ex);
        }

    }

    public string HashPassword(byte[] password)
    {
        try
        {
            byte[] bytes = SHA256.HashData(password);
            StringBuilder builder = new();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Password hashing failed.", ex);
            throw new PasswordHashingException(ex);
        }

    }

    public string HashPassword(string password)
    {
        if (password == null)
            throw new ArgumentNullException(nameof(password));
        return HashPassword(Encoding.UTF8.GetBytes(password));
    }



    public async Task<string> DecryptAsync(string encryptedText)
    {
        string passphrase = _salt;
        using Aes aes = Aes.Create();
        aes.Key = DeriveKeyFromSalt(passphrase);
        aes.IV = IV;
        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
        using MemoryStream input = new(encryptedBytes);
        using CryptoStream cryptoStream = new(input, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using MemoryStream output = new();
        await cryptoStream.CopyToAsync(output);
        return Encoding.Unicode.GetString(output.ToArray());
    }
}
