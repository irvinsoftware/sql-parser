using System.Collections.Generic;

namespace Irvin.SqlParser
{
    public class DeclareVariablesStatement : SqlStatement
    {
        public DeclareVariablesStatement()
        {
            Declarations = new List<VariableInitializationExpression>();
        }

        public List<VariableInitializationExpression> Declarations { get; set; }

        public override void AppendChild(SqlExpression sqlExpression)
        {
            if (sqlExpression is VariableInitializationExpression)
            {
                Declarations.Add(sqlExpression as VariableInitializationExpression);
            }

            base.AppendChild(sqlExpression);
        }
    }
}