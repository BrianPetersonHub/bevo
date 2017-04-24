using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Web;
using System.Web.Mvc;
using bevo.Models;
using Microsoft.AspNet.Identity;

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
    }
}