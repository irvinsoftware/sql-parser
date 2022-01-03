using System;
using Irvin.Parser;

namespace Irvin.SqlFountain.Core
{
    public class SqlParserSettings : ParserSettings
    {
        internal const string UseStatement = "USE";
        internal const string SetKeyword = "SET";
        internal const string OnKeyword = "ON";
        internal const string OffKeyword = "OFF";
        internal const string AlterKeyword = "ALTER";
        internal const string NullKeyword = "NULL";
        internal const string AsKeyword = "AS";
        internal const string OutputKeyword = "OUTPUT";
        internal const string VariablePrefix = "@";
        internal const string IntoKeyword = "INTO";
        internal const string EndKeyword = "END";
        internal const string AndKeyword = "AND";
        internal const string WithKeyword = "WITH";
        internal const string WhereKeyword = "WHERE";
        internal const string Period = ".";
        internal const string OpenParenthesis = "(";
        internal const string CloseParenthesis = ")";
        internal const string Comma = ",";
        internal const string StatementTerminator = ";";

        public SqlParserSettings()
        {
            AddDelimiter(Environment.NewLine);
            AddDelimiter("\t");
            AddDelimiter(" ");
            AddDelimiter(Period);
            AddDelimiter(Comma);
            AddDelimiter(OpenParenthesis);
            AddDelimiter(CloseParenthesis);
            AddDelimiter(StatementTerminator);
            Subgroups.Add(MultiLineCommentGroup);
            Subgroups.Add(QuotedIdentifierSubGroup);
        }

        internal static SubgroupSettings QuotedIdentifierSubGroup
        {
            get
            {
                return new SubgroupSettings
                {
                    StartSymbol = "[",
                    EndSymbol = "]"
                };
            }
        }

        internal static SubgroupSettings MultiLineCommentGroup
        {
            get
            {
                return new SubgroupSettings
                {
                    StartSymbol = "/*",
                    EndSymbol = "*/"
                };
            }
        }

        internal static string GetSubgroupValue(SubgroupSettings subGroupSettings, string subGroupValue)
        {
            return subGroupValue
                .Replace(subGroupSettings.StartSymbol, String.Empty)
                .Replace(subGroupSettings.EndSymbol, String.Empty);
        }
    }
}