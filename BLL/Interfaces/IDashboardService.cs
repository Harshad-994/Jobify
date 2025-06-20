using Shared.DTOs;

namespace BLL.Interfaces;

public interface IDashboardService
{
    public Task<AdminDashboardStatsDto> GetAdminDashboardStats();
}
