using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bevo.Models;
using bevo.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace bevo.Controllers
{
    public class StockQuoteController : Controller
    {
        AppDbContext db = new AppDbContext();
        // GET: Home
        public ActionResult Index()
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
            foreach(string ticker in tickersInAccount)
            {
                StockQuote sq1 = GetQuote.GetStock(ticker);
                Quotes.Add(sq1);
            }

            //Return the index view with a quote for each of the stocks in the user's account 
            return View(Quotes);
        }
    }
}