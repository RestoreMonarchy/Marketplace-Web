CREATE PROCEDURE dbo.UpdateProduct
	@Id INT,
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


	BEGIN TRAN;
	
	UPDATE dbo.Products
	SET Title = @Title,
		Description = @Description,
		Price = @Price,
		Icon = @Icon,
		MaxPurchases = @MaxPurchases,
		Enabled = @Enabled
	WHERE Id = @Id;

	-- Servers

	WITH CTE_SourceProductServers AS (
		SELECT
			ServerId = [value],
			ProductId = @Id
		FROM STRING_SPLIT(@Servers, ',')
	), 
	CTE_TargetProductServers AS (
		SELECT * FROM dbo.ProductServers WHERE ProductId = @Id
	)
	MERGE CTE_TargetProductServers AS t
	USING CTE_SourceProductServers AS s ON s.ServerId = t.ServerId
	WHEN NOT MATCHED BY TARGET THEN 
		INSERT (ProductId, ServerId) VALUES (s.ProductId, s.ServerId)
	WHEN NOT MATCHED BY SOURCE THEN 
		DELETE;

	-- Commands

	WITH CTE_SourceProductCommands AS (
		SELECT
			CommandId = [value],
			ProductId = @Id
		FROM STRING_SPLIT(@Commands, ',')
	), 
	CTE_TargetProductCommands AS (
		SELECT * FROM dbo.ProductCommands WHERE ProductId = @Id
	)
	MERGE CTE_TargetProductCommands AS t
	USING CTE_SourceProductCommands AS s ON s.CommandId = t.CommandId
	WHEN NOT MATCHED BY TARGET THEN 
		INSERT (ProductId, CommandId) VALUES (s.ProductId, s.CommandId)
	WHEN NOT MATCHED BY SOURCE THEN 
		DELETE;

	COMMIT;
END;