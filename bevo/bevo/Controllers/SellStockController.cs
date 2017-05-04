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
        public ActionResult Index()
        {
            // get relevant information into view bags
            //Get list of stockviewmodel objects that are available to the user in their account
            //Remember that these objects contain a property saying how many of each stock are in
            //that particular account as well as a property for the current price of the stock
            ViewBag.GetAcctStocks = GetAcctStocks();
            ViewBag.SelectStock = SelectStock();

            return View();
        }


        //TODO: Get stock changes
        //I'm not sure if this is necessary. Let's discuss it next time we meet. 
        public List<StockViewModel> GetStockDetails()
        {
            List<StockViewModel> stockDetails = new List<StockViewModel>();
            ////
            return stockDetails;
        }


        //Create Purchase: Extensive description if you expand the method
        public ActionResult SellStock(Int32 numShares, Int32 selectedStock, DateTime dateEntered)
        {
            //Get the ID for the user who is currently logged in 
            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            var user = userManager.FindById(User.Identity.GetUserId());
            //Find the portfolio belonging to this user 
            StockPortfolio portfolio = user.StockPortfolio;

            ///SUMMARY OF THIS METHOD
            //Get a list of all the transactions for buying that stock and then determine the purchase
            //price based upon that information. Subtract however many shares you need going to the first
            //purchase transaction first and then workign your way forward. 
            //Then make a transaction object to reflect all of this information 
            ///SUMMARY OF THIS METHOD 

            //First, make sure they have adequate shares to begin with 
            //If they don't, boot them back to the sale page 
            //also boot them if they try to sell a negative number of shares 
            StockDetail detailInQuestion = new StockDetail();
            foreach (StockDetail sd in portfolio.StockDetails)
            {
                if(selectedStock == sd.Stock.StockID)
                {
                    detailInQuestion = sd;
                }
            }
            if (numShares > detailInQuestion.Quantity || numShares < 0)
            {
                return View();
            }

            //Get a list of all the transactions we care about 
            List<Transaction> relevantTransactions = GetRelevantTransactions(detailInQuestion);
            
                


            List<TransactionViewModel> transViewModels = new List<TransactionViewModel>();
            //Make a list of transactionviewmodel objects to reflect each of these transactions 
            foreach(Transaction t in relevantTransactions)
            {
                TransactionViewModel trvmobj = new TransactionViewModel();

                trvmobj.NumPurchased = t.NumShares;
                trvmobj.PurchasePrice = (t.Amount / t.NumShares);
                transViewModels.Add(trvmobj);
            }


            //Running number of shares still to be subtracted
            Int32 runningShares = numShares;
            //Total ORIGINAL market value of the stocks sold from the account
            //We assume FIFO for stock sales 
            Decimal ? stockMarketValueReduction = new Decimal?();
            stockMarketValueReduction = 0;

            //Make an actual Transaction object to record the sale of stock 
            //And determine its value before you stubtract the number of shares down to one,
            //you fucking idiot 
            Transaction transToAdd = new Transaction();
            transToAdd.StockPortfolios = new List<StockPortfolio>();
            transToAdd.StockPortfolios.Add(db.StockPortfolios.Find(portfolio.StockPortfolioID));
            transToAdd.Amount = numShares * bevo.Utilities.GetQuote.GetStock(detailInQuestion.Stock.StockTicker).LastTradePrice;
            transToAdd.FromAccount = portfolio.AccountNum;
            transToAdd.Stock = detailInQuestion.Stock;
            transToAdd.Description = "Sold " + numShares.ToString() + " shares of " + detailInQuestion.Stock.StockName +
                                     " (" + detailInQuestion.Stock.StockTicker + ") stock for $" + transToAdd.Amount.ToString();
            transToAdd.Date = dateEntered;
            transToAdd.TransType = TransType.Sell_Stock;


            //Take away the appropriate number of stocks from the relevant stockdetail object attached to this 
            //portfolio 
            detailInQuestion.Quantity -= numShares;
            db.Entry(detailInQuestion).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            //Add the amount of money made from selling the stock back into the portfolio's balance
            portfolio.Balance += transToAdd.Amount;
            db.Entry(portfolio).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            //Logic to get how much the original stock market value should decrease by
            //Based on incrementally determining how many transactions need to be negated to
            //sell as many stocks as the user wants to sell 
            Int32 listIndex = 0;
            while (numShares > 0)
            {
                //Take one away from the stocks in that purchase 
                transViewModels[listIndex].NumPurchased -= 1;
                //Add that stock's original market value to the amount to be subtracted from the 
                //StockMarketValue of the portfolio
                stockMarketValueReduction += transViewModels[listIndex].PurchasePrice;
                //If that is all the stocks from that transaction, move on to the next one
                if (transViewModels[listIndex].NumPurchased == 0)
                {
                    listIndex += 1;
                }
                //Reduce the number of shares still to be sold by one
                numShares -= 1;
            }


            //Assign all the remaining values to the transaction to be created
            //Assign the SMVRedux property on the transaction record  
            transToAdd.SMVRedux = stockMarketValueReduction;

            if(detailInQuestion.Stock.feeAmount != null)
            {
                //FEE TRANSACTION
                //create a transaction to reflect the fee
                Int32 sellStockFee = (Int32)detailInQuestion.Stock.feeAmount;
                Transaction feeTransaction = new Transaction();
                feeTransaction.FromAccount = portfolio.AccountNum;
                feeTransaction.Date = dateEntered;
                feeTransaction.TransType = TransType.Fee;
                feeTransaction.Amount = sellStockFee;
                feeTransaction.Description = "Fee incurred from selling " + numShares.ToString() + " shares of " +
                                             detailInQuestion.Stock.StockName + " (" + detailInQuestion.Stock.StockTicker +
                                             ") stock on " + dateEntered.ToString();
                feeTransaction.StockPortfolios = new List<StockPortfolio>();
                feeTransaction.StockPortfolios.Add(portfolio);

                
                //Subtract the ten dollars from the stock portfolio cash section as a result of the fee for selling stock
                //Assume here that they have at least ten bucks in their account 
                db.Entry(portfolio).State = System.Data.Entity.EntityState.Modified;
                db.Transactions.Add(feeTransaction);
                db.SaveChanges();
            }

            //Save this to the DB
            db.Transactions.Add(transToAdd);
            db.SaveChanges();
            

            //Redirect to the details page
            return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Successfully sold a stock!'); window.location='../../StockPortfolio/Details';</script>");
        }

        public ActionResult SummaryScreen()
        {
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
                viewModel.PurchasePrice = 10.00m;
                viewModel.StockID = s.Stock.StockID;
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
            //Only interested in purchases from this account 
            List<Int32?> userAcctNums = new List<Int32?>();
            foreach (AccountsViewModel acct in GetAccounts())
            {
                userAcctNums.Add(acct.AccountNum);
            }
            //Get a list of all the transactions that were buystock transactions from this stock account 
            //and bought the stock in question 
            var query = db.Transactions
                        .Where(tr => tr.TransType == TransType.Purchase_Stock)
                   //   .Where(tr => userAcctNums.Contains(tr.FromAccount))
                        .Where(tr => tr.Stock.StockID == detailInQuestion.Stock.StockID)
                        .OrderBy(tr => tr.Date)
                        .Select(tr => tr.TransactionID)
                        .ToList();

            List<Transaction> queryList = new List<Transaction>();
            foreach(Int32 q in query)
            {
                Transaction trans = db.Transactions.Find(q);
                queryList.Add(trans);
            }

            List<Transaction> relevantTransactions = new List<Transaction>();
            foreach (Transaction t in queryList)
            {
                if(userAcctNums.Contains(t.FromAccount))
                {
                    relevantTransactions.Add(t);
                }
            }

            

            return relevantTransactions;
        }




        //Get all the accounts for that user that they could use to buy stocks 
        //This is relavent for getting all the transactions in which they purchased 
        //a given stock 
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