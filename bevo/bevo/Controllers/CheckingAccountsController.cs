﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bevo.Models;
using bevo.DAL;


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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CheckingAccountID,AccountNum,AccountName,Balance")] CheckingAccount checkingAccount)
        {
            if (ModelState.IsValid)
            {
                db.CheckingAccounts.Add(checkingAccount);
                db.SaveChanges();
                //TODO: create CustomerHome ActionResult and View (might be in person)
                return RedirectToAction("CustomerHome");
            }
            return View(checkingAccount);
        }

        public Int32 CountAccounts()
        {
            Int32 intCount = 1000000000;
            intCount += db.CheckingAccounts.Count();
            intCount += db.SavingAccounts.Count();
            intCount += db.IRAccounts.Count();
            intCount += db.StockPortfolio.Count();
            return intCount;
        }
    }
}