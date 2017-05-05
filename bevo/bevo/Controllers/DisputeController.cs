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
        [Authorize(Roles = "Customer")]
        public ActionResult Create(int? id)  //this is an id for a transaction (which will also be the disputes id)
        {
            if (db.Users.Find(User.Identity.GetUserId()).Disabled == true)
            {
                return Content("<script language'javascript' type = 'text/javascript'> alert('Access Denied: Your account has been disabled. You are in a view-only mode.'); window.location='../Customer/Home';</script>");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }

            if (transaction.Dispute != null)
            {
                return Content("<script language'javascript' type = 'text/javascript'> alert('Error: Cannot dispute transaction more than once.'); window.location='../../Customer/Home';</script>");
            }

            DisputeTransactionViewModel d = new DisputeTransactionViewModel();
            d.TransactionID = Convert.ToInt32(id);

            return View(d);
        }

        //POST: Create/Dispute
        [HttpPost]
        [Authorize(Roles = "Customer")]
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
                user.Disputes.Add(dispute);
                db.Disputes.Add(dispute);
                db.SaveChanges();

                return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Successfully disputed transaction!'); window.location='../../Customer/Home';</script>");

            }
            return View(dt);
        }

    }
}