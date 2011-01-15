using System;
using System.Text;

#if MONO
using Mono.Data.Sqlite;
using SqliteFunction = Mono.Data.Sqlite.SqliteFunction;
using SqliteFunctionAttribute = Mono.Data.Sqlite.SqliteFunctionAttribute;
#else
using System.Data.SQLite;
using SqliteFunction = System.Data.SQLite.SQLiteFunction;
using SqliteFunctionAttribute = System.Data.SQLite.SQLiteFunctionAttribute;
#endif

namespace ASC.Data.SQLite
{
    [SqliteFunction(Name = "concat", Arguments = -1, FuncType = FunctionType.Scalar)]
    public class ConcatFunction : SqliteFunction
    {
        public override object Invoke(object[] args)
        {
            var result = new StringBuilder();
            foreach (object arg in args)
            {
                result.Append(arg);
            }
            return result.Length != 0 ? result.ToString() : null;
        }
    }

    [SqliteFunction(Name = "concat_ws", Arguments = -1, FuncType = FunctionType.Scalar)]
    public class ConcatWSFunction : SqliteFunction
    {
        public override object Invoke(object[] args)
        {
            if (args.Length < 2 || args[0] == null || DBNull.Value.Equals(args[0])) return null;
            var result = new StringBuilder();
            for (int i = 1; i < args.Length; i++)
            {
                result.AppendFormat("{0}{1}", args[i], i == args.Length - 1 ? string.Empty : args[0]);
            }
            return result.Length != 0 ? result.ToString() : null;
        }
    }
}