using Edu_Block_dev.DAL.EF;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Edu_Block.DAL;

public interface IRepository<T> where T : class
{
    DbContext _context { get; set; }
    IEnumerable<T> GetAll(IEnumerable<string> includes = null);
    IEnumerable<T> FindAll(Expression<Func<T, bool>> match, IEnumerable<string> includes = null);
    Task<IEnumerable<T>> GetAllAsync(IEnumerable<string> includes = null);
    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> match, IEnumerable<string> includes = null);
    T Find(Expression<Func<T, bool>> match, IEnumerable<string> includes = null);
    Task<T> FindAsync(Expression<Func<T, bool>> match, IEnumerable<string> includes = null);
    T Add(T entity);
    Task<T> AddAsync(T entity);
    void Delete<I>(I id);
    Task DeleteAsync<I>(I id);
    T Update<I>(I key, T entity);
    Task<T> UpdateAsync<I>(I key, T entity);
    void SetContextTracking(bool shouldTrackChanges);
    //Task<IEnumerable<PaymentDetail>> FindAllAsync();
}
