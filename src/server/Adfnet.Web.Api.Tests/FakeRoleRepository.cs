using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Adfnet.Core;
using Adfnet.Data.DataAccess.EntityFramework;
using Adfnet.Data.DataEntities;
using Microsoft.EntityFrameworkCore;

namespace Adfnet.Web.Api.Tests
{
    internal class FakeRoleRepository : IRepository<Role>
    {
        private readonly IQueryable<Role> _dbSet;

        private Role[] _items =
        {
            new Role
            {
                Id = Guid.Parse("5d981459-8f5a-4fb6-ba4a-479590917876"),
                Code="Role1",
                CreationTime = DateTime.Now,
                Description = string.Empty,
                DisplayOrder = 1,
                IsApproved = true,
                LastModificationTime = DateTime.Now,
                Name = "Role 1",
                Version = 1
            },
            new Role
            {
                Id = Guid.Parse("208d799a-ee2f-4d07-bfe3-d5928cc6cf30"),
                Code="Role2",
                CreationTime = DateTime.Now,
                Description = string.Empty,
                DisplayOrder = 1,
                IsApproved = true,
                LastModificationTime = DateTime.Now,
                Name = "Role 2",
                Version = 1
            },
            new Role
            {
                Id = Guid.Parse("2501203a-1484-4394-8c48-ed2a29d10fb9"),
                Code="Role3",
                CreationTime = DateTime.Now,
                Description = string.Empty,
                DisplayOrder = 1,
                IsApproved = true,
                LastModificationTime = DateTime.Now,
                Name = "Role 3",
                Version = 1
            }
        };

        private bool _disposed;
        private readonly IDbContext _context;


        public FakeRoleRepository(IDbContext context)
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

        ~FakeRoleRepository()
        {
            Dispose(false);
        }

        public IQueryable<Role> Get()
        {
            return _dbSet;
        }

        public IQueryable<Role> GetNoTracking()
        {
            return _dbSet.AsNoTracking();

        }

        public IIncludableJoin<Role, TProperty> Join<TProperty>(Expression<Func<Role, TProperty>> navigationProperty)
        {
            var query = _dbSet.Join(navigationProperty);
            return query;
        }

        public Task<IQueryable<Role>> GetAsync()
        {
            return Task.FromResult(_dbSet);
        }

        public IQueryable<Role> Get(string sql)
        {

            throw new NotImplementedException();
        }

        public Task<IQueryable<Role>> GetAsync(string sql)
        {
            throw new NotImplementedException();
        }

        public Role Get(Expression<Func<Role, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public Role GetNoTracking(Expression<Func<Role, bool>> predicate)
        {
            return _dbSet.AsNoTracking().FirstOrDefault(predicate);
        }

        public Task<Role> GetAsync(Expression<Func<Role, bool>> predicate)
        {
            return _dbSet.FirstOrDefaultAsync(predicate);
        }

        public Role Add(Role entity, bool autoSave = false)
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

        public Task<Role> AddAsync(Role entity, bool autoSave = false)
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

        public Role Update(Role entity, bool autoSave = false)
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

        public Task<Role> UpdateAsync(Role entity, bool autoSave = false)
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

        public void Delete(Role entity, bool autoSave = false)
        {
            var list = _items.ToList();
            var existItem = list.FirstOrDefault(x => x.Id == entity.Id);
            list.Remove(existItem);
            if (autoSave)
            {
                SaveChanges();
            }
        }

        public Task DeleteAsync(Role entity, bool autoSave = false)
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
