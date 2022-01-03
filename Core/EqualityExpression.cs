namespace Irvin.SqlParser
{
    public class EqualityExpression : SqlExpression
    {
        public SqlExpression Operand1
        {
            get { return GetChildAtProperty<SqlExpression>(nameof(Operand1)); }
            set { SetChildProperty(nameof(Operand1), value); }
        }

        public EqualityOperator Operator { get; set; }

        public SqlExpression Operand2
        {
            get { return GetChildAtProperty<SqlExpression>(nameof(Operand2)); }
            set { SetChildProperty(nameof(Operand2), value); }
        }
    }
}