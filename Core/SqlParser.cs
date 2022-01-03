using System;
using System.Collections.Generic;
using System.Linq;
using Irvin.Parser;

namespace Irvin.SqlFountain.Core
{
    public class SqlParser : Parser.Parser
    {
        private StringComparison CompareOption { get; set; }
        public string BatchSeparator { get; set; }

        protected override ParserSettings GetSettings()
        {
            return new SqlParserSettings();
        }

        public SqlCodeUnit ParseCodeUnit(string sql)
        {
            SqlCodeUnit codeUnit = new SqlCodeUnit();

            CompareOption = StringComparison.CurrentCultureIgnoreCase;
            TokenCollection tokens = Parse(sql, CompareOption);
            while (tokens.HasNext())
            {
                //every time the loop starts, tokens.Current should be the last token of the last processed element
                tokens.MoveNextSkippingSpaces(codeUnit.AppendChild);

                if (tokens.Current.Content.Equals(SqlParserSettings.UseStatement, CompareOption))
                {
                    ParseUseStatement(codeUnit, tokens);
                }
                else if (tokens.Current.Content.Equals(BatchSeparator, CompareOption))
                {
                    ParseBatchSeparator(codeUnit, tokens);
                }
                else if (tokens.Current.Content.StartsWith(SqlParserSettings.MultiLineCommentGroup.StartSymbol))
                {
                    ParseComment(codeUnit, tokens);
                }
                else if (tokens.Current.Content.Equals(SqlParserSettings.SetKeyword, CompareOption))
                {
                    codeUnit.AppendChild(ParseSetStatement(tokens));
                }
                else if (tokens.Current.Content.Equals(SqlParserSettings.WithKeyword, CompareOption))
                {
                    SelectStatement statement = codeUnit.AddStatement<SelectStatement>();
                    statement.AppendChild(tokens.Current);
                    tokens.MoveNextSkippingDelimiters(statement.AppendChild);

                    bool parsedAllCtes = false;
                    while (!parsedAllCtes)
                    {
                        CommonTableExpression commonTableExpression = new CommonTableExpression();
                        statement.CommonTableExpressions.Add(commonTableExpression);
                        statement.AppendChild(commonTableExpression);

                        Name cteName = Name.CreateFrom(tokens.Current);
                        commonTableExpression.Name = cteName;
                        tokens.MoveNextSkippingDelimiters(commonTableExpression.AppendChild);

                        if (tokens.Current.Content.Equals("AS", CompareOption))
                        {
                            commonTableExpression.AppendChild(tokens.Current);
                            tokens.MoveNextSkippingSpacesAndNewLine(commonTableExpression.AppendChild);

                            commonTableExpression.AppendChild(tokens.Current); //'('
                            tokens.MoveNext();

                            SelectExpression body = new SelectExpression();
                            commonTableExpression.AddBodyPart(body);

                            PopulateSelectExpression(body, tokens);

                            //ends on ')'
                            commonTableExpression.AppendChild(tokens.Current);
                            tokens.MoveNextSkippingSpacesAndNewLine(commonTableExpression.AppendChild);

                            if (tokens.Current.Content.Equals(SqlParserSettings.Comma, CompareOption))
                            {
                                statement.AppendChild(tokens.Current);
                                tokens.MoveNextSkippingSpacesAndNewLine(statement.AppendChild);
                            }
                            else
                            {
                                parsedAllCtes = true;
                            }
                        }
                    }

                    PopulateSelectExpression(statement, tokens);
                }
                else if (IsSelectKeyword(tokens))
                {
                    SelectStatement statement = codeUnit.AddStatement<SelectStatement>();
                    statement.AppendChild(tokens.Current);
                    tokens.MoveNextSkippingDelimiters(statement.AppendChild);
                    PopulateSelectExpression(statement, tokens);
                }
                else if (tokens.Current.Content.Equals(SqlParserSettings.AlterKeyword, CompareOption))
                {
                    ObjectDefintionAction action = ObjectDefintionAction.Alter;
                    List<Token> firstTokens = new List<Token>();
                    firstTokens.Add(tokens.Current);
                    tokens.MoveNextSkippingSpaces(firstTokens);

                    if (tokens.Current.Content.StartsWith("PROC", CompareOption))
                    {
                        StoredProcedureStatement statement = codeUnit.AddStatement<StoredProcedureStatement>();
                        statement.AppendChildren(firstTokens);
                        statement.AppendChild(tokens.Current);

                        statement.Action = action;
                        statement.UsesFullKeyword = tokens.Current.Content.Length != 4;
                        tokens.MoveNextSkippingSpaces(statement.AppendChild);

                        statement.SchemaName = Name.CreateFrom(tokens.Current);
                        tokens.MoveNext();
                        statement.AppendChild(tokens.Current);
                        tokens.MoveNext();
                        statement.ProcedureName = Name.CreateFrom(tokens.Current);

                        MoveToNextLine(statement, tokens);
                        tokens.MoveNext();

                        ParseVariableBlock<ModuleParameter>(statement, tokens, SqlParserSettings.AsKeyword);
                        statement.AppendChild(tokens.Current);
                        MoveToNextLine(statement, tokens);
                        tokens.MoveNext();

                        statement.Body = new SqlExpression();
                        if (tokens.Current.Content.Equals(BeginKeyword.Keyword, CompareOption))
                        {
                            statement.BodyIsDelimited = true;
                            BeginKeyword beginKeyword = new BeginKeyword(tokens.Current);
                            statement.Body.AppendChild(beginKeyword);
                            MoveToNextLine(beginKeyword, tokens);
                        }

                        while (!tokens.Current.Content.Equals(SqlParserSettings.EndKeyword, CompareOption) && 
                               !tokens.Current.Content.Equals(BatchSeparator, CompareOption) && 
                               tokens.HasNext())
                        {
                            tokens.MoveNextSkippingDelimiters(statement.Body.AppendChild);
                            if (tokens.Current.Content.Equals(SqlParserSettings.SetKeyword, CompareOption))
                            {
                                statement.Body.AppendChild(ParseSetStatement(tokens));
                            }
                            else if (tokens.Current.Content.Equals("DECLARE", CompareOption))
                            {
                                DeclareVariablesStatement declareVariablesStatement = new DeclareVariablesStatement();
                                statement.Body.AppendChild(declareVariablesStatement);
                                declareVariablesStatement.AppendChild(tokens.Current);

                                tokens.MoveNextSkippingSpaces(declareVariablesStatement.AppendChild);
                                ParseVariableBlock<VariableInitializationExpression>(declareVariablesStatement, tokens, SqlParserSettings.StatementTerminator);
                            }
                            else if (tokens.Current.Content.StartsWith("EXEC", CompareOption))
                            {
                                ProcedureInvocation invocationStatement = new ProcedureInvocation();
                                statement.Body.AppendChild(invocationStatement);
                                invocationStatement.AppendChild(tokens.Current);

                                tokens.MoveNextSkippingSpaces(invocationStatement.AppendChild);
                                Name schema = Name.CreateFrom(tokens.Current);
                                invocationStatement.AppendChild(schema);
                                tokens.MoveNext();
                                invocationStatement.AppendChild(tokens.Current);
                                tokens.MoveNext();
                                Name objectName = Name.CreateFrom(tokens.Current);
                                invocationStatement.AppendChild(objectName);
                                tokens.MoveNextSkippingSpaces(invocationStatement.AppendChild);
                                invocationStatement.ProcedureName = new ObjectAddress(schema, objectName);

                                bool allParametersRecognized = false;
                                while (!allParametersRecognized && tokens.HasNext())
                                {
                                    ParameterInvocation parameter = new ParameterInvocation();
                                    invocationStatement.Parameters.Add(parameter);
                                    invocationStatement.AppendChild(parameter);

                                    parameter.Value = Name.CreateFrom(tokens.Current);
                                    parameter.AppendChild(parameter.Value);
                                    tokens.MoveNextSkippingSpaces(parameter.AppendChild);

                                    ModuleParameter.SetParameterDirection(parameter, tokens, CompareOption);

                                    if (tokens.Current.Content == ",")
                                    {
                                        invocationStatement.AppendChild(tokens.Current);
                                        tokens.MoveNextSkippingSpaces(invocationStatement.AppendChild);
                                    }
                                    else if (tokens.Current.Content == SqlParserSettings.StatementTerminator)
                                    {
                                        allParametersRecognized = true;
                                        invocationStatement.AppendChild(tokens.Current);
                                    }
                                    else
                                    {
                                        allParametersRecognized = true;
                                        tokens.MoveNextSkippingDelimiters(invocationStatement.AppendChild);
                                    }
                                }
                            }
                            else if (tokens.Current.Content.Equals(BeginKeyword.Keyword, CompareOption) ||
                                     tokens.Current.Content.Equals(TransactionDirective.CommitKeyword, CompareOption))
                            {
                                ParseTransactionDirective(statement.Body, tokens);
                            }
                            else if (tokens.Current.Content.Equals(IfStatement.IfKeyword, CompareOption))
                            {
                                IfStatement ifStatement = ParseIfStatement(tokens);
                                statement.Body.AppendChild(ifStatement);
                                AppendNewLine(statement.Body, tokens);
                            }
                            else if (tokens.Current.Content.Equals(TableInsertStatement.InsertKeyword, CompareOption))
                            {
                                TableInsertStatement insertStatement = ParseTableInsertStatement(tokens);
                                statement.Body.AppendChild(insertStatement);
                            }
                            else if (tokens.Current.Content.Equals(SqlParserSettings.EndKeyword, CompareOption))
                            {
                                statement.Body.AppendChild(tokens.Current);
                            }
                        }
                    }
                }
            }

            return codeUnit;
        }

