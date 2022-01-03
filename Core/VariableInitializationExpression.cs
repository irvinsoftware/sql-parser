using System;
using Irvin.Parser;

namespace Irvin.SqlParser
{
    public class VariableInitializationExpression : InitializationExpression
    {
        public bool UsesAsKeyword { get; set; }

        public override void BuildDataType(TokenCollection tokens, StringComparison compareOption)
        {
            if (tokens.Current.Content.Equals(SqlParserSettings.AsKeyword, compareOption))
            {
                UsesAsKeyword = true;

                AppendChild(tokens.Current);
                tokens.MoveNextSkippingSpaces(AppendChild);
            }

            base.BuildDataType(tokens, compareOption);
        }
    }
}