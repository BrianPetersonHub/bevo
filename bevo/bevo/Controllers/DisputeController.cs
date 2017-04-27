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
        public ActionResult Create(int? id)  //this is an id for a transaction
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DisputeTransactionViewModel d = new DisputeTransactionViewModel();
            d.TransactionID = Convert.ToInt32(id);

            return View(d);
        }

        //POST: Create/Dispute
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DisputeID,DisputeStatus,Message,DisputedAmount,TransactionID")] DisputeTransactionViewModel dt)
        {
            if (ModelState.IsValid)
            {
                //TODO: how to find this specific transaction
                Transaction transaction = db.Transactions.Find(dt.TransactionID);
                Dispute dispute = new Dispute();
                dispute.DisputeID = dt.DisputeID;
                dispute.DisputeStatus = DisputeStatus.Submitted;
                dispute.Message = dt.Message;
                dispute.DisputedAmount = dt.DisputedAmount;
                //transaction.Dispute = dispute;
                dispute.Transaction = transaction;
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