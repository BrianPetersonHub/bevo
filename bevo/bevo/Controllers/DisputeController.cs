using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Web;
using System.Web.Mvc;
using bevo.Models;
using Microsoft.AspNet.Identity;

namespace bevo.Controllers
{
    public class DisputeController : Controller
    {

        private AppDbContext db = new AppDbContext();

        // GET: Dispute
        public ActionResult Create()  //this is an id for a transaction (which will also be the disputes id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //Transaction transaction = db.Transactions.Find(id);
            //if (transaction == null)
            //{
            //    return HttpNotFound();
            //}

            //DisputeTransactionViewModel d = new DisputeTransactionViewModel();
            //d.TransactionID = Convert.ToInt32(id);
            Dispute d = new Dispute();
            return View(d);
        }

        //POST: Create/Dispute
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DisputeID, DisputeStatus,Message,DisputeAmount")] Dispute dispute)
        //public ActionResult Create([Bind(Include = "Message,DisputedAmount,TransactionID")] DisputeTransactionViewModel dt)
        {
            if (ModelState.IsValid)
            {
                //dispute.Transaction = db.Transactions.Single(t => t.TransactionID == id);
                dispute.DisputeStatus = DisputeStatus.Submitted;
                db.Disputes.Add(dispute);

                //transaction.Dispute = dispute;

                //Transaction transaction = db.Transactions.Find(dt.TransactionID);
                //Dispute dispute = new Dispute();
                //dispute.Message = dt.Message;
                //dispute.DisputedAmount = dt.DisputedAmount;
                //dispute.DisputeStatus = DisputeStatus.Submitted;
                //dispute.Transaction = transaction;
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Disputes] ON");

                db.Disputes.Add(dispute);

                //transaction.Dispute = dispute;
                db.SaveChanges();
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Disputes] OFF");

                return RedirectToAction("Confirmation");
            }
            return View(dispute);
        }

        public ActionResult Confirmation()
        {
            return View();
        }
    }
}