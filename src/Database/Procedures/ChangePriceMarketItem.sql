CREATE PROCEDURE ChangePriceMarketItem @Id INT, @ChangerId VARCHAR(255), @Price DECIMAL(9,2)
AS
BEGIN
	IF ((SELECT IsSold FROM dbo.MarketItems WHERE Id = @Id) = 1)
		RETURN 1
	ELSE IF (NOT ((SELECT SellerId FROM dbo.MarketItems WHERE Id = @Id) = @ChangerId))
		RETURN 2
	ELSE
		UPDATE dbo.MarketItems SET Price = @Price WHERE Id = @Id;
		RETURN 0
END