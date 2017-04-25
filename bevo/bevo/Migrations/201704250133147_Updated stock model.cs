namespace bevo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updatedstockmodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stocks", "StockTicker", c => c.String());
            AddColumn("dbo.Stocks", "TypeOfStock", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stocks", "TypeOfStock");
            DropColumn("dbo.Stocks", "StockTicker");
        }
    }
}
