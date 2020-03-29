CREATE PROCEDURE SellMarketItem @ItemId INT, @Metadata VARBINARY(128), @Quality TINYINT, @Amount TINYINT, @Price DECIMAL(9, 2), @SellerId VARCHAR(255)
AS
BEGIN
	IF ((SELECT COUNT(*) FROM dbo.MarketItems WHERE SellerId = @SellerId AND IsSold = 1) > 
	CAST((SELECT SettingValue FROM dbo.Settings WHERE SettingId = 'MaxActiveSellings') AS INT))
		RETURN 1
	ELSE
		INSERT INTO dbo.MarketItems (ItemId, Quality, Amount, Metadata, Price, SellerId)
		VALUES (@ItemId, @Quality, @Amount,  @Metadata, @Price, @SellerId)
		RETURN 0
END