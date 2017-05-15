namespace ClassLibraryEF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeletedPermanentPriceConsiderationCol : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.PermanentPrices", "Consideration");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PermanentPrices", "Consideration", c => c.Int());
        }
    }
}
