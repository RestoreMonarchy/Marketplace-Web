CREATE TABLE dbo.ProductCommands
(
	Id INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_ProductCommands PRIMARY KEY,
	ProductId INT NOT NULL CONSTRAINT FK_ProductCommands_ProductId FOREIGN KEY REFERENCES dbo.Products(Id),
	CommandId INT NOT NULL CONSTRAINT FK_ProductCommands_CommandId FOREIGN KEY REFERENCES dbo.Commands(Id),
	CONSTRAINT UK_ProductCommands UNIQUE (ProductId, CommandId)
);