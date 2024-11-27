using System.Linq.Expressions;

namespace Data.Specifications
{
    /// <summary>
    /// A helper class to correctly replace parameters when combining expressions
    /// </summary>
    /// <seealso cref="ExpressionVisitor" />
    public class ReplaceParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public ReplaceParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return ReferenceEquals(node, _oldParameter) ? _newParameter : base.VisitParameter(node);
        }
    }
}
