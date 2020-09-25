using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ADF.Net.Core;
using ADF.Net.Data.DataAccess.EntityFramework;
using ADF.Net.Data.DataEntities;
using Microsoft.EntityFrameworkCore;

namespace ADF.Net.Web.Api.Tests
{
    internal class FakeProductRepository : IRepository<Product>
    {
        private readonly IQueryable<Product> _dbSet;

        private bool _disposed;
        private readonly IDbContext _context;

        private Product[] _items =
        {
            new Product
            {
                Id = Guid.Parse("5d981459-8f5a-4fb6-ba4a-479590917876"),
                Code="Product1",
                CreationTime = DateTime.Now,
                Description = string.Empty,
                DisplayOrder = 1,
                IsApproved = true,
                LastModificationTime = DateTime.Now,
                Name = "Product 1",
                Version = 1,
                UnitPrice = 1.20m,
                Category = new Category
                {
                    Id = Guid.Parse("5d981459-8f5a-4fb6-ba4a-479590917876"),
                    Code="Category1",
                    CreationTime = DateTime.Now,
                    Description = string.Empty,
                    DisplayOrder = 1,
                    IsApproved = true,
                    LastModificationTime = DateTime.Now,
                    Name = "Category 1",
                    Version = 1
                },
            },
            new Product
            {
                Id = Guid.Parse("208d799a-ee2f-4d07-bfe3-d5928cc6cf30"),
                Code="Product2",
                CreationTime = DateTime.Now,
                Description = string.Empty,
                DisplayOrder = 1,
                IsApproved = true,
                LastModificationTime = DateTime.Now,
                Name = "Product 2",
                Version = 1, 
                UnitPrice = 10.25m,
                Category = new Category
                {
                    Id = Guid.Parse("5d981459-8f5a-4fb6-ba4a-479590917876"),
                    Code="Category1",
                    CreationTime = DateTime.Now,
                    Description = string.Empty,
                    DisplayOrder = 1,
                    IsApproved = true,
                    LastModificationTime = DateTime.Now,
                    Name = "Category 1",
                    Version = 1
                },
            },
            new Product
            {
                Id = Guid.Parse("2501203a-1484-4394-8c48-ed2a29d10fb9"),
                Code="Product3",
                CreationTime = DateTime.Now,
                Description = string.Empty,
                DisplayOrder = 1,
                IsApproved = true,
                LastModificationTime = DateTime.Now,
                Name = "Product 3",
                Version = 1,
                UnitPrice = 25,
                Category = new Category
                {
                    Id = Guid.Parse("5d981459-8f5a-4fb6-ba4a-479590917876"),
                    Code="Category1",
                    CreationTime = DateTime.Now,
                    Description = string.Empty,
                    DisplayOrder = 1,
                    IsApproved = true,
                    LastModificationTime = DateTime.Now,
                    Name = "Category 1",
                    Version = 1
                },
            }
        };
        public FakeProductRepository(IDbContext context)
        {
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
