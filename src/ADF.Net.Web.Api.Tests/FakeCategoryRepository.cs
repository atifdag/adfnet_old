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
    internal class FakeCategoryRepository : IRepository<Category>
    {
        private readonly IQueryable<Category> _dbSet;

        private Category[] _items =
        {
            new Category
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
            new Category
            {
                Id = Guid.Parse("208d799a-ee2f-4d07-bfe3-d5928cc6cf30"),
                Code="Category2",
                CreationTime = DateTime.Now,
                Description = string.Empty,
                DisplayOrder = 1,
                IsApproved = true,
                LastModificationTime = DateTime.Now,
                Name = "Category 2",
                Version = 1
            },
            new Category
            {
                Id = Guid.Parse("2501203a-1484-4394-8c48-ed2a29d10fb9"),
                Code="Category3",
                CreationTime = DateTime.Now,
                Description = string.Empty,
                DisplayOrder = 1,
                IsApproved = true,
                LastModificationTime = DateTime.Now,
                Name = "Category 3",
                Version = 1
            }
        };

        private bool _disposed;
        private readonly IDbContext _context;


        public FakeCategoryRepository(IDbContext context)
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

        ~FakeCategoryRepository()
        {
            Dispose(false);
        }

        public IQueryable<Category> Get()
        {
            return _dbSet;
        }

        public IQueryable<Category> GetNoTracking()
        {
            return _dbSet.AsNoTracking();

        }

        public IIncludableJoin<Category, TProperty> Join<TProperty>(Expression<Func<Category, TProperty>> navigationProperty)
        {
            var query = _dbSet.Join(navigationProperty);
            return query;
        }

        public Task<IQueryable<Category>> GetAsync()
        {
            return Task.FromResult(_dbSet);
        }

        public IQueryable<Category> Get(string sql)
        {

            throw new NotImplementedException();
        }

        public Task<IQueryable<Category>> GetAsync(string sql)
        {
            throw new NotImplementedException();
        }

        public Category Get(Expression<Func<Category, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public Category GetNoTracking(Expression<Func<Category, bool>> predicate)
        {
            return _dbSet.AsNoTracking().FirstOrDefault(predicate);
        }

        public Task<Category> GetAsync(Expression<Func<Category, bool>> predicate)
        {
            return _dbSet.FirstOrDefaultAsync(predicate);
        }

        public Category Add(Category entity, bool autoSave = false)
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

        public Task<Category> AddAsync(Category entity, bool autoSave = false)
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

        public Category Update(Category entity, bool autoSave = false)
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

        public Task<Category> UpdateAsync(Category entity, bool autoSave = false)
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

        public void Delete(Category entity, bool autoSave = false)
        {
            var list = _items.ToList();
            var existItem = list.FirstOrDefault(x => x.Id == entity.Id);
            list.Remove(existItem);
            if (autoSave)
            {
                SaveChanges();
            }
        }

        public Task DeleteAsync(Category entity, bool autoSave = false)
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
