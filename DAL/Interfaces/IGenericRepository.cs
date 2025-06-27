namespace DAL.Interfaces;

public interface IGenericRepository<T> where T : class
{
    IQueryable<T> GetAll();
    Task<T> GetByIdAsync(Guid id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task SaveChangesAsync();
}
