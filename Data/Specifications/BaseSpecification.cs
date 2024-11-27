using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Data.Interfaces;

namespace Data.Specifications
{
    /// <summary>
    /// Base class for query specifications
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="ISpecification&lt;T&gt;" />
    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        protected BaseSpecification() { }

        protected BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        public Expression<Func<T, bool>> Criteria { get; protected set; }

        public List<Expression<Func<T, object>>> Includes { get; } = new();

        public List<string> IncludeStrings { get; } = new();

        private readonly List<OrderExpression<T>> _orderExpressions = new();
        public IReadOnlyList<OrderExpression<T>> OrderExpressions => _orderExpressions.AsReadOnly();

        public int Take { get; private set; }

        public int Skip { get; private set; }

        public bool IsPagingEnabled { get; private set; } = false;

        public void AddCriteria(Expression<Func<T, bool>> criteria)
        {
            if (Criteria == null)
            {
                Criteria = criteria;
            }
            else
            {
                var parameter = Expression.Parameter(typeof(T));

                var combined = Expression.AndAlso(
                    new ReplaceParameterVisitor(Criteria.Parameters[0], parameter).Visit(Criteria.Body),
                    new ReplaceParameterVisitor(criteria.Parameters[0], parameter).Visit(criteria.Body));

                Criteria = Expression.Lambda<Func<T, bool>>(combined, parameter);
            }
        }

        public void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        public void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        public void ApplyCriteria(Expression<Func<T, bool>> criteriaExpression)
        {
            Criteria = criteriaExpression;
        }

        public void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            _orderExpressions.Add(new OrderExpression<T>(orderByExpression, isDescending: false));
        }

        public void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        {
            _orderExpressions.Add(new OrderExpression<T>(orderByDescExpression, isDescending: true));
        }

        public void ApplyThenOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            _orderExpressions.Add(new OrderExpression<T>(orderByExpression, isDescending: false));
        }

        public void ApplyThenOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        {
            _orderExpressions.Add(new OrderExpression<T>(orderByDescExpression, isDescending: true));
        }

        public void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }
    }
}
