﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace bevo.Models
{
    public class StockPortfolioViewModel
    {
        public Decimal CurrentValue { get; set; }
        public bool Balanced { get; set; }
        public Decimal TotalGains { get; set; }
        public Decimal TotalFees { get; set; }
        public Decimal TotalBonuses { get; set; }
        public Decimal CashAvailable { get; set; }
        public Decimal StockMarketValue { get; set; }

    }

    public class StockViewModel
    {
        public String Name { get; set; }

        //TODO: Make this unique to each stock somehow.
        //Maybe make it so that, when managers are adding new stocks 
        //it checks the db if this is an existant ticker and only adds
        //if it isn't already there
        public String Ticker { get; set; }

        public Decimal CurrentPrice { get; set; }

        public Int32 NumInAccount { get; set; }
    }

    public class AccountsViewModel
    {
        public String AccountName { get; set; }
        public Decimal Balance { get; set; }
    }

    public class StockDetailsViewModel
    {
        public String Name { get; set; }
        public String Ticker { get; set; }
        public Decimal PurchasePrice { get; set; }
        public Decimal Delta { get; set; }
    }

    public class PurchaseSummaryViewModel
    {
        public DateTime Date { get; set; }
        public String Name { get; set; }
        public String Ticker { get; set; }
        public Int32 NumSold { get; set; }
        public Int32 NumRemaining { get; set; }
        public Decimal Fees { get; set; }
        public Decimal NetProfit { get; set; }
    }


}