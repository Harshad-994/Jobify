using DAL.Data;
using DAL.Data.Models;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class JobApplicationRepository : GenericRepository<JobApplication>, IJobApplicationRepository
{
    private readonly JobManagementContext _context;
    public JobApplicationRepository(JobManagementContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(Guid jobId, Guid candidateId)
    {
        try
        {
            if (jobId == Guid.Empty || candidateId == Guid.Empty)
            {
                throw new Exception("Job ID and candidate ID cannot be empty.");
            }
            return await _context.JobApplications.AnyAsync(j => j.Id == jobId && j.UserId == candidateId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<int> CountByJobAsync(Guid jobId)
    {
        try
        {
            if (jobId == Guid.Empty)
            {
                throw new Exception("Job ID cannot be empty.");
            }
            return await _context.JobApplications.CountAsync(j => j.Id == jobId);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
