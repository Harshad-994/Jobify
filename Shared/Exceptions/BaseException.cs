namespace Shared.Exceptions
{
    public abstract class ApplicationException : Exception
    {
        public string ErrorCode { get; }

        protected ApplicationException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        protected ApplicationException(string message, string errorCode, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }

}