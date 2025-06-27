using System.Security.Claims;

namespace JMS_Presentation.Controllers
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user?.FindFirst("UserId")?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Guid.Empty;
            }
            return userId;
        }
    }
}

