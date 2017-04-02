namespace bevo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSeedingAndOneToOne : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CheckingAccounts",
                c => new
                    {
                        CheckingAccountID = c.Int(nullable: false, identity: true),
                        AccountNum = c.Int(nullable: false),
                        AccountName = c.String(),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Person_PersonID = c.Int(),
                    })
                .PrimaryKey(t => t.CheckingAccountID)
                .ForeignKey("dbo.People", t => t.Person_PersonID)
                .Index(t => t.Person_PersonID);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        PersonID = c.Int(nullable: false, identity: true),
                        PersonType = c.Int(nullable: false),
                        Enabled = c.Boolean(nullable: false),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Street = c.String(nullable: false),
                        City = c.String(nullable: false),
                        State = c.String(nullable: false),
                        ZipCode = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        PhoneNumber = c.String(nullable: false),
                        Birthday = c.String(nullable: false),
                        Password = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.PersonID);
            
            CreateTable(
                "dbo.IRAccounts",
                c => new
                    {
                        IRAccountID = c.Int(nullable: false),
                        AccountNum = c.Int(nullable: false),
                        AccountName = c.String(nullable: false),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.IRAccountID)
                .ForeignKey("dbo.People", t => t.IRAccountID)
                .Index(t => t.IRAccountID);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        TransactionID = c.Int(nullable: false, identity: true),
                        FromAccount = c.Int(nullable: false),
                        ToAccount = c.Int(nullable: false),
                        TransType = c.Int(nullable: false),
                        Dispute = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TransactionID);
            
            CreateTable(
                "dbo.SavingAccounts",
                c => new
                    {
                        SavingAccountID = c.Int(nullable: false, identity: true),
                        AccountNum = c.Int(nullable: false),
                        AccountName = c.String(),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Person_PersonID = c.Int(),
                    })
                .PrimaryKey(t => t.SavingAccountID)
                .ForeignKey("dbo.People", t => t.Person_PersonID)
                .Index(t => t.Person_PersonID);
            
            CreateTable(
                "dbo.StockPortfolios",
                c => new
                    {
                        StockPortfolioID = c.Int(nullable: false),
                        AccountNum = c.Int(nullable: false),
                        AccountName = c.String(nullable: false),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StockDetail_StockDetailID = c.Int(),
                    })
                .PrimaryKey(t => t.StockPortfolioID)
                .ForeignKey("dbo.People", t => t.StockPortfolioID)
                .ForeignKey("dbo.StockDetails", t => t.StockDetail_StockDetailID)
                .Index(t => t.StockPortfolioID)
                .Index(t => t.StockDetail_StockDetailID);
            
            CreateTable(
                "dbo.StockDetails",
                c => new
                    {
                        StockDetailID = c.Int(nullable: false, identity: true),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StockDetailID);
            
            CreateTable(
                "dbo.Stocks",
                c => new
                    {
                        StockID = c.Int(nullable: false, identity: true),
                        StockName = c.String(),
                        PurchasePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StockDetail_StockDetailID = c.Int(),
                    })
                .PrimaryKey(t => t.StockID)
                .ForeignKey("dbo.StockDetails", t => t.StockDetail_StockDetailID)
                .Index(t => t.StockDetail_StockDetailID);
            
            CreateTable(
                "dbo.Payees",
                c => new
                    {
                        PayeeID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Street = c.String(nullable: false),
                        City = c.String(nullable: false),
                        State = c.String(nullable: false),
                        Zipcode = c.String(nullable: false),
                        PhoneNumber = c.String(nullable: false),
                        PayeeType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PayeeID);
            
            CreateTable(
                "dbo.Disputes",
                c => new
                    {
                        DisputeID = c.Int(nullable: false, identity: true),
                        DisputeStatus = c.Int(nullable: false),
                        Message = c.String(),
                        DisputedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.DisputeID);
            
            CreateTable(
                "dbo.TransactionCheckingAccounts",
                c => new
                    {
                        Transaction_TransactionID = c.Int(nullable: false),
                        CheckingAccount_CheckingAccountID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Transaction_TransactionID, t.CheckingAccount_CheckingAccountID })
                .ForeignKey("dbo.Transactions", t => t.Transaction_TransactionID, cascadeDelete: true)
                .ForeignKey("dbo.CheckingAccounts", t => t.CheckingAccount_CheckingAccountID, cascadeDelete: true)
                .Index(t => t.Transaction_TransactionID)
                .Index(t => t.CheckingAccount_CheckingAccountID);
            
            CreateTable(
                "dbo.TransactionIRAccounts",
                c => new
                    {
                        Transaction_TransactionID = c.Int(nullable: false),
                        IRAccount_IRAccountID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Transaction_TransactionID, t.IRAccount_IRAccountID })
                .ForeignKey("dbo.Transactions", t => t.Transaction_TransactionID, cascadeDelete: true)
                .ForeignKey("dbo.IRAccounts", t => t.IRAccount_IRAccountID, cascadeDelete: true)
                .Index(t => t.Transaction_TransactionID)
                .Index(t => t.IRAccount_IRAccountID);
            
            CreateTable(
                "dbo.SavingAccountTransactions",
                c => new
                    {
                        SavingAccount_SavingAccountID = c.Int(nullable: false),
                        Transaction_TransactionID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SavingAccount_SavingAccountID, t.Transaction_TransactionID })
                .ForeignKey("dbo.SavingAccounts", t => t.SavingAccount_SavingAccountID, cascadeDelete: true)
                .ForeignKey("dbo.Transactions", t => t.Transaction_TransactionID, cascadeDelete: true)
                .Index(t => t.SavingAccount_SavingAccountID)
                .Index(t => t.Transaction_TransactionID);
            
            CreateTable(
                "dbo.StockPortfolioTransactions",
                c => new
                    {
                        StockPortfolio_StockPortfolioID = c.Int(nullable: false),
                        Transaction_TransactionID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.StockPortfolio_StockPortfolioID, t.Transaction_TransactionID })
                .ForeignKey("dbo.StockPortfolios", t => t.StockPortfolio_StockPortfolioID, cascadeDelete: true)
                .ForeignKey("dbo.Transactions", t => t.Transaction_TransactionID, cascadeDelete: true)
                .Index(t => t.StockPortfolio_StockPortfolioID)
                .Index(t => t.Transaction_TransactionID);
            
            CreateTable(
                "dbo.PayeePersons",
                c => new
                    {
                        Payee_PayeeID = c.Int(nullable: false),
                        Person_PersonID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Payee_PayeeID, t.Person_PersonID })
                .ForeignKey("dbo.Payees", t => t.Payee_PayeeID, cascadeDelete: true)
                .ForeignKey("dbo.People", t => t.Person_PersonID, cascadeDelete: true)
                .Index(t => t.Payee_PayeeID)
                .Index(t => t.Person_PersonID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PayeePersons", "Person_PersonID", "dbo.People");
            DropForeignKey("dbo.PayeePersons", "Payee_PayeeID", "dbo.Payees");
            DropForeignKey("dbo.StockPortfolioTransactions", "Transaction_TransactionID", "dbo.Transactions");
            DropForeignKey("dbo.StockPortfolioTransactions", "StockPortfolio_StockPortfolioID", "dbo.StockPortfolios");
            DropForeignKey("dbo.Stocks", "StockDetail_StockDetailID", "dbo.StockDetails");
            DropForeignKey("dbo.StockPortfolios", "StockDetail_StockDetailID", "dbo.StockDetails");
            DropForeignKey("dbo.StockPortfolios", "StockPortfolioID", "dbo.People");
            DropForeignKey("dbo.SavingAccountTransactions", "Transaction_TransactionID", "dbo.Transactions");
            DropForeignKey("dbo.SavingAccountTransactions", "SavingAccount_SavingAccountID", "dbo.SavingAccounts");
            DropForeignKey("dbo.SavingAccounts", "Person_PersonID", "dbo.People");
            DropForeignKey("dbo.TransactionIRAccounts", "IRAccount_IRAccountID", "dbo.IRAccounts");
            DropForeignKey("dbo.TransactionIRAccounts", "Transaction_TransactionID", "dbo.Transactions");
            DropForeignKey("dbo.TransactionCheckingAccounts", "CheckingAccount_CheckingAccountID", "dbo.CheckingAccounts");
            DropForeignKey("dbo.TransactionCheckingAccounts", "Transaction_TransactionID", "dbo.Transactions");
            DropForeignKey("dbo.IRAccounts", "IRAccountID", "dbo.People");
            DropForeignKey("dbo.CheckingAccounts", "Person_PersonID", "dbo.People");
            DropIndex("dbo.PayeePersons", new[] { "Person_PersonID" });
            DropIndex("dbo.PayeePersons", new[] { "Payee_PayeeID" });
            DropIndex("dbo.StockPortfolioTransactions", new[] { "Transaction_TransactionID" });
            DropIndex("dbo.StockPortfolioTransactions", new[] { "StockPortfolio_StockPortfolioID" });
            DropIndex("dbo.SavingAccountTransactions", new[] { "Transaction_TransactionID" });
            DropIndex("dbo.SavingAccountTransactions", new[] { "SavingAccount_SavingAccountID" });
            DropIndex("dbo.TransactionIRAccounts", new[] { "IRAccount_IRAccountID" });
            DropIndex("dbo.TransactionIRAccounts", new[] { "Transaction_TransactionID" });
            DropIndex("dbo.TransactionCheckingAccounts", new[] { "CheckingAccount_CheckingAccountID" });
            DropIndex("dbo.TransactionCheckingAccounts", new[] { "Transaction_TransactionID" });
            DropIndex("dbo.Stocks", new[] { "StockDetail_StockDetailID" });
            DropIndex("dbo.StockPortfolios", new[] { "StockDetail_StockDetailID" });
            DropIndex("dbo.StockPortfolios", new[] { "StockPortfolioID" });
            DropIndex("dbo.SavingAccounts", new[] { "Person_PersonID" });
            DropIndex("dbo.IRAccounts", new[] { "IRAccountID" });
            DropIndex("dbo.CheckingAccounts", new[] { "Person_PersonID" });
            DropTable("dbo.PayeePersons");
            DropTable("dbo.StockPortfolioTransactions");
            DropTable("dbo.SavingAccountTransactions");
            DropTable("dbo.TransactionIRAccounts");
            DropTable("dbo.TransactionCheckingAccounts");
            DropTable("dbo.Disputes");
            DropTable("dbo.Payees");
            DropTable("dbo.Stocks");
            DropTable("dbo.StockDetails");
            DropTable("dbo.StockPortfolios");
            DropTable("dbo.SavingAccounts");
            DropTable("dbo.Transactions");
            DropTable("dbo.IRAccounts");
            DropTable("dbo.People");
            DropTable("dbo.CheckingAccounts");
        }
    }
}
