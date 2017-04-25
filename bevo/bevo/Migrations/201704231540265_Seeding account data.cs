namespace bevo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Seedingaccountdata : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "TransactionNum", c => c.Int(nullable: false));
            AddColumn("dbo.Transactions", "Date", c => c.DateTime(nullable: false));
            AddColumn("dbo.Transactions", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Transactions", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transactions", "Description");
            DropColumn("dbo.Transactions", "Amount");
            DropColumn("dbo.Transactions", "Date");
            DropColumn("dbo.Transactions", "TransactionNum");
        }
    }
}
