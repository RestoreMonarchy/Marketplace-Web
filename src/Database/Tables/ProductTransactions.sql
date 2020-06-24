CREATE TABLE dbo.ProductTransactions 
(
	 Id INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_ProductTransactions PRIMARY KEY,
	 ProductId INT NOT NULL CONSTRAINT FK_ProductTransactions_ProductId FOREIGN KEY REFERENCES dbo.Products(Id),
	 ServerId INT NULL CONSTRAINT FK_ProductTransactions_ServerId FOREIGN KEY REFERENCES dbo.Servers(Id),
	 PlayerId VARCHAR(255) NOT NULL,
	 PlayerName NVARCHAR(255) NOT NULL,
	 IsComplete BIT NOT NULL CONSTRAINT DF_ProductTransactions_IsComplete DEFAULT 0,
	 CreateDate DATETIME2(0) NOT NULL CONSTRAINT DF_ProductTransactions_CreateDate DEFAULT SYSDATETIME()
);