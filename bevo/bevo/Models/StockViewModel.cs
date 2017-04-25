using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace bevo.Models
{
    public class StockViewModel
    {
        public String Name { get; set; }

        public String Ticker { get; set; }

        public Double CurrentPrice { get; set; }

        public Int32 NumInAccount { get; set; }
    }
}