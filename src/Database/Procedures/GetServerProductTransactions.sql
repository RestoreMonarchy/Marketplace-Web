CREATE PROCEDURE [dbo].[GetServerProductTransactions]
	@ServerId INT
AS
BEGIN
	SET XACT_ABORT ON;
	SET NOCOUNT ON;
	DECLARE @ProductTransactions TABLE (Id INT);

	BEGIN TRAN;

	UPDATE dbo.ProductTransactions
	SET IsComplete = 1
	OUTPUT INSERTED.Id INTO @ProductTransactions
	WHERE IsComplete = 0
	AND ServerId = @ServerId;

	SELECT
		t.Id AS TransactionId,
		t.PlayerId,
		t.PlayerName,
		t.CreateDate AS TransactionCreateDate,
		p.Id AS ProductId,
		p.Title AS ProductTitle,
		p.Price AS ProductPrice,
		c.Id,
		c.CommandName,
		c.CommandText,
		c.Expires,
		c.ExpireTime,
		c.ExpireCommand,
		c.ExecuteOnBuyerJoinServer
	FROM dbo.ProductTransactions t 
		LEFT JOIN dbo.Products p ON t.ProductId = p.Id 
		LEFT JOIN dbo.ProductCommands pc ON pc.ProductId = p.Id 
		LEFT JOIN dbo.Commands c ON pc.CommandId = c.Id
	WHERE t.Id IN (SELECT pt.Id FROM @ProductTransactions pt);

	COMMIT;

	RETURN 0;

END;