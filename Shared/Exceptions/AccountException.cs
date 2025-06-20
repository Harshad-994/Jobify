namespace Shared.Exceptions;

public abstract class AccountException : ApplicationException
{
    protected AccountException(string message, string errorCode) : base(message, errorCode)
    {
    }

    protected AccountException(string message, string errorCode, Exception innerException) : base(message, errorCode, innerException)
    {
    }

}

public class InvalidCurrentPasswordException : AccountException
{
    public InvalidCurrentPasswordException(string email)
        : base($"Invalid current password for this {email}. Please try again.", "INVALID_CURRENT_PASSWORD")
    {
    }
}

public class UserNotActiveException : AccountException
{
    public UserNotActiveException(string email)
        : base($"User with email {email} is not active.", "USER_NOT_ACTIVE")
    {
    }
}

public class UserNotFoundException : AccountException
{
    public UserNotFoundException(string email)
        : base($"User with email {email} not found.", "USER_NOT_FOUND")
    {
    }
}

public class EmailAlreadyExistsException : AccountException
{
    public EmailAlreadyExistsException(string email)
        : base($"Email {email} already exists. Please choose a different email.", "EMAIL_ALREADY_EXISTS")
    {
    }
}