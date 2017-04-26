namespace bevo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class relationshipbtntransactionanddispute : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Disputes");
            AlterColumn("dbo.Disputes", "DisputeID", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Disputes", "DisputeID");
            CreateIndex("dbo.Disputes", "DisputeID");
            AddForeignKey("dbo.Disputes", "DisputeID", "dbo.Transactions", "TransactionID");
            DropColumn("dbo.Transactions", "Dispute");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transactions", "Dispute", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.Disputes", "DisputeID", "dbo.Transactions");
            DropIndex("dbo.Disputes", new[] { "DisputeID" });
            DropPrimaryKey("dbo.Disputes");
            AlterColumn("dbo.Disputes", "DisputeID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Disputes", "DisputeID");
        }
    }
}
