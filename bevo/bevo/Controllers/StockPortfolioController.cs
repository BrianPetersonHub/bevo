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
            return View(stockPortfolio);
        }

        public List<Transaction> GetAllTransactions(String id)
        {
            StockPortfolio stockPortfolio = db.StockPortfolios.Find(id);
            List<Transaction> transactions = stockPortfolio.Transactions;
            return transactions;
        }



        //Method to check if portfolio is balanced
        public Boolean BalanceCheck(StockPortfolio portfolio)
        {
            //TODO: Make it so the manager can look at balance checks for all customers at once
            //This method will only check the account of the user who is currently logged in,
            //which will be useful only for the customer funcionality 

            //Get the Id of the user who is currently logged in. 
            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            var user = userManager.FindById(User.Identity.GetUserId());


            //Get a list of all the stock types in the account 
            List<Stock> stockList = user.StockPortfolio.StockDetail.Stocks.ToList();
            List<StockType> typesInAccount = new List<StockType>();

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
    }
}