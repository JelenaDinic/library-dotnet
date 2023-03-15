using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Library.GenericRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetByIdAsync(int id);
        Task InsertAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        IQueryable<T> Search(Expression<Func<T, bool>> expression);
        Task SaveAsync();
        DbSet<T> ExposeTable();
    }
}
