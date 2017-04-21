using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Web;
using System.Web.Mvc;
using bevo.Models;


namespace bevo.Controllers
{
    public class CheckingAccountsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        //GET: CheckingAccounts/Index/#
        //TODO: How do we get here from CustomerHome page's "view account" link?
        public ActionResult Index(int? id)
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
            return View(checkingAccount);
        }

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
                //TODO: Make sure this is redirecting to the customercontroller
                return RedirectToAction("Home", "Customer");
            }
            return View(checkingAccount);
        }

        public ActionResult Details()
        {
            return View();
        }
    }
}