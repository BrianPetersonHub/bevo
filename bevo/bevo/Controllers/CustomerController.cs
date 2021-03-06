﻿using System;
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

        [Authorize]
        public ActionResult Index()
        {
            if (User.IsInRole("Manager"))
            {
                return RedirectToAction("Home", "Manager");
            }
            else if (User.IsInRole("Employee"))
            {
                return RedirectToAction("Home", "Employee");
            }
            return RedirectToAction("CheckAccounts");
        }

        // GET: Customer/Home
        [Authorize]
        public ActionResult Home()
        {

            ViewBag.OverdraftStatus = OverdraftStatus();
            ViewBag.Name = GetUserName();
            ViewBag.CurrentUser = db.Users.Find(User.Identity.GetUserId());
            ViewBag.CheckingAccounts = GetAllCheckingAccts();
            ViewBag.SavingAccounts = GetAllSavingAccts();
            ViewBag.IRAccount = GetIRAccount();
            ViewBag.StockPortfolio = GetStockPortfolio();
            return View();
        }

        [Authorize]
        public ActionResult CheckAccounts()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            List<CheckingAccount> checkingAccounts = user.CheckingAccounts.ToList();
            List<SavingAccount> savingAccounts = user.SavingAccounts.ToList();
            IRAccount irAccount = user.IRAccount;
            StockPortfolio stockPortfolio = user.StockPortfolio;

            if (checkingAccounts.Count() == 0 && savingAccounts.Count() == 0 && irAccount == null && stockPortfolio == null)
            {
                return RedirectToAction("ChooseAccount", "Account");
            }

            else
                return RedirectToAction("Home");
        }

        [Authorize]
        public String GetUserName()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            return user.FirstName;
        }

        [Authorize]
        public bool OverdraftStatus()
        {   
            // get all accounts
            // check if overdrafted 
            bool isOverdraft = false;

            List<CheckingAccount> checkingAccounts = GetAllCheckingAccts();
            foreach (CheckingAccount c in checkingAccounts)
            {
                if (c.Balance < 0)
                { isOverdraft = true;  }
            }

            List<SavingAccount> savingAccounts = GetAllSavingAccts();
            foreach (SavingAccount s in savingAccounts)
            {
                if (s.Balance < 0)
                { isOverdraft = true; }
            }

            IRAccount irAccount = GetIRAccount();
            if (irAccount != null)
            {
                if (irAccount.Balance < 0)
                {
                    isOverdraft = true;
                }
            }

            StockPortfolio portfolio = GetStockPortfolio();
            if(portfolio != null)
            {
                if(portfolio.Balance < 0)
                {
                    isOverdraft = true;
                }
            }
            

            return isOverdraft;
        }

        [Authorize]
        public List<CheckingAccount> GetAllCheckingAccts()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            List<CheckingAccount> checkingAccounts = user.CheckingAccounts;
            return checkingAccounts;
        }

        [Authorize]
        public List<SavingAccount> GetAllSavingAccts()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            List<SavingAccount> savingAccounts = user.SavingAccounts;
            return savingAccounts;
        }

        [Authorize]
        public IRAccount GetIRAccount()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            IRAccount irAccount = user.IRAccount;
            return irAccount;
        }

        [Authorize]
        public StockPortfolio GetStockPortfolio()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            StockPortfolio stockPortfolio = user.StockPortfolio;
            return stockPortfolio;
        }

        //GET: Customer/ManageAccount
        [Authorize]
        public ActionResult ViewInfo()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            return View(user);
        }

        //GET: Customer/Edit
        [Authorize]
        public ActionResult EditInfo()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());

            if(user.Disabled == true)
            {
                return Content("<script language'javascript' type = 'text/javascript'> alert('Access Denied: Your account has been disabled. You are in a view-only mode.'); window.location='../Customer/Home';</script>");
            }

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
        [Authorize]
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
                return Content("<script language'javascript' type = 'text/javascript'> alert('Your profile has been updated!'); window.location='../Customer/ViewInfo';</script>");
            }
            return View(vm);
        }
    }
}