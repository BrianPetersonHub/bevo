namespace bevo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedactivationvariablestoaccountmodels : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CheckingAccounts", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.IRAccounts", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.SavingAccounts", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.StockPortfolios", "Active", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StockPortfolios", "Active");
            DropColumn("dbo.SavingAccounts", "Active");
            DropColumn("dbo.IRAccounts", "Active");
            DropColumn("dbo.CheckingAccounts", "Active");
        }
    }
}
