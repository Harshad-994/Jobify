namespace Shared.Exceptions;
public abstract class JobApplicationException : ApplicationException
{
    protected JobApplicationException(string message, string errorCode) : base(message, errorCode)
    {
    }

    protected JobApplicationException(string message, string errorCode, Exception innerException)
        : base(message, errorCode, innerException)
    {
    }
}

public class DuplicateApplicationException : JobApplicationException
{
    public DuplicateApplicationException(Guid candidateId, Guid jobPostingId)
        : base($"Candidate has already applied for this job.", "DUPLICATE_APPLICATION")
    {
    }
}

public class JobApplicationNotFoundException : JobApplicationException
{
    public JobApplicationNotFoundException(Guid applicationId)
        : base($"Job application with ID '{applicationId}' was not found.", "JOB_APPLICATION_NOT_FOUND")
    {
    }
}

public class InvalidApplicationStatusException : JobApplicationException
{
    public InvalidApplicationStatusException(int status)
        : base($"Invalid application status: {status}.", "INVALID_APPLICATION_STATUS")
    {
    }
}

public class JobApplicationSystemException : JobApplicationException
{
    public JobApplicationSystemException(string operation, Exception innerException)
        : base($"A system error occurred during {operation}. Please try again later.", "JOB_APPLICATION_SYSTEM_ERROR", innerException)
    {
    }
}

public class InvalidApplicationStatusToWithdrawException : JobApplicationException
{
    public InvalidApplicationStatusToWithdrawException(int status)
        : base($"Invalid application status to withdraw it: {status}.", "INVALID_APPLICATION_STATUS_TO_WITHDRAW")
    {

    }

}