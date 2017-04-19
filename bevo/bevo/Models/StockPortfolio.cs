using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bevo.Models
{
    public class StockPortfolio
    {
        //This data annotation is needed so EF knows which end of the 1:1 relationship is 
        //the parent side
        [Key, ForeignKey("AppUser")]
        public String StockPortfolioID { get; set; }
        public Int32 AccountNum { get; set; }

        [Required(ErrorMessage = "Account Name is required.")]
        [Display(Name = "Account Name")]
        public String AccountName { get; set; }
        public Decimal Balance { get; set; }

        //Stock portfolio can have many Persons
        public virtual AppUser AppUser { get; set; }

        //Stock portfolio can have one StockDetail
        public virtual StockDetail StockDetail { get; set; }
        public virtual List<Transaction> Transactions { get; set; }
    }
}