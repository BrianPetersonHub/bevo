using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace bevo.Models
{
    public class StockDetail
    {
        public Int32 StockDetailID { get; set; }
        public Int32 Quantity { get; set; }

        //Stock portfolio can have many Persons
        public virtual StockPortfolio StockPortfolio { get; set; }
        public virtual Stock Stock { get; set; }
    }
}