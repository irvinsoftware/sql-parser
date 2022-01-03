using System.Collections.Generic;

namespace Irvin.SqlFountain.Core
{
    public class SelectStatement
        : SelectExpression
    {
        public SelectStatement()
        {
            CommonTableExpressions = new List<CommonTableExpression>();
        }

        public List<CommonTableExpression> CommonTableExpressions { get; }
    }
}