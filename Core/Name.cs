using System;
using Irvin.Parser;

namespace Irvin.SqlFountain.Core
{
    public class Name : SqlExpression
    {
        public string Value { get; set; }
        public bool IsQuoted { get; set; }

        public bool IsVariable
        {
            get { return Value.StartsWith("@"); }
        }

        internal static Name CreateFrom(Token token)
        {
            Name name = new Name();

            name.Value = token.Content;
            if (token.Content.StartsWith(SqlParserSettings.QuotedIdentifierSubGroup.StartSymbol))
            {
                name.IsQuoted = true;
                name.Value = SqlParserSettings.GetSubgroupValue(SqlParserSettings.QuotedIdentifierSubGroup, name.Value);
            }
            name.AppendChild(token);

            return name;
        }
    }
}