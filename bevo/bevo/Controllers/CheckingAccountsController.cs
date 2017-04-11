using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bevo.Models;


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
                db.CheckingAccounts.Add(checkingAccount);
                db.SaveChanges();
                return RedirectToAction("CustomerHome", "PersonsController");
            }
            return View(checkingAccount);
        }
    }
}