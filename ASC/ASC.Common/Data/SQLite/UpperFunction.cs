#region usings

using System;
using System.Data.SQLite;

#endregion

#if MONOusing Mono.Data.Sqlite;
#else

#endif

namespace ASC.Data.SQLite
{
#if MONO    [SqliteFunction(Name = "upper", Arguments = 1, FuncType = FunctionType.Scalar)]
#else
    [SQLiteFunction(Name = "upper", Arguments = 1, FuncType = FunctionType.Scalar)]
#endif
    public class UpperFunction :
#if MONO	SqliteFunction#else        SQLiteFunction
#endif
    {
        public override object Invoke(object[] args)
        {
            if (args.Length == 0 || args[0] == null) return null;
            if (args[0] == DBNull.Value) return DBNull.Value;
            return ((string) args[0]).ToUpper();
        }
    }
}