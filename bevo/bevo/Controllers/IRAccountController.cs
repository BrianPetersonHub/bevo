using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Web;
using System.Web.Mvc;
using bevo.Models;
using Microsoft.AspNet.Identity;
using bevo.Controllers;

namespace bevo.Controllers
{
    public class IRAccountController : Controller
    {
        private AppDbContext db = new AppDbContext();

        //GET: IRA/Create
        public ActionResult Create()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.IRAccount == null)
            {
                return View();
            }
            else
            {
                return View("CannotCreate");
            }

        }

        //POST: IRA/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IRAccountID,AccountNum,AccountName,Balance")] IRAccount irAccount)
        {
            if (ModelState.IsValid)
            {
                AppUser user = db.Users.Find(User.Identity.GetUserId());
                user.IRAccount = irAccount;
                db.SaveChanges();
                return RedirectToAction("Home", "Customer");
            }
            return View(irAccount);
        }

        //GET: IRA/Details/#
        public ActionResult Details(String id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IRAccount irAccount = db.IRAccounts.Find(id);
            if (irAccount == null)
            {
                return HttpNotFound();
            }
            ViewBag.Transactions = GetAllTransactions(id);
            return View(irAccount);
        }

        public List<Transaction> GetAllTransactions(String id)
        {
            IRAccount irAccount = db.IRAccounts.Find(id);
            List<Transaction> transactions = irAccount.Transactions;
            return transactions;
        }

        public ActionResult DepositAgeError()
        {
            return View();
        }
        public ActionResult TransferAgeError()
        {
            return View();
        }

        public ActionResult MaxDepositError()
        {
            return View();
        }

        //for transfers going into account
        public ActionResult TransferLimitError()
        {
            return View();
        }

        public ActionResult WithdrawAgeAmountError()
        {
            return View();
        }

        //for transfers going out of account
        public ActionResult TransferAgeAmountError()
        {
            return View();
        }


    }
}