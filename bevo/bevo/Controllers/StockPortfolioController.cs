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
            ViewBag.Transactions = GetAllTransactions(id);
            ViewBag.StockViewModel = PortfolioSnapshot();
            return View(stockPortfolio);
        }

        public List<Transaction> GetAllTransactions(String id)
        {
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


        //Make a method to get all the info for the current account stocks 
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

            //Make viwebag space to hold all the stockviewmodel objects
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
    }
}