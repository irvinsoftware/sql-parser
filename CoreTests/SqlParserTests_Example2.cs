using System.Linq;
using Irvin.SqlParser;
using NUnit.Framework;

namespace CoreTests
{
    public class SqlParserTests_Example2 : SqlParserFileExampleTestBase
    {
        protected override string FileName
        {
            get { return "example2.sql"; }
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement()
        {
            SqlCodeUnit actual = Parse();

            Assert.AreEqual(1, actual.Statements.Count);
            Assert.IsInstanceOf<SelectStatement>(actual.Statements.First());
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_WithTwoCTEs()
        {
            SqlCodeUnit codeUnit = Parse();

            SelectStatement actual = (SelectStatement) codeUnit.Statements[0];
            Assert.AreEqual(2, actual.CommonTableExpressions.Count);
            Assert.False(actual.CommonTableExpressions[0].IsRecursive);
            Assert.AreEqual("LoanIncomeInfo", actual.CommonTableExpressions[0].Name.Value);
            Assert.AreEqual(1, actual.CommonTableExpressions[0].Body.Count);
            Assert.False(actual.CommonTableExpressions[1].IsRecursive);
            Assert.AreEqual("LoanFees", actual.CommonTableExpressions[1].Name.Value);
            Assert.AreEqual(1, actual.CommonTableExpressions[1].Body.Count);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_FirstCTEBody()
        {
            SqlCodeUnit codeUnit = Parse();

            SelectExpression actual = ((SelectStatement) codeUnit.Statements[0]).CommonTableExpressions[0].Body.First() as SelectExpression;
            Assert.AreEqual(7, actual.ColumnExpressions.Count);
            Assert.IsNull(actual.TopSpecification);
            Assert.False(actual.IsDistinct);
            Assert.AreEqual("dbo", actual.PrimaryTable.Name.SchemaName.Value);
            Assert.AreEqual("LoanPayments", actual.PrimaryTable.Name.ObjectName.Value);
            Assert.AreEqual(0, actual.PrimaryTable.TableHints.Count);
            Assert.AreEqual(0, actual.Joins.Count);
            Assert.True(actual.HasWhereClause);
            Assert.AreEqual(1, actual.WherePredicates.Count);
            Assert.True(actual.IsAggregateQuery);
            Assert.AreEqual(1, actual.GroupByColumnExpressions.Count);
            Assert.False(actual.HasHavingClause);
            Assert.AreEqual(0, actual.HavingPredicates.Count);

            /*
            Assert.AreEqual(@"LoanIncomeInfo AS
(
	SELECT 
		LoanID,
		SUM(CASE WHEN Amount > 0 THEN 1 ELSE 0 END) AS ChargeCount,
		SUM(CASE WHEN Amount > 0 AND Returned = 1 THEN 1 ELSE 0 END) AS ReturnCount,
		SUM(CASE WHEN Amount < 0 AND Returned = 0 THEN 1 ELSE 0 END) AS RefundCount,
		SUM(CASE WHEN Amount > 0 THEN Amount ELSE 0 END) AS AmountCharged,
		SUM(CASE WHEN Amount > 0 AND Returned = 1 THEN Amount ELSE 0 END) AS AmountReturned,
		SUM(CASE WHEN Amount < 0 AND Returned = 0 THEN Amount ELSE 0 END) AS AmountRefunded
	FROM dbo.LoanPayments
	WHERE IsDeleted = 0
	GROUP BY LoanID
)", actual.ToString());*/
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_FirstCTEColumn1()
        {
            SqlCodeUnit codeUnit = Parse();

            SelectExpression actual = ((SelectStatement)codeUnit.Statements[0]).CommonTableExpressions[0].Body.First() as SelectExpression;
            Assert.IsInstanceOf<ColumnAddress>(actual.ColumnExpressions[0].Expression);
            Assert.Null(((ColumnAddress)actual.ColumnExpressions[0].Expression).DatabaseName);
            Assert.Null(((ColumnAddress)actual.ColumnExpressions[0].Expression).SchemaName);
            Assert.Null(((ColumnAddress)actual.ColumnExpressions[0].Expression).ObjectName);
            Assert.AreEqual("LoanID", ((ColumnAddress)actual.ColumnExpressions[0].Expression).ColumnName.Value);
            Assert.Null(actual.ColumnExpressions[0].UsesExplicitAlias);
            Assert.Null(actual.ColumnExpressions[0].Alias);
            Assert.AreEqual("LoanID", actual.ColumnExpressions[0].OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_FirstCTEColumn2()
        {
            SqlCodeUnit codeUnit = Parse();

            SelectExpression actual = ((SelectStatement)codeUnit.Statements[0]).CommonTableExpressions[0].Body.First() as SelectExpression;
            Assert.IsInstanceOf<FunctionInvocation>(actual.ColumnExpressions[1].Expression);
            Assert.True(actual.ColumnExpressions[1].UsesExplicitAlias.Value);
            Assert.AreEqual("ChargeCount", actual.ColumnExpressions[1].Alias.Value);
            Assert.AreEqual("ChargeCount", actual.ColumnExpressions[1].OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_FirstCTEColumn3()
        {
            SqlCodeUnit codeUnit = Parse();

            SelectExpression actual = ((SelectStatement)codeUnit.Statements[0]).CommonTableExpressions[0].Body.First() as SelectExpression;
            Assert.IsInstanceOf<FunctionInvocation>(actual.ColumnExpressions[2].Expression);
            Assert.True(actual.ColumnExpressions[2].UsesExplicitAlias.Value);
            Assert.AreEqual("ReturnCount", actual.ColumnExpressions[2].Alias.Value);
            Assert.AreEqual("ReturnCount", actual.ColumnExpressions[2].OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_FirstCTEColumn4()
        {
            SqlCodeUnit codeUnit = Parse();

            SelectExpression actual = ((SelectStatement)codeUnit.Statements[0]).CommonTableExpressions[0].Body.First() as SelectExpression;
            Assert.IsInstanceOf<FunctionInvocation>(actual.ColumnExpressions[3].Expression);
            Assert.True(actual.ColumnExpressions[3].UsesExplicitAlias.Value);
            Assert.AreEqual("RefundCount", actual.ColumnExpressions[3].Alias.Value);
            Assert.AreEqual("RefundCount", actual.ColumnExpressions[3].OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_FirstCTEColumn5()
        {
            SqlCodeUnit codeUnit = Parse();

            SelectExpression actual = ((SelectStatement)codeUnit.Statements[0]).CommonTableExpressions[0].Body.First() as SelectExpression;
            Assert.IsInstanceOf<FunctionInvocation>(actual.ColumnExpressions[4].Expression);
            Assert.True(actual.ColumnExpressions[4].UsesExplicitAlias.Value);
            Assert.AreEqual("AmountCharged", actual.ColumnExpressions[4].Alias.Value);
            Assert.AreEqual("AmountCharged", actual.ColumnExpressions[4].OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_FirstCTEColumn6()
        {
            SqlCodeUnit codeUnit = Parse();

            SelectExpression actual = ((SelectStatement)codeUnit.Statements[0]).CommonTableExpressions[0].Body.First() as SelectExpression;
            Assert.IsInstanceOf<FunctionInvocation>(actual.ColumnExpressions[5].Expression);
            Assert.True(actual.ColumnExpressions[5].UsesExplicitAlias.Value);
            Assert.AreEqual("AmountReturned", actual.ColumnExpressions[5].Alias.Value);
            Assert.AreEqual("AmountReturned", actual.ColumnExpressions[5].OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_FirstCTEColumn7()
        {
            SqlCodeUnit codeUnit = Parse();

            SelectExpression actual = ((SelectStatement)codeUnit.Statements[0]).CommonTableExpressions[0].Body.First() as SelectExpression;
            Assert.IsInstanceOf<FunctionInvocation>(actual.ColumnExpressions[6].Expression);
            Assert.True(actual.ColumnExpressions[6].UsesExplicitAlias.Value);
            Assert.AreEqual("AmountRefunded", actual.ColumnExpressions[6].Alias.Value);
            Assert.AreEqual("AmountRefunded", actual.ColumnExpressions[6].OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_SecondCTEBody()
        {
            SqlCodeUnit codeUnit = Parse();

            SelectExpression actual = ((SelectStatement)codeUnit.Statements[0]).CommonTableExpressions[1].Body.First() as SelectExpression;
            Assert.AreEqual(3, actual.ColumnExpressions.Count);
            Assert.IsNull(actual.TopSpecification);
            Assert.False(actual.IsDistinct);
            Assert.AreEqual("dbo", actual.PrimaryTable.Name.SchemaName.Value);
            Assert.AreEqual("LoanPaymentFeesOwed", actual.PrimaryTable.Name.ObjectName.Value);
            Assert.AreEqual(0, actual.PrimaryTable.TableHints.Count);
            Assert.AreEqual(0, actual.Joins.Count);
            Assert.False(actual.HasWhereClause);
            Assert.AreEqual(0, actual.WherePredicates.Count);
            Assert.True(actual.IsAggregateQuery);
            Assert.AreEqual(1, actual.GroupByColumnExpressions.Count);
            Assert.False(actual.HasHavingClause);
            Assert.AreEqual(0, actual.HavingPredicates.Count);

            /*
            Assert.AreEqual(@"LoanFees AS
(
	SELECT 
		LoanID,
		SUM(Amount) AS TotalFees,
		SUM(CASE WHEN WaivedDate IS NOT NULL THEN Amount ELSE 0 END) AS FeesWavied
	FROM dbo.LoanPaymentFeesOwed
	GROUP BY LoanID
)", actual.ToString());*/
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_SecondCTEColumn1()
        {
            SqlCodeUnit codeUnit = Parse();

            SelectExpression actual = ((SelectStatement)codeUnit.Statements[0]).CommonTableExpressions[1].Body.First() as SelectExpression;
            Assert.IsInstanceOf<ColumnAddress>(actual.ColumnExpressions[0].Expression);
            Assert.Null(((ColumnAddress)actual.ColumnExpressions[0].Expression).DatabaseName);
            Assert.Null(((ColumnAddress)actual.ColumnExpressions[0].Expression).SchemaName);
            Assert.Null(((ColumnAddress)actual.ColumnExpressions[0].Expression).ObjectName);
            Assert.AreEqual("LoanID", ((ColumnAddress)actual.ColumnExpressions[0].Expression).ColumnName.Value);
            Assert.Null(actual.ColumnExpressions[0].UsesExplicitAlias);
            Assert.Null(actual.ColumnExpressions[0].Alias);
            Assert.AreEqual("LoanID", actual.ColumnExpressions[0].OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_SecondCTEColumn2()
        {
            SqlCodeUnit codeUnit = Parse();

            SelectExpression actual = ((SelectStatement)codeUnit.Statements[0]).CommonTableExpressions[1].Body.First() as SelectExpression;
            Assert.IsInstanceOf<FunctionInvocation>(actual.ColumnExpressions[1].Expression);
            Assert.True(actual.ColumnExpressions[1].UsesExplicitAlias.Value);
            Assert.AreEqual("TotalFees", actual.ColumnExpressions[1].Alias.Value);
            Assert.AreEqual("TotalFees", actual.ColumnExpressions[1].OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_SecondCTEColumn3()
        {
            SqlCodeUnit codeUnit = Parse();

            SelectExpression actual = ((SelectStatement)codeUnit.Statements[0]).CommonTableExpressions[1].Body.First() as SelectExpression;
            Assert.IsInstanceOf<FunctionInvocation>(actual.ColumnExpressions[2].Expression);
            Assert.True(actual.ColumnExpressions[2].UsesExplicitAlias.Value);
            Assert.AreEqual("FeesWavied", actual.ColumnExpressions[2].Alias.Value);
            Assert.AreEqual("FeesWavied", actual.ColumnExpressions[2].OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_Body()
        {
            SqlCodeUnit codeUnit = Parse();

            SelectStatement actual = (SelectStatement) codeUnit.Statements[0];
            Assert.AreEqual(23, actual.ColumnExpressions.Count);
            Assert.AreEqual("dbo", actual.PrimaryTable.Name.SchemaName.Value);
            Assert.AreEqual("Loans", actual.PrimaryTable.Name.ObjectName.Value);
            Assert.AreEqual(3, actual.Joins.Count);
            Assert.True(actual.HasWhereClause);
            Assert.AreEqual(1, actual.WherePredicates.Count);
            Assert.False(actual.IsAggregateQuery);
            Assert.False(actual.HasHavingClause);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyColumn1()
        {
            SqlCodeUnit codeUnit = Parse();

            ColumnExpression actual = ((SelectStatement)codeUnit.Statements[0]).ColumnExpressions[0];
            Assert.IsInstanceOf<ColumnAddress>(actual.Expression);
            Assert.Null(((ColumnAddress)actual.Expression).DatabaseName);
            Assert.Null(((ColumnAddress)actual.Expression).SchemaName);
            Assert.AreEqual("Loans", ((ColumnAddress)actual.Expression).ObjectName.Value);
            Assert.AreEqual("ID", ((ColumnAddress)actual.Expression).ColumnName.Value);
            Assert.True(actual.UsesExplicitAlias.Value);
            Assert.AreEqual("Lease ID", actual.Alias.Value);
            Assert.AreEqual("Lease ID", actual.OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyColumn2()
        {
            SqlCodeUnit codeUnit = Parse();

            ColumnExpression actual = ((SelectStatement)codeUnit.Statements[0]).ColumnExpressions[1];
            Assert.IsInstanceOf<CaseExpression>(actual.Expression);
            Assert.True(actual.UsesExplicitAlias.Value);
            Assert.AreEqual("Owner ID", actual.Alias.Value);
            Assert.AreEqual("Owner ID", actual.OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyColumn3()
        {
            SqlCodeUnit codeUnit = Parse();

            ColumnExpression actual = ((SelectStatement)codeUnit.Statements[0]).ColumnExpressions[2];
            Assert.IsInstanceOf<ColumnAddress>(actual.Expression);
            Assert.Null(((ColumnAddress)actual.Expression).DatabaseName);
            Assert.Null(((ColumnAddress)actual.Expression).SchemaName);
            Assert.AreEqual("Dealers", ((ColumnAddress)actual.Expression).ObjectName.Value);
            Assert.AreEqual("ID", ((ColumnAddress)actual.Expression).ColumnName.Value);
            Assert.True(actual.UsesExplicitAlias.Value);
            Assert.AreEqual("Retailer ID", actual.Alias.Value);
            Assert.AreEqual("Retailer ID", actual.OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyColumn4()
        {
            SqlCodeUnit codeUnit = Parse();

            ColumnExpression actual = ((SelectStatement)codeUnit.Statements[0]).ColumnExpressions[3];
            Assert.IsInstanceOf<ColumnAddress>(actual.Expression);
            Assert.Null(((ColumnAddress)actual.Expression).DatabaseName);
            Assert.Null(((ColumnAddress)actual.Expression).SchemaName);
            Assert.AreEqual("Loans", ((ColumnAddress)actual.Expression).ObjectName.Value);
            Assert.AreEqual("ApplicantID", ((ColumnAddress)actual.Expression).ColumnName.Value);
            Assert.True(actual.UsesExplicitAlias.Value);
            Assert.AreEqual("Customer ID", actual.Alias.Value);
            Assert.AreEqual("Customer ID", actual.OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyColumn5()
        {
            SqlCodeUnit codeUnit = Parse();

            ColumnExpression actual = ((SelectStatement)codeUnit.Statements[0]).ColumnExpressions[4];
            Assert.IsInstanceOf<FunctionInvocation>(actual.Expression);
            Assert.True(actual.UsesExplicitAlias.Value);
            Assert.AreEqual("Submitted Date", actual.Alias.Value);
            Assert.AreEqual("Submitted Date", actual.OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyColumn6()
        {
            SqlCodeUnit codeUnit = Parse();

            ColumnExpression actual = ((SelectStatement)codeUnit.Statements[0]).ColumnExpressions[5];
            Assert.IsInstanceOf<FunctionInvocation>(actual.Expression);
            Assert.True(actual.UsesExplicitAlias.Value);
            Assert.AreEqual("Funded Date", actual.Alias.Value);
            Assert.AreEqual("Funded Date", actual.OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyColumn7()
        {
            SqlCodeUnit codeUnit = Parse();

            ColumnExpression actual = ((SelectStatement)codeUnit.Statements[0]).ColumnExpressions[6];
            Assert.IsInstanceOf<ColumnAddress>(actual.Expression);
            Assert.Null(((ColumnAddress)actual.Expression).DatabaseName);
            Assert.Null(((ColumnAddress)actual.Expression).SchemaName);
            Assert.AreEqual("Loans", ((ColumnAddress)actual.Expression).ObjectName.Value);
            Assert.AreEqual("StatusID", ((ColumnAddress)actual.Expression).ColumnName.Value);
            Assert.True(actual.UsesExplicitAlias.Value);
            Assert.AreEqual("LeaseQueueKey", actual.Alias.Value);
            Assert.AreEqual("LeaseQueueKey", actual.OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyColumn8()
        {
            SqlCodeUnit codeUnit = Parse();

            ColumnExpression actual = ((SelectStatement)codeUnit.Statements[0]).ColumnExpressions[7];
            Assert.IsInstanceOf<ColumnAddress>(actual.Expression);
            Assert.Null(((ColumnAddress)actual.Expression).DatabaseName);
            Assert.Null(((ColumnAddress)actual.Expression).SchemaName);
            Assert.Null(((ColumnAddress)actual.Expression).ObjectName);
            Assert.AreEqual("DisplayID", ((ColumnAddress)actual.Expression).ColumnName.Value);
            Assert.True(actual.UsesExplicitAlias.Value);
            Assert.AreEqual("Agreement Number", actual.Alias.Value);
            Assert.AreEqual("Agreement Number", actual.OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyColumn9()
        {
            SqlCodeUnit codeUnit = Parse();

            ColumnExpression actual = ((SelectStatement)codeUnit.Statements[0]).ColumnExpressions[8];
            Assert.IsInstanceOf<ColumnAddress>(actual.Expression);
            Assert.Null(((ColumnAddress)actual.Expression).DatabaseName);
            Assert.Null(((ColumnAddress)actual.Expression).SchemaName);
            Assert.Null(((ColumnAddress)actual.Expression).ObjectName);
            Assert.AreEqual("QualifiedAmount", ((ColumnAddress)actual.Expression).ColumnName.Value);
            Assert.True(actual.UsesExplicitAlias.Value);
            Assert.AreEqual("Qualified Amount", actual.Alias.Value);
            Assert.AreEqual("Qualified Amount", actual.OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyColumn10()
        {
            SqlCodeUnit codeUnit = Parse();

            ColumnExpression actual = ((SelectStatement)codeUnit.Statements[0]).ColumnExpressions[9];
            Assert.IsInstanceOf<FunctionInvocation>(actual.Expression);
            Assert.True(actual.UsesExplicitAlias.Value);
            Assert.AreEqual("Approval Amount", actual.Alias.Value);
            Assert.AreEqual("Approval Amount", actual.OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyColumn11()
        {
            SqlCodeUnit codeUnit = Parse();

            ColumnExpression actual = ((SelectStatement)codeUnit.Statements[0]).ColumnExpressions[10];
            Assert.IsInstanceOf<FunctionInvocation>(actual.Expression);
            Assert.True(actual.UsesExplicitAlias.Value);
            Assert.AreEqual("Application Fee", actual.Alias.Value);
            Assert.AreEqual("Application Fee", actual.OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyColumn12()
        {
            SqlCodeUnit codeUnit = Parse();

            ColumnExpression actual = ((SelectStatement)codeUnit.Statements[0]).ColumnExpressions[11];
            Assert.IsInstanceOf<ColumnAddress>(actual.Expression);
            Assert.Null(((ColumnAddress)actual.Expression).DatabaseName);
            Assert.Null(((ColumnAddress)actual.Expression).SchemaName);
            Assert.AreEqual("Loans", ((ColumnAddress)actual.Expression).ObjectName.Value);
            Assert.AreEqual("Amount", ((ColumnAddress)actual.Expression).ColumnName.Value);
            Assert.True(actual.UsesExplicitAlias.Value);
            Assert.AreEqual("Financed Amount", actual.Alias.Value);
            Assert.AreEqual("Financed Amount", actual.OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyColumn13()
        {
            SqlCodeUnit codeUnit = Parse();

            ColumnExpression actual = ((SelectStatement)codeUnit.Statements[0]).ColumnExpressions[12];
            Assert.IsInstanceOf<FunctionInvocation>(actual.Expression);
            Assert.True(actual.UsesExplicitAlias.Value);
            Assert.AreEqual("Funded Amount", actual.Alias.Value);
            Assert.AreEqual("Funded Amount", actual.OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyColumn14()
        {
            SqlCodeUnit codeUnit = Parse();

            ColumnExpression actual = ((SelectStatement)codeUnit.Statements[0]).ColumnExpressions[13];
            Assert.IsInstanceOf<ColumnAddress>(actual.Expression);
            Assert.Null(((ColumnAddress)actual.Expression).DatabaseName);
            Assert.Null(((ColumnAddress)actual.Expression).SchemaName);
            Assert.Null(((ColumnAddress)actual.Expression).ObjectName);
            Assert.AreEqual("TotalNote", ((ColumnAddress)actual.Expression).ColumnName.Value);
            Assert.True(actual.UsesExplicitAlias.Value);
            Assert.AreEqual("Total Note", actual.Alias.Value);
            Assert.AreEqual("Total Note", actual.OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyColumn15()
        {
            SqlCodeUnit codeUnit = Parse();

            ColumnExpression actual = ((SelectStatement)codeUnit.Statements[0]).ColumnExpressions[14];
            Assert.IsInstanceOf<FunctionInvocation>(actual.Expression);
            Assert.True(actual.UsesExplicitAlias.Value);
            Assert.AreEqual("Payments Charged", actual.Alias.Value);
            Assert.AreEqual("Payments Charged", actual.OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyColumn16()
        {
            SqlCodeUnit codeUnit = Parse();

            ColumnExpression actual = ((SelectStatement)codeUnit.Statements[0]).ColumnExpressions[15];
            Assert.IsInstanceOf<FunctionInvocation>(actual.Expression);
            Assert.True(actual.UsesExplicitAlias.Value);
            Assert.AreEqual("Payments Returned", actual.Alias.Value);
            Assert.AreEqual("Payments Returned", actual.OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyColumn17()
        {
            SqlCodeUnit codeUnit = Parse();

            ColumnExpression actual = ((SelectStatement)codeUnit.Statements[0]).ColumnExpressions[16];
            Assert.IsInstanceOf<FunctionInvocation>(actual.Expression);
            Assert.True(actual.UsesExplicitAlias.Value);
            Assert.AreEqual("Refunds Issued", actual.Alias.Value);
            Assert.AreEqual("Refunds Issued", actual.OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyColumn18()
        {
            SqlCodeUnit codeUnit = Parse();

            ColumnExpression actual = ((SelectStatement)codeUnit.Statements[0]).ColumnExpressions[17];
            Assert.IsInstanceOf<FunctionInvocation>(actual.Expression);
            Assert.True(actual.UsesExplicitAlias.Value);
            Assert.AreEqual("Amount Charged", actual.Alias.Value);
            Assert.AreEqual("Amount Charged", actual.OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyColumn19()
        {
            SqlCodeUnit codeUnit = Parse();

            ColumnExpression actual = ((SelectStatement)codeUnit.Statements[0]).ColumnExpressions[18];
            Assert.IsInstanceOf<FunctionInvocation>(actual.Expression);
            Assert.True(actual.UsesExplicitAlias.Value);
            Assert.AreEqual("Amount Returned", actual.Alias.Value);
            Assert.AreEqual("Amount Returned", actual.OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyColumn20()
        {
            SqlCodeUnit codeUnit = Parse();

            ColumnExpression actual = ((SelectStatement)codeUnit.Statements[0]).ColumnExpressions[19];
            Assert.IsInstanceOf<FunctionInvocation>(actual.Expression);
            Assert.True(actual.UsesExplicitAlias.Value);
            Assert.AreEqual("Amount Refunded", actual.Alias.Value);
            Assert.AreEqual("Amount Refunded", actual.OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyColumn21()
        {
            SqlCodeUnit codeUnit = Parse();

            ColumnExpression actual = ((SelectStatement)codeUnit.Statements[0]).ColumnExpressions[20];
            Assert.IsInstanceOf<FunctionInvocation>(actual.Expression);
            Assert.True(actual.UsesExplicitAlias.Value);
            Assert.AreEqual("Past Due Balance", actual.Alias.Value);
            Assert.AreEqual("Past Due Balance", actual.OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyColumn22()
        {
            SqlCodeUnit codeUnit = Parse();

            ColumnExpression actual = ((SelectStatement)codeUnit.Statements[0]).ColumnExpressions[21];
            Assert.IsInstanceOf<FunctionInvocation>(actual.Expression);
            Assert.True(actual.UsesExplicitAlias.Value);
            Assert.AreEqual("Fees Applied", actual.Alias.Value);
            Assert.AreEqual("Fees Applied", actual.OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyColumn23()
        {
            SqlCodeUnit codeUnit = Parse();

            ColumnExpression actual = ((SelectStatement)codeUnit.Statements[0]).ColumnExpressions[22];
            Assert.IsInstanceOf<FunctionInvocation>(actual.Expression);
            Assert.True(actual.UsesExplicitAlias.Value);
            Assert.AreEqual("Fees Waived", actual.Alias.Value);
            Assert.AreEqual("Fees Waived", actual.OutputName.Value);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyJoin1()
        {
            SqlCodeUnit codeUnit = Parse();

            SelectStatement actual = (SelectStatement)codeUnit.Statements[0];
            Assert.AreEqual(JoinKind.Inner, actual.Joins[0].Kind);
            Assert.AreEqual("dbo", actual.Joins[0].Source.Name.SchemaName.Value);
            Assert.AreEqual("Dealers", actual.Joins[0].Source.Name.ObjectName.Value);
            Assert.Null(actual.Joins[0].Source.Alias);
            Assert.AreEqual(0, actual.Joins[0].Source.TableHints.Count);
            Assert.AreEqual(2, actual.Joins[0].Predicates.Count);
            Assert.False(actual.Joins[0].UsesFullKind);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyJoin2()
        {
            SqlCodeUnit codeUnit = Parse();

            SelectStatement actual = (SelectStatement)codeUnit.Statements[0];
            Assert.AreEqual(JoinKind.LeftOuter, actual.Joins[1].Kind);
            Assert.Null(actual.Joins[1].Source.Name.SchemaName);
            Assert.AreEqual("LoanIncomeInfo", actual.Joins[1].Source.Name.ObjectName.Value);
            Assert.Null(actual.Joins[1].Source.Alias);
            Assert.AreEqual(0, actual.Joins[1].Source.TableHints.Count);
            Assert.AreEqual(1, actual.Joins[1].Predicates.Count);
            Assert.False(actual.Joins[1].UsesFullKind);
        }

        [Test]
        public void ParseCodeUnit_ReturnsSelectStatement_BodyJoin3()
        {
            SqlCodeUnit codeUnit = Parse();

            SelectStatement actual = (SelectStatement)codeUnit.Statements[0];
            Assert.AreEqual(JoinKind.LeftOuter, actual.Joins[2].Kind);
            Assert.Null(actual.Joins[2].Source.Name.SchemaName);
            Assert.AreEqual("LoanFees", actual.Joins[2].Source.Name.ObjectName.Value);
            Assert.Null(actual.Joins[2].Source.Alias);
            Assert.AreEqual(0, actual.Joins[2].Source.TableHints.Count);
            Assert.AreEqual(1, actual.Joins[2].Predicates.Count);
            Assert.False(actual.Joins[2].UsesFullKind);
        }
    }
}