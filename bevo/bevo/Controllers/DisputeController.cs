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

            Transaction transaction = db.Transactions.Find(id);

            if (transaction == null)
            {
                return HttpNotFound();
            }

            ViewBag.Transaction = transaction;
            return View();
        }

        //POST: Create/Dispute
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DisputeID,DisputeStatus,Message,DisputedAmount")] Dispute dispute)
        {
            if (ModelState.IsValid)
            {
                //TODO: how to find this specific transaction
                Transaction transaction = dispute.Transaction;

                transaction.Dispute = dispute;
                db.SaveChanges();

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