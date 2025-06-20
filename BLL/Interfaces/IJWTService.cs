using System.Security.Claims;

namespace BLL.Interfaces;

public interface IJWTService
{
    string GenerateToken(string userId, string email, string role, string name);
    ClaimsPrincipal? ValidateToken(string token);
    string GenerateRefreshToken(string userId);
    string ExtractFromToken(string token, string key);
}
