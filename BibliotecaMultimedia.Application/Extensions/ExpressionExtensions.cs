using System.Linq.Expressions;

namespace BibliotecaMultimedia.Application.Extensions;

public static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> And<T>(
        this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
        ReplaceExpressionVisitor visitor = new ReplaceExpressionVisitor(right.Parameters[0], parameter);
        Expression body = Expression.AndAlso(left.Body, visitor.Visit(right.Body)!);
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    private sealed class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression? Visit(Expression? node)
        {
            return node == _oldValue ? _newValue : base.Visit(node);
        }
    }
}