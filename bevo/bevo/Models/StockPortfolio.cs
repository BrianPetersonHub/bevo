﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using bevo.DAL;

namespace bevo.Models
{
    public class StockPortfolio
    {
        //This data annotation is needed so EF knows which end of the 1:1 relationship is 
        //the parent side
        [Key, ForeignKey("Person")]
        public Int32 StockPortfolioID { get; set; }
        public Int32 AccountNum { get; set; }

        [Required(ErrorMessage = "Account Name is required.")]
        [Display(Name = "Account Name")]
        public String AccountName { get; set; }
        public Decimal Balance { get; set; }

        public StockPortfolio(String AcctName)
        {
            AccountNum = GetAcctNum();
            AccountName = AcctName;
            Balance = 0;
        }

        //Stock portfolio can have many Persons
        public virtual Person Person { get; set; }

        //Stock portfolio can have one StockDetail
        public virtual StockDetail StockDetail { get; set; }
        public virtual List<Transaction> Transactions { get; set; }

        AppDbContext db = new AppDbContext();
        public Int32 GetAcctNum()
        {
            Int32 intCount = 1000000000;
            intCount += db.CheckingAccounts.Count();
            intCount += db.SavingAccounts.Count();
            intCount += db.IRAccounts.Count();
            intCount += db.StockPortfolio.Count();
            return intCount;
        }
    }
}