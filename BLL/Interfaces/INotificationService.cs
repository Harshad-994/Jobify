using Shared.DTOs;

namespace BLL.Interfaces;

public interface INotificationService
{
    Task<NotificationDto> CreateNotificationAsync(NotificationDto notificationDto);
    Task<List<NotificationDto>> GetUnreadNotifications(Guid userId);
    Task<NotificationDto> MarkNotificationAsReadAsync(Guid notificationId);
    Task<int> GetCountOfNotification(Guid userId);
}
