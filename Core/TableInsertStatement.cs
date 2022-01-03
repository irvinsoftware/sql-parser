using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Irvin.Parser;

namespace Irvin.SqlFountain.Core
{
    public class TableInsertStatement
        : SqlStatement
    {
        public const string InsertKeyword = "INSERT";
        internal const string ValuesKeyword = "VALUES";

        private readonly List<Name> _columnNames;
        private readonly List<SqlExpression> _valueExpressions;

        public TableInsertStatement()
        {
            _columnNames = new List<Name>();
            _valueExpressions = new List<SqlExpression>();
        }

        public ObjectAddress Table
        {
            get { return GetChildAtProperty<ObjectAddress>(nameof(Table)); }
            set { SetChildProperty(nameof(Table), value); }
        }

        public ReadOnlyCollection<Name> ColumnNames
        {
            get { return new ReadOnlyCollection<Name>(_columnNames); }
        }

        public ReadOnlyCollection<SqlExpression> ValueExpressions
        {
            get { return new ReadOnlyCollection<SqlExpression>(_valueExpressions); }
        }

        public static TableInsertStatement Create(TokenCollection tokens, StringComparison compareOption)
        {
            TableInsertStatement statement = new TableInsertStatement();

            statement.AppendChild(tokens.Current);
            tokens.MoveNextSkippingDelimiters(statement.AppendChild);
            if (tokens.Current.Content.Equals(SqlParserSettings.IntoKeyword, compareOption))
            {
                statement.AppendChild(tokens.Current);
                tokens.MoveNextSkippingDelimiters(statement.AppendChild);
            }
            else
            {
                throw new NotImplementedException();
            }

            return statement;
        }

        public void AddColumnName(Name columnName)
        {
            _columnNames.Add(columnName);
            AppendChild(columnName);
        }

        public void AddColumnValue(SqlExpression valueExpression)
        {
            _valueExpressions.Add(valueExpression);
            AppendChild(valueExpression);
        }

        internal void ParseColumnHeaders(TokenCollection tokens)
        {
            ParseColumnListing(tokens, (me, headerExpression) => me.AddColumnName((Name) headerExpression));
        }

        internal void ParseColumnValues(TokenCollection tokens)
        {
            ParseColumnListing(tokens, (me, valueExpression) => me.AddColumnValue(valueExpression));
        }

        private void ParseColumnListing(TokenCollection tokens, Action<TableInsertStatement, SqlExpression> addAction)
        {
            AppendChild(tokens.Current);
            tokens.MoveNextSkippingDelimiters(AppendChild);

            while (tokens.Current.Content != SqlParserSettings.CloseParenthesis)
            {
                List<Token> tokenBlock = tokens.MoveUntil(IsColumnDelimiter).ToList();
                Token firstToken = tokenBlock.First();

                SqlExpression columnExpression;
                if (char.IsDigit(firstToken.Content.First()))
                {
                    columnExpression = new LiteralValue(firstToken);
                }
                else
                {
                    columnExpression = Name.CreateFrom(firstToken);
                }

                IEnumerable<Token> extraTokens = tokenBlock.GetRange(1, tokenBlock.Count - 1);
                columnExpression.AppendChildren(extraTokens);
                addAction(this, columnExpression);

                if (tokens.Current.Content == SqlParserSettings.Comma)
                {
                    AppendChild(tokens.Current);
                    tokens.MoveNext();
                }

                AppendUntil(tokens, token => !token.IsSpaceOrNewLine);
            }

            AppendChild(tokens.Current);
        }

        private static bool IsColumnDelimiter(Token token)
        {
            return token.Content == SqlParserSettings.Comma ||
                   token.Content == SqlParserSettings.CloseParenthesis;
        }
    }
}