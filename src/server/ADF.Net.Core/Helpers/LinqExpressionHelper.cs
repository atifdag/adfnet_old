using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ADF.Net.Core.Helpers
{

    public static class LinqExpressionHelper
    {
        private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {

            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            var secondBody = LinqExpressionVisitor.ReplaceParameters(map, second.Body);

            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);

        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {

            return first.Compose(second, Expression.And);

        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {

            return first.Compose(second, Expression.Or);

        }
    }

    internal class LinqExpressionVisitor : ExpressionVisitor
    {

        private readonly Dictionary<ParameterExpression, ParameterExpression> _map;

        public LinqExpressionVisitor(Dictionary<ParameterExpression, ParameterExpression> map)
        {

            _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();

        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {

            return new LinqExpressionVisitor(map).Visit(exp);

        }

        protected override Expression VisitParameter(ParameterExpression p)
        {

            if (_map.TryGetValue(p, out var replacement))
            {
                p = replacement;
            }

            return base.VisitParameter(p);

        }

    }

}
