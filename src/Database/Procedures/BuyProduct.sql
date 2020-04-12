CREATE PROCEDURE [dbo].[BuyProduct]
	@ProductId INT,
	@ServerId INT,
	@BuyerId VARCHAR(255),
	@Balance DECIMAL(9,2),
	@PlayerName NVARCHAR(255)
AS
BEGIN

	DECLARE @maxPurchases INT, @price DECIMAL(9,2), @enabled BIT, @purchases INT;

	SELECT
		@maxPurchases = MaxPurchases,
		@price = Price,
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
	ELSE IF @price > @Balance
		RETURN 4;
	ELSE
		BEGIN
			INSERT INTO dbo.ProductTransactions (ProductId, ServerId, PlayerId, PlayerName) 
			VALUES (@ProductId, @ServerId, @BuyerId, @PlayerName);
			RETURN 0;
		END;
END;