        private void PopulateSelectExpression(SelectExpression selectExpression, TokenCollection tokens)
        {
            if (!tokens.Current.Content.Equals(SelectExpression.SelectKeyword, CompareOption))
            {
                tokens.MoveNextSkippingSpacesAndNewLine(selectExpression.AppendChild);
            }
            selectExpression.AppendChild(tokens.Current);//"SELECT"

            tokens.MoveNextSkippingSpacesAndNewLine(selectExpression.AppendChild);

            List<ColumnExpression> columnExpressions = ParseColumnList(tokens);
            selectExpression.ColumnExpressions.AddRange(columnExpressions);
            selectExpression.AppendChild(tokens.Current);
            tokens.MoveNextSkippingDelimiters(selectExpression.AppendChild);

            AppendAndAdvance(selectExpression, tokens);//"FROM"
            selectExpression.PrimaryTable = ParseTableReference(tokens);

            while (!tokens.Current.Content.Equals(SqlParserSettings.WhereKeyword, CompareOption) && 
                   !tokens.Current.Content.Equals("GROUP", CompareOption))
            {
                string currentContent = tokens.Current.Content;
                if (currentContent.Equals("JOIN", CompareOption) || currentContent.Equals("LEFT", CompareOption))
                {
                    JoinSpecification joinInfo = new JoinSpecification();
                    selectExpression.Joins.Add(joinInfo);

                    if (currentContent.Equals("JOIN", CompareOption))
                    {
                        joinInfo.Kind = JoinKind.Inner;
                    }
                    else if (currentContent.Equals("LEFT", CompareOption))
                    {
                        joinInfo.Kind = JoinKind.LeftOuter;
                    }

                    while (tokens.Current.Content.Equals("LEFT", CompareOption) ||
                           tokens.Current.Content.Equals("JOIN", CompareOption))
                    {
                        AppendAndAdvance(joinInfo, tokens);
                    }

                    joinInfo.Source = ParseTableReference(tokens);

                    bool done = false;
                    while (!done)
                    {
                        Token nextToken = tokens.PeekNext();
                        string nextContent = nextToken.Content;
                        if (nextContent.Equals(SqlParserSettings.OnKeyword, CompareOption) ||
                            nextContent.Equals(SqlParserSettings.AndKeyword, CompareOption) ||
                            nextToken.IsSpaceOrNewLine)
                        {
                            tokens.MoveNext();
                            joinInfo.AppendChild(tokens.Current);
                        }
                        else if (nextContent.Equals("JOIN", CompareOption) ||
                                 nextContent.Equals("LEFT", CompareOption) ||
                                 nextContent.Equals(SqlParserSettings.WhereKeyword, CompareOption))
                        {
                            done = true;
                        }
                        else
                        {
                            tokens.MoveNext();
                            EqualityExpression predicate = ParseEqualityExpression(tokens);
                            joinInfo.Predicates.Add(predicate);
                        }
                        
                    }
                }
                else
                {
                    selectExpression.AppendChild(tokens.Current);
                    tokens.MoveNext();
                }
            }

            if (tokens.Current.Content.Equals(SqlParserSettings.WhereKeyword, CompareOption))
            {
                selectExpression.AppendChild(tokens.Current);
                tokens.MoveNextSkippingSpaces(selectExpression.AppendChild);

                EqualityExpression equalityExpression = ParseEqualityExpression(tokens);
                selectExpression.WherePredicates.Add(equalityExpression);
                tokens.MoveNextSkippingDelimiters(equalityExpression.AppendChild);
            }

            if (tokens.Current.Content.Equals("GROUP", CompareOption))
            {
                selectExpression.AppendChild(tokens.Current);
                tokens.MoveNextSkippingSpaces(selectExpression.AppendChild);

                //"BY"
                selectExpression.AppendChild(tokens.Current);
                tokens.MoveNextSkippingSpaces(selectExpression.AppendChild);

                List<ColumnExpression> groupByColumns = ParseColumnList(tokens);
                selectExpression.GroupByColumnExpressions.AddRange(groupByColumns);
            }

            AppendAndAdvance(selectExpression, tokens);
        }

