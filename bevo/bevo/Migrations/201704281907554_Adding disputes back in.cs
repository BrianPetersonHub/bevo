namespace bevo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addingdisputesbackin : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Disputes",
                c => new
                    {
                        DisputeID = c.Int(nullable: false),
                        DisputeStatus = c.Int(nullable: false),
                        Message = c.String(),
                        DisputedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.DisputeID)
                .ForeignKey("dbo.Transactions", t => t.DisputeID)
                .Index(t => t.DisputeID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Disputes", "DisputeID", "dbo.Transactions");
            DropIndex("dbo.Disputes", new[] { "DisputeID" });
            DropTable("dbo.Disputes");
        }
    }
}
