using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Yanyitec
{
    public class Criteria<T>
    {
        public Criteria()
        {
            this.Parameter = Expression.Parameter(typeof(T), "entity");
        }

        public Criteria(Expression<Func<T, bool>> initCriteria)
        {
            if (initCriteria != null)
            {
                this.Parameter = initCriteria.Parameters[0];
                this.Expression = initCriteria.Body;
            }

        }

        public Expression<Func<T, object>> Asc { get; set; }

        public Expression<Func<T, object>> Desc { get; set; }

        public int PageIndex { get; set; }

        public int? PageSize { get; set; }

        public ParameterExpression Parameter { get; private set; }
        public Expression Expression { get; private set; }

        

        public Expression<Func<T, bool>> ToLamda()
        {
            if (this.Expression == null) return null;
            return Expression.Lambda<Func<T, bool>>(Expression, this.Parameter);
        }



        public Criteria<T> AndAlso(Expression<Func<T, bool>> criteria)
        {
            if (criteria == null) return this;
            if (Expression == null)
            {
                this.Parameter = criteria.Parameters[0];
                this.Expression = criteria.Body;
            }
            else
            {
                this.Expression = Expression.AndAlso(this.Expression, criteria);
                //this.Expression = Expression.AndAlso(this.Expression, Convert(criteria, criteria.Parameters[0]));
            }
            return this;
        }

        public Criteria<T> AndAlso(Criteria<T> criteria)
        {
            return this.AndAlso(criteria.ToLamda());
        }

        public Criteria<T> OrElse(Expression<Func<T, bool>> criteria)
        {
            if (Expression == null)
            {
                this.Parameter = criteria.Parameters[0];
                this.Expression = criteria.Body;
            }
            else
            {
                this.Expression = Expression.OrElse(this.Expression, criteria);
                //this.Expression = Expression.OrElse(this.Expression, Convert(criteria, criteria.Parameters[0]));
            }
            return this;
        }

        public Criteria<T> OrElse(Criteria<T> criteria)
        {
            return this.OrElse(criteria.ToLamda());
        }

        Expression Convert(Expression expr, ParameterExpression param)
        {
            if (expr == param) return this.Parameter;
            BinaryExpression bExpr = null;
            UnaryExpression uExpr = null;
            switch (expr.NodeType)
            {
                case ExpressionType.And:
                    bExpr = expr as BinaryExpression;
                    return Expression.And(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.Add:
                    bExpr = expr as BinaryExpression;
                    return Expression.Add(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.AndAlso:
                    bExpr = expr as BinaryExpression;
                    return Expression.OrElse(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.Call:
                    var call = expr as MethodCallExpression;
                    var list = new List<Expression>();
                    foreach (var arg in call.Arguments)
                    {
                        list.Add(Convert(arg, param));
                    }
                    return Expression.Call(Convert(call.Object, param), call.Method, list);
                case ExpressionType.Convert:
                    uExpr = expr as UnaryExpression;
                    return Expression.Convert(Convert(uExpr.Operand, param), uExpr.Type);
                case ExpressionType.Divide:
                    bExpr = expr as BinaryExpression;
                    return Expression.Divide(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.Equal:
                    bExpr = expr as BinaryExpression;
                    return Expression.Equal(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.GreaterThan:
                    bExpr = expr as BinaryExpression;
                    return Expression.GreaterThan(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.GreaterThanOrEqual:
                    bExpr = expr as BinaryExpression;
                    return Expression.GreaterThanOrEqual(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.LessThan:
                    bExpr = expr as BinaryExpression;
                    return Expression.LessThan(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.LessThanOrEqual:
                    bExpr = expr as BinaryExpression;
                    return Expression.LessThan(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.LeftShift:
                    bExpr = expr as BinaryExpression;
                    return Expression.LeftShift(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.Multiply:
                    bExpr = expr as BinaryExpression;
                    return Expression.Multiply(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.Negate:
                    uExpr = expr as UnaryExpression;
                    return Expression.Negate(Convert(uExpr.Operand, param));
                case ExpressionType.Not:
                    uExpr = expr as UnaryExpression;
                    return Expression.Not(Convert(uExpr.Operand, param));
                case ExpressionType.NotEqual:
                    bExpr = expr as BinaryExpression;
                    return Expression.NotEqual(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.Or:
                    bExpr = expr as BinaryExpression;
                    return Expression.Or(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.OrElse:
                    bExpr = expr as BinaryExpression;
                    return Expression.OrElse(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.Power:
                    bExpr = expr as BinaryExpression;
                    return Expression.Power(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.RightShift:
                    bExpr = expr as BinaryExpression;
                    return Expression.RightShift(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.Subtract:
                    bExpr = expr as BinaryExpression;
                    return Expression.Subtract(Convert(bExpr.Left, param), Convert(bExpr.Right, param));

            }
            throw new NotSupportedException();
        }

        public static Criteria<T> operator &(Criteria<T> criteria, Expression<Func<T, bool>> expr)
        {
            return criteria == null ? new Criteria<T>(expr) : criteria.AndAlso(expr);
        }

        public static Criteria<T> operator |(Criteria<T> criteria, Expression<Func<T, bool>> expr)
        {
            return criteria == null ? new Criteria<T>(expr) : criteria.OrElse(expr);
        }

        public static implicit operator Criteria<T>(Expression<Func<T, bool>> expr)
        {
            return new Criteria<T>(expr);
        }
    }
}
