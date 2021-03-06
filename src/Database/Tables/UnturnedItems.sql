﻿CREATE TABLE [dbo].[UnturnedItems]
(
	ItemId INT NOT NULL CONSTRAINT PK_UnturnedItems PRIMARY KEY,
	ItemName NVARCHAR(255) NOT NULL,
	ItemType VARCHAR(255) NOT NULL,
	ItemDescription NVARCHAR(800) NOT NULL,
	Amount TINYINT NOT NULL CONSTRAINT DF_UnturnedItems_Amount DEFAULT 1,
	Icon VARBINARY(MAX) NULL
)