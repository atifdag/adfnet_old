using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ADF.Net.Core
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class, IEntity, new()
    {

        IQueryable<TEntity> Get();
        
        IQueryable<TEntity> GetNoTracking();
        
        IIncludableJoin<TEntity, TProperty> Join<TProperty>(Expression<Func<TEntity, TProperty>> navigationProperty);

        Task<IQueryable<TEntity>> GetAsync();

        IQueryable<TEntity> Get(string sql);

        Task<IQueryable<TEntity>> GetAsync(string sql);

        TEntity Get(Expression<Func<TEntity, bool>> predicate);

        TEntity GetNoTracking(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity Add(TEntity entity, bool autoSave = false);

        Task<TEntity> AddAsync(TEntity entity, bool autoSave = false);

        TEntity Update(TEntity entity, bool autoSave = false);

        Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false);

        void Delete(TEntity entity, bool autoSave = false);

        Task DeleteAsync(TEntity entity, bool autoSave = false);

        int SaveChanges();
        
        Task<int> SaveChangesAsync();

    }
}