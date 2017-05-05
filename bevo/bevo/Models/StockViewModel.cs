using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace bevo.Models
{
    //Used when evaluating an EXISTING stock portfolio as a whole 
    public class StockPortfolioViewModel
    {
        public Int32? StockID { get; set; }
        public Decimal? CurrentValue { get; set; }
        public bool Balanced { get; set; }
        public Decimal? TotalGains { get; set; }
        public Decimal? TotalFees { get; set; }
        public Decimal? TotalBonuses { get; set; }
        public Decimal? CashAvailable { get; set; }
        public Decimal? StockMarketValue { get; set; }

    }

    //Used when information for each stock within an EXISTING portfolio
    public class StockViewModel
    {
        public String Name { get; set; }

        //TODO: Make this unique to each stock somehow.
        //Maybe make it so that, when managers are adding new stocks 
        //it checks the db if this is an existant ticker and only adds
        //if it isn't already there
        public String Ticker { get; set; }
        public Decimal? PurchasePrice { get; set; }
        public StockType Type { get; set; }

        public Decimal CurrentPrice { get; set; }
        public Int32? StockID { get; set; }

        public Int32 NumInAccount { get; set; }
    }

    //Used when displaying information about a POTENTIALLY PURCHASABLE stock
    public class AvailableStock
    {   
        public Int32? StockID { get; set; }
        public StockType Type { get; set; }
        public String Name { get; set; }
        public String Ticker { get; set; }
        public Decimal CurrentPrice { get; set; }
    }

    //Used when looking at an account independent of what account type it is
    public class AccountsViewModel
    {
        public Int32 AccountNum { get; set; }
        public String AccountName { get; set; }
        public Decimal Balance { get; set; }
    }

    //Used when looking at stock purchase transactions to just store how many shares of the stock
    //were bought and at what price for each individual transaction
    public class TransactionViewModel
    {
        public Int32? NumPurchased { get; set; }
        public Decimal? PurchasePrice { get; set; }

    }

    public class StockSummaryViewModel
    {
        public String Stock { get; set; }
        public Int32 Quantity { get; set; }
        public Int32 Remaining { get; set; }
        public Decimal Fees { get; set; }
        public Decimal Profit { get; set; }
    }















    //I don't know what this is or why it's here. Please don't use this. I think I'm going to delete it
    public class StockDetailsViewModel
    {
        public String Name { get; set; }
        public String Ticker { get; set; }
        public Int32? Quantity { get; set; }
        public Decimal? PurchasePrice { get; set; }
        public Decimal? CurrentPrice { get; set; }
        public Decimal? Delta { get; set; }
    }

    //I don't know what this is for either. Don't use it, please. 
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