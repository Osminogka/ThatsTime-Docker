using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using webapi.Models;


namespace webapi.DL.Repositories
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> exp);
        IQueryable<T> Where(Expression<Func<T, bool>> exp);
        Task AddAsync(T entity);
        Task Update(T entity);
        void Delete(T entity);
        void DeleteRange(List<T> entities);
        Task<int> SaveChanges();
    }
}
