using System;
using System.Data;
using System.Text;

namespace ASC.Common.Data.Sql
{
    public class SqlDialect : ISqlDialect
    {
        public static readonly ISqlDialect Default = new SqlDialect();

        #region ISqlDialect Members

        public virtual string GetIdentityQuery
        {
            get { return "last_insert_rowid()"; }
        }

        public virtual string InsertOrReplace
        {
            get { return "insert or replace"; }
        }

        public virtual string Autoincrement
        {
            get { return "autoincrement"; }
        }

        public virtual bool SupportNotNamedParameters
        {
            get { return true; }
        }

        public virtual bool SeparateCreateIndex
        {
            get { return true; }
        }

        public virtual char ParameterSymbol
        {
            get { return '@'; }
        }

        public virtual string Bracket(string name)
        {
            return name;
        }

        public virtual void SetPagging(StringBuilder select, int max, int offset)
        {
            if (0 < max)
            {
                select.AppendFormat(" limit {0}", max);
                if (0 < offset) select.AppendFormat(" offset {0}", offset);
            }
        }

        public virtual string DbTypeToString(DbType type, int size, int precision)
        {
            switch (type)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                case DbType.Xml:
                case DbType.Guid:
                    return "TEXT";

                case DbType.Binary:
                case DbType.Object:
                    return "BLOB";

                case DbType.Boolean:
                case DbType.Currency:
                case DbType.Decimal:
                case DbType.VarNumeric:
                    return "NUMERIC";

                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                case DbType.Time:
                    return "DATETIME";

                case DbType.Byte:
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                case DbType.SByte:
                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                    return "INTEGER";

                case DbType.Double:
                case DbType.Single:
                    return "REAL";
            }
            throw new ArgumentOutOfRangeException(type.ToString());
        }

        #endregion
    }
}