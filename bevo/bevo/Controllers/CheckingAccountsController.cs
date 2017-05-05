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
        [Authorize]
        public ActionResult Create()
        {
            if (db.Users.Find(User.Identity.GetUserId()).Disabled == true)
            {
                return Content("<script language'javascript' type = 'text/javascript'> alert('Access Denied: Your account has been disabled. You are in a view-only mode.'); window.location='../Customer/Home';</script>");
            }

            CheckingAccount ca = new CheckingAccount();
            ca.AccountName = "Longhorn Checking";
            return View(ca);
            Int32 myint = 1000;
            String mystring = myint.ToString();
        }

        //POST: CheckingAccount/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CheckingAccountID,AccountNum,AccountName,Balance")] CheckingAccount checkingAccount)
        {
            if (ModelState.IsValid)
            {
                if (checkingAccount.Balance < 0)
                {
                    return Content("<script language'javascript' type = 'text/javascript'> alert('Error: Your starting balance must be positive.'); window.location='../CheckingAccounts/Create';</script>");
                }
                AppUser user = db.Users.Find(User.Identity.GetUserId());
                if (checkingAccount.AccountName == null)
                {
                    checkingAccount.AccountName = "Longhorn Checking";
                }

                Transaction t = new Transaction();
                t.Date = DateTime.Today;
                t.ToAccount = checkingAccount.AccountNum;
                t.TransType = TransType.Deposit;
                t.Description = "Initial deposit";
                t.Amount = checkingAccount.Balance;
                if (checkingAccount.Balance > 5000)
                {
                    checkingAccount.Balance = 0;
                    t.NeedsApproval = true;
                }

                checkingAccount.Transactions = new List<Transaction>();
                checkingAccount.Transactions.Add(t);
                user.CheckingAccounts.Add(checkingAccount);
                db.SaveChanges();

                return Content("<script language'javascript' type = 'text/javascript'> alert('You have successfully created a new checking account!'); window.location='../Customer/Home';</script>");
            }
            return View(checkingAccount);
        }

        //GET: CheckingAccounts/Details/#
        [Authorize]
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
            ViewBag.PendingTransactions = GetPendingTransactions(id);
            ViewBag.Balance = GetValue(id);
            ViewBag.Transactions = GetAllTransactions(id);
            return View(checkingAccount);
        }

        //GET: CheckingAccounts/EditName/#
        [HttpGet]
        [Authorize]
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
            CheckingAccount checkingAccount = db.CheckingAccounts.Find(id);
            if (checkingAccount == null)
            {
                return HttpNotFound();
            }

            ViewBag.AccountID = id;
            EditAccountNameViewModel editAccountNameVM = new EditAccountNameViewModel();
            editAccountNameVM.AccountName = checkingAccount.AccountName;

            return View(editAccountNameVM);
        }

        //POST: SavingAccounts/EditName/#
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult EditName([Bind(Include = "AccountName")] EditAccountNameViewModel vm, int? id)
        {
            if (ModelState.IsValid)
            {
                CheckingAccount checkingAccount = db.CheckingAccounts.Find(id);
                checkingAccount.AccountName = vm.AccountName;
                db.SaveChanges();
                return Content("<script language'javascript' type = 'text/javascript'> alert('Your checking account name has been updated'); window.location='../../Customer/Home';</script>");
            }
            return View(vm);
        }


        [Authorize]
        public List<Transaction> GetAllTransactions(int? id)
        {
            CheckingAccount checkingAccount = db.CheckingAccounts.Find(id);
            List<Transaction> transactions = new List<Transaction>();
            foreach (Transaction t in checkingAccount.Transactions)
            {
                if (t.NeedsApproval != true)
                {
                    transactions.Add(t);
                }
            }
            return transactions;
        }

        [Authorize]
        public Decimal GetValue(int? id)
        {
            CheckingAccount checkingAccount = db.CheckingAccounts.Find(id);
            return checkingAccount.Balance;
        }

        [Authorize]
        public List<Transaction> GetPendingTransactions(int? id)
        {
            CheckingAccount checkingAccount = db.CheckingAccounts.Find(id);
            List<Transaction> transactions = new List<Transaction>();
            foreach (Transaction t in checkingAccount.Transactions)
            {
                if (t.NeedsApproval == true)
                {
                    transactions.Add(t);
                }
            }
            return transactions;
        }

    }
}