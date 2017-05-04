using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;


//TODO: update database for AppUser connections
namespace bevo.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class AppUser : IdentityUser
    {
     
        //DONE: Put any additional fields that you need for your user here
        //For instance
        public bool Enabled { get; set; }

        [Required(ErrorMessage = "First Name is a Required field.")]
        [Display(Order = 1, Name = "First Name")]
        public String FirstName { get; set; }

        [Display(Order = 2, Name = "Middle Initial")]
        public String MiddleInitial { get; set; }

        [Required(ErrorMessage = "Last Name is a Required field.")]
        [Display(Order = 3, Name = "Last Name")]
        public String LastName { get; set; }

        [Required(ErrorMessage = "Street is a Required field.")]
        [RegularExpression("^((?!^Address$)[0-9A-Za-z #.,])+$")]
        [Display(Order = 4, Name = "Street Address")]
        public String Street { get; set; }

        [Required(ErrorMessage = "City is a Required field.")]
        [RegularExpression("^((?!^City$)[a-zA-Z '])+$")]
        [Display(Order = 5, Name = "City")]
        public String City { get; set; }

        [Required(ErrorMessage = "State is a Required field.")]
        [DataType(DataType.Text)]
        [Display(Order = 6, Name = "State")]
        public String State { get; set; }

        [Required(ErrorMessage = "Zipcode is a Required field.")]
        [RegularExpression("\\d{5}", ErrorMessage = "Zipcode must be five digits long")]
        [Display(Order = 7, Name = "Zipcode")]
        public String ZipCode { get; set; }

        [Required(ErrorMessage = "Date of Birth is a Required field.")]
        [Display(Order = 10, Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public String Birthday { get; set; }

        //Variable to determine whether the user's account is disabled or not
        public Boolean? Disabled { get; set; }


        // Navigational 
        public virtual List<CheckingAccount> CheckingAccounts { get; set; }
        public virtual List<SavingAccount> SavingAccounts { get; set; }
        public virtual IRAccount IRAccount { get; set; }
        public virtual StockPortfolio StockPortfolio { get; set; }
        public virtual List<Payee> Payees { get; set; }
        public virtual List<Dispute> Disputes { get; set; }


        //This method allows you to create a new user
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AppUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    //DONE: Here's your db context for the project.  All of your db sets should go in here
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Transaction>().HasOptional(f => f.Dispute).WithRequired(x => x.Transaction);
        }

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

        //public System.Data.Entity.DbSet<bevo.Models.AppUser> AppUsers { get; set; }


        //public System.Data.Entity.DbSet<bevo.Models.AppUser> AppUsers { get; set; }
        // public DbSet <AppUser> AppUsers {get;set;}
    }
}