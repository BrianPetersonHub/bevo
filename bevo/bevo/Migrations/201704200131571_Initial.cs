namespace bevo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.CheckingAccounts",
                c => new
                    {
                        CheckingAccountID = c.Int(nullable: false, identity: true),
                        AccountNum = c.Int(nullable: false),
                        AccountName = c.String(),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AppUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CheckingAccountID)
                .ForeignKey("dbo.AspNetUsers", t => t.AppUser_Id)
                .Index(t => t.AppUser_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Enabled = c.Boolean(nullable: false),
                        FirstName = c.String(nullable: false),
                        MiddleInitial = c.String(),
                        LastName = c.String(nullable: false),
                        Street = c.String(nullable: false),
                        City = c.String(nullable: false),
                        State = c.String(nullable: false),
                        ZipCode = c.String(nullable: false),
                        Birthday = c.String(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.IRAccounts",
                c => new
                    {
                        IRAccountID = c.String(nullable: false, maxLength: 128),
                        AccountNum = c.Int(nullable: false),
                        AccountName = c.String(nullable: false),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.IRAccountID)
                .ForeignKey("dbo.AspNetUsers", t => t.IRAccountID)
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
                        AppUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.SavingAccountID)
                .ForeignKey("dbo.AspNetUsers", t => t.AppUser_Id)
                .Index(t => t.AppUser_Id);
            
            CreateTable(
                "dbo.StockPortfolios",
                c => new
                    {
                        StockPortfolioID = c.String(nullable: false, maxLength: 128),
                        AccountNum = c.Int(nullable: false),
                        AccountName = c.String(nullable: false),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StockDetail_StockDetailID = c.Int(),
                    })
                .PrimaryKey(t => t.StockPortfolioID)
                .ForeignKey("dbo.AspNetUsers", t => t.StockPortfolioID)
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
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
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
                        IRAccount_IRAccountID = c.String(nullable: false, maxLength: 128),
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
                        StockPortfolio_StockPortfolioID = c.String(nullable: false, maxLength: 128),
                        Transaction_TransactionID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.StockPortfolio_StockPortfolioID, t.Transaction_TransactionID })
                .ForeignKey("dbo.StockPortfolios", t => t.StockPortfolio_StockPortfolioID, cascadeDelete: true)
                .ForeignKey("dbo.Transactions", t => t.Transaction_TransactionID, cascadeDelete: true)
                .Index(t => t.StockPortfolio_StockPortfolioID)
                .Index(t => t.Transaction_TransactionID);
            
            CreateTable(
                "dbo.PayeeAppUsers",
                c => new
                    {
                        Payee_PayeeID = c.Int(nullable: false),
                        AppUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Payee_PayeeID, t.AppUser_Id })
                .ForeignKey("dbo.Payees", t => t.Payee_PayeeID, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.AppUser_Id, cascadeDelete: true)
                .Index(t => t.Payee_PayeeID)
                .Index(t => t.AppUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PayeeAppUsers", "AppUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.PayeeAppUsers", "Payee_PayeeID", "dbo.Payees");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.StockPortfolioTransactions", "Transaction_TransactionID", "dbo.Transactions");
            DropForeignKey("dbo.StockPortfolioTransactions", "StockPortfolio_StockPortfolioID", "dbo.StockPortfolios");
            DropForeignKey("dbo.Stocks", "StockDetail_StockDetailID", "dbo.StockDetails");
            DropForeignKey("dbo.StockPortfolios", "StockDetail_StockDetailID", "dbo.StockDetails");
            DropForeignKey("dbo.StockPortfolios", "StockPortfolioID", "dbo.AspNetUsers");
            DropForeignKey("dbo.SavingAccountTransactions", "Transaction_TransactionID", "dbo.Transactions");
            DropForeignKey("dbo.SavingAccountTransactions", "SavingAccount_SavingAccountID", "dbo.SavingAccounts");
            DropForeignKey("dbo.SavingAccounts", "AppUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.TransactionIRAccounts", "IRAccount_IRAccountID", "dbo.IRAccounts");
            DropForeignKey("dbo.TransactionIRAccounts", "Transaction_TransactionID", "dbo.Transactions");
            DropForeignKey("dbo.TransactionCheckingAccounts", "CheckingAccount_CheckingAccountID", "dbo.CheckingAccounts");
            DropForeignKey("dbo.TransactionCheckingAccounts", "Transaction_TransactionID", "dbo.Transactions");
            DropForeignKey("dbo.IRAccounts", "IRAccountID", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CheckingAccounts", "AppUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.PayeeAppUsers", new[] { "AppUser_Id" });
            DropIndex("dbo.PayeeAppUsers", new[] { "Payee_PayeeID" });
            DropIndex("dbo.StockPortfolioTransactions", new[] { "Transaction_TransactionID" });
            DropIndex("dbo.StockPortfolioTransactions", new[] { "StockPortfolio_StockPortfolioID" });
            DropIndex("dbo.SavingAccountTransactions", new[] { "Transaction_TransactionID" });
            DropIndex("dbo.SavingAccountTransactions", new[] { "SavingAccount_SavingAccountID" });
            DropIndex("dbo.TransactionIRAccounts", new[] { "IRAccount_IRAccountID" });
            DropIndex("dbo.TransactionIRAccounts", new[] { "Transaction_TransactionID" });
            DropIndex("dbo.TransactionCheckingAccounts", new[] { "CheckingAccount_CheckingAccountID" });
            DropIndex("dbo.TransactionCheckingAccounts", new[] { "Transaction_TransactionID" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.Stocks", new[] { "StockDetail_StockDetailID" });
            DropIndex("dbo.StockPortfolios", new[] { "StockDetail_StockDetailID" });
            DropIndex("dbo.StockPortfolios", new[] { "StockPortfolioID" });
            DropIndex("dbo.SavingAccounts", new[] { "AppUser_Id" });
            DropIndex("dbo.IRAccounts", new[] { "IRAccountID" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.CheckingAccounts", new[] { "AppUser_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropTable("dbo.PayeeAppUsers");
            DropTable("dbo.StockPortfolioTransactions");
            DropTable("dbo.SavingAccountTransactions");
            DropTable("dbo.TransactionIRAccounts");
            DropTable("dbo.TransactionCheckingAccounts");
            DropTable("dbo.Disputes");
            DropTable("dbo.Payees");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.Stocks");
            DropTable("dbo.StockDetails");
            DropTable("dbo.StockPortfolios");
            DropTable("dbo.SavingAccounts");
            DropTable("dbo.Transactions");
            DropTable("dbo.IRAccounts");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.CheckingAccounts");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
        }
    }
}
