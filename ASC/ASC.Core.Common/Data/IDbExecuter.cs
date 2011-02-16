using System;
using System.Collections.Generic;
using ASC.Common.Data.Sql;

namespace ASC.Core.Data
{
    public interface IDbExecuter
    {
        T ExecScalar<T>(ISqlInstruction sql);

        int ExecNonQuery(ISqlInstruction sql);

        List<object[]> ExecList(ISqlInstruction sql);

        void ExecBatch(IEnumerable<ISqlInstruction> batch);

        void ExecAction(Action<IDbExecuter> action);
    }
}
