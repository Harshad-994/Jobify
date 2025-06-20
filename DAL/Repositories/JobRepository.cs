using DAL.Data;
using DAL.Data.Models;
using DAL.Interfaces;

namespace DAL.Repositories;

public class JobRepository : GenericRepository<JobPosting>, IJobRepository
{
    public JobRepository(JobManagementContext context) : base(context)
    {
    }

}
