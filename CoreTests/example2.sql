WITH LoanIncomeInfo AS
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
),
LoanFees AS
(
	SELECT 
		LoanID,
		SUM(Amount) AS TotalFees,
		SUM(CASE WHEN WaivedDate IS NOT NULL THEN Amount ELSE 0 END) AS FeesWavied
	FROM dbo.LoanPaymentFeesOwed
	GROUP BY LoanID
)
SELECT
	Loans.ID AS [Lease ID],
	CASE WHEN Loans.[Owner] = 0 THEN 2 ELSE Loans.[Owner] END AS [Owner ID],
	Dealers.ID AS [Retailer ID],
	Loans.ApplicantID AS [Customer ID],
	CAST(Loans.CreationDate AS DATE) AS [Submitted Date],
	CAST(FundedDate AS DATE) AS [Funded Date],
	Loans.StatusID AS LeaseQueueKey,
	DisplayID AS [Agreement Number],
	QualifiedAmount AS [Qualified Amount],
	ISNULL(ApprovalAmount, QualifiedAmount) AS [Approval Amount],
	ISNULL(Loans.ApplicationFee, 40) AS [Application Fee],
	Loans.Amount AS [Financed Amount],
	ISNULL(FundedAmount, Amount - ISNULL(DiscountCollected, Amount * 0.06) - ISNULL(Loans.ApplicationFee, 40)) AS [Funded Amount],
	TotalNote AS [Total Note],
	ISNULL(ChargeCount, 0) AS [Payments Charged],
	ISNULL(ReturnCount, 0) AS [Payments Returned],
	ISNULL(RefundCount, 0) AS [Refunds Issued],
	ISNULL(AmountCharged, 0) AS [Amount Charged],
	ISNULL(AmountReturned, 0) AS [Amount Returned],
	ISNULL(AmountRefunded, 0) AS [Amount Refunded],
	ISNULL(PastDueAmount, 0) AS [Past Due Balance],
	ISNULL(TotalFees, 0) AS [Fees Applied],
	ISNULL(FeesWavied, 0) AS [Fees Waived]
FROM dbo.Loans
	JOIN dbo.Dealers
		ON Loans.DealerID = Dealers.ID
		AND Dealers.IsDemoAccount = 0
	LEFT JOIN LoanIncomeInfo
		ON Loans.ID = LoanIncomeInfo.LoanID
	LEFT JOIN LoanFees
		ON Loans.ID = LoanFees.LoanID
WHERE CreationDate < @End
