using DAL.Data.Models;

namespace DAL.Interfaces;

public interface IJobApplicationRepository : IGenericRepository<JobApplication>
{
    Task<bool> ExistsAsync(Guid jobId, Guid candidateId);
    Task<int> CountByJobAsync(Guid jobId);
}