        private static TableReference ParseTableReference(TokenCollection tokens)
        {
            TableReference tableReference = new TableReference();
            tableReference.Name = ParseAddress<ObjectAddress>(tokens);
            tokens.MoveNextSkippingSpacesAndNewLine(tableReference.AppendChild);
            return tableReference;
        }

        private List<ColumnExpression> ParseColumnList(TokenCollection tokens)
        {
            List<ColumnExpression> list = new List<ColumnExpression>();

            bool allColumnsRecognized = false;
            while (!allColumnsRecognized)
            {
                ColumnExpression columnExpression;

                if (tokens.Current.Content.Equals("CASE", CompareOption))
                {
                    CaseExpression caseExpression = ParseCaseExpression(tokens);
                    columnExpression = new ColumnExpression(caseExpression);
                }
                else if (NativeFunctions.IsNativeFunctionName(tokens.Current))
                {
                    FunctionInvocation functionCall = ParseFunctionInvocation(tokens);
                    columnExpression = new ColumnExpression(functionCall);
                }
                else
                {
                    ColumnAddress objectAddress = ParseAddress<ColumnAddress>(tokens);
                    columnExpression = new ColumnExpression(objectAddress);
                }

                bool done = false;
                while (!done)
                {
                    if (NextIsEndColumnList(tokens))
                    {
                        allColumnsRecognized = true;
                        done = true;
                    }
                    else if (tokens.Current.Content.Equals(SqlParserSettings.AsKeyword, CompareOption))
                    {
                        tokens.MoveNextSkippingSpaces(columnExpression.AppendChild);

                        Name alias = Name.CreateFrom(tokens.Current);
                        columnExpression.Alias = alias;
                        columnExpression.UsesExplicitAlias = true;
                    }
                    else
                    {
                        tokens.MoveNext();
                        columnExpression.AppendChild(tokens.Current);
                        if (tokens.Current.Content.Equals(SqlParserSettings.Comma))
                        {
                            done = true;
                            tokens.MoveNextSkippingSpacesAndNewLine(columnExpression.AppendChild);
                        }
                    }
                }

                list.Add(columnExpression);
            }

            return list;
        }

