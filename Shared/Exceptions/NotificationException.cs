namespace Shared.Exceptions;

public abstract class NotificationException : ApplicationException
{
    protected NotificationException(string message, string errorCode) : base(message, errorCode)
    {
    }

    protected NotificationException(string message, string errorCode, Exception innerException) : base(message, errorCode, innerException)
    {
    }
}

public class NotificationNotFoundException : NotificationException
{
    public NotificationNotFoundException()
        : base($"Notification not found.", "NOTIFICATION_NOT_FOUND")
    {
    }
}