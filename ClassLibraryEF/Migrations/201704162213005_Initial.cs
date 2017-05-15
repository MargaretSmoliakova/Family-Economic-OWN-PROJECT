namespace ClassLibraryEF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CurrentPrices",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Id_monthly_prices = c.Int(),
                    Id_permanent_prices = c.Int(),
                    Price = c.String(maxLength: 20),
                    Check_box = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MonthlyPrices", t => t.Id_monthly_prices, cascadeDelete: true)
                .ForeignKey("dbo.PermanentPrices", t => t.Id_permanent_prices, cascadeDelete: true)
                .Index(t => t.Id_monthly_prices)
                .Index(t => t.Id_permanent_prices);

            CreateTable(
                "dbo.MonthlyPrices",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 100),
                    Group = c.String(maxLength: 20),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.PermanentPrices",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 100),
                    Price = c.String(nullable: false, maxLength: 20),
                    Group = c.String(maxLength: 20),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.UserData",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 20),
                    Value = c.String(maxLength: 20),
                })
                .PrimaryKey(t => t.Id);

        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CurrentPrices", "Id_permanent_prices", "dbo.PermanentPrices");
            DropForeignKey("dbo.CurrentPrices", "Id_monthly_prices", "dbo.MonthlyPrices");
            DropIndex("dbo.CurrentPrices", new[] { "Id_permanent_prices" });
            DropIndex("dbo.CurrentPrices", new[] { "Id_monthly_prices" });
            DropTable("dbo.UserData");
            DropTable("dbo.PermanentPrices");
            DropTable("dbo.MonthlyPrices");
            DropTable("dbo.CurrentPrices");
        }
    }
}
