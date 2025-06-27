using DAL.Data;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly JobManagementContext _context;
    private readonly DbSet<T> table;
    public GenericRepository(JobManagementContext context)
    {
        _context = context;
        table = context.Set<T>();
    }

    public async Task<T> AddAsync(T entity)
    {
        try
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }
            var addedEntity = await table.AddAsync(entity);
            await SaveChangesAsync();
            return addedEntity.Entity;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task DeleteAsync(T entity)
    {
        try
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }
            table.Remove(entity);
            await SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }

    }

    public IQueryable<T> GetAll()
    {
        try
        {
            return table.AsQueryable();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        try
        {
            var res = await table.FindAsync(id) ?? throw new Exception("Entity not found");
            return res;
        }
        catch (Exception)
        {
            throw;
        }

    }

    public async Task SaveChangesAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task UpdateAsync(T entity)
    {
        try
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }
            table.Update(entity);
            await SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

}
