#region usings

using System;
using System.Linq.Expressions;
using ASC.Common.Data.Sql.Expressions;

#endregion

namespace ASC.Common.Data.Mapper.Expressions
{
    public class ExpressionParser
    {
        public static Exp Parse(Expression expression, Exp exp)
        {
            Exp expOut = null;
            var binary = expression as BinaryExpression;
            var unary = expression as UnaryExpression;
            var member = expression as MemberExpression;
            if (binary != null)
            {
                if (IsInBinaryType(binary.NodeType))
                {
                    var memberExp = binary.Left as MemberExpression;
                    var constExp = binary.Right as ConstantExpression;
                    if (constExp == null && binary.Right is MemberExpression)
                    {
                        LambdaExpression lambda = Expression.Lambda(binary.Right);
                        Delegate fn = lambda.Compile();
                        constExp = Expression.Constant(fn.DynamicInvoke(null), binary.Right.Type);
                    }
                    if (memberExp != null && constExp != null)
                    {
                        object[] dbprops = memberExp.Member.GetCustomAttributes(typeof (DbColumnAttribute), true);
                        if (dbprops.Length > 0)
                        {
                            string column = ((DbColumnAttribute) dbprops[0]).Column;
                            switch (binary.NodeType)
                            {
                                case ExpressionType.Equal:
                                    expOut = Exp.Eq(column, constExp.Value);
                                    break;
                                case ExpressionType.GreaterThan:
                                    expOut = Exp.Gt(column, constExp.Value);
                                    break;
                                case ExpressionType.GreaterThanOrEqual:
                                    expOut = Exp.Ge(column, constExp.Value);
                                    break;
                                case ExpressionType.LessThan:
                                    expOut = Exp.Lt(column, constExp.Value);
                                    break;
                                case ExpressionType.LessThanOrEqual:
                                    expOut = Exp.Le(column, constExp.Value);
                                    break;
                                case ExpressionType.NotEqual:
                                    expOut = !Exp.Eq(column, constExp.Value);
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    switch (binary.NodeType)
                    {
                        case ExpressionType.Or:
                            expOut = (Parse(binary.Left, exp) |
                                      Parse(binary.Right, exp));
                            break;
                        case ExpressionType.OrElse:
                            expOut = exp | (Parse(binary.Left, exp) |
                                            Parse(binary.Right, exp));
                            break;
                        case ExpressionType.And:
                            expOut = (Parse(binary.Left, exp) &
                                      Parse(binary.Right, exp));
                            break;
                        case ExpressionType.AndAlso:
                            expOut = exp & (Parse(binary.Left, exp) &
                                            Parse(binary.Right, exp));
                            break;
                    }
                }
            }
            else if (unary != null)
            {
                switch (unary.NodeType)
                {
                    case ExpressionType.Not:
                        expOut = !Parse(unary.Operand, exp);
                        break;
                }
            }
            else if (member != null)
            {
                object[] dbprops = member.Member.GetCustomAttributes(typeof (DbColumnAttribute), true);
                if (dbprops.Length > 0)
                {
                    string column = ((DbColumnAttribute) dbprops[0]).Column;
                    if (member.Type == typeof (bool))
                    {
                        expOut = exp | Exp.Eq(column, true);
                    }
                }
                if (expOut == null)
                {
                    expOut = Exp.True;
                }
            }
            return expOut;
        }

        private static bool IsInBinaryType(ExpressionType type)
        {
            return type == ExpressionType.Equal || type == ExpressionType.GreaterThan ||
                   type == ExpressionType.GreaterThanOrEqual || type == ExpressionType.LessThan ||
                   type == ExpressionType.NotEqual || type == ExpressionType.LessThanOrEqual;
        }
    }
}