namespace bevo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Movedstockpriceproperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockDetails", "PurchasePrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Stocks", "PurchasePrice");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Stocks", "PurchasePrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.StockDetails", "PurchasePrice");
        }
    }
}
