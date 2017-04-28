﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace bevo.Models
{
    public enum TransType
    {
        Deposit,
        Withdrawal,
        Transfer,
        Purchase_Stock,
        Sell_Stock,
        Pay_Payee,
        Fee
    }
    public class Transaction
    {
        public Int32 TransactionID { get; set; }
        public Int32 TransactionNum { get; set; }

        public Transaction()
        {
            TransactionNum = GetTransactionNum();
        }

        [Display(Name = "Date (MM/DD/YYYY)")]
        public DateTime Date { get; set; }


        //NOTE: I made these nullable for the purposes of buying and selling stocks when you'll have
        //either a to account or a from account but not both. Also, we will need the from account to be 
        //null for putting in deposits since they come in from the outside world and therefore
        //don't originate from another account. 
        //This means that on the Transfer, Deposit, and Withdraw controllers I changed the data types to
        //nullable ints. This shouldn't affect functionality since the logic there shouldn't be
        //asking for an account that doesn't exist, so our business logic ought to stop us from running
        //in to any sort of run time error here. 
        [Display(Name = "From Account")]
        public Int32? FromAccount { get; set; }

        [Display(Name = "To Account")]
        public Int32? ToAccount { get; set; }

        public TransType TransType { get; set; }

        public Decimal Amount { get; set; }
        
        public String Description { get; set; }


        // Navigational
        public virtual List<CheckingAccount> CheckingAccounts { get; set; }
        public virtual List<SavingAccount> SavingAccounts { get; set; }
        public virtual List<IRAccount> IRAccounts { get; set; }
        public virtual List<StockPortfolio> StockPortfolios { get; set; }
        public virtual Dispute Dispute { get; set; }
        public virtual Stock Stock { get; set; }



        public Int32 GetTransactionNum()
        {
            AppDbContext db = new AppDbContext();
            Int32 intCount = 1000000000;
            intCount = intCount + db.Transactions.Count();
            return intCount;
        }
    }
}