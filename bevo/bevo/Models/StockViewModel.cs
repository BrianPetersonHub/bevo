using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace bevo.Models
{
    public class StockPortfolioViewModel
    {
        public Double CurrentValue { get; set; }
        public bool Balanced { get; set; }
        public Double TotalGains { get; set; }
        public Double TotalFees { get; set; }
        public Double TotalBonuses { get; set; }
        public Double CashAvailable { get; set; }
    }

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

    public class AccountsViewModel
    {
        public String AccountName { get; set; }
        public Double Balance { get; set; }
    }

    public class StockDetailsViewModel
    {
        public String Name { get; set; }
        public String Ticker { get; set; }
        public Double PurchasePrice { get; set; }
        public Double Delta { get; set; }
    }

    public class PurchaseSummaryViewModel
    {
        public DateTime Date { get; set; }
        public String Name { get; set; }
        public String Ticker { get; set; }
        public Int32 NumSold { get; set; }
        public Int32 NumRemaining { get; set; }
        public Double Fees { get; set; }
        public Double NetProfit { get; set; }
    }


}