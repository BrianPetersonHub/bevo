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
    public class SavingAccountsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        //GET: SavingsAccount/Create
        public ActionResult Create()
        {
            if (db.Users.Find(User.Identity.GetUserId()).Disabled == true)
            {
                return Content("<script language'javascript' type = 'text/javascript'> alert('Access Denied: Your account has been disabled. You are in a view-only mode.'); window.location='../Customer/Home';</script>");
            }

            SavingAccount sa = new SavingAccount();
            sa.AccountName = "Longhorn Savings";
            return View(sa);
        }

        //POST: SavingAccount/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SavingAccountID,AccountNum,AccountName,Balance")] SavingAccount savingAccount)
        {
            if (ModelState.IsValid)
            {
                if (savingAccount.Balance <= 0)
                {
                    return Content("<script language'javascript' type = 'text/javascript'> alert('Error: Your starting balance must be positive.'); window.location='../SavingAccounts/Create';</script>");
                }
                AppUser user = db.Users.Find(User.Identity.GetUserId());
                if (savingAccount.AccountName == null)
                {
                    savingAccount.AccountName = "Longhorn Saving";
                }

                Transaction t = new Transaction();
                t.Date = DateTime.Today;
                t.ToAccount = savingAccount.AccountNum;
                t.TransType = TransType.Deposit;
                t.Description = "Initial deposit";
                t.Amount = savingAccount.Balance;
                if (savingAccount.Balance > 5000)
                {
                    savingAccount.Balance = 0;
                    t.NeedsApproval = true;
                }

                savingAccount.Transactions = new List<Transaction>();
                savingAccount.Transactions.Add(t);

                user.SavingAccounts.Add(savingAccount);
                db.SaveChanges();
                return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Successfully added new Savings Account!'); window.location='../Customer/Home';</script>");
            }
            return View(savingAccount);
        }

        //GET: SavingAccounts/Details/#
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SavingAccount savingAccount = db.SavingAccounts.Find(id);
            if (savingAccount == null)
            {
                return HttpNotFound();
            }
            ViewBag.Balance = GetValue(id);
            ViewBag.Transactions = GetAllTransactions(id);
            return View(savingAccount);
        }

        public Decimal GetValue(int? id)
        {
            SavingAccount savingAccount = db.SavingAccounts.Find(id);
            return savingAccount.Balance;
        }
        public List<Transaction> GetAllTransactions(int? id)
        {
            SavingAccount savingAccount = db.SavingAccounts.Find(id);
            List<Transaction> transactions = savingAccount.Transactions;
            return transactions;
        }

        //GET: SavingAccounts/EditName/#
        [HttpGet]
        public ActionResult EditName(int? id)
        {
            if (db.Users.Find(User.Identity.GetUserId()).Disabled == true)
            {
                return Content("<script language'javascript' type = 'text/javascript'> alert('Access Denied: Your account has been disabled. You are in a view-only mode.'); window.location='../Customer/Home';</script>");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SavingAccount savingAccount = db.SavingAccounts.Find(id);
            if (savingAccount == null)
            {
                return HttpNotFound();
            }

            ViewBag.AccountID = id;
            EditAccountNameViewModel editAccountNameVM = new EditAccountNameViewModel();
            editAccountNameVM.AccountName = savingAccount.AccountName;

            return View(editAccountNameVM);
        }

        //POST: SavingAccounts/EditName/#
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditName([Bind(Include = "AccountName")] EditAccountNameViewModel vm, int? id)
        {
            if (ModelState.IsValid)
            {
                SavingAccount savingAccount = db.SavingAccounts.Find(id);
                savingAccount.AccountName = vm.AccountName;
                db.SaveChanges();
                return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Successfully upadated savings account name!'); window.location='../Customer/Home';</script>");
            }
            return View(vm);
        }
    }
}