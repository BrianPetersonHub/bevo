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

        //TODO: Make this unique to each stock somehow.
        //Maybe make it so that, when managers are adding new stocks 
        //it checks the db if this is an existant ticker and only adds
        //if it isn't already there
        public String Ticker { get; set; }

        public Double CurrentPrice { get; set; }

        public Int32 NumInAccount { get; set; }
    }
}