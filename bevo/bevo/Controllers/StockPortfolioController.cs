using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Web;
using System.Web.Mvc;
using bevo.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using bevo.Utilities;

namespace bevo.Controllers
{
    public class StockPortfolioController : Controller
    {
        private AppDbContext db = new AppDbContext();


        //GET: StockPortfolio/Create
        public ActionResult Create()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.StockPortfolio == null)
            {
                StockPortfolio sp = new StockPortfolio();
                sp.AccountName = "My StockPortfolio";
                return View(sp);
            }
            else
            {
                return Content("<script language'javascript' type = 'text/javascript'> alert('Error: You can only have one Stock Portfolio.'); window.location='../Customer/Home';</script>");
            }
        }

        //POST: StockPortfolio/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StockPortfolioID,AccountNum,AccountName,Balance")] StockPortfolio stockPortfolio)
        {
            if (ModelState.IsValid)
            {
                AppUser user = db.Users.Find(User.Identity.GetUserId());

                if (user.StockPortfolio != null)
                {
                    return Content("<script language'javascript' type = 'text/javascript'> alert('Error: You cannot have more than one Stock Portfolio'); window.location='../Customer/Home';</script>");
                }

                Transaction t = new Transaction();
                t.Date = DateTime.Today;
                t.FromAccount = 0;
                t.ToAccount = 0;
                t.TransType = TransType.Deposit;
                t.Description = "Initial deposit";
                t.Amount = stockPortfolio.Balance;

                if (stockPortfolio.Balance <= 0)
                {
                    return Content("<script language'javascript' type = 'text/javascript'> alert('Error: Your starting balance must be positive.'); window.location='../StockPortfolio/Create';</script>");
                }

                if (stockPortfolio.Balance > 5000)
                {
                    t.NeedsApproval = true;
                    stockPortfolio.Balance = 0;
                }

                stockPortfolio.Transactions = new List<Transaction>();
                stockPortfolio.Transactions.Add(t);
                user.StockPortfolio = stockPortfolio;

                db.SaveChanges();
                return RedirectToAction("Home", "Customer");
            }

            //TODO: ** Add task to be approved by manager
            return View(stockPortfolio);
        }

        //GET: StockPortfolio/Details/#
        public ActionResult Details()
        {

            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            var user = userManager.FindById(User.Identity.GetUserId());

            String id = user.Id;

            StockPortfolio stockPortfolio = db.StockPortfolios.Find(id);
            if (stockPortfolio == null)
            {
                return HttpNotFound();
            }

            //Bag up pertinent information about this account 
            ViewBag.Transactions = GetAllTransactions();
            ViewBag.PortfolioSnapshot = PortfolioSnapshot();
            ViewBag.IsBalanced = BalanceCheck();
            ViewBag.StockViewModel = GetStocks();
            ViewBag.PortfolioInfo = GetPortfolioInfo();
            return View(stockPortfolio);
        }

        //GET: StockPortfolio/EditName/#
        [HttpGet]
        public ActionResult EditName(String id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockPortfolio stockPortfolio = db.StockPortfolios.Find(id);
            if (stockPortfolio == null)
            {
                return HttpNotFound();
            }

            ViewBag.AccountID = id;
            EditAccountNameViewModel editAccountNameVM = new EditAccountNameViewModel();
            editAccountNameVM.AccountName = stockPortfolio.AccountName;

            return View(editAccountNameVM);
        }

        //POST: StockPortfolio/EditName/#
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditName([Bind(Include = "AccountName")] EditAccountNameViewModel vm, String id)
        {
            if (ModelState.IsValid)
            {
                StockPortfolio stockPortfolio = db.StockPortfolios.Find(id);
                stockPortfolio.AccountName = vm.AccountName;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = id });
            }
            return View(vm);
        }


        //Get all stocks into stockviewmodel
        public List<StockViewModel> GetStocks()
        {
            List<StockViewModel> allStocks = new List<StockViewModel>();

            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            var user = userManager.FindById(User.Identity.GetUserId());
            List<StockDetail> stocks = user.StockPortfolio.StockDetails;
            foreach (StockDetail s in stocks )
            {
                StockViewModel stockToAdd = new StockViewModel();

                stockToAdd.Ticker = s.Stock.StockTicker;
                stockToAdd.NumInAccount = s.Quantity;
                stockToAdd.CurrentPrice = Utilities.GetQuote.GetStock(s.Stock.StockTicker).LastTradePrice;

                allStocks.Add(stockToAdd);
            }
            
            return allStocks;
        }

        //Get all total values associated with the portfolio. 
        //Return a stockportfolioviewmodel object with all of the relevant values for the current user's account 
        public StockPortfolioViewModel GetPortfolioInfo()
        {
            //Get userID for the user who is currently logged in 
            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            var user = userManager.FindById(User.Identity.GetUserId());

            StockPortfolioViewModel portfolioInfo = new StockPortfolioViewModel();

            //TODO: ** add logic to each value
            portfolioInfo.CurrentValue = 0;
            portfolioInfo.TotalGains = 0;
            portfolioInfo.TotalFees = 0;
            portfolioInfo.TotalBonuses = 0;
            portfolioInfo.CashAvailable = 0;
            portfolioInfo.Balanced = false;
            portfolioInfo.StockMarketValue = 0;

            AppUser currentUser = db.Users.Find(User.Identity.GetUserId());

            //find total of bonuses
            foreach (Transaction t in currentUser.StockPortfolio.Transactions)
            {
                if (t.TransType == TransType.Bonus)
                {
                    portfolioInfo.TotalBonuses += t.Amount;
                }
            }

            //Look at all of the fee transactions associated with the stock portfolio and add to the FEES PORTION
            //of the portfolio's value 
            foreach(Transaction tr in user.StockPortfolio.Transactions)
            {
                if (tr.TransType == bevo.Models.TransType.Fee)
                {
                    portfolioInfo.TotalFees += tr.Amount;
                }
            }

            //Look at the type of transaction for each transaction on the stock portfolio and either addto or 
            //subtract from the appropriate CASH AVAILABLE PORTION of the portfolio 
            portfolioInfo.CashAvailable = user.StockPortfolio.Balance;
            //foreach(Transaction tr in user.StockPortfolio.Transactions)
            //{
            //    if(tr.TransType == bevo.Models.TransType.Deposit)
            //    {
            //        portfolioInfo.CashAvailable += tr.Amount;
            //    }
            //    else if(tr.TransType == bevo.Models.TransType.Purchase_Stock && GetAccountNumbers().Contains(tr.FromAccount))
            //    {
            //        portfolioInfo.CashAvailable -= tr.Amount;
            //    }
            //    else if(tr.TransType == bevo.Models.TransType.Sell_Stock && tr.FromAccount == user.StockPortfolio.AccountNum)
            //    {
            //        portfolioInfo.CashAvailable += tr.Amount;
            //    }
            //    else if(tr.TransType == bevo.Models.TransType.Withdrawal)
            //    {
            //        portfolioInfo.CashAvailable -= tr.Amount;
            //    }
            //    else if(tr.TransType == bevo.Models.TransType.Transfer && tr.ToAccount == user.StockPortfolio.AccountNum)
            //    {
            //        portfolioInfo.CashAvailable += tr.Amount;
            //    }
            //    else if(tr.TransType == bevo.Models.TransType.Transfer && tr.ToAccount != user.StockPortfolio.AccountNum)
            //    {
            //        portfolioInfo.CashAvailable -= tr.Amount;
            //    }
            //}
            
            //Get a list of all the stock tickers in the account 
            //Also get the value of the STOCKMARKETVALUE PORTION of the portfolio
            List<String> tickersInAccount = new List<String>();
            foreach (Transaction t in user.StockPortfolio.Transactions)
            {
                if(t.TransType == TransType.Purchase_Stock)
                {
                    decimal stockVal = t.Amount;
                    portfolioInfo.StockMarketValue += stockVal;
                    tickersInAccount.Add(t.Stock.StockTicker);
                }
                else if(t.TransType == TransType.Sell_Stock)
                {
                    Decimal? stockMarketValueReduction = t.SMVRedux;
                    portfolioInfo.StockMarketValue -= stockMarketValueReduction;
                }

                
            }

            //Define how much is in the GAINS PORTION of the portfolio
            decimal currentMarketValue = new decimal();
            currentMarketValue = 0;
            foreach(StockViewModel s in PortfolioSnapshot())
            {
                currentMarketValue += (s.CurrentPrice * s.NumInAccount);
            }
            portfolioInfo.TotalGains = currentMarketValue - portfolioInfo.StockMarketValue;

            //Find the total value of the entire portfolio 
            portfolioInfo.CurrentValue = portfolioInfo.CashAvailable + portfolioInfo.StockMarketValue - portfolioInfo.TotalFees
                                         + portfolioInfo.TotalBonuses + portfolioInfo.TotalGains;


            return portfolioInfo;
        }

        //Finds list of transactions based on the ID of the user who is currently logged in
        public List<Transaction> GetAllTransactions()
        {
            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            var user = userManager.FindById(User.Identity.GetUserId());
            string id = user.Id;

            StockPortfolio stockPortfolio = db.StockPortfolios.Find(id);
            List<Transaction> transactions = stockPortfolio.Transactions;
            return transactions;
        }



        //Method to check if portfolio is balanced
        public Boolean BalanceCheck()
        {
            //TODO: Make it so the manager can look at balance checks for all customers at once
            //This method will only check the account of the user who is currently logged in,
            //which will be useful only for the customer funcionality 

            //Get the Id of the user who is currently logged in. 
            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            var user = userManager.FindById(User.Identity.GetUserId());


            //Get a list of all the stocks in the account 
            List<Stock> stockList = new List<Stock>();

            foreach (StockDetail s in user.StockPortfolio.StockDetails)
            {
                stockList.Add(s.Stock);
            }

            //Counts to keep track of each stock type in the account 
            Int32 numOrdinary = new Int32();
            Int32 numIndex = new Int32();
            Int32 numMutual = new Int32();
        
            foreach (Stock stock in stockList)
            {
                if(stock.TypeOfStock == StockType.Ordinary)
                {
                    numOrdinary += 1;
                }
                else if(stock.TypeOfStock == StockType.Index_Fund)
                {
                    numIndex += 1;
                }
                else if(stock.TypeOfStock == StockType.Mutual_Fund)
                {
                    numMutual += 1;
                }
            }

            //Check if the account qualifies 
            if(numOrdinary >= 2 && numIndex >= 1 && numMutual >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        //Make a method to get all the info for the current account stocks (using stockviewmodels)
        //TODO: Again, this method only works for the user who is currently logged in
        public List<StockViewModel> PortfolioSnapshot()
        {
            //Get the ID of the user who is currently logged in
            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            var user = userManager.FindById(User.Identity.GetUserId());

            //Look at each StockDetail in the person's account and make a StockViewModel to campture information about it 
            //Get a list of all the stocks in the account 
            List<StockDetail> stockDetailList = new List<StockDetail>();
            foreach (StockDetail s in user.StockPortfolio.StockDetails)
            {
                stockDetailList.Add(s);
            }

            //Make list to hold all the stockviewmodel objects
            List<StockViewModel> listToReturn = new List<StockViewModel>();

            //Add information from each stock record into the viewbag 
            foreach(StockDetail detail in stockDetailList)
            {
                StockViewModel model = new StockViewModel();
                model.Name = detail.Stock.StockName;
                model.NumInAccount = detail.Quantity;
                model.Ticker = detail.Stock.StockTicker;

                StockQuote quote = bevo.Utilities.GetQuote.GetStock(detail.Stock.StockTicker);

                model.CurrentPrice = quote.LastTradePrice;

                //Add the model to the list of models already in the viewbag
                listToReturn.Add(model);
            }

            return listToReturn;
        }


        //Method that returns quotes for each stock appearing in the current user's account
        //This is similar to the snapshot method except that it looks at the stocks in themselves
        //rather than looking at how much the user has of each stock.
        public List<StockQuote> StockQuotes()
        {
            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));

            //Get the current user as an AppUser object 
            var user = userManager.FindById(User.Identity.GetUserId());

            //Get the list of the ticker for each stock in the user's portfolio 
            List<StockDetail> stockDetailList = user.StockPortfolio.StockDetails;
            List<string> tickersInAccount = new List<string>();
            foreach (StockDetail detail in stockDetailList)
            {
                string tickerInQuestion = detail.Stock.StockTicker;
                tickersInAccount.Add(tickerInQuestion);
            }


            //Return a quote for each stock in the user's account 
            List<StockQuote> Quotes = new List<StockQuote>();
            foreach (string ticker in tickersInAccount)
            {
                StockQuote sq1 = GetQuote.GetStock(ticker);
                Quotes.Add(sq1);
            }

            //Return the list with a quote for each stock that is in the account 
            return Quotes;
        }

        //Get a list of the account numbers for each of the user's accountis 
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

        public List<Int32?> GetAccountNumbers()
        {
            List<Int32?> AcctNums = new List<Int32?>();
            foreach(AccountsViewModel avm in GetAccounts())
            {
                AcctNums.Add(avm.AccountNum);
            }

            return AcctNums;
        }

        public List<DisputeViewModel> GetUnresolvedDisputes()
        {
            AppDbContext db = new AppDbContext();

            List<Dispute> disputeList = new List<Dispute>();
            var query = from d in db.Disputes
                        select d;
            query = query.Where(d => d.DisputeStatus == DisputeStatus.Submitted);
            disputeList = query.ToList();

            List<DisputeViewModel> dvmList = new List<DisputeViewModel>();
            foreach (Dispute d in disputeList)
            {
                DisputeViewModel dvm = new DisputeViewModel();
                dvm.CorrectAmount = d.DisputedAmount;
                dvm.FirstName = d.AppUser.FirstName;
                dvm.LastName = d.AppUser.LastName;
                dvm.TransAmount = d.Transaction.Amount;
                dvm.Message = d.Message;
                dvm.CustEmail = d.AppUser.Email;
                dvm.TransName = d.Transaction.TransactionID;
                dvm.DisputeID = d.DisputeID;
                dvm.Status = DisputeStatus.Submitted;
                dvmList.Add(dvm);
            }


            return dvmList;
        }

    }
}