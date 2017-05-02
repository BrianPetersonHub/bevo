namespace bevo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Continuingidentitysetup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "NeedsApproval", c => c.Boolean());
            AddColumn("dbo.Disputes", "AppUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Disputes", "AppUser_Id");
            AddForeignKey("dbo.Disputes", "AppUser_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Disputes", "AppUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Disputes", new[] { "AppUser_Id" });
            DropColumn("dbo.Disputes", "AppUser_Id");
            DropColumn("dbo.Transactions", "NeedsApproval");
        }
    }
}
