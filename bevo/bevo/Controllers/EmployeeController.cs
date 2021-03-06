﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using bevo.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.Owin;

namespace bevo.Controllers
{
    public class EmployeeController : Controller
    {
        private AppDbContext db = new AppDbContext();

        [Authorize(Roles = "Employee")]
        // GET: Employee
        public ActionResult Home()
        {
            if (db.Users.Find(User.Identity.GetUserId()).Disabled == true)
            {
                return Content("<script language'javascript' type = 'text/javascript'> alert('Access Denied: You have been terminated. Hasta la vista.'); window.location='../Home/Index';</script>");
            }

            return View();
        }

        [Authorize(Roles = "Employee")]
        // GET: Employee/Details/5
        public ActionResult Details(string id)
        {
            if (db.Users.Find(User.Identity.GetUserId()).Disabled == true)
            {
                return Content("<script language'javascript' type = 'text/javascript'> alert('Access Denied: You have been terminated. Hasta la vista.'); window.location='../Home/Index';</script>");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUser appUser = db.Users.Find(id);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            return View(appUser);
        }

        [Authorize(Roles = "Employee")]
        // GET: Employee/Edit/5
        public ActionResult Edit(string id)
        {
            if (db.Users.Find(User.Identity.GetUserId()).Disabled == true)
            {
                return Content("<script language'javascript' type = 'text/javascript'> alert('Access Denied: You have been terminated. Hasta la vista.'); window.location='../Home/Index';</script>");
            }

            AppUser user = db.Users.Find(User.Identity.GetUserId());
            EditEmployeeViewModel eevm = new EditEmployeeViewModel();
            eevm.City = user.City;
            eevm.Email = user.Email;
            eevm.PhoneNumber = user.PhoneNumber;
            eevm.State = user.State;
            eevm.Street = user.Street;
            eevm.ZipCode = user.ZipCode;

            return View(eevm);
        }

        // POST: Employee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Employee")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Street,City,State,ZipCode,Email,PhoneNumber")] EditEmployeeViewModel eevm)
        {
            if (ModelState.IsValid)
            {
                //Get the db record for the user who is logged in 
                AppUser user = db.Users.Find(User.Identity.GetUserId());

                user.Street = eevm.Street;
                user.City = eevm.City;
                user.State = eevm.State;
                user.ZipCode = eevm.ZipCode;
                user.Email = eevm.Email;
                user.PhoneNumber = eevm.PhoneNumber;

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Home");
            }
            return View(eevm);
        }

        [Authorize(Roles = "Employee")]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }









        //Go to transaction search page 
        [Authorize(Roles = "Employee")]
        public ActionResult GoToTransactionSearch()
        {
            if (db.Users.Find(User.Identity.GetUserId()).Disabled == true)
            {
                return Content("<script language'javascript' type = 'text/javascript'> alert('Access Denied: You have been terminated. Hasta la vista.'); window.location='../Home/Index';</script>");
            }

            return RedirectToAction("Home", "Transactions");
        }

        [Authorize(Roles = "Employee")]
        //Enable and disable customer accounts 
        //Make a method to get a list of all the customers and puts it in the viewbag for the freeze customer view
        public ActionResult FreezeCustomer()
        {
            if (db.Users.Find(User.Identity.GetUserId()).Disabled == true)
            {
                return Content("<script language'javascript' type = 'text/javascript'> alert('Access Denied: You have been terminated. Hasta la vista.'); window.location='../Home/Index';</script>");
            }

            ViewBag.AllCustomers = GetCustomers();
            ViewBag.SelectCustomer = SelectCustomer();
            List<AppUser> customers = GetCustomers();
            return View(customers);
        }

        //Make the post edit method to freeze customer account
        [HttpPost]
        [Authorize(Roles = "Employee")]
        [ValidateAntiForgeryToken]
        public ActionResult FreezeCustomer(String id)
        {
            AppDbContext db = new AppDbContext();

            //Get the user we want 
            var query = from user in db.Users
                        select user;
            query = query.Where(user => user.Id == id);
            List<AppUser> queryList = query.ToList();
            AppUser userInQuestion = queryList[0];

            //change the user's disabled variable to true 
            userInQuestion.Disabled = true;

            //Save Changes
            db.Entry(userInQuestion).State = EntityState.Modified;
            db.SaveChanges();

            return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Successfully froze customer account!'); window.location='../Employee/Home';</script>");
        }

