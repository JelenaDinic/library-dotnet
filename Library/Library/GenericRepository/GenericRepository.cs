using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Library.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbContext _context;
        private DbSet<T> table;

        public GenericRepository(DbContext _context)
        {
            this._context = _context;
            table = _context.Set<T>();
        }
        public async Task<IEnumerable<T>> GetAll()
        {
            return await table.ToListAsync();
        }
        public async Task<T?> GetByIdAsync(int id)
        {
            return await table.FindAsync(id);
        }
        public async Task InsertAsync(T entity)
        {
            await table.AddAsync(entity);
            await SaveAsync();
        }
        public void Update(T entity)
        {
            table.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
        public void Delete(T entity)
        {
            table.Remove(entity);
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        public IQueryable<T> Search(Expression<Func<T, bool>> expression)
        {
            return table.Where(expression);
        }

        public DbSet<T> ExposeTable()
        {
            return table;
        }
    }
}
