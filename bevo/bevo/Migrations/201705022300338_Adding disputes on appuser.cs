namespace bevo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addingdisputesonappuser : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.TransactionIRAccounts", newName: "IRAccountTransactions");
            DropPrimaryKey("dbo.IRAccountTransactions");
            AddPrimaryKey("dbo.IRAccountTransactions", new[] { "IRAccount_IRAccountID", "Transaction_TransactionID" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.IRAccountTransactions");
            AddPrimaryKey("dbo.IRAccountTransactions", new[] { "Transaction_TransactionID", "IRAccount_IRAccountID" });
            RenameTable(name: "dbo.IRAccountTransactions", newName: "TransactionIRAccounts");
        }
    }
}
