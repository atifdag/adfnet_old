using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace ADF.Net.Core.Helpers
{

    public class SortHelper<T>
    {
        private class SortObject
        {
            public bool IsThenBy { get; set; }
            public bool IsDescending { get; set; }
        }

        private Collection<Dictionary<LambdaExpression, SortObject>> _sortingExpressionList;
        private Collection<Dictionary<LambdaExpression, SortObject>> SortingExpressionList => _sortingExpressionList ??= new Collection<Dictionary<LambdaExpression, SortObject>>();

        public IOrderedQueryable<T> GenerateOrderedQuery(IOrderedQueryable<T> query)
        {

            foreach (var sortColumn in SortingExpressionList)
            {

                var sortObj = sortColumn.Values.First();

                var sortExpression = sortColumn.Keys.First();

                if (sortObj.IsThenBy)
                {
                    query = sortObj.IsDescending ? Queryable.ThenByDescending(query, (dynamic)sortExpression) : Queryable.ThenBy(query, (dynamic)sortExpression);
                }
                else
                {
                    query = sortObj.IsDescending ? Queryable.OrderByDescending(query, (dynamic)sortExpression) : Queryable.OrderBy(query, (dynamic)sortExpression);
                }

            }

            return query;

        }

        public SortHelper<T> OrderBy<TOrderType>(Expression<Func<T, TOrderType>> orderExpression)
        {

            var sortObject = new SortObject
            {
                IsThenBy = false,
                IsDescending = false
            };

            AddSortStatement(orderExpression, sortObject);

            return this;

        }

        public SortHelper<T> OrderByDescending<TOrderType>(Expression<Func<T, TOrderType>> orderExpression)
        {

            var sortObject = new SortObject
            {
                IsThenBy = false,
                IsDescending = true
            };

            AddSortStatement(orderExpression, sortObject);

            return this;

        }

        public SortHelper<T> ThenBy<TOrderType>(Expression<Func<T, TOrderType>> orderExpression)
        {

            var sortObject = new SortObject
            {
                IsThenBy = true,
                IsDescending = false
            };

            AddSortStatement(orderExpression, sortObject);

            return this;

        }

        public SortHelper<T> ThenByDescending<TOrderType>(Expression<Func<T, TOrderType>> orderExpression)
        {

            var sortObject = new SortObject
            {
                IsThenBy = true,
                IsDescending = true
            };

            AddSortStatement(orderExpression, sortObject);

            return this;

        }

        private void AddSortStatement<TOrderType>(Expression<Func<T, TOrderType>> orderExpression, SortObject sortObject)
        {

            var sortMap = new Dictionary<LambdaExpression, SortObject>
            {
                {
                    orderExpression,
                    sortObject
                }
            };

            SortingExpressionList.Add(sortMap);

        }
    }
}
