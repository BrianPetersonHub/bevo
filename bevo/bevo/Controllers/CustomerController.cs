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

        // GET: Customer/Home
        public ActionResult Home()
        {
            ViewBag.CurrentUser = db.Users.Find(User.Identity.GetUserId());
            ViewBag.CheckingAccounts = GetAllCheckingAccts();
            ViewBag.SavingAccounts = GetAllSavingAccts();
            ViewBag.IRAccount = GetIRAccount();
            ViewBag.StockPortfolio = GetStockPortfolio();
            return View();
        }

        public bool OverdraftStatus()
        {
            bool isOverdraft = true;
            // add logic to check if checkings or savings is overdrafted 
            return isOverdraft;
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
        public IRAccount GetIRAccount()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            IRAccount irAccount = user.IRAccount;
            return irAccount;
        }
        public StockPortfolio GetStockPortfolio()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            StockPortfolio stockPortfolio = user.StockPortfolio;
            return stockPortfolio;
        }

        //GET: Customer/ManageAccount
        public ActionResult ViewInfo()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            return View(user);
        }

        //GET: Customer/Edit
        public ActionResult EditInfo()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            EditUserViewModel editUserVM = new EditUserViewModel();
            editUserVM.FirstName = user.FirstName;
            editUserVM.MiddleInitial = user.MiddleInitial;
            editUserVM.LastName = user.LastName;
            editUserVM.Street = user.Street;
            editUserVM.City = user.City;
            editUserVM.State = user.State;
            editUserVM.ZipCode = user.ZipCode;
            editUserVM.Birthday = user.Birthday;
            editUserVM.Email = user.Email;
            editUserVM.PhoneNumber = user.PhoneNumber;
            return View(editUserVM);
        }

        //GET: Customer/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditInfo([Bind(Include = "FirstName,MiddleInitial,LastName,Street,City,State,ZipCode,Birthday,Email,PhoneNumber")] EditUserViewModel vm)
        {
            if (ModelState.IsValid)
            {
                AppUser user = db.Users.Find(User.Identity.GetUserId());
                user.FirstName = vm.FirstName;
                user.MiddleInitial = vm.MiddleInitial;
                user.LastName = vm.LastName;
                user.Street = vm.Street;
                user.City = vm.City;
                user.State = vm.State;
                user.ZipCode = vm.ZipCode;
                user.Birthday = vm.Birthday;
                user.Email = vm.Email;
                user.PhoneNumber = vm.PhoneNumber;
                db.SaveChanges();
                return RedirectToAction("ViewInfo");
            }
            return View(vm);
        }
    }
}