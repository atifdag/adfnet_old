using System;

namespace ADF.Net.Data
{
    
    public interface IUnitOfWork<TContext> : IDisposable where TContext : IDbContext
    {

        void BeginTransaction();

        void Commit();

        void Rollback();

        TContext Context { get; set; }
    }
}
