CREATE PROCEDURE dbo.CreateProduct
	@Title NVARCHAR(255),
	@Description NVARCHAR(2555),
	@Price DECIMAL(9,2),
	@Icon VARBINARY(MAX),
	@MaxPurchases INT,
	@Enabled BIT,	
	@Servers VARCHAR(255),
	@Commands VARCHAR(255)
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @productId INT;

	BEGIN TRAN;
	
	INSERT INTO dbo.Products (Title, Description, Price, Icon, MaxPurchases, Enabled) 
	VALUES (@Title, @Description, @Price, @Icon, @MaxPurchases, @Enabled);

	SET @productId = SCOPE_IDENTITY();

	INSERT INTO dbo.ProductServers (ServerId, ProductId)
	SELECT
		[value],
		@productId
	FROM STRING_SPLIT(@Servers, ',');

	INSERT INTO dbo.ProductCommands (CommandId, ProductId)
	SELECT
		[value],
		@productId
	FROM STRING_SPLIT(@Commands, ',');

	COMMIT;
	RETURN @productId;
END;