namespace Shared.Exceptions;

public abstract class JWTException : ApplicationException
{
    protected JWTException(string message, string errorCode) : base(message, errorCode)
    {
    }

    protected JWTException(string message, string errorCode, Exception innerException) : base(message, errorCode, innerException)
    {
    }

}

public class JWTConfigurationException : JWTException
{
    public JWTConfigurationException(string configKey)
        : base($"JWT configuration error in {configKey}. Please contact support.", "JWT_CONFIGURATION_ERROR")
    {
    }
}

public class TokenGenerationException : JWTException
{
    public TokenGenerationException(Exception innerException)
        : base("Token generation failed. Please try again.", "TOKEN_GENERATION_ERROR", innerException)
    {
    }
}

public class TokenValidationException : JWTException
{
    public TokenValidationException(Exception innerException)
        : base("Invalid or expired token. Error in token validation.", "TOKEN_VALIDATION_ERROR", innerException)
    {
    }
}

public class TokenExtractionException : JWTException
{
    public TokenExtractionException(string claimType, Exception innerException)
        : base("Failed to extract data from token. Error in token extraction.", "TOKEN_EXTRACTION_ERROR", innerException)
    {
    }
}

public class JWTServiceException : JWTException
{
    public JWTServiceException(string operation, Exception innerException)
        : base($"JWT service error during {operation}. Please try again.", "JWT_SERVICE_ERROR", innerException)
    {
    }
}