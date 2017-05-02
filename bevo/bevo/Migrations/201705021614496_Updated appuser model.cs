namespace bevo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updatedappusermodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Active", c => c.Boolean());
            AddColumn("dbo.Disputes", "ManResolvedEmail", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Disputes", "ManResolvedEmail");
            DropColumn("dbo.AspNetUsers", "Active");
        }
    }
}
