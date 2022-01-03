using Irvin.Parser;

namespace Irvin.SqlParser
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