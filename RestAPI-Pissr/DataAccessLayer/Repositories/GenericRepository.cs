using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{

    protected readonly ApiDbContext _dbContext;
    private static readonly SemaphoreSlim RateLimit = new SemaphoreSlim(1, 1);


    public GenericRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<T>> GetAll()
    {
        return await _dbContext.Set<T>().ToListAsync();
    }

    public async Task<T?> GetById(int id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public async Task<T> Insert(T entity)
    {
        await RateLimit.WaitAsync();

        try
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }
        finally
        {
            RateLimit.Release();
        }
        return entity;
    }

    public async Task<T> Update(T entity)
    {
       _dbContext.Set<T>().Update(entity);
       return entity;
    }

    public async Task<T> Delete(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        return entity;
    }
}