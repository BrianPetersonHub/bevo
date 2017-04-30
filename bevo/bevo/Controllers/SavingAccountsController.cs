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
            return View();
        }

        //POST: SavingAccount/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SavingAccountID,AccountNum,AccountName,Balance")] SavingAccount savingAccount)
        {
            if (ModelState.IsValid)
            {
                AppUser user = db.Users.Find(User.Identity.GetUserId());
                if (savingAccount.AccountName == null)
                {
                    savingAccount.AccountName = "Longhorn Saving";
                }
                user.SavingAccounts.Add(savingAccount);
                db.SaveChanges();
                return RedirectToAction("Home", "Customer");
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
            ViewBag.Transactions = GetAllTransactions(id);
            return View(savingAccount);
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
                return RedirectToAction("Details", new { id = id });
            }
            return View(vm);
        }
    }
}