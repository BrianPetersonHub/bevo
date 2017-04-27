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

        //TODO: Create Purchase
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