CREATE PROCEDURE dbo.GetProducts 
AS
BEGIN
	SET NOCOUNT ON;
	SELECT 
		p.Id,
		p.Title,
		p.Description,
		p.Price,
		p.ExecuteCommands,
		p.MaxPurchases,
		p.Expires,
		p.Enabled,
		s.Id,
		s.ServerName,
		s.ServerIP,
		s.ServerPort,
		s.Enabled
	FROM dbo.Products p 
	LEFT JOIN dbo.ProductServers ps ON p.Id = ps.ProductId 
	LEFT JOIN dbo.Servers s ON ps.ServerId = s.Id;
END;
GO