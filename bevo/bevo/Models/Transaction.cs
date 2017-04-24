using System;
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
        Pay_Payee
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

        public Int32 FromAccount { get; set; }

        public Int32 ToAccount { get; set; }

        public TransType TransType { get; set; }

        public Decimal Amount { get; set; }
        
        public String Description { get; set; }

        public bool Dispute { get; set; }

        // Navigational
        public virtual List<CheckingAccount> CheckingAccounts { get; set; }
        public virtual List<SavingAccount> SavingAccounts { get; set; }
        public virtual List<IRAccount> IRAccounts { get; set; }
        public virtual List<StockPortfolio> StockPortfolios { get; set; }

        public Int32 GetTransactionNum()
        {
            AppDbContext db = new AppDbContext();
            Int32 intCount = 1000000000;
            intCount = intCount + db.Transactions.Count();
            return intCount;
        }
    }
}