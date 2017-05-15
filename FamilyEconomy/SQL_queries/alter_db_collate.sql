USE FamilyEconomic_db;

ALTER TABLE currentPrices DROP [foreign_monthlyPrices]
GO
ALTER TABLE currentPrices ADD CONSTRAINT [foreign_monthlyPrices] FOREIGN KEY ([id_monthly_prices]) REFERENCES [dbo].[monthlyPrices] ([id]) ON DELETE CASCADE
GO