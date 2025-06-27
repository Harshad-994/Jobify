using Microsoft.AspNetCore.SignalR;
using Shared.DTOs;

namespace JMS_Presentation.SignalR;

public class NotificationHub : Hub
{
    public async Task SendNotificationToUser(string userId, NotificationDto notification)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", notification);
    }
}