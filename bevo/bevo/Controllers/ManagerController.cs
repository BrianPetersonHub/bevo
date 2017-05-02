using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using bevo.Models;
using System.Collections.Generic;
using System.Net;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace bevo.Controllers
{
    public class ManagerController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Manager
        public ActionResult Home()
        {
            //Put useful information in the viewbag
            ViewBag.TransactionMasterList = GetTrMasterList();
            ViewBag.TransactionToApprove = GetTrToApprove();
            ViewBag.UnresolvedDisputes = GetUnresolvedDisputes();
            ViewBag.AllDisputes = GetAllDisputes();

            return View();
        }

        // GET: Manager/Details/5
        public ActionResult Details(string id)
        {
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

        // GET: Manager/Create
        public ActionResult Create()
        {
            ViewBag.Id = new SelectList(db.IRAccounts, "IRAccountID", "AccountName");
            ViewBag.Id = new SelectList(db.StockPortfolios, "StockPortfolioID", "AccountName");
            return View();
        }

        // POST: Manager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Enabled,FirstName,MiddleInitial,LastName,Street,City,State,ZipCode,Birthday,Active,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(appUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id = new SelectList(db.IRAccounts, "IRAccountID", "AccountName", appUser.Id);
            ViewBag.Id = new SelectList(db.StockPortfolios, "StockPortfolioID", "AccountName", appUser.Id);
            return View(appUser);
        }

        // GET: Manager/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUser appUser = db.Users.Find(id);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = new SelectList(db.IRAccounts, "IRAccountID", "AccountName", appUser.Id);
            ViewBag.Id = new SelectList(db.StockPortfolios, "StockPortfolioID", "AccountName", appUser.Id);
            return View(appUser);
        }

        // POST: Manager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Enabled,FirstName,MiddleInitial,LastName,Street,City,State,ZipCode,Birthday,Active,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] AppUser appUser)
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

        // GET: Manager/Delete/5
        public ActionResult Delete(string id)
        {
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

        // POST: Manager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AppUser appUser = db.Users.Find(id);
            db.Users.Remove(appUser);
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








        //Make a get method for editing the dispute
        //The view for this method should be bound to the disputeviewmodel class 
        public ActionResult EditDispute(Int32 id)
        {
            AppDbContext db = new AppDbContext();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Get the dispute object we're concerned with 
            var query = from d in db.Disputes
                        select d;
            query = query.Where(d => d.DisputeID == id);

            if (query == null)
            {
                return HttpNotFound();
            }
            else
            {
                Dispute theDispute = query.ToList()[0];
                //Apply this dispute's information to a dispute viewmodel which will then be passed
                //to the edit object view 

                DisputeViewModel dvm = new DisputeViewModel();
                dvm.CorrectAmount = theDispute.DisputedAmount;
                dvm.CustEmail = theDispute.AppUser.Email;
                dvm.FirstName = theDispute.AppUser.FirstName;
                dvm.LastName = theDispute.AppUser.LastName;
                dvm.Message = theDispute.Message;
                dvm.Status = theDispute.DisputeStatus;
                dvm.TransName = theDispute.Transaction.TransactionID;
                dvm.TransAmount = theDispute.Transaction.Amount;
                dvm.DisputeID = theDispute.DisputeID;

                return View(dvm);
            }
        }

        //Make a post method for editing disputes 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDispute([Bind(Include = "CorrectAmount,CustEmail,FirstName,LastName,Message,Status,TransName,TransAmount,DisputeID")] DisputeViewModel dvm, Decimal? adjustedAmount)
        {
            AppDbContext db = new AppDbContext();

            if (ModelState.IsValid)
            {
                Dispute disToChange = db.Disputes.Find(dvm.DisputeID);
                Transaction transToChange = db.Transactions.Find(dvm.TransName);

                //get user currently logged in 
                UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
                var user = userManager.FindById(User.Identity.GetUserId());

                //Make appropriate changes to the dispute and transaction in quetion 
                //based on whether the dispute was accepted, rejected, or adjusted
                if (dvm.Status == DisputeStatus.Accepted)
                {
                    disToChange.DisputeStatus = dvm.Status;
                    if(dvm.Message != null)
                    {
                        disToChange.Message = disToChange.Message + "\n" + dvm.Message;
                    }
                    transToChange.Amount = dvm.CorrectAmount;
                    transToChange.Description = "Dispute [Accepted] " + transToChange.Description;
                    disToChange.ManResolvedEmail = user.Email;

                    db.Entry(disToChange).State = EntityState.Modified;
                    db.Entry(transToChange).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Home");
                }
                if(dvm.Status == DisputeStatus.Rejected)
                {
                    //Send an email to the user in question 
                    String bodyForEmail = null;
                    if(dvm.Message != null)
                    {
                        bodyForEmail = "Your dispute on transaction number " + transToChange.TransactionID + " has been rejected."
                       + "Additional messages from the manager who resolved your dispute: " + "\n" + dvm.Message;
                    }
                    else
                    {
                        bodyForEmail = "Your dispute on transaction number " + transToChange.TransactionID + " has been rejected.";
                    }

                    bevo.Messaging.EmailMessaging.SendEmail(disToChange.AppUser.Email, "Dispute Rejected", bodyForEmail);

                    //Change the appropriate information and save it to the DB
                    disToChange.DisputeStatus = dvm.Status;
                    transToChange.Description = "Dispute [Rejected] " + transToChange.Description;
                    db.Entry(disToChange).State = EntityState.Modified;
                    db.Entry(transToChange).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Home");
                }
                if(dvm.Status == DisputeStatus.Adjusted)
                {
                    disToChange.DisputeStatus = dvm.Status;
                    if (dvm.Message != null)
                    {
                        disToChange.Message = disToChange.Message + "\n" + dvm.Message;
                    }
                    transToChange.Amount = (decimal)adjustedAmount;
                    transToChange.Description = "Dispute [Accepted] " + transToChange.Description;
                    disToChange.ManResolvedEmail = user.Email;

                    db.Entry(disToChange).State = EntityState.Modified;
                    db.Entry(transToChange).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Home");
                }


            }
            return View(dvm);
        }


        //public ActionResult PromoteEmployee












        


        //Get a list of all transactions 
        public List<Transaction> GetTrMasterList()
        {
            AppDbContext db = new AppDbContext();

            List<Transaction> returnList = db.Transactions.ToList();

            return returnList;
        }

        //Get a list of all transactions requiring manager approval
        public List<Transaction> GetTrToApprove()
        {
            AppDbContext db = new AppDbContext();

            List<Transaction> returnList = new List<Transaction>();
            var query = from t in db.Transactions
                        select t;
            query = query.Where(t => t.NeedsApproval == true);
            returnList = query.ToList();

            return returnList;
        }

        public List<DisputeViewModel> GetUnresolvedDisputes()
        {
            AppDbContext db = new AppDbContext();

            List<Dispute> disputeList = new List<Dispute>();
            var query = from d in db.Disputes
                        select d;
            query = query.Where(d => d.DisputeStatus == DisputeStatus.Submitted);
            disputeList = query.ToList();

            List<DisputeViewModel> dvmList = new List<DisputeViewModel>();
            foreach (Dispute d in disputeList)
            {
                DisputeViewModel dvm = new DisputeViewModel();
                dvm.CorrectAmount = d.DisputedAmount;
                dvm.FirstName = d.AppUser.FirstName;
                dvm.LastName = d.AppUser.LastName;
                dvm.TransAmount = d.Transaction.Amount;
                dvm.Message = d.Message;
                dvm.CustEmail = d.AppUser.Email;
                dvm.TransName = d.Transaction.TransactionID;
                dvm.DisputeID = d.DisputeID;
                dvm.Status = DisputeStatus.Submitted;
                dvmList.Add(dvm);
            }


            return dvmList;
        }

        public List<DisputeViewModel> GetAllDisputes()
        {
            AppDbContext db = new AppDbContext();

            List<Dispute> disputeList = db.Disputes.ToList();

            List<DisputeViewModel> dvmList = new List<DisputeViewModel>();
            foreach (Dispute d in disputeList)
            {
                DisputeViewModel dvm = new DisputeViewModel();
                dvm.CorrectAmount = d.DisputedAmount;
                dvm.FirstName = d.AppUser.FirstName;
                dvm.LastName = d.AppUser.LastName;
                dvm.TransAmount = d.Transaction.Amount;
                dvm.Message = d.Message;
                dvm.CustEmail = d.AppUser.Email;
                dvm.TransName = d.Transaction.TransactionID;
                dvm.DisputeID = d.DisputeID;
                dvm.Status = d.DisputeStatus;

                dvmList.Add(dvm);
            }

            return dvmList;
        }

        //public List<AppUser> GetEmployees()
        //{
        //    AppDbContext db = new AppDbContext();

        //    var query = from e in db.Users
        //                select e;
        //    query = query.Where(e => e.Roles == );


        //}


    }
}
