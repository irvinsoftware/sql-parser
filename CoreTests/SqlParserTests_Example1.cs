using System.Collections.Generic;
using System.Data;
using System.Linq;
using Irvin.SqlParser;
using NUnit.Framework;

namespace CoreTests
{
    public class SqlParserTests_Example1 : SqlParserFileExampleTestBase
    {
        protected override string FileName
        {
            get { return "example1.sql"; }
        }

        [Test]
        public void ParseCodeUnit_ReturnsUnitWithSameText()
        {
            SqlCodeUnit actual = _classUnderTest.ParseCodeUnit(_fileContents);
            Assert.AreEqual(_fileContents, actual.ToString());
        }

        [Test]
        public void ParseCodeUnit_ReturnsUseStatement()
        {
            SqlCodeUnit actual = _classUnderTest.ParseCodeUnit(_fileContents);

            UseStatement actualUseStatement = actual.Statements[0] as UseStatement;
            Assert.IsNotNull(actualUseStatement);
            Assert.AreEqual("IrvinPOC", actualUseStatement.DatabaseName.Value);
            Assert.True(actualUseStatement.DatabaseName.IsQuoted);
            Assert.False(actualUseStatement.DatabaseName.IsVariable);
            Assert.AreEqual("[IrvinPOC]", actualUseStatement.DatabaseName.ToString());
            Assert.AreEqual("USE [IrvinPOC]\r\n", actualUseStatement.ToString());
        }

        [Test]
        public void ParseCodeUnit_ParsesBatchSeparators()
        {
            SqlCodeUnit codeUnit = _classUnderTest.ParseCodeUnit(_fileContents);

            AssertBatchSeparatorSetup(codeUnit, 1);
            AssertBatchSeparatorSetup(codeUnit, 4);
            AssertBatchSeparatorSetup(codeUnit, 6, "\r\n");
        }

        [Test]
        public void ParseCodeUnit_ParsesSingleLineComment_CstyleMarkup()
        {
            SqlCodeUnit codeUnit = _classUnderTest.ParseCodeUnit(_fileContents);

            SqlComment actual = codeUnit.Statements[2] as SqlComment;
            Assert.IsNotNull(actual);
            Assert.AreEqual("***** Object:  StoredProcedure [Quid].[Debt_Insert]    Script Date: 08/20/2014 18:56:52 *****", actual.Comment);
            Assert.AreEqual("/****** Object:  StoredProcedure [Quid].[Debt_Insert]    Script Date: 08/20/2014 18:56:52 ******/\r\n", actual.ToString());
            Assert.True(actual.UsesMultiLineMarkup);
        }

        [Test]
        public void ParseCodeUnit_ParsesOptionsStatement_AnsiNulls()
        {
            SqlCodeUnit codeUnit = _classUnderTest.ParseCodeUnit(_fileContents);

            SetOptionStatement actual = codeUnit.Statements[3] as SetOptionStatement;
            Assert.NotNull(actual);
            Assert.AreEqual(ExecutionOption.AnsiNulls, actual.Option);
            Assert.AreEqual(ExecutionOptionState.On, actual.State);
            Assert.AreEqual("SET ANSI_NULLS ON\r\n", actual.ToString());
            Assert.False(actual.IsExplicitlyTerminated);
        }

        [Test]
        public void ParseCodeUnit_ParsesOptionsStatement_QuotedIdentifier()
        {
            SqlCodeUnit codeUnit = _classUnderTest.ParseCodeUnit(_fileContents);

            SetOptionStatement actual = codeUnit.Statements[5] as SetOptionStatement;
            Assert.NotNull(actual);
            Assert.AreEqual(ExecutionOption.QuotedIdentifier, actual.Option);
            Assert.AreEqual(ExecutionOptionState.On, actual.State);
            Assert.AreEqual("SET QUOTED_IDENTIFIER ON\r\n", actual.ToString());
            Assert.False(actual.IsExplicitlyTerminated);
        }

        [Test]
        public void ParseCodeUnit_ParsesProcedureStatement_Header()
        {
            SqlCodeUnit codeUnit = _classUnderTest.ParseCodeUnit(_fileContents);

            StoredProcedureStatement actual = codeUnit.Statements[7] as StoredProcedureStatement;
            Assert.NotNull(actual);
            Assert.AreEqual(ObjectDefintionAction.Alter, actual.Action);
            Assert.True(actual.UsesFullKeyword);
            Assert.AreEqual("[Quid]", actual.SchemaName.ToString());
            Assert.AreEqual("[Debt_Insert]", actual.ProcedureName.ToString());
        }

