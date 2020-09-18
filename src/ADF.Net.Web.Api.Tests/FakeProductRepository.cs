using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ADF.Net.Data;
using ADF.Net.Data.DataAccess.EF;
using ADF.Net.Data.DataEntities;
using Microsoft.EntityFrameworkCore;

namespace ADF.Net.Web.Api.Tests
{
    internal class FakeProductRepository : IRepository<Product>
    {
        private Product[] _items;
        private readonly IQueryable<Product> _dbSet;

        private bool _disposed;
        private readonly IDbContext _context;


        public FakeProductRepository(IDbContext context, Product[] items)
        {
            _items = items;
            _context = context;
            _dbSet = _items.AsQueryable();
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

        ~FakeProductRepository()
        {
            Dispose(false);
        }

        public IQueryable<Product> Get()
        {
            return _dbSet;
        }

        public IQueryable<Product> GetNoTracking()
        {
            return _dbSet.AsNoTracking();

        }

        public IIncludableJoin<Product, TProperty> Join<TProperty>(Expression<Func<Product, TProperty>> navigationProperty)
        {
            var query = _dbSet.Join(navigationProperty);
            return query;
        }

        public Task<IQueryable<Product>> GetAsync()
        {
            return Task.FromResult(_dbSet);
        }

        public IQueryable<Product> Get(string sql)
        {

            throw new NotImplementedException();
        }

        public Task<IQueryable<Product>> GetAsync(string sql)
        {
            throw new NotImplementedException();
        }

        public Product Get(Expression<Func<Product, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public Product GetNoTracking(Expression<Func<Product, bool>> predicate)
        {
            return _dbSet.AsNoTracking().FirstOrDefault(predicate);
        }

        public Task<Product> GetAsync(Expression<Func<Product, bool>> predicate)
        {
            return _dbSet.FirstOrDefaultAsync(predicate);
        }

        public Product Add(Product entity, bool autoSave = false)
        {
            var list = _items.ToList();

            list.Add(entity);

            _items = list.ToArray();
            if (autoSave)
            {
                SaveChanges();
            }
            return entity;

        }

        public Task<Product> AddAsync(Product entity, bool autoSave = false)
        {
            var list = _items.ToList();

            list.Add(entity);

            _items = list.ToArray();

            if (autoSave)
            {
                SaveChangesAsync();
            }
            return Task.FromResult(entity);
        }

        public Product Update(Product entity, bool autoSave = false)
        {
            var list = _items.ToList();
            var existItem = list.FirstOrDefault(x => x.Id == entity.Id);
            list.Remove(existItem);
            list.Add(entity);
            if (autoSave)
            {
                SaveChanges();
            }
            return entity;
        }

        public Task<Product> UpdateAsync(Product entity, bool autoSave = false)
        {
            var list = _items.ToList();
            var existItem = list.FirstOrDefault(x => x.Id == entity.Id);
            list.Remove(existItem);
            list.Add(entity);
            if (autoSave)
            {
                SaveChangesAsync();
            }
            return Task.FromResult(entity);
        }

        public void Delete(Product entity, bool autoSave = false)
        {
            var list = _items.ToList();
            var existItem = list.FirstOrDefault(x => x.Id == entity.Id);
            list.Remove(existItem);
            if (autoSave)
            {
                SaveChanges();
            }
        }

        public Task DeleteAsync(Product entity, bool autoSave = false)
        {
            var list = _items.ToList();
            var existItem = list.FirstOrDefault(x => x.Id == entity.Id);
            list.Remove(existItem);
            if (autoSave)
            {
                SaveChangesAsync();
            }
            return Task.CompletedTask;
        }

        public int SaveChanges()
        {
            return 0;
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult(0);
        }



    }
}
