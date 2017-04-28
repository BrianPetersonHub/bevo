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
        public ActionResult Create(int? id)  //this is an id for a transaction (which will also be the disputes id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            ////Transaction transaction = db.Transactions.Find(id);
            //if (transaction == null)
            //{
            //    return HttpNotFound();
            //}

            //DisputeTransactionViewModel d = new DisputeTransactionViewModel();
            //d.TransactionID = Convert.ToInt32(id);
            //return View(d);

            return View();
        }

        //POST: Create/Dispute
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "DisputeID, DisputeStatus,Message,DisputeAmount")] Dispute dispute, int? id)
        public ActionResult Create([Bind(Include = "Message,DisputedAmount,TransactionID")] DisputeTransactionViewModel dt)
        {
            if (ModelState.IsValid)
            {
                //dispute.Transaction = db.Transactions.Single(t => t.TransactionID == id);
                //dispute.DisputeStatus = DisputeStatus.Submitted;
                //db.Disputes.Add(dispute);

                //transaction.Dispute = dispute;

                Transaction transaction = db.Transactions.Find(dt.TransactionID);
                Dispute dispute = new Dispute();
                dispute.Message = dt.Message;
                dispute.DisputedAmount = dt.DisputedAmount;
                dispute.DisputeStatus = DisputeStatus.Submitted;
                //transaction.Dispute = dispute;
                db.Disputes.Add(dispute);

                db.SaveChanges();
                return RedirectToAction("Confirmation");
            }
            return View(dt);
        }

        public ActionResult Confirmation()
        {
            return View();
        }
    }
}