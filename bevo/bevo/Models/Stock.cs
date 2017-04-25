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
        public String StockName { get; set; }
        public String StockTicker { get; set; }
        public Decimal PurchasePrice { get; set; }
        public StockType TypeOfStock { get; set; }


        //Stock can have one stock detail
        public virtual List<StockDetail> StockDetails { get; set; }
    }
}