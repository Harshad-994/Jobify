using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BLL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Shared.Exceptions;

namespace BLL.Implementations;

public class JWTService : IJWTService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JWTService> _logger;
    private readonly string _jwtKey;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;

    public JWTService(IConfiguration configuration, ILogger<JWTService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        _jwtKey = _configuration["Jwt:Key"] ?? throw new JWTConfigurationException("Jwt:Key");
        _jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new JWTConfigurationException("Jwt:Issuer");
        _jwtAudience = _configuration["Jwt:Audience"] ?? throw new JWTConfigurationException("Jwt:Audience");

        if (string.IsNullOrWhiteSpace(_jwtKey) || _jwtKey.Length < 32)
        {
            _logger.LogWarning("Jwt key is invalid or too short.");
            throw new JWTConfigurationException("Jwt:Key");
        }
    }

    public string GenerateToken(string userId, string email, string role, string name)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("UserId", userId),
                new Claim("Email", email),
                new Claim("Name",name),
                new Claim(ClaimTypes.Role,role)
            }),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = _jwtIssuer,
                Audience = _jwtAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to generate token. Error in token generation.", ex);
            throw new TokenGenerationException(ex);
        }

    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtIssuer,
                ValidAudience = _jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken) ?? throw new Exception("Invalid jwt token");
            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to validate token. Error in token validation.", ex);
            throw new TokenValidationException(ex);
        }
    }

    public string GenerateRefreshToken(string userId)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("UserId", userId)
            }),
                Expires = DateTime.UtcNow.AddDays(30),
                Issuer = _jwtIssuer,
                Audience = _jwtAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to generate refresh token. Error in token generation.", ex);
            throw new TokenGenerationException(ex);
        }

    }

    public string ExtractFromToken(string token, string key)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning("Jwt Token cannot be null or empty", nameof(token));
            throw new ArgumentException("Token cannot be null or empty", nameof(token));
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            _logger.LogWarning("Jwt Key cannot be null or empty", nameof(key));
            throw new ArgumentException("Claim type cannot be null or empty", nameof(key));
        }
        try
        {
            var principal = ValidateToken(token) ?? throw new Exception("Invalid jwt token");
            return principal!.Claims.FirstOrDefault(t => t.Type == key)!.Value;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to extract data from token. Error in token extraction.", ex);
            throw new TokenExtractionException(key, ex);
        }

    }
}
