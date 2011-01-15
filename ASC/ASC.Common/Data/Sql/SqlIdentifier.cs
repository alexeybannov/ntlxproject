namespace ASC.Common.Data.Sql
{
    internal class SqlIdentifier : ISqlInstruction
    {
        public SqlIdentifier(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier { get; private set; }

        #region ISqlInstruction Members

        public string ToString(ISqlDialect dialect)
        {
            return dialect.Bracket(Identifier);
        }

        public object[] GetParameters()
        {
            return new object[0];
        }

        #endregion

        public static explicit operator SqlIdentifier(string identifier)
        {
            return new SqlIdentifier(identifier);
        }
    }
}