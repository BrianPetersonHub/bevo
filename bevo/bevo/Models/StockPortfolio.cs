using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace bevo.Models
{
    public class StockPortfolio
    {
        public Int32 StockPortfolioID { get; set; }
        public Int32 AccountNum { get; set; }

        [Required(ErrorMessage = "Account Name is required.")]
        [Display(Name = "Account Name")]
        public String AccountName { get; set; }
        public Decimal Balance { get; set; }

        //Stock portfolio can have many Persons
        public virtual List<Person> Persons { get; set; }

        //Stock portfolio can have one StockDetail
        public virtual StockDetail StockDetail { get; set; }
        public virtual List<Transaction> Transactions { get; set; }
    }
}