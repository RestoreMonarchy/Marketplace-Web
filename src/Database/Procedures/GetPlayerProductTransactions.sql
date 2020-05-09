CREATE PROCEDURE [dbo].[GetPlayerProductTransactions]
	@PlayerId VARCHAR(255)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		t.Id,
		t.PlayerId,
		t.PlayerName,
		t.IsComplete,
		t.CreateDate,
		p.Id,
		p.Title,
		p.Price,
		p.Enabled,
		s.Id,
		s.ServerName
	FROM dbo.ProductTransactions t 
	JOIN dbo.Products p ON t.ProductId = p.Id 
	JOIN dbo.Servers s ON t.ServerId = s.Id
	WHERE PlayerId = @PlayerId;
END;