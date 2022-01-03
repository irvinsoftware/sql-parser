namespace Irvin.SqlFountain.Core
{
    public class IfStatement : SqlStatement
    {
        internal const string IfKeyword = "IF";

        public SqlExpression Condition
        {
            get { return GetChildAtProperty<SqlExpression>(nameof(Condition)); }
            set { SetChildProperty(nameof(Condition), value); }
        }

        public SqlExpression MainBody
        {
            get { return GetChildAtProperty<SqlExpression>(nameof(MainBody)); }
            set { SetChildProperty(nameof(MainBody), value); }
        }

        public SqlExpression ElseBody { get; set; }
    }
}