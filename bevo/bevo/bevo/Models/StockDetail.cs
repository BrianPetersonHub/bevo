using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace team1_longhornBank.Models
{
    public class StockDetail
    {
        [Key]
        public Int32 AccountID { get; set; }

        [Key]
        public String StockName { get; set; }

        public Int32 Quantity { get; set; }
    }
}