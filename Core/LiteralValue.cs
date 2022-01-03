using Irvin.Parser;

namespace Irvin.SqlParser
{
    public class LiteralValue
        : SqlExpression
    {
        public LiteralValue(Token token)
        {
            AppendChild(token);
            SetPropertyChildIndex(nameof(Value), 0);
        }

        public string Value
        {
            get
            {
                SqlExpression child = GetChildAtProperty<SqlExpression>(nameof(Value));
                return child.ToString();
            }
        }
    }
}