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
                IRAccount ira = new IRAccount();
                ira.AccountName = "My IRA Account";
                return View(ira);
            }
            else
            {
                return Content("<script language'javascript' type = 'text/javascript'> alert('Error: You can only have one IRA.'); window.location='../Customer/Home';</script>");
            }

        }

        //POST: IRA/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IRAccountID,AccountNum,AccountName,Balance")] IRAccount irAccount)
        {
            if (ModelState.IsValid)
            {
                if (irAccount.Balance <= 0)
                {
                    return Content("<script language'javascript' type = 'text/javascript'> alert('Error: Your starting balance must be positive.'); window.location='../IRAccount/Create';</script>");
                }
                AppUser user = db.Users.Find(User.Identity.GetUserId());
                user.IRAccount = irAccount;


                //TODO: make the initial balance count as a deposit
                //Transaction t = new Transaction();

                //t.Date = DateTime.Today;
                //t.ToAccount = irAccount.AccountNum;
                //t.TransType = TransType.Deposit;
                //t.Amount = irAccount.Balance;
                //t.Description = "Initial deposit amount";
                //t.ToAccount = irAccount.AccountNum;
                //irAccount.Transactions.Add(t);

                db.SaveChanges();
                return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: You have successfully created an IRA!'); window.location='../Customer/Home';</script>");
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
            ViewBag.Balance = GetValue(id);
            ViewBag.Transactions = GetAllTransactions(id);
            return View(irAccount);
        }

        public Decimal GetValue(String id)
        {
            IRAccount irAccount = db.IRAccounts.Find(id);
            return irAccount.Balance;
        }
        public List<Transaction> GetAllTransactions(String id)
        {
            IRAccount irAccount = db.IRAccounts.Find(id);
            List<Transaction> transactions = irAccount.Transactions;
            return transactions;
        }

        //GET: IRAccount/EditName/#
        [HttpGet]
        public ActionResult EditName(String id)
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

            ViewBag.AccountID = id;
            EditAccountNameViewModel editAccountNameVM = new EditAccountNameViewModel();
            editAccountNameVM.AccountName = irAccount.AccountName;

            return View(editAccountNameVM);
        }

        //POST: IRAccount/EditName/#
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditName([Bind(Include = "AccountName")] EditAccountNameViewModel vm, String id)
        {
            if (ModelState.IsValid)
            {
                IRAccount irAccount = db.IRAccounts.Find(id);
                irAccount.AccountName = vm.AccountName;
                db.SaveChanges();
                return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: IRA name updated.'); window.location='../Customer/Home';</script>");
            }
            return View(vm);
        }
    }
}