using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using webapi.Models;

namespace webapi.DL.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        private readonly DataContext _context;

        private readonly DbSet<T> _entities;


        public BaseRepository(DataContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }
        public async Task<IEnumerable<T>> GetAll()
        {
            return await _entities.ToListAsync();
        }
        public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> exp)
        {
            return await _entities.SingleOrDefaultAsync(exp);
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> exp)
        {
            return _entities.Where(exp);
        }
        public async Task AddAsync(T entity)
        {
            await _entities.AddAsync(entity);
        }
        public async Task Update(T entity)
        {
            var oldEntity = await _context.FindAsync<T>(entity.Id);
            _context.Entry(oldEntity).CurrentValues.SetValues(entity);
        }
        public void Delete(T entity)
        {
            _entities.Remove(entity);
        }

        public void DeleteRange(List<T> entities)
        {
            _entities.RemoveRange(entities);
        }

        public async Task<int> SaveChanges()
        {
            return await _context.SaveChangesAsync();
        }

    }
}