        private bool NextIsEndColumnList(TokenCollection tokens)
        {
            string nextContent = tokens.PeekNext().Content;
            return nextContent.Equals("FROM", CompareOption) || nextContent.Equals(")");
        }

        private CaseExpression ParseCaseExpression(TokenCollection tokens)
        {
            CaseExpression caseExpression = new CaseExpression();

            AppendAndAdvance(caseExpression, tokens);

            if (tokens.Current.Content.Equals("WHEN", CompareOption))
            {
                AppendAndAdvance(caseExpression, tokens);

                EqualityExpression equalityExpression = ParseEqualityExpression(tokens);
                caseExpression.AppendChild(equalityExpression);

                tokens.MoveNextSkippingSpaces(caseExpression.AppendChild);

                //THEN
                AppendAndAdvance(caseExpression, tokens);

                caseExpression.AppendChild(new LiteralValue(tokens.Current));
                tokens.MoveNextSkippingSpaces(caseExpression.AppendChild);

                //ELSE
                AppendAndAdvance(caseExpression, tokens);

                ObjectAddress address = ParseAddress<ObjectAddress>(tokens);
                caseExpression.AppendChild(address);
                tokens.MoveNextSkippingSpaces(caseExpression.AppendChild);

                //END
                AppendAndAdvance(caseExpression, tokens);
            }
            return caseExpression;
        }

