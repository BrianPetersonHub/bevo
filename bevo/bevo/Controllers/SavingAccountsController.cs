using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bevo.Models;
using bevo.DAL;

namespace bevo.Controllers
{
    public class SavingsAccountController : Controller
    {
        private AppDbContext db = new AppDbContext();

        //GET: SavingsAccount/Create
        public ActionResult Create()
        {
            return View();
        }

        //POST: SavingAccount/Create
        //TODO: look at if the way the correct acctnum is added is correct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SavingAccountID,AccountNum,AccountName,Balance")] SavingAccount savingAccount)
        {
            if (ModelState.IsValid)
            {
                db.SavingAccounts.Add(savingAccount);
                db.SaveChanges();
                return RedirectToAction("CustomerHome", "PersonsController");
            }
            return View(savingAccount);
        }
    }
}