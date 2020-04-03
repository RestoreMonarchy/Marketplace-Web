CREATE PROCEDURE dbo.CreateProduct
	@Title NVARCHAR(255),
	@Description NVARCHAR(2555),
	@Price DECIMAL(9,2),
	@ExecuteCommands NVARCHAR(MAX),
	@Icon VARBINARY(MAX),
	@MaxPurchases INT,
	@Expires BIT,
	@Enabled BIT,
	@Servers VARCHAR(255)
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @productId INT;

	BEGIN TRAN;
	
	INSERT INTO dbo.Products (Title, Description, Price, ExecuteCommands, Icon, MaxPurchases, Expires, Enabled) 
	VALUES (@Title, @Description, @Price, @ExecuteCommands, @Icon, @MaxPurchases, @Expires, @Enabled);

	SET @productId = SCOPE_IDENTITY();

	INSERT INTO dbo.ProductServers (ServerId, ProductId)
	SELECT
		[value],
		@productId
	FROM STRING_SPLIT(@Servers, ',');
	COMMIT;
	RETURN @productId;
END;