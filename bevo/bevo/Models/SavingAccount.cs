using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace bevo.Models
{
    public class SavingAccount
    {
        public Int32 AccountID { get; set; }

        [DisplayName(Name = "Account Name")]
        public String AccountName { get; set; }
        public Decimal Balance { get; set; }

        //SavingAccount can have many Persons
        public virtual List<Person> Persons { get; set; }
    }
}