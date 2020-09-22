using System;

namespace ADF.Net.Core
{
    public interface IDbContext : IDisposable
    {
        int SaveChanges();
    }
}
