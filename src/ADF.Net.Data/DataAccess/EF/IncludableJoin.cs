using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ADF.Net.Core;
using Microsoft.EntityFrameworkCore.Query;

namespace ADF.Net.Data.DataAccess.EF
{
    internal class IncludableJoin<TEntity, TPreviousProperty> : IIncludableJoin<TEntity, TPreviousProperty> where TEntity : class, IEntity, new()
    {
        private readonly IIncludableQueryable<TEntity, TPreviousProperty> _query;
        public Expression Expression => _query.Expression;
        public Type ElementType => _query.ElementType;
        public IQueryProvider Provider => _query.Provider;

        public IncludableJoin(IIncludableQueryable<TEntity, TPreviousProperty> query)
        {
            _query = query;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _query.GetEnumerator();
        }

        internal IIncludableQueryable<TEntity, TPreviousProperty> GetQuery()
        {
            return _query;
        }
    }
}
