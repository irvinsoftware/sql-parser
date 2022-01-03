USE [IrvinPOC]
GO
/****** Object:  StoredProcedure [Quid].[Debt_Insert]    Script Date: 08/20/2014 18:56:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [Quid].[Debt_Insert]
	@MerchantName VARCHAR(512),	
	@Description VARCHAR(256),
	@Amount DECIMAL(9,2),
	@Minimum DECIMAL(7,2) = NULL,
	@LastUpdate DATE,
	@AccountNumber VARCHAR(64) = NULL,
	@Deadline DATE = NULL,
	@Priority TINYINT = NULL,
	@ReportsToCredit BIT = NULL,
	@DebtId SMALLINT OUTPUT
AS
BEGIN

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

END