        private static void AppendAndAdvance(SqlExpression expression, TokenCollection tokens)
        {
            expression.AppendChild(tokens.Current);
            tokens.MoveNextSkippingSpaces(expression.AppendChild);
        }

        private bool IsSelectKeyword(TokenCollection tokens)
        {
            return tokens.Current.Content.Equals(SelectExpression.SelectKeyword, CompareOption);
        }

        private static void MoveToNextLine(SqlExpression expression, TokenCollection tokens)
        {
            tokens.MoveNextSkippingSpaces(expression.AppendChild);
            AppendNewLine(expression, tokens);
        }

        private static void ParseUseStatement(SqlCodeUnit codeUnit, TokenCollection tokens)
        {
            UseStatement useStatement = codeUnit.AddStatement<UseStatement>();
            useStatement.AppendChild(tokens.Current);
            tokens.MoveNextSkippingSpaces(useStatement.AppendChild);

            Name name = Name.CreateFrom(tokens.Current);
            useStatement.DatabaseName = name;
            useStatement.AppendChild(name);

            tokens.MoveNextSkippingSpaces(name.AppendChild);
            AppendNewLine(useStatement, tokens);
        }

        private static void ParseBatchSeparator(SqlCodeUnit codeUnit, TokenCollection tokens)
        {
            BatchSeparator batchSeparator = codeUnit.AddStatement<BatchSeparator>();
            batchSeparator.AppendChild(tokens.Current);
            MoveToNextLine(batchSeparator, tokens);
        }

        private static void ParseComment(SqlCodeUnit codeUnit, TokenCollection tokens)
        {
            SqlComment comment = codeUnit.AddStatement<SqlComment>();
            comment.AppendChild(tokens.Current);

            comment.UsesMultiLineMarkup = true;
            comment.Comment = SqlParserSettings.GetSubgroupValue(SqlParserSettings.MultiLineCommentGroup, comment.ToString());

            MoveToNextLine(comment, tokens);
        }

        private SqlExpression ParseSetStatement(TokenCollection tokens)
        {
            List<Token> tokenList = CaptureTokenBatch(tokens);

            SqlExpression statement;
            if (tokens.Current.Content.StartsWith(SqlParserSettings.VariablePrefix))
            {
                statement = ParseSetVariablesStatement(tokens);
            }
            else
            {
                statement = ParseSetOptionsStatement(tokens);
            }

            statement.InsertChildren(tokenList);

            return statement;
        }

        private SetOptionStatement ParseSetOptionsStatement(TokenCollection tokens)
        {
            SetOptionStatement statement = new SetOptionStatement();

            statement.SetOption(tokens.Current);
            tokens.MoveNextSkippingSpaces(statement.AppendChild);

            statement.SetState(tokens.Current, CompareOption);

            tokens.MoveNext();
            if (tokens.Current.Content == SqlParserSettings.StatementTerminator)
            {
                statement.AppendChild(tokens.Current);
                tokens.MoveNext();
            }
            AppendNewLine(statement, tokens);

            return statement;
        }

        private SetVariablesStatement ParseSetVariablesStatement(TokenCollection tokens)
        {
            SetVariablesStatement setVariablesStatement = new SetVariablesStatement();

            Name targetVariable = Name.CreateFrom(tokens.Current);
            setVariablesStatement.Variable = targetVariable;
            tokens.MoveNextSkippingDelimiters(targetVariable.AppendChild);

            // the '='
            setVariablesStatement.AppendChild(tokens.Current);
            tokens.MoveNextSkippingDelimiters(setVariablesStatement.AppendChild);

            if (NativeFunctions.IsNativeFunctionName(tokens.Current, CompareOption))
            {
                setVariablesStatement.Value = ParseFunctionInvocation(tokens);
            }

            if (IsStatementEnd(tokens.Current))
            {
                setVariablesStatement.AppendChild(tokens.Current);
            }

            return setVariablesStatement;
        }

