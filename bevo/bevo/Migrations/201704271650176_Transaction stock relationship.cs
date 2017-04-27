namespace bevo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Transactionstockrelationship : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "Stock_StockID", c => c.Int());
            CreateIndex("dbo.Transactions", "Stock_StockID");
            AddForeignKey("dbo.Transactions", "Stock_StockID", "dbo.Stocks", "StockID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "Stock_StockID", "dbo.Stocks");
            DropIndex("dbo.Transactions", new[] { "Stock_StockID" });
            DropColumn("dbo.Transactions", "Stock_StockID");
        }
    }
}
