using System.Data;

namespace Irvin.SqlParser
{
    public class ParameterInvocation : SqlExpression, IParameterInfo
    {
        public ParameterInvocation()
        {
            Direction = ParameterDirection.Input;
        }

        public Name Name { get; set; }
        public SqlExpression Value { get; set; }
        public ParameterDirection Direction { get; set; }
    }
}