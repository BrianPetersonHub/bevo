namespace bevo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removingfieldfromdetailtable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.StockDetails", "PurchasePrice");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StockDetails", "PurchasePrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
