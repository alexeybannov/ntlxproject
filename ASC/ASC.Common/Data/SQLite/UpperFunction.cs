#region usings

using System;
using System.Data.SQLite;

#endregion

#if MONO
#else

#endif

namespace ASC.Data.SQLite
{
#if MONO
#else
    [SQLiteFunction(Name = "upper", Arguments = 1, FuncType = FunctionType.Scalar)]
#endif
    public class UpperFunction :
#if MONO
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