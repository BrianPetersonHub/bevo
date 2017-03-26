using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace bevo.Models
{
    public class Stock
    {
        public Int32 StockID { get; set; }
        public String StockName { get; set; }
        public Decimal PurchasePrice { get; set; }

        //Stock can have one stock detail
        public virtual StockDetail StockDetail { get; set; }
    }
}