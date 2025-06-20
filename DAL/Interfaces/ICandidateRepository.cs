using DAL.Data.Models;

namespace DAL.Interfaces;

public interface ICandidateRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}
