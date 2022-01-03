namespace Irvin.SqlFountain.Core
{
    public class ColumnExpression
        : SqlExpression
    {
        public ColumnExpression(SqlExpression expression)
        {
            Expression = expression;
        }

        public SqlExpression Expression
        {
            get { return GetChildAtProperty<SqlExpression>(nameof(Expression)); }
            set { SetChildProperty(nameof(Expression), value); }
        }

        public bool? UsesExplicitAlias { get; set; }

        public Name Alias
        {
            get { return GetChildAtProperty<Name>(nameof(Alias)); }
            set { SetChildProperty(nameof(Alias), value); }
        }

        public Name OutputName
        {
            get { return Alias ?? (Expression as ColumnAddress)?.ColumnName; }
        }
    }
}