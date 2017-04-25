namespace bevo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fixingstockandstockdetailmodels : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StockPortfolios", "StockDetail_StockDetailID", "dbo.StockDetails");
            DropForeignKey("dbo.Stocks", "StockDetail_StockDetailID", "dbo.StockDetails");
            DropIndex("dbo.StockPortfolios", new[] { "StockDetail_StockDetailID" });
            DropIndex("dbo.Stocks", new[] { "StockDetail_StockDetailID" });
            AddColumn("dbo.StockDetails", "Stock_StockID", c => c.Int());
            AddColumn("dbo.StockDetails", "StockPortfolio_StockPortfolioID", c => c.String(maxLength: 128));
            CreateIndex("dbo.StockDetails", "Stock_StockID");
            CreateIndex("dbo.StockDetails", "StockPortfolio_StockPortfolioID");
            AddForeignKey("dbo.StockDetails", "Stock_StockID", "dbo.Stocks", "StockID");
            AddForeignKey("dbo.StockDetails", "StockPortfolio_StockPortfolioID", "dbo.StockPortfolios", "StockPortfolioID");
            DropColumn("dbo.StockPortfolios", "StockDetail_StockDetailID");
            DropColumn("dbo.Stocks", "StockDetail_StockDetailID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Stocks", "StockDetail_StockDetailID", c => c.Int());
            AddColumn("dbo.StockPortfolios", "StockDetail_StockDetailID", c => c.Int());
            DropForeignKey("dbo.StockDetails", "StockPortfolio_StockPortfolioID", "dbo.StockPortfolios");
            DropForeignKey("dbo.StockDetails", "Stock_StockID", "dbo.Stocks");
            DropIndex("dbo.StockDetails", new[] { "StockPortfolio_StockPortfolioID" });
            DropIndex("dbo.StockDetails", new[] { "Stock_StockID" });
            DropColumn("dbo.StockDetails", "StockPortfolio_StockPortfolioID");
            DropColumn("dbo.StockDetails", "Stock_StockID");
            CreateIndex("dbo.Stocks", "StockDetail_StockDetailID");
            CreateIndex("dbo.StockPortfolios", "StockDetail_StockDetailID");
            AddForeignKey("dbo.Stocks", "StockDetail_StockDetailID", "dbo.StockDetails", "StockDetailID");
            AddForeignKey("dbo.StockPortfolios", "StockDetail_StockDetailID", "dbo.StockDetails", "StockDetailID");
        }
    }
}
