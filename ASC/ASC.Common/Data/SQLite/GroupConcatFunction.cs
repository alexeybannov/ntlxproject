#region usings

using System;
using System.Data.SQLite;

#endregion

#if MONO
using Mono.Data.Sqlite;
#else
#endif

namespace ASC.Data.SQLite
{
#if MONO
	[SqliteFunction(Name = "group_concat", Arguments = 2, FuncType = FunctionType.Aggregate)]
#else
    [SQLiteFunction(Name = "group_concat", Arguments = 2, FuncType = FunctionType.Aggregate)]
#endif
    public class GroupConcatFunction :
#if MONO
	SqliteFunction
#else
        SQLiteFunction
#endif
    {
        public override object Final(object contextData)
        {
            return contextData != null ? contextData.ToString() : null;
        }

        public override void Step(object[] args, int stepNumber, ref object contextData)
        {
            if (contextData == null)
            {
                contextData = new GroupConcater(1 < args.Length ? args[1] : ',');
            }
            ((GroupConcater) contextData).Step(args[0]);
        }

        private class GroupConcater
        {
            private readonly string separator;
            private string text;

            public GroupConcater(object separator)
            {
                this.separator = IsDBNull(separator) ? "," : separator.ToString();
            }

            public void Step(object arg)
            {
                if (!IsDBNull(arg))
                {
                    text += string.Format("{0}{1}", arg, separator);
                }
            }

            public override string ToString()
            {
                if (string.IsNullOrEmpty(text)) return null;
                string result = text.Remove(text.Length - separator.Length, separator.Length);
                return !string.IsNullOrEmpty(result) ? result : null;
            }

            private bool IsDBNull(object arg)
            {
                return arg == null || DBNull.Value.Equals(arg);
            }
        }
    }

#if MONO
	[SqliteFunction(Name = "group_concat", Arguments = 1, FuncType = FunctionType.Aggregate)]
#else
    [SQLiteFunction(Name = "group_concat", Arguments = 1, FuncType = FunctionType.Aggregate)]
#endif
    public class GroupConcatFunction2 : GroupConcatFunction
    {
    }
}