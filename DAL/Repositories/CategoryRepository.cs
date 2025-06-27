using DAL.Data;
using DAL.Data.Models;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class CategoryRepository : GenericRepository<JobCategory>, ICategoryRepository
{
    private readonly JobManagementContext _context;
    public CategoryRepository(JobManagementContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid? id = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ApplicationException("Category name cannot be null or empty.");
            }
            return await _context.JobCategories.AnyAsync(c => c.Name.ToLower().Trim() == name.ToLower().Trim() && (id == null || c.Id != id));
        }
        catch (ApplicationException)
        {
            throw;
        }
    }

    public async Task<bool> HasActiveJobsAsync(Guid categoryId)
    {
        try
        {
            var category = await _context.JobCategories.Include(j => j.JobPostings).FirstOrDefaultAsync(c => c.Id == categoryId) ?? throw new ApplicationException("Category not found.");
            return category.JobPostings.Any(j => j.IsActive == true);
        }
        catch (ApplicationException)
        {
            throw;
        }
    }
}
