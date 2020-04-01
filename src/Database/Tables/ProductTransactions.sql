CREATE TABLE dbo.ProductTransactions 
(
	 Id INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_ProductTransactions PRIMARY KEY,
	 ProductId INT NOT NULL CONSTRAINT FK_ProductTransactions_ProductId FOREIGN KEY REFERENCES dbo.Products(Id),
	 ExecuteCommands VARCHAR(1000) NOT NULL,
	 BuyerId VARCHAR(255) NOT NULL,
	 BuyerName NVARCHAR(255) NOT NULL,
	 CreateDate DATETIME2(0) NOT NULL CONSTRAINT DF_ProductTransactions_CreateDate DEFAULT SYSDATETIME()
);