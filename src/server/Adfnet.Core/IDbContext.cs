using System;

namespace Adfnet.Core
{
    public interface IDbContext : IDisposable
    {
        int SaveChanges();
    }
}
