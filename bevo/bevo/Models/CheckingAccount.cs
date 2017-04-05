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

        //create AccountNum property
        //1 constructor with AcctNum parameter
        public Int32 AccountNum { get; set; }
        public CheckingAccount(Int32 intAccounttNum)
        {
            AccountNum = intAccounttNum;
        }

        //create AccountName property
        //1st constructor with no parameter: makes AccountName = "Longhorn Checking"
        //TODO: think about what problems could arise when making a accountToChange object in constructor. Katie mentioned this in class on Tuesday 4/4/17
        [Display(Name = "Account Name")]
        public String AccountName { get; set; }
        public CheckingAccount()
        {
            AccountName = "Longhorn Checking";
        }

        public Decimal Balance { get; set; }

        //CheckingAccount can have many Persons
        public virtual Person Person { get; set; }
        public virtual List<Transaction> Transactions { get; set; }

    }
}
