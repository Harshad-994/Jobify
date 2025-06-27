using System.Net;
using System.Text.Json;
using Shared.Exceptions;
using Shared.Models;

namespace JMS_Presentation.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (!IsBusinessException(ex))
            {
                _logger.LogError(ex, "An unhandled exception occurred");
            }
            await HandleExceptionAsync(context, ex);
        }
    }

    private static bool IsBusinessException(Exception ex)
    {
        return ex is JobApplicationException ||
               ex is JobPostingException ||
               ex is JobCategoryException ||
               ex is UserException ||
               ex is AccountException ||
               ex is CloudinaryException ||
               ex is JWTException ||
               ex is PasswordEncryptionException ||
               ex is ArgumentException;
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse();

        switch (exception)
        {
            case ArgumentException ex:
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "Invalid input parameters in request.",
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
                break;

            case CandidateNotFoundException ex:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "Candidate not found. Please check your login status.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.NotFound
                };
                break;

            case UserNotFoundException ex:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "User not found with this email.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.NotFound
                };
                break;

            case UserException ex:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "A system error occurred. Please try again later.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
                break;

            case InvalidCurrentPasswordException ex:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "Your password is incorrect.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
                break;

            case UserNotActiveException ex:
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "Your account is not active. Please contact support.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.Forbidden
                };
                break;

            case EmailAlreadyExistsException ex:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "This email is already used.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.Conflict
                };
                break;

            case AccountException ex:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "A system error occurred. Please try again later.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
                break;

            case NullOrCorruptFileException ex:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "The uploaded file is corrupted or invalid. Please try uploading a different file.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
                break;

            case InvalidFileTypeException ex:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "Please upload a PDF file only.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
                break;

            case FileSizeExceededException ex:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "File size is too large. Please upload a file smaller than 2MB.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
                break;

            case InvalidCloudinaryFileUrlException ex:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "Invalid file URL. Please try uploading the file again.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
                break;

            case CloudinarySystemException ex:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "File upload service is temporarily unavailable. Please try again later.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
                break;

            case DuplicateApplicationException ex:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "You have already applied for this job.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.Conflict
                };
                _logger.LogWarning("Duplicate application: {Message}", ex.Message);
                break;

            case JobApplicationNotFoundException ex:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "Job application not found.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.NotFound
                };
                break;

            case InvalidApplicationStatusException ex:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "Invalid application status provided.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
                break;

            case InvalidApplicationStatusToWithdrawException ex:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "can't withdraw application in current status.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
                break;

            case JobApplicationSystemException ex:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "A system error occurred. Please try again later.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
                break;

            case JobCategoryNotFoundException ex:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "Job category not found.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.NotFound
                };
                break;

            case DuplicateCategoryNameException ex:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "This category name already exists.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.Conflict
                };
                break;

            case CategoryHasActiveJobsException ex:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "Can't delete category with active jobs.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.Conflict
                };
                break;

            case JobCategoryException ex:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "A system error occurred. Please try again later.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
                break;


            case JobPostingNotFoundException ex:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "Job posting not found or no longer available.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.NotFound
                };
                break;

            case JobPostingNotActiveException ex:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "This job posting is not accepting applications.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
                break;

            case InvalidJobClosingDateException ex:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "Job closing date can't be in the past.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
                break;

            case JobPostingExpiredException ex:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "This job posting has already closed.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
                break;

            case DeleteJobWithApplicationException ex:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "can't delete job with applications.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.Conflict
                };
                break;

            case JobPostingException ex:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "A system error occurred. Please try again later.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
                break;

            case JWTConfigurationException ex:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "System configuration error. Please contact support.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
                break;

            case TokenGenerationException ex:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "Authentication service error. Please try again.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
                break;

            case TokenValidationException ex:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "Invalid or expired token. Please login again.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.Unauthorized
                };
                break;

            case TokenExtractionException ex:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "Invalid token data. Please login again.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.Unauthorized
                };
                break;

            case JWTServiceException ex:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "Authentication service error. Please try again.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
                break;

            case JWTException ex:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "Authentication error occurred. Please try again.",
                    ErrorCode = ex.ErrorCode ?? "JWT_ERROR",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
                break;

            case PasswordEncryptionConfigurationException ex:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "System configuration error. Please contact support.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
                break;

            case DecryptionFailedException ex:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "Error occured with the decryption.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
                break;

            case KeyDerivationException ex:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "Password processing failed. Please try again.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
                break;

            case PasswordEncryptionFailedException ex:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "Password processing failed. Please try again.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
                break;

            case PasswordHashingException ex:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "Password processing failed. Please try again.",
                    ErrorCode = ex.ErrorCode,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
                break;

            case PasswordEncryptionException ex:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "Password processing error. Please try again.",
                    ErrorCode = ex.ErrorCode ?? "PASSWORD_ENCRYPTION_ERROR",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
                break;

            case NotificationNotFoundException ex:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "Notification not found. Please check your request.",
                    ErrorCode = ex.ErrorCode ?? "NOTIFICATION_NOT_FOUND",
                    StatusCode = (int)HttpStatusCode.NotFound
                };
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "An unexpected error occurred.",
                    ErrorCode = "INTERNAL_SERVER_ERROR",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }

}