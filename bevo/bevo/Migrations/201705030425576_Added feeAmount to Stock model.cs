namespace bevo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedfeeAmounttoStockmodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stocks", "feeAmount", c => c.Int());
            AlterColumn("dbo.Stocks", "StockName", c => c.String(nullable: false));
            AlterColumn("dbo.Stocks", "StockTicker", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Stocks", "StockTicker", c => c.String());
            AlterColumn("dbo.Stocks", "StockName", c => c.String());
            DropColumn("dbo.Stocks", "feeAmount");
        }
    }
}
