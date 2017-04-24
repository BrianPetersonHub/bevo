using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;
using bevo.Models;
using Microsoft.AspNet.Identity;

namespace bevo.Controllers
{
    public class CustomerController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Customer
        public ActionResult Home()
        {
            ViewBag.CheckingAccounts = GetAllCheckingAccts();
            ViewBag.SavingAccounts = GetAllSavingAccts();
            return View();
        }

        public List<CheckingAccount> GetAllCheckingAccts()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            List<CheckingAccount> checkingAccounts = user.CheckingAccounts;
            return checkingAccounts;
        }
        public List<SavingAccount> GetAllSavingAccts()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            List<SavingAccount> savingAccounts = user.SavingAccounts;
            return savingAccounts;
        }
    }
}