        //Get method to reactivate customer accounts 
        [Authorize(Roles = "Employee")]
        public ActionResult ReactivateCustomer()
        {
            if (db.Users.Find(User.Identity.GetUserId()).Disabled == true)
            {
                return Content("<script language'javascript' type = 'text/javascript'> alert('Access Denied: You have been terminated. Hasta la vista.'); window.location='../Home/Index';</script>");
            }

            ViewBag.SelectCustomers = SelectDisabledCustomer();
            ViewBag.AllCustomers = GetDisabledCustomers();
            List<AppUser> customers = GetDisabledCustomers();
            return View(customers);
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        [ValidateAntiForgeryToken]
        public ActionResult ReactivateCustomer(String id)
        {
            AppDbContext db = new AppDbContext();

            //Get the user we want 
            var query = from user in db.Users
                        select user;
            query = query.Where(user => user.Id == id);
            List<AppUser> queryList = query.ToList();
            AppUser userInQuestion = queryList[0];

            //change the user's disabled variable to true 
            userInQuestion.Disabled = false;

            //Save Changes
            db.Entry(userInQuestion).State = EntityState.Modified;
            db.SaveChanges();

            return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Successfully reactivated customer account!'); window.location='../Employee/Home';</script>");

        }



        //Go to the view for selecting which customer you want to change the password for 
        [Authorize(Roles = "Employee")]
        public ActionResult ChangeCustomerPassword()
        {
            if (db.Users.Find(User.Identity.GetUserId()).Disabled == true)
            {
                return Content("<script language'javascript' type = 'text/javascript'> alert('Access Denied: You have been terminated. Hasta la vista.'); window.location='../Home/Index';</script>");
            }

            ViewBag.AllCustomers = GetCustomers();
            ViewBag.SelectCustomer = SelectCustomer();

            return View();
        }

        //Post method for changing a customer's password 
        [HttpPost]
        [Authorize(Roles = "Employee")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeCustomerPassword(String id, String newPassword)
        {
            AppDbContext db = new AppDbContext();
            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));

            //Get the user we want 
            var query = from user in db.Users
                        select user;
            query = query.Where(user => user.Id == id);
            List<AppUser> queryList = query.ToList();
            AppUser userInQuestion = queryList[0];

            //Stuff from stackoverflow
            var provider = new DpapiDataProtectionProvider("Sample");
            userManager.UserTokenProvider = new DataProtectorTokenProvider<AppUser>(provider.Create("GeneratePassword"));

            String resetToken = userManager.GeneratePasswordResetToken(id);
            userManager.ResetPassword(id, resetToken, newPassword);

            db.Entry(userInQuestion).State = EntityState.Modified;
            db.SaveChanges();

            return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Successfully changed customer password!'); window.location='../Employee/Home';</script>");
        }

        //Go to page to edit a customer's account 
        [Authorize(Roles = "Employee")]
        public ActionResult ChangeCustomerInfo()
        {
            if (db.Users.Find(User.Identity.GetUserId()).Disabled == true)
            {
                return Content("<script language'javascript' type = 'text/javascript'> alert('Access Denied: You have been terminated. Hasta la vista.'); window.location='../Home/Index';</script>");
            }

            ViewBag.AllCustomers = GetCustomers();
            ViewBag.SelectCustomer = SelectCustomer();
            List<AppUser> customers = GetCustomers();
            return View(customers);
        }

        [Authorize(Roles = "Employee")]
        public ActionResult EditCustomerInfo(string id)
        {
            AppUser user = db.Users.Find(id);
            if (id == null )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EditUserViewModel edvm = new EditUserViewModel();
            edvm.FirstName = user.FirstName;
            edvm.LastName = user.LastName;
            edvm.MiddleInitial = user.MiddleInitial;
            edvm.PhoneNumber = user.PhoneNumber;
            edvm.Birthday = user.Birthday;
            edvm.City = user.City;
            edvm.State = user.State;
            edvm.Street = user.Street;
            edvm.ZipCode = user.ZipCode;
            edvm.Email = user.Email;
            return View(edvm);
        }

        //Post method for editing the customer's account 
        [HttpPost]
        [Authorize(Roles = "Employee")]
        [ValidateAntiForgeryToken]
        public ActionResult EditCustomerInfo([Bind(Include = "Id,FirstName,MiddleInitial,LastName,Street,City,State,ZipCode,Birthday,Email,PhoneNumber")] EditUserViewModel evm)
        {
            AppUser user = db.Users.Find(evm.Id);

            if(ModelState.IsValid)
            {
                user.Birthday = evm.Birthday;
                user.City = evm.City;
                user.Email = evm.Email;
                user.FirstName = evm.FirstName;
                user.LastName = evm.LastName;
                user.MiddleInitial = evm.MiddleInitial;
                user.PhoneNumber = evm.PhoneNumber;
                user.State = evm.State;
                user.Street = evm.Street;
                user.ZipCode = evm.ZipCode;

                db.SaveChanges();
                return Content("<script language'javascript' type = 'text/javascript'> alert('Successfully updated customer info!'); window.location='../../Employee/Home';</script>");

            }

            return View(evm);

        }









        public List<AppUser> GetDisabledCustomers()
        {
            AppDbContext db = new AppDbContext();

            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            List<AppUser> customerList = new List<AppUser>();

            foreach (AppUser user in db.Users)
            {
                if (userManager.GetRoles(user.Id).Contains("Customer") && user.Disabled == true)
                {
                    customerList.Add(user);
                }
            }


            return customerList;
        }

        public IEnumerable<SelectListItem> SelectDisabledCustomer()
        {
            List<AppUser> customers = GetDisabledCustomers();
            SelectList selectCustomer = new SelectList(customers, "Id", "Email");
            return selectCustomer;
        }


        //Get list of customers from the db 
        public List<AppUser> GetCustomers()
        {
            AppDbContext db = new AppDbContext();

            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            List<AppUser> customerList = new List<AppUser>();

            foreach (AppUser user in db.Users)
            {
                if (userManager.GetRoles(user.Id).Contains("Customer"))
                {
                    customerList.Add(user);
                }
            }


            return customerList;
        }

        //Make a select list for all of the customeres an employee could choose 
        public IEnumerable<SelectListItem> SelectCustomer()
        {
            List<AppUser> customers = GetCustomers();
            SelectList selectCustomer = new SelectList(customers, "Id", "Email");
            return selectCustomer;
        }


    }
}
