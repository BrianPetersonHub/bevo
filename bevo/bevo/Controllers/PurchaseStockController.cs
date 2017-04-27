using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bevo.Models;

namespace bevo.Controllers
{
    public class PurchaseStockController : Controller
    {
        // GET: PurchaseStock
        public ActionResult Index()
        {
            // add relevant information to viewbag
            ViewBag.AllAccounts = GetAccounts();
            ViewBag.AllStocks = GetStocks();

            return View();
        }

        //TODO: Get information for all the stocks they are allowed to buy based on which stocks are in the DB 
        public List<StockViewModel> GetStocks()
        {
            List<StockViewModel> allStocks = new List<StockViewModel>();
            // logic to get stock details
            return allStocks;
        }

        //TODO: Get all the accounts for that user that they could use to buy stocks 
        public List<AccountsViewModel> GetAccounts()
        {
            List<AccountsViewModel> allAccounts = new List<AccountsViewModel>();
            // logic to get checkings, savings, cash-portion and respective balances
            return allAccounts;
        }

        //TODO: Create Purchase based on the number of shares, the stock they want to buy, the date they enter for the
        //purchase and the account that they wanted to execute the purchase 
        public ActionResult PurchaseStock(Int32 numShares, Int32 selectedAccount, Int32 selectedStock, Date date)
        {
            //check if funds are available
            //if not, error view with description
            //if yes, money withdrawn from selected account
            // transaction create: 
                    // type: withdraw
                    // amount: $amount
                    // description: stock purchase- stock portfolio (account #)
            // redirect to confirmation page or message

            return RedirectToAction("Details", "StockPortfolio");
        }


    }
}