using System;

namespace ADF.Net.Data
{
    public interface IDbContext : IDisposable
    {
        int SaveChanges();
    }
}
