using DAL.Data.Models;

namespace DAL.Interfaces;

public interface ICategoryRepository : IGenericRepository<JobCategory>
{
    Task<bool> ExistsByNameAsync(string name,Guid? id);
    Task<bool> HasActiveJobsAsync(Guid categoryId);
}
