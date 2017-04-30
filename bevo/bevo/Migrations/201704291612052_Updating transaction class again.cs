namespace bevo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updatingtransactionclassagain : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "NumShares", c => c.Int());
            AddColumn("dbo.Transactions", "SMVRedux", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transactions", "SMVRedux");
            DropColumn("dbo.Transactions", "NumShares");
        }
    }
}
