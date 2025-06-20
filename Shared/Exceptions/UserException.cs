namespace Shared.Exceptions;

public abstract class UserException : ApplicationException
{
    protected UserException(string message, string errorCode) : base(message, errorCode)
    {
    }

    protected UserException(string message, string errorCode, Exception innerException)
        : base(message, errorCode, innerException)
    {
    }
}

public class CandidateNotFoundException : UserException
{
    public CandidateNotFoundException(Guid candidateId)
                : base($"Candidate with ID '{candidateId}' was not found.", "CANDIDATE_NOT_FOUND")
    {
    }
}
