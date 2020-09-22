using System;
using ADF.Net.Core;
using Microsoft.EntityFrameworkCore.Storage;

namespace ADF.Net.Data.DataAccess.EntityFramework
{
    public class UnitOfWork : IUnitOfWork<EfDbContext>
    {
        private bool _disposed;

        private IDbContextTransaction _transaction;

        public EfDbContext Context { get; set; }

        public UnitOfWork(EfDbContext context)
        {
            Context = context;
        }

        ~UnitOfWork()
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
                    Context.Dispose();
                }
            }
            _disposed = true;
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
