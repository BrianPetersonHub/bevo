namespace bevo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatingtheDBforBarrett : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Transactions", "FromAccount", c => c.Int());
            AlterColumn("dbo.Transactions", "ToAccount", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Transactions", "ToAccount", c => c.Int(nullable: false));
            AlterColumn("dbo.Transactions", "FromAccount", c => c.Int(nullable: false));
        }
    }
}
