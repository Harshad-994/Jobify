using DAL.Data;
using DAL.Data.Models;
using DAL.Interfaces;

namespace DAL.Repositories;

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(JobManagementContext context) : base(context)
    {
    }

}
