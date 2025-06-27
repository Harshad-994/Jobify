using DAL.Data;
using DAL.Data.Models;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class CandidateRepository : GenericRepository<User>, ICandidateRepository
{
    private readonly JobManagementContext _context;
    public CandidateRepository(JobManagementContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ApplicationException("Email cannot be null or empty.");
            }
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }
        catch (ApplicationException)
        {
            throw;
        }

    }

}

