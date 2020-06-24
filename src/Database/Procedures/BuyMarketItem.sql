CREATE PROCEDURE BuyMarketItem 
	@Id INT, 
	@BuyerId VARCHAR(255)
AS
BEGIN

	DECLARE @sellerId VARCHAR(255), @isSold BIT;
	DECLARE @maxActiveShoppings INT = 2147483647;

	SELECT 
		@maxActiveShoppings = CAST(SettingValue AS INT) 
	FROM dbo.Settings 
	WHERE SettingId = 'MaxActiveShoppings'
	AND ISNUMERIC(SettingValue) = 1;

	SELECT 
		@sellerId = SellerId,
		@isSold = IsSold
	FROM dbo.MarketItems 
	WHERE @Id = Id;

	IF @@ROWCOUNT = 0
		RETURN 1;
	ELSE IF @sellerId = @BuyerId
		RETURN 2;
	ELSE IF @isSold = 1 
		RETURN 3;
	ELSE IF (SELECT COUNT(*) FROM dbo.MarketItems WHERE BuyerId = @BuyerId AND IsClaimed = 0) > @maxActiveShoppings
		RETURN 4;

	RETURN 0;

END;