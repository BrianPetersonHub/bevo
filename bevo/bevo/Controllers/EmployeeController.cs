using System;
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

namespace bevo.Controllers
{
    public class EmployeeController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Employee
        public ActionResult Index()
        {
            var appUsers = db.AppUsers.Include(a => a.IRAccount).Include(a => a.StockPortfolio);
            return View(appUsers.ToList());
        }

        // GET: Employee/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUser appUser = db.AppUsers.Find(id);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            return View(appUser);
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            ViewBag.Id = new SelectList(db.IRAccounts, "IRAccountID", "AccountName");
            ViewBag.Id = new SelectList(db.StockPortfolios, "StockPortfolioID", "AccountName");
            return View();
        }

        // POST: Employee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Enabled,FirstName,MiddleInitial,LastName,Street,City,State,ZipCode,Birthday,Disabled,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                db.AppUsers.Add(appUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id = new SelectList(db.IRAccounts, "IRAccountID", "AccountName", appUser.Id);
            ViewBag.Id = new SelectList(db.StockPortfolios, "StockPortfolioID", "AccountName", appUser.Id);
            return View(appUser);
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUser appUser = db.AppUsers.Find(id);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = new SelectList(db.IRAccounts, "IRAccountID", "AccountName", appUser.Id);
            ViewBag.Id = new SelectList(db.StockPortfolios, "StockPortfolioID", "AccountName", appUser.Id);
            return View(appUser);
        }

        // POST: Employee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Enabled,FirstName,MiddleInitial,LastName,Street,City,State,ZipCode,Birthday,Disabled,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id = new SelectList(db.IRAccounts, "IRAccountID", "AccountName", appUser.Id);
            ViewBag.Id = new SelectList(db.StockPortfolios, "StockPortfolioID", "AccountName", appUser.Id);
            return View(appUser);
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUser appUser = db.AppUsers.Find(id);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            return View(appUser);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AppUser appUser = db.AppUsers.Find(id);
            db.AppUsers.Remove(appUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }









        //Go to transaction search page 
        public ActionResult GoToTransactionSearch()
        {
            return RedirectToAction("Home", "Transactions");
        }

        //Enable and disable customer accounts 
        //Make a method to get a list of all the customers and puts it in the viewbag for the freeze customer view
        public ActionResult FreezeCustomer()
        {
            ViewBag.AllCustomers = GetCustomers();
            return View();
        }

        //Make the post edit method to fire the employee
        [HttpPost]
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

            return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Successfully froze customer account!'); window.location='../Customer/Home';</script>");
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


    }
}
