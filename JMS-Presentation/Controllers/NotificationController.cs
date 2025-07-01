using System.Threading.Tasks;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace JMS_Presentation.Controllers;

[Authorize]
public class NotificationController : Controller
{
    private readonly INotificationService _notificationService;
    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<List<NotificationDto>> GetUnreadNotifications()
    {
        var userId = User.GetUserId();
        if (Guid.Empty == userId)
        {
            return new List<NotificationDto> { };
        }

        var unreadNotifications = await _notificationService.GetUnreadNotifications(userId);
        return unreadNotifications;
    }

    [HttpPost]
    public async Task<IActionResult> MarkNotificationAsRead(Guid notificationId)
    {
        var notification = await _notificationService.MarkNotificationAsReadAsync(notificationId);
        return Ok(notification);
    }

    [HttpGet]
    public async Task<IActionResult> GetUnreadNotificationCount()
    {
        var userId = User.GetUserId();
        if (Guid.Empty == userId)
        {
            return RedirectToAction("Login", "Account");
        }
        var notificationCount = await _notificationService.GetCountOfNotification(userId);
        return Ok(notificationCount);
    }

}

