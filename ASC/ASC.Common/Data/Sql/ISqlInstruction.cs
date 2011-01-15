namespace ASC.Common.Data.Sql
{
    public interface ISqlInstruction
    {
        string ToString(ISqlDialect dialect);
        object[] GetParameters();
    }
}