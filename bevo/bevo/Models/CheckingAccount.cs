using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace bevo.Models
{
    public class CheckingAccount
    {
        public Int32 CheckingAccountID { get; set; }
        public Int32 AccountNum { get; set; }

        [Display(Name = "Account Name")]
        public String AccountName { get; set; }
        public Decimal Balance { get; set; }

        //CheckingAccount can have many Persons
        public virtual Person Person { get; set; }
        public virtual List<Transaction> Transactions { get; set; }
    }
}