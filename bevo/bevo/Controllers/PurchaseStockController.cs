using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bevo.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace bevo.Controllers
{
    public class PurchaseStockController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: PurchaseStock
        public ActionResult Index()
        {
            // add relevant information to viewbag
            //Returns all the accounts that the user is allowed to use to purchase stock 
            ViewBag.AllAccounts = GetAccounts();
            //Gets availablestock viewmodel objects for each available stock
            ViewBag.AvailableStocks = GetStocks();
            //Returns a selectlist with stocks to choose 
            ViewBag.SelectStock = SelectStock();
            //Returns a selectlist with the account they choase 
            ViewBag.SelectAccount = SelectAccount();

            return View();
        }

        //Create Purchase based on the number of shares, the stock they want to buy, the date they enter for the
        //purchase and the account that they wanted to execute the purchase
        //POST METHOD
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PurchaseStock(Int32 numShares, Int32 selectedAccount, Int32 selectedStock, DateTime enteredDate)
        {
            //SUMMARY
            //THIS WILL MAKE A NEW TRANSACTION OBJECT THAT WILL REFLECT THE INFORMATION FROM THE PURCHASE STOCK PAGE
            //SUMMARY

            //Make a transaction to store this info
            Transaction trans = new Transaction();





            //Get the stock from the DB that we are concerned with buying here
            var stockQuery = from s in db.Stocks
                             select s;
            stockQuery = stockQuery.Where(s => s.StockID == selectedStock);

            List<Stock> StockList = stockQuery.ToList();
            Stock stockInQuestion = StockList[0];

            //Assign this stock to the transaction object 
            trans.Stock = stockInQuestion;

            //Assign the submitted date as the transaction date 
            trans.Date = enteredDate;

            //Assign type as stock purchase 
            trans.TransType = TransType.Purchase_Stock;





            //Query the DB to get the checking account that matches the selected account number 
            var caQuery = from ca in db.CheckingAccounts
                          select ca;
            caQuery = caQuery.Where(ca => ca.AccountNum == selectedAccount);
            //Make a list even though it will only contain one object 
            List<CheckingAccount> connectedCheckingAccount = caQuery.ToList();
            //If a checking account was found, then add it to the transaction's nav property for checking accounts 
            if (caQuery != null)
            {
                trans.CheckingAccounts.Add(connectedCheckingAccount[0]);
                //Assign the from account as that account's AccountNum
                trans.FromAccount = connectedCheckingAccount[0].AccountNum;
                //If there isn't enough money in the account, send them back to the purchase stock page
                //Otherwise, subtract the cost of the transaction from the appropriate account balance 
                if(connectedCheckingAccount[0].Balance < (numShares * bevo.Utilities.GetQuote.GetStock(stockInQuestion.StockTicker).LastTradePrice))
                {
                    return View();
                }
                else
                {
                    connectedCheckingAccount[0].Balance -= (numShares * bevo.Utilities.GetQuote.GetStock(stockInQuestion.StockTicker).LastTradePrice);
                }
            }

            //Do the same as above but for the saving accounts 
            var saQuery = from sa in db.SavingAccounts
                          select sa;
            saQuery = saQuery.Where(sa => sa.AccountNum == selectedAccount);
            List<SavingAccount> connectedSavingsAccount = saQuery.ToList();
            if (saQuery != null)
            {
                trans.SavingAccounts.Add(connectedSavingsAccount[0]);
                trans.FromAccount = connectedCheckingAccount[0].AccountNum;
                if (connectedSavingsAccount[0].Balance < (numShares * bevo.Utilities.GetQuote.GetStock(stockInQuestion.StockTicker).LastTradePrice))
                {
                    return View();
                }
                else
                {
                    connectedSavingsAccount[0].Balance -= (numShares * bevo.Utilities.GetQuote.GetStock(stockInQuestion.StockTicker).LastTradePrice);
                }

            }

            //Do the same as above but for the stock portfolio 
            var spQuery = from sp in db.StockPortfolios
                          select sp;
            spQuery = spQuery.Where(sp => sp.AccountNum == selectedAccount);
            List<StockPortfolio> connectedStockPortfolio = spQuery.ToList();
            if (spQuery != null)
            {
                trans.StockPortfolios.Add(connectedStockPortfolio[0]);
                trans.FromAccount = connectedCheckingAccount[0].AccountNum;
                if (connectedStockPortfolio[0].Balance < (numShares * bevo.Utilities.GetQuote.GetStock(stockInQuestion.StockTicker).LastTradePrice))
                {
                    return View();
                }
                else
                {
                    connectedStockPortfolio[0].Balance -= (numShares * bevo.Utilities.GetQuote.GetStock(stockInQuestion.StockTicker).LastTradePrice);
                }
            }



            //Set transaction amount 
            trans.Amount = (numShares * bevo.Utilities.GetQuote.GetStock(stockInQuestion.StockTicker).LastTradePrice);

            //Give a description of the transaction
            trans.Description = "Bought " + numShares.ToString() + " shares of " + stockInQuestion.StockName +
                                " (" + stockInQuestion.StockTicker + ") stock for $" + trans.Amount.ToString();

            //Save this to the DB
            db.Transactions.Add(trans);
            db.SaveChanges();



            // THIS WILL GET EITHER ADD THE STOCK PURCHASE QUANTITY TO THE APPROPRIATE DETAIL OBJECT LINKED TO THE 
            // PORTFOLIO OBJECT OR IT WILL ADD A NEW DETAIL OBJECT WITH THE APPROPRIATE INFORMTION 


            //Get the ID for the user who is currently logged in 
            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            var user = userManager.FindById(User.Identity.GetUserId());
            //Find the portfolio belonging to this user 
            StockPortfolio portfolio = user.StockPortfolio;
            //Get all the stock details attached to this user's stock portfolio
            var sdQuery = from sd in db.StockDetails
                          select sd;
            sdQuery = sdQuery.Where(sd => sd.StockPortfolio == portfolio);
            //Make a list of all of those stock detail tables 
            List<StockDetail> stockDetails = sdQuery.ToList();
            List<Int32> stockIDsInAccount = new List<Int32>();
            foreach(StockDetail detail in stockDetails)
            {
                stockIDsInAccount.Add(detail.Stock.StockID);
            }
            //If the user already has that stock in their account, add numShares to their detail table
            //Otherwise, make a new stock detail for them 
            if (stockIDsInAccount.Contains(selectedStock))
            {
                foreach (StockDetail detail in stockDetails)
                {
                    if (selectedStock == detail.Stock.StockID)
                    {
                        detail.Quantity += numShares;
                        //Save this change to the DB
                        db.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }    
            else
            {
                StockDetail detail = new StockDetail();
                detail.StockPortfolio = user.StockPortfolio;
                detail.Stock = db.Stocks.Find(selectedStock);
                detail.Quantity = numShares;
                detail.PurchasePrice = bevo.Utilities.GetQuote.GetStock(stockInQuestion.StockTicker).LastTradePrice;
                //Save this new stock detail in the DB
                db.StockDetails.Add(detail);
                db.SaveChanges();
            }

            //Redirect the user to the details page on the stockportfoliocontroller
            return RedirectToAction("Details", "StockPortfolio");
        }


        //Get information for all the stocks they are allowed to buy based on which stocks are in the DB 
        public List<AvailableStock> GetStocks()
        {
            List<AvailableStock> allStocks = new List<AvailableStock>();

            //Get all the stocks in the DB
            var Query = from s in db.Stocks
                        select s;
            List<Stock> stocksList = Query.ToList();

            //make an available stock viewmodel for each one of thsoe 
            foreach(Stock s in stocksList)
            {
                AvailableStock avail = new AvailableStock();
                avail.Name = s.StockName;
                avail.Ticker = s.StockTicker;
                avail.CurrentPrice = bevo.Utilities.GetQuote.GetStock(s.StockTicker).LastTradePrice;
            }


            return allStocks;
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
            foreach (var s in savingAccounts)
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
        
        //Create a select list of all the accoutns for the user  
        //BASED ON ACCOUNTSVIEWMODEL OBJECTS
        public IEnumerable<SelectListItem> SelectAccount()
        {
            List<AccountsViewModel> allAccounts = GetAccounts();
            SelectList selectAccount = new SelectList(allAccounts.OrderBy(a => a.AccountName), "AccountNum", "AccountName");
            return selectAccount;
        }

        //Create a select list of all the available stocks for the user
        //BAED ON STOCK OBJECTS
        public IEnumerable<SelectListItem> SelectStock()
        {
            var query = from s in db.Stocks
                        select s;

            List<Stock> allStocks = query.ToList();


            SelectList selectStock = new SelectList(allStocks, "StockID", "StockName");
            return selectStock;
        }


    }
}