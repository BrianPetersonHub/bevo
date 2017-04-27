using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bevo.Models;

namespace bevo.Controllers
{
    public class SellStockController : Controller
    {
        // GET: SellStock
        public ActionResult Index()
        {
            // get relevant information into view bags
            ViewBag.AllAccounts = GetAccounts();
            ViewBag.AllStocks = GetStocks();
            ViewBag.StockDetails = GetStockDetails();
            return View();
        }

        //TODO: Get Stock Detail View Models
        public List<StockViewModel> GetStocks()
        {
            List<StockViewModel> allStocks = new List<StockViewModel>();
            // logic to get stock details
            return allStocks;
        }

        //TODO: Get savings, checkings, and cash-portions
        public List<AccountsViewModel> GetAccounts()
        {
            List<AccountsViewModel> allAccounts = new List<AccountsViewModel>();
            // logic to get checkings, savings, cash-portion and respective balances
            return allAccounts;
        }


        //TODO: Get stock changes
        public List<StockDetailsViewModel> GetStockDetails()
        {
            List<StockDetailsViewModel> stockDetails = new List<StockDetailsViewModel>();
            // logic to get stock details
            // this is to display the changes in stock prices
            // i am making the decision here to not show a graph of stock prices bc that's too complicated 
            return stockDetails;
        }


        //TODO: Create Purchase
        public ActionResult SellStock(Int32 numShares, Int32 selectedStock, Date date)
        {
            //numShares cannot be <0 or > currentshares 
            //if unsuccessful: error message
            //if successful:
                // create two transactions
                // 1) type: deposit, amount: $amount, Description: string
                // 2) type: Fee, amount: $amount, Description: string
                // create summary screen View model object
                // return RedirectToAction(SummaryScreen); ? I think this is the right way to do this, not sure

            return RedirectToAction("Details", "StockPortfolio");
        }

        //TODO: summary screen display view
        public ActionResult SummaryScreen()
        {
            //TODO: create view for summary screen
            return View();
        }
    }
}