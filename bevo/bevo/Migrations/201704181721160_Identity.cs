namespace bevo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Identity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CheckingAccounts", "Person_PersonID", "dbo.People");
            DropForeignKey("dbo.IRAccounts", "IRAccountID", "dbo.People");
            DropForeignKey("dbo.SavingAccounts", "Person_PersonID", "dbo.People");
            DropForeignKey("dbo.StockPortfolios", "StockPortfolioID", "dbo.People");
            DropForeignKey("dbo.PayeePersons", "Payee_PayeeID", "dbo.Payees");
            DropForeignKey("dbo.PayeePersons", "Person_PersonID", "dbo.People");
            DropForeignKey("dbo.TransactionIRAccounts", "IRAccount_IRAccountID", "dbo.IRAccounts");
            DropForeignKey("dbo.StockPortfolioTransactions", "StockPortfolio_StockPortfolioID", "dbo.StockPortfolios");
            DropIndex("dbo.CheckingAccounts", new[] { "Person_PersonID" });
            DropIndex("dbo.IRAccounts", new[] { "IRAccountID" });
            DropIndex("dbo.SavingAccounts", new[] { "Person_PersonID" });
            DropIndex("dbo.StockPortfolios", new[] { "StockPortfolioID" });
            DropIndex("dbo.TransactionIRAccounts", new[] { "IRAccount_IRAccountID" });
            DropIndex("dbo.StockPortfolioTransactions", new[] { "StockPortfolio_StockPortfolioID" });
            DropIndex("dbo.PayeePersons", new[] { "Payee_PayeeID" });
            DropIndex("dbo.PayeePersons", new[] { "Person_PersonID" });
            DropPrimaryKey("dbo.IRAccounts");
            DropPrimaryKey("dbo.StockPortfolios");
            DropPrimaryKey("dbo.TransactionIRAccounts");
            DropPrimaryKey("dbo.StockPortfolioTransactions");
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
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Enabled = c.Boolean(nullable: false),
                        FirstName = c.String(nullable: false),
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
            
            AddColumn("dbo.CheckingAccounts", "AppUser_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.SavingAccounts", "AppUser_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.IRAccounts", "IRAccountID", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.StockPortfolios", "StockPortfolioID", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.TransactionIRAccounts", "IRAccount_IRAccountID", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.StockPortfolioTransactions", "StockPortfolio_StockPortfolioID", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.IRAccounts", "IRAccountID");
            AddPrimaryKey("dbo.StockPortfolios", "StockPortfolioID");
            AddPrimaryKey("dbo.TransactionIRAccounts", new[] { "Transaction_TransactionID", "IRAccount_IRAccountID" });
            AddPrimaryKey("dbo.StockPortfolioTransactions", new[] { "StockPortfolio_StockPortfolioID", "Transaction_TransactionID" });
            CreateIndex("dbo.CheckingAccounts", "AppUser_Id");
            CreateIndex("dbo.IRAccounts", "IRAccountID");
            CreateIndex("dbo.SavingAccounts", "AppUser_Id");
            CreateIndex("dbo.StockPortfolios", "StockPortfolioID");
            CreateIndex("dbo.TransactionIRAccounts", "IRAccount_IRAccountID");
            CreateIndex("dbo.StockPortfolioTransactions", "StockPortfolio_StockPortfolioID");
            AddForeignKey("dbo.CheckingAccounts", "AppUser_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.IRAccounts", "IRAccountID", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.SavingAccounts", "AppUser_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.StockPortfolios", "StockPortfolioID", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.TransactionIRAccounts", "IRAccount_IRAccountID", "dbo.IRAccounts", "IRAccountID", cascadeDelete: true);
            AddForeignKey("dbo.StockPortfolioTransactions", "StockPortfolio_StockPortfolioID", "dbo.StockPortfolios", "StockPortfolioID", cascadeDelete: true);
            DropColumn("dbo.CheckingAccounts", "Person_PersonID");
            DropColumn("dbo.SavingAccounts", "Person_PersonID");
            DropTable("dbo.People");
            DropTable("dbo.PayeePersons");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PayeePersons",
                c => new
                    {
                        Payee_PayeeID = c.Int(nullable: false),
                        Person_PersonID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Payee_PayeeID, t.Person_PersonID });
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        PersonID = c.Int(nullable: false, identity: true),
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
            
            AddColumn("dbo.SavingAccounts", "Person_PersonID", c => c.Int());
            AddColumn("dbo.CheckingAccounts", "Person_PersonID", c => c.Int());
            DropForeignKey("dbo.StockPortfolioTransactions", "StockPortfolio_StockPortfolioID", "dbo.StockPortfolios");
            DropForeignKey("dbo.TransactionIRAccounts", "IRAccount_IRAccountID", "dbo.IRAccounts");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PayeeAppUsers", "AppUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.PayeeAppUsers", "Payee_PayeeID", "dbo.Payees");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.StockPortfolios", "StockPortfolioID", "dbo.AspNetUsers");
            DropForeignKey("dbo.SavingAccounts", "AppUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.IRAccounts", "IRAccountID", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CheckingAccounts", "AppUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.PayeeAppUsers", new[] { "AppUser_Id" });
            DropIndex("dbo.PayeeAppUsers", new[] { "Payee_PayeeID" });
            DropIndex("dbo.StockPortfolioTransactions", new[] { "StockPortfolio_StockPortfolioID" });
            DropIndex("dbo.TransactionIRAccounts", new[] { "IRAccount_IRAccountID" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.StockPortfolios", new[] { "StockPortfolioID" });
            DropIndex("dbo.SavingAccounts", new[] { "AppUser_Id" });
            DropIndex("dbo.IRAccounts", new[] { "IRAccountID" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.CheckingAccounts", new[] { "AppUser_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropPrimaryKey("dbo.StockPortfolioTransactions");
            DropPrimaryKey("dbo.TransactionIRAccounts");
            DropPrimaryKey("dbo.StockPortfolios");
            DropPrimaryKey("dbo.IRAccounts");
            AlterColumn("dbo.StockPortfolioTransactions", "StockPortfolio_StockPortfolioID", c => c.Int(nullable: false));
            AlterColumn("dbo.TransactionIRAccounts", "IRAccount_IRAccountID", c => c.Int(nullable: false));
            AlterColumn("dbo.StockPortfolios", "StockPortfolioID", c => c.Int(nullable: false));
            AlterColumn("dbo.IRAccounts", "IRAccountID", c => c.Int(nullable: false));
            DropColumn("dbo.SavingAccounts", "AppUser_Id");
            DropColumn("dbo.CheckingAccounts", "AppUser_Id");
            DropTable("dbo.PayeeAppUsers");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            AddPrimaryKey("dbo.StockPortfolioTransactions", new[] { "StockPortfolio_StockPortfolioID", "Transaction_TransactionID" });
            AddPrimaryKey("dbo.TransactionIRAccounts", new[] { "Transaction_TransactionID", "IRAccount_IRAccountID" });
            AddPrimaryKey("dbo.StockPortfolios", "StockPortfolioID");
            AddPrimaryKey("dbo.IRAccounts", "IRAccountID");
            CreateIndex("dbo.PayeePersons", "Person_PersonID");
            CreateIndex("dbo.PayeePersons", "Payee_PayeeID");
            CreateIndex("dbo.StockPortfolioTransactions", "StockPortfolio_StockPortfolioID");
            CreateIndex("dbo.TransactionIRAccounts", "IRAccount_IRAccountID");
            CreateIndex("dbo.StockPortfolios", "StockPortfolioID");
            CreateIndex("dbo.SavingAccounts", "Person_PersonID");
            CreateIndex("dbo.IRAccounts", "IRAccountID");
            CreateIndex("dbo.CheckingAccounts", "Person_PersonID");
            AddForeignKey("dbo.StockPortfolioTransactions", "StockPortfolio_StockPortfolioID", "dbo.StockPortfolios", "StockPortfolioID", cascadeDelete: true);
            AddForeignKey("dbo.TransactionIRAccounts", "IRAccount_IRAccountID", "dbo.IRAccounts", "IRAccountID", cascadeDelete: true);
            AddForeignKey("dbo.PayeePersons", "Person_PersonID", "dbo.People", "PersonID", cascadeDelete: true);
            AddForeignKey("dbo.PayeePersons", "Payee_PayeeID", "dbo.Payees", "PayeeID", cascadeDelete: true);
            AddForeignKey("dbo.StockPortfolios", "StockPortfolioID", "dbo.People", "PersonID");
            AddForeignKey("dbo.SavingAccounts", "Person_PersonID", "dbo.People", "PersonID");
            AddForeignKey("dbo.IRAccounts", "IRAccountID", "dbo.People", "PersonID");
            AddForeignKey("dbo.CheckingAccounts", "Person_PersonID", "dbo.People", "PersonID");
        }
    }
}
