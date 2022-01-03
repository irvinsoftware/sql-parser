using System;
using System.Collections.Generic;
using System.Linq;
using Irvin.Parser;

namespace Irvin.SqlParser
{
    public class TransactionDirective : SqlStatement
    {
        public const string CommitKeyword = "COMMIT";
        internal const string ShortTransactionKeyword = "TRAN";
        private const string LongTransactionKeyword = "TRANSACTION";

        public TransactionAction Action { get; set; }
        public bool UsesLongKeyword { get; set; }
        public string TransactionName { get; set; }

        public static TransactionDirective Create(List<Token> tokens, StringComparison compareOption)
        {
            TransactionDirective item = new TransactionDirective();

            string actionKeyword = tokens.First().Content;
            if (actionKeyword.Equals(BeginKeyword.Keyword, compareOption))
            {
                item.Action = TransactionAction.Begin;
            }
            else if (actionKeyword.Equals(CommitKeyword, compareOption))
            {
                item.Action = TransactionAction.Commit;
            }

            if (tokens.Last().Content.Equals(LongTransactionKeyword, compareOption))
            {
                item.UsesLongKeyword = true;
            }
            item.AppendChildren(tokens);

            return item;
        }
    }
}