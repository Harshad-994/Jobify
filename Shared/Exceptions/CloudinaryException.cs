namespace Shared.Exceptions;

public abstract class CloudinaryException : ApplicationException
{
    protected CloudinaryException(string message, string errorCode) : base(message, errorCode)
    {
    }

    protected CloudinaryException(string message, string errorCode, Exception innerException) : base(message, errorCode, innerException)
    {
    }

}

public class NullOrCorruptFileException : CloudinaryException
{
    public NullOrCorruptFileException(string filename)
        : base($"The file {filename} is null or corrupt.", "NULL_OR_CORRUPT_FILE")
    {
    }
}

public class InvalidFileTypeException : CloudinaryException
{
    public InvalidFileTypeException(string filename)
        : base($"The file type of {filename} is invalid.", "INVALID_FILE_TYPE")
    {
    }
}

public class FileSizeExceededException : CloudinaryException
{
    public FileSizeExceededException(string filename, long maxSize)
        : base($"The file {filename} exceeds the maximum size of {maxSize} bytes.", "FILE_SIZE_EXCEEDED")
    {
    }
}

public class InvalidCloudinaryFileUrlException : CloudinaryException
{
    public InvalidCloudinaryFileUrlException(string url)
        : base($"The Cloudinary file URL {url} is invalid.", "INVALID_CLOUDINARY_FILE_URL")
    {
    }
}

public class CloudinarySystemException : CloudinaryException
{
    public CloudinarySystemException(string operation, Exception innerException)
        : base($"Cloudinary {operation} failed. Please try again.", "CLOUDINARY_SYSTEM_ERROR", innerException)
    {
    }
}