namespace bevo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updatingenabledvsdisabledvariables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CheckingAccounts", "Disabled", c => c.Boolean());
            AddColumn("dbo.AspNetUsers", "Disabled", c => c.Boolean());
            AddColumn("dbo.IRAccounts", "Disabled", c => c.Boolean());
            AddColumn("dbo.SavingAccounts", "Disabled", c => c.Boolean());
            AddColumn("dbo.StockPortfolios", "Disabled", c => c.Boolean());
            DropColumn("dbo.CheckingAccounts", "Active");
            DropColumn("dbo.AspNetUsers", "Active");
            DropColumn("dbo.IRAccounts", "Active");
            DropColumn("dbo.SavingAccounts", "Active");
            DropColumn("dbo.StockPortfolios", "Active");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StockPortfolios", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.SavingAccounts", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.IRAccounts", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "Active", c => c.Boolean());
            AddColumn("dbo.CheckingAccounts", "Active", c => c.Boolean(nullable: false));
            DropColumn("dbo.StockPortfolios", "Disabled");
            DropColumn("dbo.SavingAccounts", "Disabled");
            DropColumn("dbo.IRAccounts", "Disabled");
            DropColumn("dbo.AspNetUsers", "Disabled");
            DropColumn("dbo.CheckingAccounts", "Disabled");
        }
    }
}
