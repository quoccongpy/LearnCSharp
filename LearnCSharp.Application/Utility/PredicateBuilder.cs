using System.Linq.Expressions;

namespace LearnCSharp.Application.Utility
{
    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> True<T>() => x => true;

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,Expression<Func<T, bool>> expr2)
        {
            var parameter = expr1.Parameters[0];

            var right = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter).Visit(expr2.Body);

            var body = Expression.AndAlso(expr1.Body, right);

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _oldValue;
            private readonly ParameterExpression _newValue;

            public ReplaceExpressionVisitor(ParameterExpression oldValue, ParameterExpression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldValue ? _newValue : base.VisitParameter(node);
            }
        }
    }
}