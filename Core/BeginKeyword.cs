using Irvin.Parser;

namespace Irvin.SqlFountain.Core
{
    public class BeginKeyword 
        : SqlStatement
    {
        internal const string Keyword = "BEGIN";

        public BeginKeyword(Token token)
        {
            AppendChild(token);
        }
    }
}