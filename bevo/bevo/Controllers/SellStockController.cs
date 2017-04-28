using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bevo.Models;
using bevo.Controllers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace bevo.Controllers
{
    public class SellStockController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: SellStock
        public ActionResult Sellstock()
        {
            // get relevant information into view bags
            //Get list of stockviewmodel objects that are available to the user in their account
            //Remember that these objects contain a property saying how many of each stock are in
            //that particular account as well as a property for the current price of the stock
            ViewBag.GetAcctStocks = GetAcctStocks();
            ViewBag.StockDetails = GetStockDetails();
            return View();
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

            //Get the ID for the user who is currently logged in 
            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            var user = userManager.FindById(User.Identity.GetUserId());
            //Find the portfolio belonging to this user 
            StockPortfolio portfolio = user.StockPortfolio;

            //Get a list of all the transactions for buying that stock and then determine the purchase
            //price based upon that information. Subtract however many shares you need going to the first
            //purchase transaction first and then workign your way forward. For each one, remove the 
            //corresponding value from the current value portion of the stock portfolio we're dealing with
            //and then subtract however much the stock is worth NOW from gains. The gains part is easy.
            //The other part may prove troublesome. 

            //First, make sure they have adequate shares to begin with 
            //If they don't, boot them back to the sale page 
            StockDetail detailInQuestion = new StockDetail();
            foreach (StockDetail sd in portfolio.StockDetails)
            {
                if(selectedStock == sd.Stock.StockID)
                {
                    detailInQuestion = sd;
                }
            }
            if(numShares > detailInQuestion.Quantity)
            {
                return View();
            }

            //Get a list of all the transactions we care about 
            List<Transaction> relevantTransactions = GetRelevantTransactions(detailInQuestion);


            List<TransactionViewModel> transViewModel = new List<TransactionViewModel>();
            //Make a list of transactionviewmodel objects to reflect each of these transactions 
            foreach(Transaction t in relevantTransactions)
            {
                TransactionViewModel trvmobj = new TransactionViewModel();
                trvmobj.NumPurchased = 
                    //HOW THE FUCK DO I KNOW HOW MUCH THE STOCK WAS ORIGINALLY PURCHASED FOR
                    //I MAY NEED TO ADD A NULLABLE PROPERTY ON THE TRANSACTION MODEL AND JUST 
                    //MAKE A NOTE OF HOW MANY OF THAT STOCK WERE PURCHASED WHEN I MAKE THE TRANSACTION
                    //OTHERWISE I DON'T KNOW HOW I'M GOING TO HAV THIS INFORMATION
            }




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






        //Get a list of all the stocks in the current user's stock portfolio and put them in 
        //Stock viwe model objects so they're easier to deal with. 
        public List<StockViewModel> GetAcctStocks()
        {
            List<StockViewModel> acctStock = new List<StockViewModel>();

            //Get the ID for the user who is currently logged in 
            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            var user = userManager.FindById(User.Identity.GetUserId());
            //Find the portfolio belonging to this user 
            StockPortfolio portfolio = user.StockPortfolio;

            List<StockViewModel> listViewModels = new List<StockViewModel>();

            //add all the stocks in the user's portfolio to the list of stockviewmodels  
            foreach (StockDetail s in portfolio.StockDetails)
            {
                StockViewModel viewModel = new StockViewModel();
                viewModel.CurrentPrice = bevo.Utilities.GetQuote.GetStock(s.Stock.StockTicker).LastTradePrice;
                viewModel.Name = s.Stock.StockName;
                viewModel.NumInAccount = s.Quantity;
                viewModel.Ticker = s.Stock.StockTicker;
                listViewModels.Add(viewModel);
            }


            return listViewModels;
        }

        //Make selectlist of all stocks that the user has available to sell 
        //BASED ON STOCK OBJECT
        public IEnumerable<SelectListItem> SelectStock()
        {
            //Get a list of the tickers for each stock in the account 
            List<StockViewModel> acctStocks = GetAcctStocks();
            List<String> acctTickers = new List<String>();
            foreach(StockViewModel svm in acctStocks)
            {
                acctTickers.Add(svm.Ticker);
            }

            //Make a list of all the stocks in the DB with tickers that match the ticker list for the portfolio
            var query = from s in db.Stocks
                        select s;
            query = query.Where(s => acctTickers.Contains(s.StockTicker));
            List<Stock> stocksToChoose = query.ToList();

            SelectList selectStock = new SelectList(stocksToChoose, "StockID", "StockName");
            return selectStock;
        }

        //Get a list of transactions that purchased this stock from this account 
        public List<Transaction> GetRelevantTransactions(StockDetail detailInQuestion)
        {
            //Get a list of all the transactions that were buystock transactions from this stock account 
            //and bought the stock in question 
            var query = from tr in db.Transactions
                        select tr;
            //Only interested in stock purchase 
            query = query.Where(tr => tr.TransType == TransType.Purchase_Stock);
            //Only interested in purchases from this account 
            List<Int32?> userAcctNums = new List<Int32?>();
            foreach (AccountsViewModel acct in GetAccounts())
            {
                userAcctNums.Add(acct.AccountNum);
            }
            query = query.Where(tr => userAcctNums.Contains(tr.FromAccount));
            //Only interested in purchases of the stock in question 
            query = query.Where(tr => tr.Stock == detailInQuestion.Stock);

            return query.OrderBy(tr => tr.Date).ToList();
        }












        //Get all the accounts for that user that they could use to buy stocks 
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
    }
}