        [Test]
        public void ParseCodeUnit_ParsesProcedureStatement_Parameter1()
        {
            SqlCodeUnit codeUnit = _classUnderTest.ParseCodeUnit(_fileContents);

            StoredProcedureStatement actual = codeUnit.Statements[7] as StoredProcedureStatement;
            Assert.AreEqual("@MerchantName", actual.Parameters[0].Name.Value);
            Assert.True(actual.Parameters[0].Name.IsVariable);
            Assert.False(actual.Parameters[0].Name.IsQuoted);
            Assert.AreEqual("VARCHAR(512)", actual.Parameters[0].DataType.ToString());
            Assert.AreEqual(SqlDbType.VarChar, actual.Parameters[0].DataType.TypeName);
            Assert.False(actual.Parameters[0].DataType.IsQuoted);
            Assert.AreEqual(0, actual.Parameters[0].DataType.MinimumCharacters);
            Assert.AreEqual(512, actual.Parameters[0].DataType.MaximumCharacters);
            Assert.True(actual.Parameters[0].DataType.IsNullable);
            Assert.AreEqual("	@MerchantName VARCHAR(512),	\r\n", actual.Parameters[0].ToString());
            Assert.Null(actual.Parameters[0].InitialValue);
            Assert.AreEqual(ParameterDirection.Input, actual.Parameters[0].Direction);
        }

        [Test]
        public void ParseCodeUnit_ParsesProcedureStatement_Parameter2()
        {
            SqlCodeUnit codeUnit = _classUnderTest.ParseCodeUnit(_fileContents);

            StoredProcedureStatement actual = codeUnit.Statements[7] as StoredProcedureStatement;
            ModuleParameter actualParameter = actual.Parameters[1];
            Assert.AreEqual("@Description", actualParameter.Name.Value);
            Assert.True(actualParameter.Name.IsVariable);
            Assert.False(actualParameter.Name.IsQuoted);
            Assert.AreEqual("VARCHAR(256)", actualParameter.DataType.ToString());
            Assert.AreEqual(SqlDbType.VarChar, actualParameter.DataType.TypeName);
            Assert.False(actualParameter.DataType.IsQuoted);
            Assert.AreEqual(0, actualParameter.DataType.MinimumCharacters);
            Assert.AreEqual(256, actualParameter.DataType.MaximumCharacters);
            Assert.True(actualParameter.DataType.IsNullable);
            Assert.AreEqual("	@Description VARCHAR(256),\r\n", actualParameter.ToString());
            Assert.Null(actualParameter.InitialValue);
            Assert.AreEqual(ParameterDirection.Input, actualParameter.Direction);
        }

        [Test]
        public void ParseCodeUnit_ParsesProcedureStatement_Parameter3()
        {
            SqlCodeUnit codeUnit = _classUnderTest.ParseCodeUnit(_fileContents);

            StoredProcedureStatement actual = codeUnit.Statements[7] as StoredProcedureStatement;
            ModuleParameter actualParameter = actual.Parameters[2];
            Assert.AreEqual("@Amount", actualParameter.Name.Value);
            Assert.True(actualParameter.Name.IsVariable);
            Assert.False(actualParameter.Name.IsQuoted);
            Assert.AreEqual("DECIMAL(9,2)", actualParameter.DataType.ToString());
            Assert.AreEqual(SqlDbType.Decimal, actualParameter.DataType.TypeName);
            Assert.False(actualParameter.DataType.IsQuoted);
            Assert.AreEqual(9, actualParameter.DataType.Precision);
            Assert.AreEqual(9, actualParameter.DataType.TotalDigits);
            Assert.AreEqual(2, actualParameter.DataType.Scale);
            Assert.AreEqual(7, actualParameter.DataType.TotalIntegralDigits);
            Assert.AreEqual(2, actualParameter.DataType.TotalFractionalDigits);
            Assert.Null(actualParameter.DataType.MinimumCharacters);
            Assert.Null(actualParameter.DataType.MaximumCharacters);
            Assert.True(actualParameter.DataType.IsNullable);
            Assert.AreEqual("	@Amount DECIMAL(9,2),\r\n", actualParameter.ToString());
            Assert.Null(actualParameter.InitialValue);
            Assert.AreEqual(ParameterDirection.Input, actualParameter.Direction);
        }

