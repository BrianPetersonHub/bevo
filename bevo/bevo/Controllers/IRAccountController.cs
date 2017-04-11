using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bevo.Models;

namespace bevo.Controllers
{
    public class IRAccountController : Controller        
    {

        private AppDbContext db = new AppDbContext();

        // GET: IRAccount
        public ActionResult Index()
        {
            return View();
        }

        //POST: IRAccount/Create
        //TODO: look at if the way the correct acctnum is added is correct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CheckingAccountID,AccountNum,AccountName,Balance")] IRAccount IRAccount)
        {
            if (ModelState.IsValid)
            {
                db.IRAccounts.Add(IRAccount);
                db.SaveChanges();
                return RedirectToAction("CustomerHome", "PersonsController");
            }
            return View(IRAccount);
        }
    }
}