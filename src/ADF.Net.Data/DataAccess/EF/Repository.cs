using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ADF.Net.Core;
using Microsoft.EntityFrameworkCore;

namespace ADF.Net.Data.DataAccess.EF
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity, new()
    {
        private bool _disposed;
        private readonly EfDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(EfDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }
        
        ~Repository()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public IQueryable<TEntity> Get()
        {
            return _dbSet;
        }


        public IQueryable<TEntity> GetNoTracking()
        {
            return _dbSet.AsNoTracking();
        }

        public IIncludableJoin<TEntity, TProperty> Join<TProperty>(Expression<Func<TEntity, TProperty>> navigationProperty)
        {
            var query = _dbSet.Join(navigationProperty);
            return query;
        }

        public async Task<IQueryable<TEntity>> GetAsync()
        {
            return await Task.FromResult(_dbSet);
        }
        public IQueryable<TEntity> Get(string sql)
        {
            return _dbSet.FromSqlRaw(sql);
        }

        public async Task<IQueryable<TEntity>> GetAsync(string sql)
        {
            return await Task.FromResult(_dbSet.FromSqlRaw(sql));
        }

        public TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public TEntity GetNoTracking(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.AsNoTracking().FirstOrDefault(predicate);
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }
        public TEntity Add(TEntity entity, bool autoSave = false)
        {
            var entry = _dbSet.Add(entity);
            if (autoSave)
            {
                SaveChanges();
            }
            return entry.Entity;
        }

        public async Task<TEntity> AddAsync(TEntity entity, bool autoSave = false)
        {
            var entry = await _dbSet.AddAsync(entity);
            if (autoSave)
            {
                await SaveChangesAsync();
            }
            return entry.Entity;
        }

        public TEntity Update(TEntity entity, bool autoSave = false)
        {
            var entry = _dbSet.Update(entity);
            if (autoSave)
            {
                SaveChanges();
            }

            return entry.Entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false)
        {

            var affectedEntity = Task.FromResult(Update(entity));
            if (autoSave)
            {
                await SaveChangesAsync();
            }
            return await affectedEntity;
        }

        public void Delete(TEntity entity, bool autoSave = false)
        {
            _dbSet.Remove(entity);
            if (autoSave)
            {
                SaveChanges();
            }
        }

        public async Task DeleteAsync(TEntity entity, bool autoSave = false)
        {
            _dbSet.Remove(entity);
            if (autoSave)
            {
                await SaveChangesAsync();
            }
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}
