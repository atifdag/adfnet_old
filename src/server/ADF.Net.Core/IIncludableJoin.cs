using System.Linq;

namespace ADF.Net.Core
{
    public interface IIncludableJoin<out TEntity, out TProperty> : IQueryable<TEntity> where TEntity : class, IEntity, new()
    {
    }
}
