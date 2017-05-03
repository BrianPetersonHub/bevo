namespace bevo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixedDisputeID : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Disputes", new[] { "DisputeId" });
            CreateIndex("dbo.Disputes", "DisputeID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Disputes", new[] { "DisputeID" });
            CreateIndex("dbo.Disputes", "DisputeId");
        }
    }
}
