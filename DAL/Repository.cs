using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Edu_Block.DAL;

public class Repository<T> : IDisposable, IRepository<T> where T : class
{
    public DbContext _context { get; set; }
    protected readonly DbSet<T> _dbSet;


    public Repository(DbContext context, bool withTracking = false)
    {
        _context = context;
        _dbSet = _context.Set<T>();
        //disable tracking by default
        SetContextTracking(withTracking);
    }

    #region [Add Methods]
    public T Add(T entity)
    {
        _context.Add(entity);
        _context.SaveChanges();

        return entity;
    }

    public async Task<T> AddAsync(T entity)
    {
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();

        return entity;
    }
    #endregion

    #region [Update Methods]
    public T Update<I>(I key, T entity)
    {
        if (entity == null)
            return null;

        T existing = _dbSet.Find(key);
        if (existing != null)
        {
            _context.Entry(existing).State = EntityState.Modified;
            _context.Entry(existing).CurrentValues.SetValues(entity);
            _context.SaveChanges();
        }
        return existing;
    }

    public async Task<T> UpdateAsync<I>(I key, T entity)
    {
        if (entity == null)
            return null;

        T existing = await _dbSet.FindAsync(key);
        if (existing != null)
        {
            _context.Entry(existing).State = EntityState.Modified;
            _context.Entry(existing).CurrentValues.SetValues(entity);

            await _context.SaveChangesAsync();
        }
        return existing;
    }

    /// <summary>
    /// Used to enable/disable the change tracker.
    /// </summary>
    /// <param name="autoDetectChanges"></param>
    public void SetContextTracking(bool autoDetectChanges)
    {
        //enable-disable change detection
        _context.ChangeTracker.AutoDetectChangesEnabled = autoDetectChanges;
    }

    #endregion

    #region [Delete Methods]

    public void Delete<I>(I id)
    {
        var existing = _dbSet.Find(id);
        if (existing != null)
        {
            _dbSet.Remove(existing);
            _context.SaveChanges();
        }
    }

    public async Task DeleteAsync<I>(I id)
    {
        var existing = await _dbSet.FindAsync(id);
        if (existing != null)
        {
            _dbSet.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
    #endregion

    #region [Find Methods]

    protected IQueryable<T> Filter(IQueryable<T> queryable, Expression<Func<T, bool>> match, IEnumerable<string> includes = null)
    {
        var query = queryable.Where(match);
        if (includes != null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        return query;
    }

    public T Find(Expression<Func<T, bool>> match, IEnumerable<string> includes = null)
    {
        return Filter(_dbSet, match, includes).FirstOrDefault();
    }

    public async Task<T> FindAsync(Expression<Func<T, bool>> match, IEnumerable<string> includes = null)
    {
        return await Filter(_dbSet, match, includes).FirstOrDefaultAsync();
    }

    public IEnumerable<T> FindAll(Expression<Func<T, bool>> match, IEnumerable<string> includes = null)
    {
        return Filter(_dbSet, match, includes).AsEnumerable();
    }

    public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> match, IEnumerable<string> includes = null)
    {
        return await Filter(_dbSet, match, includes).ToListAsync();
    }

    public IEnumerable<T> GetAll(IEnumerable<string> includes = null)
    {
        return Filter(_dbSet, _ => true, includes).AsEnumerable();
    }

    public async Task<IEnumerable<T>> GetAllAsync(IEnumerable<string> includes = null)
    {
        return await Filter(_dbSet, _ => true, includes).ToListAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    #endregion
}
