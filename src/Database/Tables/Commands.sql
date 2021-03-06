﻿CREATE TABLE dbo.Commands
(
	Id INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_Commands PRIMARY KEY,
	CommandName NVARCHAR(50) NOT NULL CONSTRAINT UK_Commands UNIQUE,
	CommandHelp NVARCHAR(255) NOT NULL,
	CommandText NVARCHAR(255) NOT NULL,
	Expires BIT NOT NULL,
	ExpireTime INT NOT NULL,
	ExpireCommand NVARCHAR(255) NULL,
	ExecuteOnBuyerJoinServer BIT NOT NULL
);