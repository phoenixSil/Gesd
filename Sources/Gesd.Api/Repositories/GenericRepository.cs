using System.Linq.Expressions;

using Gesd.Api.Context;
using Gesd.Api.Repositories.Contrats;

using Microsoft.EntityFrameworkCore;

namespace Gesd.Api.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T>
         where T : class
    {
        protected readonly GesdContext _dbContext;

        public GenericRepository(GesdContext context)
        {
            _dbContext = context;
        }

        public async Task<T> Add(T entite)
        {
            await _dbContext.AddAsync(entite);
            await _dbContext.SaveChangesAsync();
            return entite;
        }

        public async Task<IEnumerable<T>> Get(params Expression<Func<T, object>>[] includes)
        {
            if(includes == null || includes.Length == 0)
            {
                return await _dbContext.Set<T>().ToListAsync();
            }

            IQueryable<T> query = _dbContext.Set<T>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public async Task<T> Get(Guid id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<T> Update(T entite)
        {
            try
            {
                _dbContext.Entry(entite).State = EntityState.Modified;
                return entite;
            }
            catch (Exception)
            {

                return null;
            }

        }

        public async Task<bool> Delete(T entite)
        {
            _dbContext.Set<T>().Remove(entite);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Exists(Guid id)
        {
            var entity = await Get(id);
            return entity != null;
        }
    }
}
