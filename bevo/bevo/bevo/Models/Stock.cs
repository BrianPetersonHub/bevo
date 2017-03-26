using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace team1_longhornBank.Models
{
    public class Stock
    {
        [Key]
        public String StockName { get; set; }

        public Decimal PurchasePrice { get; set; }

    }
}