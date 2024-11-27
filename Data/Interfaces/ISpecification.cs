using System.Collections.Generic;
using System.Linq.Expressions;
using System;

namespace Data.Interfaces
{
    /// <summary>
    /// Represents an ordering expression with its direction.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public class OrderExpression<T>
    {
        public Expression<Func<T, object>> Expression { get; }
        public bool IsDescending { get; }

        public OrderExpression(Expression<Func<T, object>> expression, bool isDescending)
        {
            Expression = expression;
            IsDescending = isDescending;
        }
    }

    /// <summary>
    /// Query specification for retrieving data from the database
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISpecification<T>
    {
        /// <summary>
        /// The criteria that defines the specification
        /// </summary>
        Expression<Func<T, bool>> Criteria { get; }

        /// <summary>
        /// Include expressions for eager loading related entities
        /// </summary>
        List<Expression<Func<T, object>>> Includes { get; }

        /// <summary>
        /// Include strings for specifying navigation properties by name
        /// </summary>
        List<string> IncludeStrings { get; }

        /// <summary>
        /// Collection of order expressions.
        /// </summary>
        IReadOnlyList<OrderExpression<T>> OrderExpressions { get; }

        // Pagination
        int Take { get; }
        int Skip { get; }
        bool IsPagingEnabled { get; }
    }
}
