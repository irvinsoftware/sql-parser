using System;
using Irvin.Parser;

namespace Irvin.SqlParser
{
    public class SetOptionStatement : SqlStatement
    {
        private const string AnsiNulls = "ANSI_NULLS";
        private const string QuotedIdentifier = "QUOTED_IDENTIFIER";
        private const string XactAbort = "XACT_ABORT";

        public ExecutionOption Option { get; private set; }
        public ExecutionOptionState State { get; private set; }

        internal void SetOption(Token token)
        {
            switch (token.Content.ToUpper())
            {
                case AnsiNulls:
                    Option = ExecutionOption.AnsiNulls;
                    break;
                case QuotedIdentifier:
                    Option = ExecutionOption.QuotedIdentifier;
                    break;
                case XactAbort:
                    Option = ExecutionOption.TransactionAbort;
                    break;
            }

            AppendChild(token);
        }

        internal void SetState(Token token, StringComparison compareOption = StringComparison.CurrentCultureIgnoreCase)
        {
            if (token.Content.Equals(SqlParserSettings.OnKeyword, compareOption))
            {
                State = ExecutionOptionState.On;
            }
            else if (token.Content.Equals(SqlParserSettings.OffKeyword, compareOption))
            {
                State = ExecutionOptionState.Off;
            }

            AppendChild(token);
        }
    }
}