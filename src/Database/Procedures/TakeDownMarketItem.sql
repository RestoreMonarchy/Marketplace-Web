CREATE PROCEDURE [dbo].[TakeDownMarketItem]
	@Id INT,
	@PlayerId VARCHAR(255),
	@PlayerName NVARCHAR(255)
AS
BEGIN
	DECLARE @sellerId VARCHAR(255), @isSold BIT, @isEnabled VARCHAR(255);

	SELECT 
		@isEnabled = SettingValue
	FROM dbo.Settings
	WHERE SettingId = 'MarketItemsTakeDownEnabled';

	IF (@isEnabled = 'false')
		RETURN 1; -- MarketItems Take Down disabled

	SELECT
		@sellerId = SellerId,
		@isSold = IsSold
	FROM dbo.MarketItems
	WHERE Id = @Id;

	IF @@ROWCOUNT = 0
		RETURN 2; -- Item not found
	IF @isSold = 1
		RETURN 3; -- Item Already Sold
	ELSE IF @sellerId != @PlayerId
		RETURN 4; -- Player is not a seller
	ELSE
	BEGIN
		UPDATE dbo.MarketItems 
		SET 
			BuyerId = @sellerId,
			BuyerName = @PlayerName,
			SoldDate = SYSDATETIME()
		WHERE Id = @Id;
		RETURN 0; -- Successfully taken down the item
	END;
END;