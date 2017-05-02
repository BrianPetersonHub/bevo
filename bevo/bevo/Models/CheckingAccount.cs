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
        [DisplayFormat]
        public Int32 AccountNum { get; set; }
        [Display(Name = "Account Name")]
        public String AccountName { get; set; }
        public Decimal Balance { get; set; }
        public Boolean? Disabled { get; set; }

        public CheckingAccount()
        {
            AccountNum = GetAcctNum();
            AccountName = "Longhorn Checking";
            Balance = 0;
            Disabled = false;
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
