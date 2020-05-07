CREATE PROCEDURE [dbo].[GetBuyerMarketItems]
	@PlayerId VARCHAR(255)
AS
	SELECT
		m.Id,
		m.Metadata,
		m.Quality, 
		m.Amount, 
		m.Price, 
		m.SellerId,
		m.CreateDate, 
		m.IsSold, 
		m.IsClaimed,
		m.ClaimDate,
		u.ItemId,
		u.ItemName,
		u.ItemType,
		u.ItemDescription,
		u.Amount
	FROM dbo.MarketItems m
	LEFT JOIN dbo.UnturnedItems u ON u.ItemId = m.ItemId
	WHERE
		BuyerId = @PlayerId;
RETURN;
