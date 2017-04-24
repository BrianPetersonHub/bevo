using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bevo.Models;
using System.Net;

namespace bevo.Controllers
{
    public class IRAccountController : Controller        
    {

        private AppDbContext db = new AppDbContext();

        // GET: IRAccount
        public ActionResult Index(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IRAccount customerIRA = db.IRAccounts.Find(id);
            if (customerIRA == null)
            {
                return HttpNotFound();
            }
            return View(customerIRA);
        }

        //GET IRAccount/Create
        public ActionResult Create()
        {
            return View();
        }

        //POST: IRAccount/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IRAccountID,AccountNum,AccountName,Balance")] IRAccount IRAccount)
        {
            if (ModelState.IsValid)
            {
                db.IRAccounts.Add(IRAccount);
                db.SaveChanges();
                return RedirectToAction("Home", "Customer");
            }
            return View(IRAccount);
        }

        public ActionResult Details()
        {
            return View();
        }
    }
}