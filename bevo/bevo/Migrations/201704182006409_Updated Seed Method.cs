namespace bevo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedSeedMethod : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "MiddleInitial", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "MiddleInitial");
        }
    }
}
