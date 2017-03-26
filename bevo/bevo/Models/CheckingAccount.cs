using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace bevo.Models
{
    public class CheckingAccount
    {
        public Int32 AccountID { get; set; }

        [DisplayName(Name = "Account Name")]
        public String AccountName { get; set; }
        public Decimal Balance { get; set; }

        //CheckingAccount can have many Persons
        public virtual List<Person> Persons { get; set; }
    }
}