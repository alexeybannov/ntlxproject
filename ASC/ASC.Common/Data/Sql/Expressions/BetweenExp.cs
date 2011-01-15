namespace ASC.Common.Data.Sql.Expressions
{
    public class BetweenExp : Exp
    {
        private readonly string column;
        private readonly object maxValue;
        private readonly object minValue;

        public BetweenExp(string column, object minValue, object maxValue)
        {
            this.column = column;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public override string ToString(ISqlDialect dialect)
        {
            return string.Format("{0} {1}between ? and ?", dialect.Bracket(column), Not ? "not " : string.Empty);
        }

        public override object[] GetParameters()
        {
            return new[] {minValue, maxValue};
        }
    }
}