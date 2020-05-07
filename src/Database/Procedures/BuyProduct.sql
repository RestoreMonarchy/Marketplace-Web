CREATE PROCEDURE [dbo].[BuyProduct]
	@ProductId INT,
	@ServerId INT,
	@BuyerId VARCHAR(255)
AS
BEGIN

	DECLARE @maxPurchases INT, @enabled BIT, @purchases INT;

	SELECT
		@maxPurchases = MaxPurchases,
		@enabled = Enabled
	FROM dbo.Products
	WHERE Id = @ProductId;

	IF @@ROWCOUNT = 0
		RETURN 1;

	SELECT
		@purchases = COUNT(*)
	FROM dbo.ProductTransactions
	WHERE ProductId = @ProductId AND PlayerId = @BuyerId;

	IF @enabled = 0
		RETURN 2;
	ELSE IF @maxPurchases > 0 AND @maxPurchases <= @purchases
		RETURN 3;
	ELSE
		RETURN 0;
END;
