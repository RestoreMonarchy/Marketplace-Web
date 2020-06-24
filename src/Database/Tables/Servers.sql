﻿CREATE TABLE dbo.Servers 
(
	Id INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_Servers PRIMARY KEY,
	ServerName NVARCHAR(255) NOT NULL,
	ServerIP VARCHAR(255) NOT NULL,
	ServerPort INT NOT NULL,
	Enabled BIT NOT NULL CONSTRAINT DF_Servers_Enabled DEFAULT 1
);