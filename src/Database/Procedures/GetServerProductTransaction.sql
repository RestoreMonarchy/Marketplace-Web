CREATE PROCEDURE [dbo].[GetServerProductTransaction]
	@Id INT
AS
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
	WHERE t.Id = @Id AND t.IsComplete = 0;

	UPDATE dbo.ProductTransactions
	SET IsComplete = 1
	WHERE IsComplete = 0
	AND Id = @Id;
RETURN 0
