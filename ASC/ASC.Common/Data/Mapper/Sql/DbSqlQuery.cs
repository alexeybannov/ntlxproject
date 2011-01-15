#region usings

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ASC.Common.Data.Mapper.Expressions;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;

#endregion

namespace ASC.Common.Data.Mapper.Sql
{
    public class DbSqlQuery<T> : SqlQuery where T : new()
    {
        private readonly DbList<T> _list;

        public DbSqlQuery(string table)
            : base(table)
        {
        }

        internal DbSqlQuery(DbList<T> list)
            : base(list.Table)
        {
            _list = list;
        }

        public IEnumerable<T> Execute()
        {
            if (_list != null)
            {
                return _list.Execute(this);
            }
            throw new ArgumentException("List is not initialized");
        }

        public long Count()
        {
            if (_list != null)
            {
                return _list.ExecuteCount(this);
            }
            throw new ArgumentException("List is not initialized");
        }

        public DbSqlQuery<T> Where(Expression<Func<T, object>> expression)
        {
            Exp exp = null;
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                var unary = expression.Body as UnaryExpression;
                if (unary != null && unary.Operand is BinaryExpression)
                {
                    Expression binary = unary.Operand;
                    exp = ExpressionParser.Parse(binary, exp);
                }
            }
            where = where & exp;
            return this;
        }

        public DbSqlQuery<T> OrderBy(Expression<Func<T, object>> expression)
        {
            return OrderBy(expression, false);
        }

        public DbSqlQuery<T> OrderBy(Expression<Func<T, object>> expression, bool asc)
        {
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                var unary = expression.Body as UnaryExpression;
                if (unary != null)
                {
                    var memberExp = unary.Operand as MemberExpression;
                    if (memberExp != null)
                    {
                        object[] dbprops = memberExp.Member.GetCustomAttributes(typeof (DbColumnAttribute), true);
                        if (dbprops.Length > 0)
                        {
                            string column = ((DbColumnAttribute) dbprops[0]).Column;
                            orders.Add(column, asc);
                        }
                    }
                }
            }
            return this;
        }
    }
}