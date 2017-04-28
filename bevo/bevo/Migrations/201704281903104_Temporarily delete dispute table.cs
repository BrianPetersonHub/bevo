namespace bevo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Temporarilydeletedisputetable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Disputes", "DisputeId", "dbo.Transactions");
            DropIndex("dbo.Disputes", new[] { "DisputeId" });
            DropTable("dbo.Disputes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Disputes",
                c => new
                    {
                        DisputeId = c.Int(nullable: false),
                        DisputeStatus = c.Int(nullable: false),
                        Message = c.String(),
                        DisputedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.DisputeId);
            
            CreateIndex("dbo.Disputes", "DisputeId");
            AddForeignKey("dbo.Disputes", "DisputeId", "dbo.Transactions", "TransactionID");
        }
    }
}
