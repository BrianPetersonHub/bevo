using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using bevo.Controllers;
namespace bevo.Models
{
    public class CheckingAccount
    {
        public Int32 CheckingAccountID { get; set; }
        public Int32 AccountNum { get; set; }
        //TODO: think about what problems could arise when making a accountToChange object in constructor. Katie mentioned this in class on Tuesday 4/4/17
        [Display(Name = "Account Name")]
        public String AccountName { get; set; }
        public Decimal Balance { get; set; }
        public CheckingAccount()
        {
            AccountNum = GetAcctNum();
            //TODO: get this to give name when name not specified
            AccountName = "Longhorn Checking";
            Balance = 0;
        }

        //CheckingAccount can have many Persons
        public virtual AppUser AppUser { get; set; }
        public virtual List<Transaction> Transactions { get; set; }


        //method to get next account number
        private AppDbContext db = new AppDbContext();
        public Int32 GetAcctNum()
        {
            Int32 intCount = 1000000000;
            intCount += db.CheckingAccounts.Count();
            intCount += db.SavingAccounts.Count();
            intCount += db.IRAccounts.Count();
            intCount += db.StockPortfolios.Count();
            return intCount;
        }
    }
}