        private void ParseVariableBlock<T>(SqlExpression parentExpression, TokenCollection tokens, string endDelimiter)
            where T : InitializationExpression, new()
        {
            while (!tokens.Current.Content.Equals(endDelimiter, CompareOption))
            {
                ParseVariable<T>(parentExpression, tokens);

                if (!tokens.Current.Content.Equals(endDelimiter, CompareOption))
                {
                    tokens.MoveNext();
                }
                else
                {
                    parentExpression.AppendChild(tokens.Current);
                }
            }
        }

        private void ParseVariable<T>(SqlExpression parentExpression, TokenCollection tokens)
            where T : InitializationExpression, new()
        {
            T variable = new T();
            parentExpression.AppendChild(variable);
            if (tokens.Current.IsDelimiter)
            {
                variable.AppendChild(tokens.Current);
                tokens.MoveNext();
            }

            variable.BuildName(tokens);
            variable.BuildDataType(tokens, CompareOption);

            if (tokens.Current.Content.Equals("="))
            {
                variable.AppendChild(tokens.Current);

                tokens.MoveNextSkippingSpaces(variable.AppendChild);

                SqlExpression defaultValue = null;
                if (tokens.Current.Content.Equals(SqlParserSettings.NullKeyword, CompareOption))
                {
                    defaultValue = new NullKeyword(tokens.Current);
                    tokens.MoveNext();
                }
                variable.AppendChild(defaultValue);
                variable.InitialValue = defaultValue;
            }

            if (tokens.Current.Content == ",")
            {
                variable.AppendChild(tokens.Current);
                tokens.MoveNextSkippingSpaces(variable.AppendChild);
            }

            AppendNewLine(variable, tokens);
        }

        private void ParseTransactionDirective(SqlExpression parent, TokenCollection tokens)
        {
            List<Token> startTokens = CaptureTokenBatch(tokens);
            if (tokens.Current.Content.StartsWith(TransactionDirective.ShortTransactionKeyword, CompareOption))
            {
                startTokens.Add(tokens.Current);
                TransactionDirective transactionDirective = TransactionDirective.Create(startTokens, CompareOption);

                tokens.MoveNextSkippingSpaces(transactionDirective.AppendChild);
                if (tokens.Current.Content.Equals(SqlParserSettings.StatementTerminator))
                {
                    transactionDirective.AppendChild(tokens.Current);
                }

                parent.AppendChild(transactionDirective);
            }
        }

        private IfStatement ParseIfStatement(TokenCollection tokens)
        {
            IfStatement ifStatement = new IfStatement();
            ifStatement.AppendChild(tokens.Current);
            tokens.MoveNextSkippingDelimiters(ifStatement.AppendChild);

            if (tokens.Current.Content.StartsWith(SqlParserSettings.VariablePrefix))
            {
                EqualityExpression condition = ParseEqualityExpression(tokens);
                ifStatement.Condition = condition;
                tokens.MoveNextSkippingDelimiters(condition.AppendChild);

                if (tokens.Current.Content.Equals(BeginKeyword.Keyword, CompareOption))
                {
                    ifStatement.AppendChild(tokens.Current);
                    tokens.MoveNextSkippingDelimiters(ifStatement.AppendChild);

                    SqlExpression bodyExpression = null;
                    if (tokens.Current.Content.Equals(SqlParserSettings.SetKeyword, CompareOption))
                    {
                        bodyExpression = ParseSetStatement(tokens);
                    }

                    ifStatement.MainBody = bodyExpression;
                }
            }

            tokens.MoveNextSkippingDelimiters(ifStatement.AppendChild);
            if (tokens.Current.Content.Equals(SqlParserSettings.EndKeyword, CompareOption))
            {
                ifStatement.AppendChild(tokens.Current);
                tokens.MoveNext();
            }

            return ifStatement;
        }

