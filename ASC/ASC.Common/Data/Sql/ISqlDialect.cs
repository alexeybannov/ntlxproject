using System.Data;
using System.Text;

namespace ASC.Common.Data.Sql
{
    public interface ISqlDialect
    {
        string GetIdentityQuery { get; }

        string InsertOrReplace { get; }

        string Autoincrement { get; }
        
        bool SupportNotNamedParameters { get; }

        bool SeparateCreateIndex { get; }
        
        char ParameterSymbol { get; }
        
        string Bracket(string name);
        
        void SetPagging(StringBuilder select, int max, int offset);

        string DbTypeToString(DbType type, int size, int precision);
    }
}