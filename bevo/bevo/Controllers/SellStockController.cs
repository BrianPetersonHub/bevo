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
            ViewBag.AllStocks = PortfolioSnapshot();
            ViewBag.StockDetails = GetStockDetails();
            return View();
        }

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
            foreach (StockDetail detail in stockDetailList)
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
    }
}