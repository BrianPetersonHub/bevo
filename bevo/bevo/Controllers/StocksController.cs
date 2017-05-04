using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using bevo.Models;

namespace bevo.Controllers
{
    public class StocksController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Stocks
        public ActionResult Index()
        {
            return View(db.Stocks.ToList());
        }

        // GET: Stocks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return HttpNotFound();
            }

            StockDetailsViewModel details = new StockDetailsViewModel();
            List<Transaction> trans = stock.Transactions.ToList();
            Decimal? decTotal = 0;
            Decimal? decAddP = 0;
            Int32? intQuantity = 0;
            foreach(Transaction t in trans)
            {
                Decimal? decPrice = t.Amount / t.NumShares;
                Decimal? decCurrentP = Utilities.GetQuote.GetStock(stock.StockTicker).LastTradePrice;
                Decimal? decDiff = decPrice - decCurrentP;
                Int32? intQuant = t.NumShares;
                decAddP += decPrice;
                decTotal += decDiff;
                intQuantity += t.NumShares;
            }
            Decimal? decAvgP = decAddP / intQuantity;

            details.Name = stock.StockName;
            details.Ticker = stock.StockTicker;
            details.PurchasePrice = decAvgP;
            details.Quantity = intQuantity; 
            details.CurrentPrice = Utilities.GetQuote.GetStock(stock.StockTicker).LastTradePrice;
            details.Delta = details.PurchasePrice - details.CurrentPrice;
            ViewBag.YahooImg = "https://chart.finance.yahoo.com/z?s=" + details.Ticker + "&t=6m&q=l&l=on&z=s&p=m50,m200";
            return View(details);
        }

        // GET: Stocks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Stocks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StockID,StockName,StockTicker,TypeOfStock")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                db.Stocks.Add(stock);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stock);
        }

        // GET: Stocks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return HttpNotFound();
            }
            return View(stock);
        }

        // POST: Stocks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StockID,StockName,StockTicker,TypeOfStock")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stock).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stock);
        }

        // GET: Stocks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return HttpNotFound();
            }
            return View(stock);
        }

        // POST: Stocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Stock stock = db.Stocks.Find(id);
            db.Stocks.Remove(stock);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