        [Test]
        public void ParseCodeUnit_ParsesProcedureStatement_Parameter4()
        {
            SqlCodeUnit codeUnit = _classUnderTest.ParseCodeUnit(_fileContents);

            StoredProcedureStatement actual = codeUnit.Statements[7] as StoredProcedureStatement;
            ModuleParameter actualParameter = actual.Parameters[3];
            Assert.AreEqual("@Minimum", actualParameter.Name.Value);
            Assert.True(actualParameter.Name.IsVariable);
            Assert.False(actualParameter.Name.IsQuoted);
            Assert.AreEqual("DECIMAL(7,2) ", actualParameter.DataType.ToString());
            Assert.AreEqual(SqlDbType.Decimal, actualParameter.DataType.TypeName);
            Assert.False(actualParameter.DataType.IsQuoted);
            Assert.AreEqual(7, actualParameter.DataType.Precision);
            Assert.AreEqual(7, actualParameter.DataType.TotalDigits);
            Assert.AreEqual(2, actualParameter.DataType.Scale);
            Assert.AreEqual(5, actualParameter.DataType.TotalIntegralDigits);
            Assert.AreEqual(2, actualParameter.DataType.TotalFractionalDigits);
            Assert.Null(actualParameter.DataType.MinimumCharacters);
            Assert.Null(actualParameter.DataType.MaximumCharacters);
            Assert.True(actualParameter.DataType.IsNullable);
            Assert.AreEqual("	@Minimum DECIMAL(7,2) = NULL,\r\n", actualParameter.ToString());
            Assert.NotNull(actualParameter.InitialValue);
            Assert.IsInstanceOf<NullKeyword>(actualParameter.InitialValue);
            Assert.AreEqual("NULL", actualParameter.InitialValue.ToString());
            Assert.True(((NullKeyword)actualParameter.InitialValue).IsUpperCase);
            Assert.AreEqual(ParameterDirection.Input, actualParameter.Direction);
        }

        [Test]
        public void ParseCodeUnit_ParsesProcedureStatement_Parameter5()
        {
            SqlCodeUnit codeUnit = _classUnderTest.ParseCodeUnit(_fileContents);

            StoredProcedureStatement actual = codeUnit.Statements[7] as StoredProcedureStatement;
            ModuleParameter actualParameter = actual.Parameters[4];
            Assert.AreEqual("@LastUpdate", actualParameter.Name.Value);
            Assert.True(actualParameter.Name.IsVariable);
            Assert.False(actualParameter.Name.IsQuoted);
            Assert.AreEqual("DATE", actualParameter.DataType.ToString());
            Assert.AreEqual(SqlDbType.Date, actualParameter.DataType.TypeName);
            Assert.False(actualParameter.DataType.IsQuoted);
            Assert.Null(actualParameter.DataType.Precision);
            Assert.Null(actualParameter.DataType.TotalDigits);
            Assert.Null(actualParameter.DataType.Scale);
            Assert.Null(actualParameter.DataType.TotalIntegralDigits);
            Assert.Null(actualParameter.DataType.TotalFractionalDigits);
            Assert.Null(actualParameter.DataType.MinimumCharacters);
            Assert.Null(actualParameter.DataType.MaximumCharacters);
            Assert.True(actualParameter.DataType.IsNullable);
            Assert.AreEqual("	@LastUpdate DATE,\r\n", actualParameter.ToString());
            Assert.Null(actualParameter.InitialValue);
            Assert.AreEqual(ParameterDirection.Input, actualParameter.Direction);
        }

        [Test]
        public void ParseCodeUnit_ParsesProcedureStatement_Parameter6()
        {
            SqlCodeUnit codeUnit = _classUnderTest.ParseCodeUnit(_fileContents);

            StoredProcedureStatement actual = codeUnit.Statements[7] as StoredProcedureStatement;
            ModuleParameter actualParameter = actual.Parameters[5];
            Assert.AreEqual("@AccountNumber", actualParameter.Name.Value);
            Assert.True(actualParameter.Name.IsVariable);
            Assert.False(actualParameter.Name.IsQuoted);
            Assert.AreEqual("VARCHAR(64) ", actualParameter.DataType.ToString());
            Assert.AreEqual(SqlDbType.VarChar, actualParameter.DataType.TypeName);
            Assert.False(actualParameter.DataType.IsQuoted);
            Assert.AreEqual(0, actualParameter.DataType.MinimumCharacters);
            Assert.AreEqual(64, actualParameter.DataType.MaximumCharacters);
            Assert.True(actualParameter.DataType.IsNullable);
            Assert.AreEqual("	@AccountNumber VARCHAR(64) = NULL,\r\n", actualParameter.ToString());
            Assert.NotNull(actualParameter.InitialValue);
            Assert.IsInstanceOf<NullKeyword>(actualParameter.InitialValue);
            Assert.AreEqual("NULL", actualParameter.InitialValue.ToString());
            Assert.True(((NullKeyword)actualParameter.InitialValue).IsUpperCase);
            Assert.AreEqual(ParameterDirection.Input, actualParameter.Direction);
        }

