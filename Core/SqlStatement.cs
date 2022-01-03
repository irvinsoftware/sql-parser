using System.Linq;

namespace Irvin.SqlParser
{
    public class SqlStatement : SqlExpression
    {
        public bool IsExplicitlyTerminated
        {
            get { return Children.Last().ToString() == SqlParserSettings.StatementTerminator; }
        }
    }
}