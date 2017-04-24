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
    public class CheckingAccountsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        //GET: CheckingAccount/Create
        public ActionResult Create()
        {
            return View();
        }

        //POST: ChechingAccount/Create
        //TODO: look at if the way the correct acctnum is added is correct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CheckingAccountID,AccountNum,AccountName,Balance")] CheckingAccount checkingAccount)
        {
            if (ModelState.IsValid)
            {
                AppUser user = db.Users.Find(User.Identity.GetUserId());
                if (checkingAccount.AccountName == null)
                {
                    checkingAccount.AccountName = "Longhorn Checking";
                }
                user.CheckingAccounts.Add(checkingAccount);
                db.SaveChanges();

                return RedirectToAction("Home", "Customer");
            }
            return View(checkingAccount);
        }

        //GET: CheckingAccounts/Details/#
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CheckingAccount checkingAccount = db.CheckingAccounts.Find(id);
            if (checkingAccount == null)
            {
                return HttpNotFound();
            }
            ViewBag.Transactions = GetAllTransactions(id);
            return View(checkingAccount);
        }



        public List<Transaction> GetAllTransactions(int? id)
        {
            CheckingAccount checkingAccount = db.CheckingAccounts.Find(id);
            List<Transaction> transactions = checkingAccount.Transactions;
            return transactions;
        }
    }
}