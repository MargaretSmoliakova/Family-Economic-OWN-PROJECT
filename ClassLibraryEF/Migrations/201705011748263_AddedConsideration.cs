namespace ClassLibraryEF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedConsideration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CurrentPrices", "Consideration", c => c.Int());
            AddColumn("dbo.PermanentPrices", "Consideration", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PermanentPrices", "Consideration");
            DropColumn("dbo.CurrentPrices", "Consideration");
        }
    }
}
