using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;
using bevo.Models;

namespace bevo.Controllers
{
    public class CustomerController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Customer
        public ActionResult Home()
        {
            ViewBag.CheckingAccounts = GetAllCheckingAccts();
            return View();
        }

        public List<CheckingAccount> GetAllCheckingAccts()
        {
            var query = from c in db.CheckingAccounts
                        orderby c.AccountNum
                        select c;

            List<CheckingAccount> checkingAccts = query.ToList();
            return checkingAccts;
        }
    }
}