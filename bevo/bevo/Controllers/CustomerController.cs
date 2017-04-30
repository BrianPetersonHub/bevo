using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;
using bevo.Models;
using Microsoft.AspNet.Identity;

namespace bevo.Controllers
{
    public class CustomerController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Customer/Home
        public ActionResult Home()
        {
            ViewBag.CurrentUser = db.Users.Find(User.Identity.GetUserId());
            ViewBag.CheckingAccounts = GetAllCheckingAccts();
            ViewBag.SavingAccounts = GetAllSavingAccts();
            ViewBag.IRAccount = GetIRAccount();
            ViewBag.StockPortfolio = GetStockPortfolio();
            return View();
        }

        public List<CheckingAccount> GetAllCheckingAccts()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            List<CheckingAccount> checkingAccounts = user.CheckingAccounts;
            return checkingAccounts;
        }
        public List<SavingAccount> GetAllSavingAccts()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            List<SavingAccount> savingAccounts = user.SavingAccounts;
            return savingAccounts;
        }
        public IRAccount GetIRAccount()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            IRAccount irAccount = user.IRAccount;
            return irAccount;
        }
        public StockPortfolio GetStockPortfolio()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            StockPortfolio stockPortfolio = user.StockPortfolio;
            return stockPortfolio;
        }

        //GET: Customer/ManageAccount
        public ActionResult ViewInfo()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            return View(user);
        }

        //GET: Customer/Edit
        public ActionResult EditInfo()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            return View(user);
        }

        //GET: Customer/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditInfo([Bind(Include = "FirstName,MiddleInitial,LastName,Street,City,State,ZipCode,Birthday")] AppUser user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ViewInfo");
            }
            return View(user);
        }
    }
}