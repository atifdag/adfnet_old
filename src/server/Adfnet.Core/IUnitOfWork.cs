using System;

namespace Adfnet.Core
{
    
    public interface IUnitOfWork<TContext> : IDisposable where TContext : IDbContext
    {
        TContext Context { get; set; }

        void BeginTransaction();

        void Commit();

        void Rollback();

       
    }
}
