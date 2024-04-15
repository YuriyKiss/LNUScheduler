using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ScheduleLNU.DataAccess.Entities;

namespace ScheduleLNU.DataAccess.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly IdentityDbContext<Student> dataBaseContext;

        private readonly DbSet<TEntity> entitiesDataSet;

        public Repository(IdentityDbContext<Student> context)
        {
            dataBaseContext = context;
            entitiesDataSet = context.Set<TEntity>();
        }

        public async Task InsertAsync(TEntity entity)
        {
            entitiesDataSet.Add(entity);
            await dataBaseContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            dataBaseContext.Attach(entity).State = EntityState.Modified;
            await dataBaseContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            entitiesDataSet.Remove(entity);
            await dataBaseContext.SaveChangesAsync();
        }

        public virtual async Task<TEntity> SelectAsync(
            Expression<Func<TEntity, bool>> selector,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return await GetValueWithInclude(includeProperties).FirstOrDefaultAsync(selector);
        }

        public async Task<IEnumerable<TEntity>> SelectAllAsync(
            Expression<Func<TEntity, bool>> selector,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return await GetValueWithInclude(includeProperties).Where(selector).ToListAsync();
        }

        public async Task SetNullAsync(TEntity entity, string propertyName)
        {
            dataBaseContext.Attach(entity).Reference(propertyName).CurrentValue = null;
            await dataBaseContext.SaveChangesAsync();
        }

        private IQueryable<TEntity> GetValueWithInclude(
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> querriedEntities = entitiesDataSet;

            return includeProperties.Aggregate(
                querriedEntities,
                (current, includeProperty) => current.Include(includeProperty));
        }
    }
}
