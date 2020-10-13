using System.Linq;

namespace Adfnet.Core
{
    public interface IIncludableJoin<out TEntity, out TProperty> : IQueryable<TEntity> where TEntity : class, IEntity, new()
    {
    }
}
