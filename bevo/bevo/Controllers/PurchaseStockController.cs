using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bevo.Models;
using Microsoft.AspNet.Identity;

namespace bevo.Controllers
{
    public class PurchaseStockController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: PurchaseStock
        public ActionResult Index()
        {
            // add relevant information to viewbag
            ViewBag.AllAccounts = GetAccounts();
            ViewBag.AllStocks = GetStocks();
            ViewBag.SelectAccount = SelectAccount();

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
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            List<AccountsViewModel> allAccounts = new List<AccountsViewModel>();
            List<CheckingAccount> checkingAccounts = user.CheckingAccounts;
            List<SavingAccount> savingAccounts = user.SavingAccounts;
            StockPortfolio stockPortfolio = user.StockPortfolio;
            
            // get checkings
            foreach (var c in checkingAccounts)
            {
                AccountsViewModel accountToAdd = new AccountsViewModel();
                accountToAdd.AccountNum = c.AccountNum;
                accountToAdd.AccountName = c.AccountName;
                accountToAdd.Balance = c.Balance;
                allAccounts.Add(accountToAdd);
            }

            // get savings
            foreach (var s in checkingAccounts)
            {
                AccountsViewModel accountToAdd = new AccountsViewModel();
                accountToAdd.AccountNum = s.AccountNum;
                accountToAdd.AccountName = s.AccountName;
                accountToAdd.Balance = s.Balance;
                allAccounts.Add(accountToAdd);
            }


            // get cash portion stock portfolio
            AccountsViewModel p = new AccountsViewModel();
            p.AccountNum = stockPortfolio.AccountNum;
            p.AccountName = stockPortfolio.AccountName;
            p.Balance = stockPortfolio.Balance;
            allAccounts.Add(p);

            return allAccounts;
        }

        public IEnumerable<SelectListItem> SelectAccount()
        {
            List<AccountsViewModel> allAccounts = GetAccounts();
            SelectList selectAccount = new SelectList(allAccounts.OrderBy(a => a.AccountName), "AccountNum", "AccountName");
            return selectAccount;
        }

        //POST: 
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