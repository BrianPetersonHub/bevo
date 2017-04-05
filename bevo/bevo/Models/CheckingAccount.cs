using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using bevo.Controllers;
using bevo.DAL;
namespace bevo.Models
{
    public class CheckingAccount
    {
        public Int32 CheckingAccountID { get; set; }
        public Int32 AccountNum { get; set; }
        public CheckingAccount(Int32 AcctNum)
        {
            AccountNum = AcctNum;
        }

        [Display(Name = "Account Name")]
        public String AccountName { get; set; }
        public CheckingAccount()
        {
            AccountName = "Longhorn Checking";
        }
        public CheckingAccount(String strAccountName)
        {
            AccountName = strAccountName;
        }

        public Decimal Balance { get; set; }
        public CheckingAccount(Decimal decBBalance)
        {
            Balance = decBBalance;
        }

        //CheckingAccount can have many Persons
        public virtual Person Person { get; set; }
        public virtual List<Transaction> Transactions { get; set; }

    }
}
