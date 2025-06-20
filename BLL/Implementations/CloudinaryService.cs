using BLL.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;

namespace BLL.Implementations;

public class CloudinaryService : ICloudinaryService
{
    private readonly IConfiguration _configuration;
    private readonly string cloudName;
    private readonly string apiKey;
    private readonly string apiSecret;
    private readonly ILogger<CloudinaryService> _logger;
    public CloudinaryService(IConfiguration configuration, ILogger<CloudinaryService> logger)
    {
        _configuration = configuration;
        cloudName = _configuration["Cloudinary:CloudName"]!;
        apiKey = _configuration["Cloudinary:ApiKey"]!;
        apiSecret = _configuration["Cloudinary:ApiSecret"]!;
        _logger = logger;
    }

    public async Task<string> UploadResumeFile(IFormFile file)
    {
        if (file == null || file.Length == 0 || file.ContentType == null)
        {
            _logger.LogWarning("File has no content or is null.");
            throw new NullOrCorruptFileException(file?.FileName ?? "NO FILE");
        }

        if (file.Length > 1024 * 1000 * 2)
        {
            _logger.LogWarning("File size exceeds the maximum allowed size 2MB.");
            throw new FileSizeExceededException(file.FileName, file.Length);
        }

        if (file.ContentType != "application/pdf")
        {
            _logger.LogWarning("File type is not PDF.");
            throw new InvalidFileTypeException(file.FileName);
        }
        var cloudinary = new Cloudinary(new Account(
        cloudName,
        apiKey,
        apiSecret
        ));

        using var stream = file.OpenReadStream();
        var uploadParams = new RawUploadParams()
        {
            File = new FileDescription(file.FileName, stream)
        };

        var uploadResult = await cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
        {
            _logger.LogWarning("Cloudinary upload failed. Error: {error}", uploadResult.Error.Message);
            throw new CloudinarySystemException("upload", new Exception(uploadResult.Error.Message));
        }
        return uploadResult.SecureUrl.ToString();

    }

    public async Task<bool> DeleteFileBySecureUrlAsync(string secureUrl)
    {
        if (string.IsNullOrWhiteSpace(secureUrl))
            throw new InvalidCloudinaryFileUrlException(secureUrl);

        var cloudinary = new Cloudinary(new Account(cloudName, apiKey, apiSecret));

        var uploadIndex = secureUrl.IndexOf("/upload/");
        if (uploadIndex == -1)
        {
            _logger.LogWarning("Invalid file secure URL format.");
            throw new InvalidCloudinaryFileUrlException(secureUrl);
        }

        var pathWithVersionAndExt = secureUrl[(uploadIndex + "/upload/".Length)..];

        var firstSlash = pathWithVersionAndExt.IndexOf('/');
        if (firstSlash == -1)
        {
            _logger.LogWarning("Invalid file secure URL format.");
            throw new InvalidCloudinaryFileUrlException(secureUrl);
        }

        var pathWithExt = pathWithVersionAndExt[(firstSlash + 1)..];

        var lastDot = pathWithExt.LastIndexOf('.');
        if (lastDot == -1)
        {
            _logger.LogWarning("Invalid file secure URL format.");
            throw new InvalidCloudinaryFileUrlException(secureUrl);
        }

        var publicId = pathWithExt[..lastDot];

        var deletionParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Raw
        };

        var result = await cloudinary.DestroyAsync(deletionParams);

        return result.Result == "ok";
    }

}
