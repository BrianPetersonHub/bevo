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

        public ActionResult Index()
        {
            return RedirectToAction("CheckAccounts");
        }

        // GET: Customer/Home
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

        public String GetUserName()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            return user.FirstName;
        }

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
                return Content("<script language'javascript' type = 'text/javascript'> alert('Your profile has been updated!'); window.location='../Customer/ViewInfo';</script>");
            }
            return View(vm);
        }
    }
}