using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bevo.DAL;
using bevo.Models;

namespace bevo.Controllers
{
    public class StockPortfolioController : Controller
    {

        private AppDbContext db = new AppDbContext();

        // GET: StockPortfolio
        public ActionResult Index()
        {
            return View();
        }

        //POST: StockPortfolio/Create
        //TODO: look at if the way the correct acctnum is added is correct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StockPortfolioID,AccountNum,AccountName,Balance")] StockPortfolio StockPortfolio)
        {
            if (ModelState.IsValid)
            {
                db.StockPortfolio.Add(StockPortfolio);
                db.SaveChanges();
                return RedirectToAction("CustomerHome", "PersonsController");
            }
            return View(StockPortfolio);
        }
    }
}