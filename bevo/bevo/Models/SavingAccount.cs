using System;
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

        //SavingAccount can have many Persons
        public virtual Person Person { get; set; }
        public virtual List<Transaction> Transactions { get; set; }
    }
}