CREATE PROCEDURE ChangePriceMarketItem 
	@Id INT, 
	@PlayerId VARCHAR(255), 
	@Price DECIMAL(9,2)
AS
BEGIN
	DECLARE @isSold BIT, @sellerId VARCHAR(255); 
	
	SELECT
		@isSold = IsSold,
		@sellerId = SellerId
	FROM dbo.MarketItems
	WHERE Id = @Id;

	IF @@ROWCOUNT = 0
		RETURN 1; -- Item not found, not found
	ELSE IF @isSold = 1 
		RETURN 2; -- Item is already sold, gone
	ELSE IF @sellerId != @PlayerId
		RETURN 3; -- Player not a seller, forbidden
	ELSE IF @Price < 0
		RETURN 4; -- Price is negative, bad request
	ELSE
	BEGIN
		UPDATE dbo.MarketItems
		SET Price = @Price
		WHERE Id = @Id;
		RETURN 0;
	END;
END;