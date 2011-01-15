#region usings

using System;
using System.Text;

#endregion

namespace ASC.Common.Data.Sql.Expressions
{
    public class InExp : Exp
    {
        private readonly string column;
        private readonly SqlQuery subQuery;
        private readonly object[] values;

        public InExp(string column, object[] values)
        {
            this.column = column;
            this.values = values;
        }

        public InExp(string column, SqlQuery subQuery)
        {
            this.column = column;
            this.subQuery = subQuery;
        }

        public override string ToString(ISqlDialect dialect)
        {
            if (values != null && values.Length == 0) return "1 = 0";
            var sql = new StringBuilder();
            sql.AppendFormat("{0} {1}in (", dialect.Bracket(column), Not ? "not " : string.Empty);
            if (values != null)
            {
                Array.ForEach(values, v => sql.Append("?,"));
                sql.Remove(sql.Length - 1, 1);
            }
            if (subQuery != null)
            {
                sql.Append(subQuery.ToString(dialect));
            }
            return sql.Append(")").ToString();
        }

        public override object[] GetParameters()
        {
            if (values != null) return values;
            if (subQuery != null) return subQuery.GetParameters();
            return new object[0];
        }
    }
}