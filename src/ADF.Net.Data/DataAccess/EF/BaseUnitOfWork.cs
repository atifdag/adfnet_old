using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ADF.Net.Data.DataAccess.EF
{
    public class BaseUnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext, IDbContext
    {
        private bool _disposed;
        private IDbContextTransaction _transaction;

        public TContext Context { get; set; }
        public BaseUnitOfWork(TContext context)
        {
            Context = context;
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
                    Context.Dispose();
                }
            }
            _disposed = true;
        }
        ~BaseUnitOfWork()
        {
            Dispose(false);
        }

        public void BeginTransaction()
        {
            _transaction = Context.Database.BeginTransaction();
        }

        public void Commit()
        {
            Context.SaveChanges();
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        
    }
}
