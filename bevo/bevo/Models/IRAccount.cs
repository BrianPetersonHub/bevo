using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace bevo.Models
{
    public class IRAccount
    {
        public Int32 IRAccountID { get; set; }
        public Int32 AccountNum { get; set; }

        [Required(ErrorMessage = "Account Name is required.")]
        [Display(Name = "Account Name")]
        public String AccountName { get; set; }
        public Decimal Balance { get; set; }

        //IRAccount can have many Persons
        public virtual List<Person> Persons { get; set; }
        public virtual List<Transaction> Transactions { get; set; }
    }
}