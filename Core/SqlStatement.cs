using System.Collections.Generic;
using System.Linq;

namespace Irvin.SqlFountain.Core
{
    public class SqlStatement : SqlExpression
    {
        public bool IsExplicitlyTerminated
        {
            get { return Children.Last().ToString() == SqlParserSettings.StatementTerminator; }
        }
    }
}