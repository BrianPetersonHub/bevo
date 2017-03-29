using bevo.Models;
using System.Data.Entity;

namespace bevo.DAL
{
    public class AppDbContext : DbContext
    {
        //Constructor that invokes the base constructor
        public AppDbContext() : base("DBConnection") { }

        //Create the db set
        public DbSet<CheckingAccount> CheckingAccounts { get; set; }
        public DbSet<Dispute> Disputes { get; set; }
        public DbSet<IRAccount> IRAccounts { get; set; }
        public DbSet<Payee> Payees { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<SavingAccount> SavingAccounts { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<StockDetail> StockDetail { get; set; }
        public DbSet<StockPortfolio> StockPortfolio { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}