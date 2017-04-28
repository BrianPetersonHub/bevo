﻿using System;
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
            return View();
        }

        //POST: StockPortfolio/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StockPortfolioID,AccountNum,AccountName,Balance")] StockPortfolio stockPortfolio)
        {
            if (ModelState.IsValid)
            {
                AppUser user = db.Users.Find(User.Identity.GetUserId());
                user.StockPortfolio = stockPortfolio;
                //TODO: error here bc adding duplicate primary key
                db.SaveChanges();
                return RedirectToAction("Home", "Customer");
            }

            //TODO: ** Add task to be approved by manager
            return View(stockPortfolio);
        }

        //GET: StockPortfolio/Details/#
        public ActionResult Details(String id)
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

            //Bag up pertinent information about this account 
            ViewBag.Transactions = GetAllTransactions();
            ViewBag.PortfolioSnapshot = PortfolioSnapshot();
            ViewBag.IsBalanced = BalanceCheck();

            ViewBag.PortfolioInfo = GetPortfolioInfo();
            return View(stockPortfolio);
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
            foreach(Transaction tr in user.StockPortfolio.Transactions)
            {
                if(tr.TransType == bevo.Models.TransType.Deposit)
                {
                    portfolioInfo.CashAvailable += tr.Amount;
                }
                else if(tr.TransType == bevo.Models.TransType.Purchase_Stock && tr.FromAccount == user.StockPortfolio.AccountNum)
                {
                    portfolioInfo.CashAvailable -= tr.Amount;
                }
                else if(tr.TransType == bevo.Models.TransType.Sell_Stock && tr.FromAccount == user.StockPortfolio.AccountNum)
                {
                    portfolioInfo.CashAvailable += tr.Amount;
                }
                else if(tr.TransType == bevo.Models.TransType.Withdrawal)
                {
                    portfolioInfo.CashAvailable -= tr.Amount;
                }
                else if(tr.TransType == bevo.Models.TransType.Transfer && tr.ToAccount == user.StockPortfolio.AccountNum)
                {
                    portfolioInfo.CashAvailable += tr.Amount;
                }
                else if(tr.TransType == bevo.Models.TransType.Transfer && tr.ToAccount != user.StockPortfolio.AccountNum)
                {
                    portfolioInfo.CashAvailable -= tr.Amount;
                }
            }
            
            //Get a list of all the stock tieckers in the account 
            //Also get the value of the STOCKMARKETVALUE PORTION of the portfolio
            List<String> tickersInAccount = new List<String>();
            foreach (Transaction t in user.StockPortfolio.Transactions)
            {
                if(t.TransType == TransType.Purchase_Stock)
                {
                    decimal stockVal = t.Amount;
                    portfolioInfo.StockMarketValue += stockVal;
                }
                else if(t.TransType == TransType.Sell_Stock)
                {
                    decimal stockVal = t.Amount;
                    portfolioInfo.StockMarketValue -= stockVal;
                }

                tickersInAccount.Add(t.Stock.StockTicker);
            }


            //Check to see if the portfolio is balanced 
            bool bal = BalanceCheck();
            if (bal == true)
            {
                portfolioInfo.Balanced = true;
                portfolioInfo.TotalBonuses += (portfolioInfo.CashAvailable - portfolioInfo.TotalFees) * .1m;
                portfolioInfo.TotalBonuses += portfolioInfo.StockMarketValue * .1m;
            }

            //Define how much is in the GAINS PORTION of the portfolio
            decimal currentMarketValue = new decimal();
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
    }
}