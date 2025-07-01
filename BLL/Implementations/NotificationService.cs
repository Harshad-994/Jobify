using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Data.Models;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.DTOs;
using Shared.Exceptions;

namespace BLL.Implementations;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<NotificationService> _logger;
    public NotificationService(INotificationRepository notificationRepository, ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _logger = logger;
    }

    public async Task<NotificationDto> CreateNotificationAsync(NotificationDto notificationDto)
    {
        if (notificationDto == null)
        {
            _logger.LogWarning("NotificationDto cannot be null.");
            throw new ArgumentNullException(nameof(notificationDto));
        }
        var notification = new Notification
        {
            Title = notificationDto.Title,
            Message = notificationDto.Message,
            UserId = notificationDto.UserId,
            CreatedAt = DateTime.UtcNow,
        };
        var addedNotification = await _notificationRepository.AddAsync(notification);
        var addedNotificationDto = new NotificationDto
        {
            Id = addedNotification.Id,
            Title = addedNotification.Title,
            Message = addedNotification.Message,
            CreatedAt = addedNotification.CreatedAt,
        };
        return addedNotificationDto;
    }

    public async Task<List<NotificationDto>> GetUnreadNotifications(Guid userId)
    {
        if (Guid.Empty == userId)
        {
            _logger.LogWarning("UserId cannot be null.");
            throw new ArgumentNullException(nameof(userId));
        }
        var notifications = await _notificationRepository.GetAll().Where(n => n.UserId == userId && n.IsRead == false).Select(n => new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,
            Message = n.Message,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt,
            ReadAt = n.ReadAt,
        }).ToListAsync();

        return notifications;
    }

    public async Task<NotificationDto> MarkNotificationAsReadAsync(Guid notificationId)
    {
        if (Guid.Empty == notificationId)
        {
            _logger.LogWarning("NotificationId cannot be null.");
            throw new ArgumentNullException(nameof(notificationId));
        }
        var notification = await _notificationRepository.GetByIdAsync(notificationId);
        if (notification == null)
        {
            _logger.LogWarning("Notification with id {NotificationId} not found.", notificationId);
            throw new NotificationNotFoundException();
        }
        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        await _notificationRepository.UpdateAsync(notification);
        return new NotificationDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt,
            ReadAt = notification.ReadAt,
        };
    }

    public async Task<int> GetCountOfNotification(Guid userId)
    {
        if (Guid.Empty == userId)
        {
            _logger.LogWarning("UserId cannot be null.");
            throw new ArgumentNullException(nameof(userId));
        }
        var notifications = _notificationRepository.GetAll().Where(n => n.UserId == userId && !n.IsRead);
        return await notifications.CountAsync();
    }
}