        [Test]
        public void ParseCodeUnit_ParsesProcedureStatement_Parameter7()
        {
            SqlCodeUnit codeUnit = _classUnderTest.ParseCodeUnit(_fileContents);

            StoredProcedureStatement actual = codeUnit.Statements[7] as StoredProcedureStatement;
            ModuleParameter actualParameter = actual.Parameters[6];
            Assert.AreEqual("@Deadline", actualParameter.Name.Value);
            Assert.True(actualParameter.Name.IsVariable);
            Assert.False(actualParameter.Name.IsQuoted);
            Assert.AreEqual("DATE ", actualParameter.DataType.ToString());
            Assert.AreEqual(SqlDbType.Date, actualParameter.DataType.TypeName);
            Assert.False(actualParameter.DataType.IsQuoted);
            Assert.True(actualParameter.DataType.IsNullable);
            Assert.AreEqual("	@Deadline DATE = NULL,\r\n", actualParameter.ToString());
            Assert.NotNull(actualParameter.InitialValue);
            Assert.IsInstanceOf<NullKeyword>(actualParameter.InitialValue);
            Assert.AreEqual("NULL", actualParameter.InitialValue.ToString());
            Assert.True(((NullKeyword)actualParameter.InitialValue).IsUpperCase);
            Assert.AreEqual(ParameterDirection.Input, actualParameter.Direction);
        }

        [Test]
        public void ParseCodeUnit_ParsesProcedureStatement_Parameter8()
        {
            SqlCodeUnit codeUnit = _classUnderTest.ParseCodeUnit(_fileContents);

            StoredProcedureStatement actual = codeUnit.Statements[7] as StoredProcedureStatement;
            ModuleParameter actualParameter = actual.Parameters[7];
            Assert.AreEqual("@Priority", actualParameter.Name.Value);
            Assert.True(actualParameter.Name.IsVariable);
            Assert.False(actualParameter.Name.IsQuoted);
            Assert.AreEqual("TINYINT ", actualParameter.DataType.ToString());
            Assert.AreEqual(SqlDbType.TinyInt, actualParameter.DataType.TypeName);
            Assert.False(actualParameter.DataType.IsQuoted);
            Assert.True(actualParameter.DataType.IsNullable);
            Assert.AreEqual("	@Priority TINYINT = NULL,\r\n", actualParameter.ToString());
            Assert.NotNull(actualParameter.InitialValue);
            Assert.IsInstanceOf<NullKeyword>(actualParameter.InitialValue);
            Assert.AreEqual("NULL", actualParameter.InitialValue.ToString());
            Assert.True(((NullKeyword)actualParameter.InitialValue).IsUpperCase);
            Assert.AreEqual(ParameterDirection.Input, actualParameter.Direction);
        }

        [Test]
        public void ParseCodeUnit_ParsesProcedureStatement_Parameter9()
        {
            SqlCodeUnit codeUnit = _classUnderTest.ParseCodeUnit(_fileContents);

            StoredProcedureStatement actual = codeUnit.Statements[7] as StoredProcedureStatement;
            ModuleParameter actualParameter = actual.Parameters[8];
            Assert.AreEqual("@ReportsToCredit", actualParameter.Name.Value);
            Assert.True(actualParameter.Name.IsVariable);
            Assert.False(actualParameter.Name.IsQuoted);
            Assert.AreEqual("BIT ", actualParameter.DataType.ToString());
            Assert.AreEqual(SqlDbType.Bit, actualParameter.DataType.TypeName);
            Assert.False(actualParameter.DataType.IsQuoted);
            Assert.True(actualParameter.DataType.IsNullable);
            Assert.AreEqual("	@ReportsToCredit BIT = NULL,\r\n", actualParameter.ToString());
            Assert.NotNull(actualParameter.InitialValue);
            Assert.IsInstanceOf<NullKeyword>(actualParameter.InitialValue);
            Assert.AreEqual("NULL", actualParameter.InitialValue.ToString());
            Assert.True(((NullKeyword)actualParameter.InitialValue).IsUpperCase);
            Assert.AreEqual(ParameterDirection.Input, actualParameter.Direction);
        }

