using System.Linq;
using ADF.Net.Core;

namespace ADF.Net.Data
{
    public interface IIncludableJoin<out TEntity, out TProperty> : IQueryable<TEntity> where TEntity : class, IEntity, new()
    {
    }
}
