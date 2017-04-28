namespace bevo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Dispute2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Disputes", new[] { "DisputeID" });
            DropPrimaryKey("dbo.Disputes");
            AlterColumn("dbo.Disputes", "DisputeID", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Disputes", "DisputeID");
            CreateIndex("dbo.Disputes", "DisputeID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Disputes", new[] { "DisputeID" });
            DropPrimaryKey("dbo.Disputes");
            AlterColumn("dbo.Disputes", "DisputeID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Disputes", "DisputeID");
            CreateIndex("dbo.Disputes", "DisputeID");
        }
    }
}
