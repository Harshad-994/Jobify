namespace Shared.Exceptions;

public abstract class JobPostingException : ApplicationException
{
    protected JobPostingException(string message, string errorCode) : base(message, errorCode)
    {
    }

    protected JobPostingException(string message, string errorCode, Exception innerException) : base(message, errorCode, innerException)
    {
    }
}

public class JobPostingNotFoundException : JobPostingException
{
    public JobPostingNotFoundException(Guid jobPostingId)
            : base($"Job posting with ID '{jobPostingId}' was not found.", "JOB_POSTING_NOT_FOUND")
    {
    }
}

public class JobPostingNotActiveException : JobPostingException
{
    public JobPostingNotActiveException(Guid jobPostingId)
        : base($"Job posting {jobPostingId} is not active and cannot accept applications.", "JOB_POSTING_NOT_ACTIVE")
    {
    }
}

public class InvalidJobClosingDateException : JobPostingException
{
    public InvalidJobClosingDateException(DateOnly closingDate)
        : base($"Job closing date cannot be in the past {closingDate:MM/dd/yyyy}", "INVALID_JOB_CLOSING_DATE")
    {
    }
}

public class JobPostingExpiredException : JobPostingException
{
    public JobPostingExpiredException(Guid jobPostingId, DateOnly closingDate)
        : base($"Job posting {jobPostingId} has already closed on {closingDate:yyyy-MM-dd}.", "JOB_POSTING_EXPIRED")
    {
    }
}

public class DeleteJobWithApplicationException : JobPostingException
{
    public DeleteJobWithApplicationException(Guid jobPostingId)
        : base($"Cannot delete job with application {jobPostingId}.", "DELETE_JOB_WITH_APPLICATION")
    {
    }
}