using Microsoft.EntityFrameworkCore;

namespace ADF.Net.Data.DataAccess.EF
{
    public class EfDbContext : BaseDbContext
    {
          
           public EfDbContext(DbContextOptions options) : base(options)
           {
           }

    }
}
