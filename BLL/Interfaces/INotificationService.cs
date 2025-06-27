using Shared.DTOs;

namespace BLL.Interfaces;

public interface INotificationService
{
    Task<NotificationDto> CreateNotificationAsync(NotificationDto notificationDto);
    List<NotificationDto> GetUnreadNotifications(Guid userId);
    Task<NotificationDto> MarkNotificationAsReadAsync(Guid notificationId);
    Task<int> GetCountOfNotification(Guid userId);
}
