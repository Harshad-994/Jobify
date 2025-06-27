namespace Shared.Exceptions;

public abstract class PasswordEncryptionException : ApplicationException
{
    protected PasswordEncryptionException(string message, string errorCode) : base(message, errorCode)
    {
    }

    protected PasswordEncryptionException(string message, string errorCode, Exception innerException) : base(message, errorCode, innerException)
    {
    }

}

public class KeyDerivationException : PasswordEncryptionException
{
    public KeyDerivationException(Exception innerException)
        : base("Password key generation failed. Please try again.", "KEY_DERIVATION_ERROR", innerException)
    {
    }
}

public class PasswordEncryptionConfigurationException : PasswordEncryptionException
{
    public PasswordEncryptionConfigurationException(string configKey)
        : base("Password encryption configuration error. Please contact support.", "PASSWORD_ENCRYPTION_CONFIGURATION_ERROR")
    {
    }
}

public class PasswordEncryptionFailedException : PasswordEncryptionException
{
    public PasswordEncryptionFailedException(Exception innerException)
        : base("Password encryption failed. Please try again.", "PASSWORD_ENCRYPTION_FAILED", innerException)
    {
    }
}

public class DecryptionFailedException : PasswordEncryptionException
{
    public DecryptionFailedException(Exception innerException)
        : base("Decryption failed. Please try again.", "DECRYPTION_FAILED", innerException)
    {
    }
}

public class PasswordHashingException : PasswordEncryptionException
{
    public PasswordHashingException(Exception innerException)
        : base("Password hashing failed. Please try again.", "PASSWORD_HASHING_ERROR", innerException)
    {
    }
}