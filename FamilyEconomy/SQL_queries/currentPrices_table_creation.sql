CREATE TABLE currentPrices (
	id INT IDENTITY(1,1) NOT NULL,
	id_monthly_prices INT NULL,
	id_permanent_prices INT NULL,
	[price] NVARCHAR(20) NULL,
	[check_box] INT NULL,
	CONSTRAINT prim_currentPrices PRIMARY KEY CLUSTERED ([id] ASC),
	CONSTRAINT foreign_monthlyPrices FOREIGN KEY (id_monthly_prices) REFERENCES monthlyPrices (id),
	CONSTRAINT foreign_permanentPrices FOREIGN KEY (id_permanent_prices) REFERENCES permanentPrices (id),
	CONSTRAINT check_check_box CHECK ([check_box] IN (1))
);
