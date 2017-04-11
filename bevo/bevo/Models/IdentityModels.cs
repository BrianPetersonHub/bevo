using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace bevo.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class AppUser : IdentityUser
    {
     
        //TODO: Put any additional fields that you need for your user here
        //For instance
        public string FName { get; set; }
        
        
        //This method allows you to create a new user
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AppUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    //TODO: Here's your db context for the project.  All of your db sets should go in here
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        //DONE:  Add dbsets here, for instance there's one for books
        public DbSet<CheckingAccount> CheckingAccounts { get; set; }
        public DbSet<Dispute> Disputes { get; set; }
        public DbSet<IRAccount> IRAccounts { get; set; }
        public DbSet<Payee> Payees { get; set; }
        public DbSet<SavingAccount> SavingAccounts { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<StockDetail> StockDetails { get; set; }
        public DbSet<StockPortfolio> StockPortfolios { get; set; }
        public DbSet<Transaction> Transactions { get; set; }


        //DONE: Make sure that your connection string name is correct here.
        public AppDbContext()
            : base("DBConnection", throwIfV1Schema: false)
        {
        }

        public static AppDbContext Create()
        {
            return new AppDbContext();
        }

        public DbSet<AppRole> AppRoles { get; set; }
    }
}