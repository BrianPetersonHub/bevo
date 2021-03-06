﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace bevo.Models
{
    public class SavingAccount
    {
        public Int32 SavingAccountID { get; set; }
        public Int32 AccountNum { get; set; }

        [Display(Name = "Account Name")]
        public String AccountName { get; set; }
        public Decimal Balance { get; set; }
        public Boolean? Disabled { get; set; }

        public SavingAccount()
        {
            AccountNum = GetAcctNum();
            AccountName = "Longhorn Saving";
            Balance = 0;
            Disabled = false;
        }

        //SavingAccount can have many Persons
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