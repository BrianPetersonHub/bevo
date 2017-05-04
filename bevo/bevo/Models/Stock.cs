using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace bevo.Models
{
    public enum StockType { Ordinary, Index_Fund, ETF, Mutual_Fund, Future_Share };

    public class Stock
    {
        public Int32 StockID { get; set; }

        [Required(ErrorMessage = "Stock name is a Required field.")]
        public String StockName { get; set; }

        [Required(ErrorMessage = "Ticker symbol is a Required field.")]
        public String StockTicker { get; set; }

        [Required(ErrorMessage = "Type of stock is a Required field.")]
        public StockType TypeOfStock { get; set; }

        [Display(Name = "Fee Amount")]
        public Int32? feeAmount { get; set; }


        //Stock can have one stock detail
        public virtual List<StockDetail> StockDetails { get; set; }
        public virtual List<Transaction> Transactions { get; set; }
    }
}