        private EqualityExpression ParseEqualityExpression(TokenCollection tokens)
        {
            EqualityExpression condition = new EqualityExpression();

            ColumnAddress columnAddress = ParseAddress<ColumnAddress>(tokens);
            if (columnAddress.ObjectName == null)
            {
                condition.Operand1 = columnAddress.ColumnName;
            }
            else
            {
                condition.Operand1 = columnAddress;
            }

            tokens.MoveNextSkippingDelimiters(condition.AppendChild);

            if (tokens.Current.Content.Equals("IS", CompareOption))
            {
                condition.Operator = EqualityOperator.Is;
                condition.AppendChild(tokens.Current);
                tokens.MoveNextSkippingDelimiters(condition.AppendChild);

                if (tokens.Current.Content.Equals("NULL", CompareOption))
                {
                    condition.Operand2 = new NullKeyword(tokens.Current);
                }
            }
            else if (tokens.Current.Content.Equals("=", CompareOption))
            {
                condition.Operator = EqualityOperator.EqualTo;    
                condition.AppendChild(tokens.Current);
                tokens.MoveNextSkippingDelimiters(condition.AppendChild);

                if (char.IsDigit(tokens.Current.Content.First()))
                {
                    condition.Operand2 = new LiteralValue(tokens.Current);
                }
                else
                {
                    condition.Operand2 = ParseAddress<ColumnAddress>(tokens);
                }
            }

            return condition;
        }

        private FunctionInvocation ParseFunctionInvocation(TokenCollection tokens)
        {
            FunctionInvocation invokeStatement = new FunctionInvocation(tokens);
            tokens.MoveNextSkippingSpaces(invokeStatement.AppendChild);

            //the '('
            invokeStatement.AppendChild(tokens.Current);
            tokens.MoveNext();

            while (!tokens.Current.Content.Equals(SqlParserSettings.CloseParenthesis))
            {
                if (NativeFunctions.IsNativeFunctionName(tokens.Current))
                {
                    FunctionInvocation subCall = ParseFunctionInvocation(tokens);
                    invokeStatement.AppendChild(subCall);
                }
                else
                {
                    invokeStatement.AppendChild(tokens.Current);
                    tokens.MoveNext();
                }
            }

            invokeStatement.AppendChild(tokens.Current);
            tokens.MoveNext();

            return invokeStatement;
        }

        private TableInsertStatement ParseTableInsertStatement(TokenCollection tokens)
        {
            TableInsertStatement insertStatement = TableInsertStatement.Create(tokens, CompareOption);

            ObjectAddress table = ParseAddress<ObjectAddress>(tokens);
            insertStatement.Table = table;
            tokens.MoveNext();

            table.AppendUntil(tokens, IsNextTableInsertExpressionPart);

            if (tokens.Current.Content == SqlParserSettings.OpenParenthesis)
            {
                insertStatement.ParseColumnHeaders(tokens);
                tokens.MoveNextSkippingDelimiters(insertStatement.AppendChild);
            }

            if (tokens.Current.Content.Equals(TableInsertStatement.ValuesKeyword, CompareOption))
            {
                insertStatement.ParseColumnValues(tokens);
                tokens.MoveNext();
            }

            insertStatement.AppendUntil(tokens, x => x.Content == SqlParserSettings.StatementTerminator);
            insertStatement.AppendChild(tokens.Current);

            return insertStatement;
        }

        private static T ParseAddress<T>(TokenCollection tokens)
            where T : ObjectAddress, new()
        {
            T address = new T();

            Name name = Name.CreateFrom(tokens.Current);
            address.AddName(name);

            while (tokens.PeekNext().Content.Equals(SqlParserSettings.Period))
            {
                tokens.MoveNext();
                address.AppendChild(tokens.Current);
                tokens.MoveNext();

                Name nextName = Name.CreateFrom(tokens.Current);
                address.AddName(nextName);
            }

            return address;
        }

        private bool IsNextTableInsertExpressionPart(Token token)
        {
            return token.Content == SqlParserSettings.OpenParenthesis ||
                   token.Content.Equals(TableInsertStatement.ValuesKeyword,CompareOption);
        }

        private static void AppendNewLine(SqlExpression expression, TokenCollection tokens)
        {
            if (tokens.Current.Content == Environment.NewLine)
            {
                expression.AppendChild(tokens.Current);
            }
            if (tokens.PeekNext().Content == Environment.NewLine)
            {
                AppendNewLine(expression, tokens.MoveNext());
            }
        }

        private static List<Token> CaptureTokenBatch(TokenCollection tokens)
        {
            List<Token> tokenList = new List<Token>();
            tokenList.Add(tokens.Current);
            tokens.MoveNextSkippingDelimiters(tokenList.Add);
            return tokenList;
        }

        private static bool IsStatementEnd(Token token)
        {
            return token.Content.Equals(SqlParserSettings.StatementTerminator) ||
                   token.Content.Equals(Environment.NewLine);
        }
    }
}