CREATE TABLE dbo.ProductServers 
(
	ServerId INT CONSTRAINT FK_ProductServers_ServerId FOREIGN KEY REFERENCES dbo.Servers(Id) ON DELETE CASCADE,
	ProductId INT CONSTRAINT FK_ProductServers_ProductId FOREIGN KEY REFERENCES dbo.Products(Id) ON DELETE CASCADE,
	CONSTRAINT PK_ProductServers PRIMARY KEY (ServerId, ProductId)
);