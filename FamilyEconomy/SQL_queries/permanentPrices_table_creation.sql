USE FamilyEconomic_db;

CREATE TABLE [dbo].[permanentPrices] (
    [id]            INT IDENTITY(1,1)	NOT NULL,
    [name]			NVARCHAR(100)		NOT NULL,
    [price]			FLOAT(2)			NULL,
    
    CONSTRAINT [prim_permanentPrices] PRIMARY KEY CLUSTERED ([id] ASC)
);