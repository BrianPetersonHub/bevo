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
            Int32 myint = 1000;
            String mystring = myint.ToString();
        }

        //POST: ChechingAccount/Create
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
            ViewBag.Balance = GetValue(id);
            ViewBag.Transactions = GetAllTransactions(id);
            return View(checkingAccount);
        }

        //GET: CheckingAccounts/EditName/#
        [HttpGet]
        public ActionResult EditName(int? id)
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

            ViewBag.AccountID = id;
            EditAccountNameViewModel editAccountNameVM = new EditAccountNameViewModel();
            editAccountNameVM.AccountName = checkingAccount.AccountName;

            return View(editAccountNameVM);
        }

        //POST: SavingAccounts/EditName/#
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditName([Bind(Include = "AccountName")] EditAccountNameViewModel vm, int? id)
        {
            if (ModelState.IsValid)
            {
                CheckingAccount checkingAccount = db.CheckingAccounts.Find(id);
                checkingAccount.AccountName = vm.AccountName;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = id });
            }
            return View(vm);
        }


        public List<Transaction> GetAllTransactions(int? id)
        {
            CheckingAccount checkingAccount = db.CheckingAccounts.Find(id);
            List<Transaction> transactions = checkingAccount.Transactions;
            return transactions;
        }

        public Decimal GetValue(int? id)
        {
            CheckingAccount checkingAccount = db.CheckingAccounts.Find(id);
            return checkingAccount.Balance;
        }


    }
}