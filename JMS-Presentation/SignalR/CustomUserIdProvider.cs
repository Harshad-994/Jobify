using Microsoft.AspNetCore.SignalR;

namespace JMS_Presentation.SignalR;

public class CustomUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst("UserId")?.Value;
    }
}