        [Test]
        public void ParseCodeUnit_ParsesProcedureStatement_Parameter10()
        {
            SqlCodeUnit codeUnit = _classUnderTest.ParseCodeUnit(_fileContents);

            StoredProcedureStatement actual = codeUnit.Statements[7] as StoredProcedureStatement;
            ModuleParameter actualParameter = actual.Parameters[9];
            Assert.AreEqual("@DebtId", actualParameter.Name.Value);
            Assert.True(actualParameter.Name.IsVariable);
            Assert.False(actualParameter.Name.IsQuoted);
            Assert.AreEqual("SMALLINT ", actualParameter.DataType.ToString());
            Assert.AreEqual(SqlDbType.SmallInt, actualParameter.DataType.TypeName);
            Assert.False(actualParameter.DataType.IsQuoted);
            Assert.True(actualParameter.DataType.IsNullable);
            Assert.AreEqual("	@DebtId SMALLINT OUTPUT\r\n", actualParameter.ToString());
            Assert.Null(actualParameter.InitialValue);
            Assert.AreEqual(ParameterDirection.InputOutput, actualParameter.Direction);
        }

        [Test]
        public void ParseCodeUnit_ParsesProcedureBody()
        {
            SqlCodeUnit codeUnit = Parse();

            StoredProcedureStatement actual = codeUnit.Statements[7] as StoredProcedureStatement;
            Assert.AreEqual(10, actual.Parameters.Count);
            Assert.NotNull(actual.Body);
            Assert.True(actual.BodyIsDelimited);
            
            Assert.AreEqual(@"BEGIN

	SET XACT_ABORT ON;

	DECLARE @MerchantId AS SMALLINT;	
	EXEC Quid.Merchant_Insert @MerchantName, @MerchantId OUTPUT;	
	
	BEGIN TRANSACTION;		

		IF @LastUpdate IS NULL
		BEGIN
			SET @LastUpdate = GETDATE();
		END

		INSERT INTO Quid.Debt 
			( 
				MerchantId, AccountNumber, DebtDescription, OriginalAmount, CurrentAmount, 
				Deadline, MonthlyMinimumAmount, DebtPriority, ReportsToCredit,
				LastUpdate, Active				
			)
		VALUES 
			( 
				@MerchantId, @AccountNumber, @Description, @Amount, @Amount, 
				@Deadline, @Minimum, @Priority, @ReportsToCredit,
				@LastUpdate, 1 
			);
		
		SET @DebtId = SCOPE_IDENTITY();
	
	COMMIT TRANSACTION;

END", actual.Body.ToString());
        }

        [Test]
        public void ParseCodeUnit_ParsesProcedureBody_SetXactAbortStatement()
        {
            SqlCodeUnit codeUnit = _classUnderTest.ParseCodeUnit(_fileContents);

            StoredProcedureStatement statement = codeUnit.Statements[7] as StoredProcedureStatement;
            IEnumerator<SqlExpression> children = statement.Body.Children.GetEnumerator();
            children.MoveNext();
            Assert.IsInstanceOf<BeginKeyword>(children.Current);
            Assert.AreEqual("BEGIN\r\n\r\n", children.Current.ToString());
            MovePast<ExtraMark>(children);
            SetOptionStatement actual = children.Current as SetOptionStatement;
            Assert.NotNull(actual);
            Assert.AreEqual(ExecutionOption.TransactionAbort, actual.Option);
            Assert.AreEqual(ExecutionOptionState.On, actual.State);
        }

        [Test]
        public void ParseCodeUnit_ParsesProcedureBody_VariableDeclaration()
        {
            SqlCodeUnit codeUnit = _classUnderTest.ParseCodeUnit(_fileContents);

            StoredProcedureStatement statement = codeUnit.Statements[7] as StoredProcedureStatement;
            IEnumerator<SqlExpression> children = MoveToFirstDeclare(statement);

            DeclareVariablesStatement actual = children.Current as DeclareVariablesStatement;
            Assert.NotNull(actual);
            Assert.AreEqual(1, actual.Declarations.Count);
            VariableInitializationExpression firstDeclaration = actual.Declarations.FirstOrDefault();
            Assert.NotNull(firstDeclaration);
            Assert.AreEqual("@MerchantId", firstDeclaration.Name.Value);
            Assert.True(firstDeclaration.Name.IsVariable);
            Assert.False(firstDeclaration.Name.IsQuoted);
            Assert.AreEqual(SqlDbType.SmallInt, firstDeclaration.DataType.TypeName);
            Assert.AreEqual("SMALLINT", firstDeclaration.DataType.ToString());
            Assert.True(firstDeclaration.UsesAsKeyword);
            Assert.Null(firstDeclaration.InitialValue);
            Assert.AreEqual("DECLARE @MerchantId AS SMALLINT;", actual.ToString());
            Assert.True(actual.IsExplicitlyTerminated);
        }

        [Test]
        public void ParseCodeUnit_ParsesProcedureBody_InvocationExpression()
        {
            SqlCodeUnit codeUnit = _classUnderTest.ParseCodeUnit(_fileContents);

            StoredProcedureStatement statement = codeUnit.Statements[7] as StoredProcedureStatement;
            IEnumerator<SqlExpression> children = MoveToFirstDeclare(statement);
            MovePast<DeclareVariablesStatement>(children);
            MovePast<ExtraMark>(children);

            ProcedureInvocation actual = children.Current as ProcedureInvocation;
            Assert.NotNull(actual);
            Assert.False(actual.UsesLongKeyword);
            Assert.Null(actual.ProcedureName.DatabaseName);
            Assert.AreEqual("Quid", actual.ProcedureName.SchemaName.Value);
            Assert.False(actual.ProcedureName.SchemaName.IsQuoted);
            Assert.AreEqual("Merchant_Insert", actual.ProcedureName.ObjectName.Value);
            Assert.False(actual.ProcedureName.ObjectName.IsQuoted);
            Assert.AreEqual(2, actual.Parameters.Count);
            Assert.Null(actual.Parameters[0].Name);
            Assert.IsInstanceOf<Name>(actual.Parameters[0].Value);
            Assert.AreEqual("@MerchantName", ((Name)actual.Parameters[0].Value).Value);
            Assert.AreEqual(ParameterDirection.Input, actual.Parameters[0].Direction);
            Assert.Null(actual.Parameters[1].Name);
            Assert.IsInstanceOf<Name>(actual.Parameters[1].Value);
            Assert.AreEqual("@MerchantId", ((Name)actual.Parameters[1].Value).Value);
            Assert.AreEqual(ParameterDirection.InputOutput, actual.Parameters[1].Direction);
            Assert.True(actual.IsExplicitlyTerminated);
            Assert.AreEqual("EXEC Quid.Merchant_Insert @MerchantName, @MerchantId OUTPUT;", actual.ToString());
        }

        [Test]
        public void ParseCodeUnit_ParsesProcedureBody_BeginTransaction()
        {
            SqlCodeUnit codeUnit = _classUnderTest.ParseCodeUnit(_fileContents);

            StoredProcedureStatement statement = codeUnit.Statements[7] as StoredProcedureStatement;
            IEnumerator<SqlExpression> children = MoveToFirstDeclare(statement);
            MovePast<DeclareVariablesStatement>(children);
            MovePast<ExtraMark>(children);
            MovePast<ProcedureInvocation>(children);
            MovePast<ExtraMark>(children);

            TransactionDirective actual = children.Current as TransactionDirective;
            Assert.NotNull(actual);
            Assert.AreEqual(TransactionAction.Begin, actual.Action);
            Assert.True(actual.UsesLongKeyword);
            Assert.Null(actual.TransactionName);
            Assert.True(actual.IsExplicitlyTerminated);
            Assert.AreEqual("BEGIN TRANSACTION;", actual.ToString());
        }

        [Test]
        public void ParseCodeUnit_ParsesProcedureBody_IfStatement()
        {
            SqlCodeUnit codeUnit = _classUnderTest.ParseCodeUnit(_fileContents);

            StoredProcedureStatement statement = codeUnit.Statements[7] as StoredProcedureStatement;
            IEnumerator<SqlExpression> children = MoveToFirstDeclare(statement);
            MovePast<DeclareVariablesStatement>(children);
            MovePast<ExtraMark>(children);
            MovePast<ProcedureInvocation>(children);
            MovePast<ExtraMark>(children);
            MovePast<TransactionDirective>(children);
            MovePast<ExtraMark>(children);

            IfStatement actual = children.Current as IfStatement;
            Assert.NotNull(actual);
            Assert.IsInstanceOf<EqualityExpression>(actual.Condition);
            EqualityExpression actualCondition = (EqualityExpression)actual.Condition;
            Assert.IsInstanceOf<Name>(actualCondition.Operand1);
            Assert.AreEqual("@LastUpdate", ((Name)actualCondition.Operand1).Value);
            Assert.AreEqual(EqualityOperator.Is, actualCondition.Operator);
            Assert.IsInstanceOf<NullKeyword>(actualCondition.Operand2);
            Assert.AreEqual(@"@LastUpdate IS NULL
		", actualCondition.ToString());
            Assert.IsInstanceOf<SetVariablesStatement>(actual.MainBody);
            SetVariablesStatement actualMainBody = (SetVariablesStatement)actual.MainBody;
            Assert.AreEqual("@LastUpdate", actualMainBody.Variable.Value);
            Assert.True(actualMainBody.Variable.IsVariable);
            Assert.IsInstanceOf<FunctionInvocation>(actualMainBody.Value);
            FunctionInvocation actualSetBody = actualMainBody.Value as FunctionInvocation;
            Assert.Null(actualSetBody.FunctionName.DatabaseName);
            Assert.Null(actualSetBody.FunctionName.SchemaName);
            Assert.AreEqual("GETDATE", actualSetBody.FunctionName.ObjectName.Value);
            Assert.AreEqual(0, actualSetBody.Parameters.Count);
            Assert.AreEqual("GETDATE()", actualSetBody.ToString());
            Assert.AreEqual(@"SET @LastUpdate = GETDATE();", actualMainBody.ToString());
            Assert.True(actualMainBody.IsExplicitlyTerminated);
            Assert.IsNull(actual.ElseBody);
            Assert.AreEqual(@"IF @LastUpdate IS NULL
		BEGIN
			SET @LastUpdate = GETDATE();
		END", actual.ToString());
        }

        [Test]
        public void ParseCodeUnit_ParsesProcedureBody_InsertStatement()
        {
            SqlCodeUnit codeUnit = _classUnderTest.ParseCodeUnit(_fileContents);

            StoredProcedureStatement statement = codeUnit.Statements[7] as StoredProcedureStatement;
            IEnumerator<SqlExpression> children = MoveToFirstDeclare(statement);
            MovePast<DeclareVariablesStatement>(children);
            MovePast<ExtraMark>(children);
            MovePast<ProcedureInvocation>(children);
            MovePast<ExtraMark>(children);
            MovePast<TransactionDirective>(children);
            MovePast<ExtraMark>(children);
            MovePast<IfStatement>(children);
            MovePast<ExtraMark>(children);

            TableInsertStatement actual = children.Current as TableInsertStatement;
            Assert.NotNull(actual);
            Assert.AreEqual("Quid", actual.Table.SchemaName.Value);
            Assert.AreEqual("Debt", actual.Table.ObjectName.Value);
            Assert.AreEqual(11, actual.ColumnNames.Count);
            Assert.AreEqual("MerchantId", actual.ColumnNames[0].Value);
            Assert.AreEqual("AccountNumber", actual.ColumnNames[1].Value);
            Assert.AreEqual("DebtDescription", actual.ColumnNames[2].Value);
            Assert.AreEqual("OriginalAmount", actual.ColumnNames[3].Value);
            Assert.AreEqual("CurrentAmount", actual.ColumnNames[4].Value);
            Assert.AreEqual("Deadline", actual.ColumnNames[5].Value);
            Assert.AreEqual("MonthlyMinimumAmount", actual.ColumnNames[6].Value);
            Assert.AreEqual("DebtPriority", actual.ColumnNames[7].Value);
            Assert.AreEqual("ReportsToCredit", actual.ColumnNames[8].Value);
            Assert.AreEqual("LastUpdate", actual.ColumnNames[9].Value);
            Assert.AreEqual("Active", actual.ColumnNames[10].Value);
            Assert.AreEqual(11, actual.ValueExpressions.Count);
            Assert.AreEqual("@MerchantId", ((Name)actual.ValueExpressions[0]).Value);
            Assert.AreEqual("@AccountNumber", ((Name)actual.ValueExpressions[1]).Value);
            Assert.AreEqual("@Description", ((Name)actual.ValueExpressions[2]).Value);
            Assert.AreEqual("@Amount", ((Name)actual.ValueExpressions[3]).Value);
            Assert.AreEqual("@Amount", ((Name)actual.ValueExpressions[4]).Value);
            Assert.AreEqual("@Deadline", ((Name)actual.ValueExpressions[5]).Value);
            Assert.AreEqual("@Minimum", ((Name)actual.ValueExpressions[6]).Value);
            Assert.AreEqual("@Priority", ((Name)actual.ValueExpressions[7]).Value);
            Assert.AreEqual("@ReportsToCredit", ((Name)actual.ValueExpressions[8]).Value);
            Assert.AreEqual("@LastUpdate", ((Name)actual.ValueExpressions[9]).Value);
            Assert.IsInstanceOf<LiteralValue>(actual.ValueExpressions[10]);
            Assert.AreEqual("1", ((LiteralValue)actual.ValueExpressions[10]).Value);
            Assert.True(actual.IsExplicitlyTerminated);
            Assert.AreEqual(@"INSERT INTO Quid.Debt 
			( 
				MerchantId, AccountNumber, DebtDescription, OriginalAmount, CurrentAmount, 
				Deadline, MonthlyMinimumAmount, DebtPriority, ReportsToCredit,
				LastUpdate, Active				
			)
		VALUES 
			( 
				@MerchantId, @AccountNumber, @Description, @Amount, @Amount, 
				@Deadline, @Minimum, @Priority, @ReportsToCredit,
				@LastUpdate, 1 
			);", actual.ToString());
        }

        [Test]
        public void ParseCodeUnit_ParsesProcedureBody_UnconditionalSetStatement()
        {
            SqlCodeUnit codeUnit = _classUnderTest.ParseCodeUnit(_fileContents);

            StoredProcedureStatement statement = codeUnit.Statements[7] as StoredProcedureStatement;
            IEnumerator<SqlExpression> children = MoveToFirstDeclare(statement);
            MovePast<DeclareVariablesStatement>(children);
            MovePast<ExtraMark>(children);
            MovePast<ProcedureInvocation>(children);
            MovePast<ExtraMark>(children);
            MovePast<TransactionDirective>(children);
            MovePast<ExtraMark>(children);
            MovePast<IfStatement>(children);
            MovePast<ExtraMark>(children);
            MovePast<TableInsertStatement>(children);
            MovePast<ExtraMark>(children);

            SetVariablesStatement actual = children.Current as SetVariablesStatement;
            Assert.NotNull(actual);
            Assert.AreEqual("@DebtId", actual.Variable.Value);
            Assert.True(actual.Variable.IsVariable);
            Assert.IsInstanceOf<FunctionInvocation>(actual.Value);
            FunctionInvocation actualSetBody = actual.Value as FunctionInvocation;
            Assert.Null(actualSetBody.FunctionName.DatabaseName);
            Assert.Null(actualSetBody.FunctionName.SchemaName);
            Assert.AreEqual("SCOPE_IDENTITY", actualSetBody.FunctionName.ObjectName.Value);
            Assert.AreEqual(0, actualSetBody.Parameters.Count);
            Assert.AreEqual("SCOPE_IDENTITY()", actualSetBody.ToString());
            Assert.AreEqual("SET @DebtId = SCOPE_IDENTITY();", actual.ToString());
            Assert.True(actual.IsExplicitlyTerminated);
        }

        [Test]
        public void ParseCodeUnit_ParsesProcedureBody_TransactionCommit()
        {
            SqlCodeUnit codeUnit = _classUnderTest.ParseCodeUnit(_fileContents);

            StoredProcedureStatement statement = codeUnit.Statements[7] as StoredProcedureStatement;
            IEnumerator<SqlExpression> children = MoveToFirstDeclare(statement);
            MovePast<DeclareVariablesStatement>(children);
            MovePast<ExtraMark>(children);
            MovePast<ProcedureInvocation>(children);
            MovePast<ExtraMark>(children);
            MovePast<TransactionDirective>(children);
            MovePast<ExtraMark>(children);
            MovePast<IfStatement>(children);
            MovePast<ExtraMark>(children);
            MovePast<TableInsertStatement>(children);
            MovePast<ExtraMark>(children);
            MovePast<SetVariablesStatement>(children);
            MovePast<ExtraMark>(children);

            TransactionDirective actual = children.Current as TransactionDirective;
            Assert.NotNull(actual);
            Assert.AreEqual(TransactionAction.Commit, actual.Action);
            Assert.True(actual.UsesLongKeyword);
            Assert.True(actual.IsExplicitlyTerminated);
        }

        private static void AssertBatchSeparatorSetup(SqlCodeUnit codeUnit, int index, string extraSpaces = null)
        {
            BatchSeparator actual = codeUnit.Statements[index] as BatchSeparator;
            Assert.IsNotNull(actual);
            Assert.AreEqual("GO\r\n" + extraSpaces, actual.ToString());
        }

        private static IEnumerator<SqlExpression> MoveToFirstDeclare(StoredProcedureStatement statement)
        {
            IEnumerator<SqlExpression> children = statement.Body.Children.GetEnumerator();
            children.MoveNext();
            MovePast<BeginKeyword>(children);
            MovePast<ExtraMark>(children);
            MovePast<SetOptionStatement>(children);
            MovePast<ExtraMark>(children);
            return children;
        }

        private static void MovePast<T>(IEnumerator<SqlExpression> expressionCollection)
            where T : SqlExpression
        {
            do
            {
                expressionCollection.MoveNext();
            } while (expressionCollection.Current is T);
        }
    }
}