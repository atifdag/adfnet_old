using ADF.Net.Core;

namespace ADF.Net.Data.DataAccess.EF
{
    public class Repository<TEntity> : BaseRepository<TEntity, EfDbContext> where TEntity : class, IEntity, new()
    {
        public Repository(EfDbContext context) : base(context)
        {
        }
    }
}
