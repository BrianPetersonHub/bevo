using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace bevo.Models
{
    public class StockPortfolio
    {
        public Int32 AccountID { get; set; }

        [Required(ErrorMessage = "Account Name is required.")]
        [DisplayName(Name = "Account Name")]
        public String AccountName { get; set; }
        public Decimal Balance { get; set; }

        //Stock portfolio can have many Persons
        public virtual List<Person> Persons { get; set; }

        //Stock portfolio can have one StockDetail
        public virtual StockDetail StockDetail { get; set; }
    }
}