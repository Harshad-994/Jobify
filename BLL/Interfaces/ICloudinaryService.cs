using Microsoft.AspNetCore.Http;

namespace BLL.Interfaces;

public interface ICloudinaryService
{
    Task<string> UploadResumeFile(IFormFile file);
    Task<bool> DeleteFileBySecureUrlAsync(string secureUrl);
}
