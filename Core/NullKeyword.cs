using Irvin.Parser;

namespace Irvin.SqlFountain.Core
{
    public class NullKeyword : SqlExpression
    {
        public NullKeyword(Token actualToken)
        {
            AppendChild(actualToken);
        }

        public bool IsUpperCase
        {
            get { return ToString().Trim().Equals("NULL"); }
        }
    }
}