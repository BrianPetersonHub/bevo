using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Web;
using System.Web.Mvc;
using bevo.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace bevo.Controllers
{
    public class DisputeController : Controller
    {

        private AppDbContext db = new AppDbContext();

        // GET: Dispute
        public ActionResult Create(int? id)  //this is an id for a transaction (which will also be the disputes id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }

            DisputeTransactionViewModel d = new DisputeTransactionViewModel();
            d.TransactionID = Convert.ToInt32(id);

            return View(d);
        }

        //POST: Create/Dispute
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Message,DisputedAmount,TransactionID")] DisputeTransactionViewModel dt)
        {
            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            var user = userManager.FindById(User.Identity.GetUserId());

            if (ModelState.IsValid)
            {
                Transaction transaction = db.Transactions.Find(dt.TransactionID);
                Dispute dispute = new Dispute();
                dispute.Message = dt.Message;
                dispute.DisputedAmount = dt.DisputedAmount;
                dispute.DisputeStatus = DisputeStatus.Submitted;
                dispute.Transaction = transaction;
                dispute.AppUser = user;
                db.Disputes.Add(dispute);
                db.SaveChanges();

                return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Successfully disputed transaction!'); window.location='../Customer/Home';</script>");

            }
            return View(dt);
        }

        //Make a get method for editing the dispute
        //The view for this method should be bound to the disputeviewmodel class 
        public ActionResult EditDispute(int? id)
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
                    if (dvm.Message != null)
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
                if (dvm.Status == DisputeStatus.Rejected)
                {
                    //Send an email to the user in question 
                    String bodyForEmail = null;
                    if (dvm.Message != null)
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
                if (dvm.Status == DisputeStatus.Adjusted)
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
    }
}