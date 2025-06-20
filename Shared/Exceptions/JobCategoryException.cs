namespace Shared.Exceptions;

public abstract class JobCategoryException : ApplicationException
{
    protected JobCategoryException(string message, string errorCode) : base(message, errorCode)
    {
    }

    protected JobCategoryException(string message, string errorCode, Exception innerException) : base(message, errorCode, innerException)
    {
    }
}

public class JobCategoryNotFoundException : JobCategoryException
{
    public JobCategoryNotFoundException(Guid categoryId)
            : base($"Job category with ID '{categoryId}' was not found.", "JOB_CATEGORY_NOT_FOUND")
    {
    }
}

public class DuplicateCategoryNameException : JobCategoryException
{
    public DuplicateCategoryNameException(string name)
        : base($"Category with name '{name}' already exists.", "DUPLICATE_CATEGORY_NAME")
    {
    }
}

public class CategoryHasActiveJobsException : JobCategoryException
{
    public CategoryHasActiveJobsException(Guid categoryId)
        : base($"Category with ID '{categoryId}' has active jobs and cannot be deleted.", "CATEGORY_HAS_ACTIVE_JOBS")
    {
    }
}