namespace Irvin.SqlFountain.Core
{
    public class SetVariablesStatement : SqlStatement
    {
        public Name Variable
        {
            get { return GetChildAtProperty<Name>(nameof(Variable)); }
            set { SetChildProperty(nameof(Variable), value); }
        }

        public SqlExpression Value
        {
            get { return GetChildAtProperty<SqlExpression>(nameof(Value)); }
            set { SetChildProperty(nameof(Value), value); }
        }
    }
}