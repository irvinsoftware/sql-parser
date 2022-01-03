using System.Collections.Generic;
using System.Linq;

namespace Irvin.SqlParser
{
    public class SqlCodeUnit : SqlExpression
    {
        public List<SqlExpression> Statements
        {
            get { return Children.ToList(); }
        }

        public T AddStatement<T>()
            where T : SqlExpression, new()
        {
            T statement = new T();
            AppendChild(statement);
            return statement;
        }
    }
}