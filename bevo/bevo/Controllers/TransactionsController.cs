using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bevo.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc.Html;
using Microsoft.AspNet.Identity;

namespace bevo.Controllers
{
    public enum Range
    {
        [Display(Name = "0-100")]
        One,

        [Display(Name = "100-200")]
        Two,

        [Display(Name = "200-300")]
        Three,

        [Display(Name = "300+")]
        Four,

        [Display(Name = "Custom")]
        Custom
    }

    public enum Date
    {
        [Display(Name = "Last 15 days")]
        One,

        [Display(Name = "Last 30 days")]
        Two,

        [Display(Name = "Last 60 days")]
        Three,

        All,

        Custom
    }
    public class TransactionsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: All transactions
        // For employees and managers 
        public ActionResult Index()
        {
            return View();
        }

        // GET: Transactions/Detail/#
        public ActionResult Detail(int? id)
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

            ViewBag.SimilarTransactions = Get5SimilarTransactions(transaction);
            return View(transaction);
        }

        //returns a list of 5 most recent similar transactions
        public List<Transaction> Get5SimilarTransactions(Transaction transaction)
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            var query = (from t in db.Transactions
                         where t.TransType == transaction.TransType
                         where (t.FromAccount == transaction.FromAccount && transaction.FromAccount != 0) || (t.ToAccount == transaction.ToAccount && transaction.ToAccount != 0)
                        orderby t.Date descending
                        select t).Take(5);

            List<Transaction> listTransactions = query.ToList();
            return listTransactions;
        }


        // Search Results
        public ActionResult SearchTransactions( String description,
                                                int selectedTransType,
                                                Range selectedRange,
                                                String transactionNumber,
                                                Date selectedDate )
        {

            //DONE: start query
            var query = from t in db.Transactions
                        select t;

            //DONE: Description textbox search
            if (description != null && description != "")
            {
                query = query.Where(t => t.Description.Contains(description));
            }

            //DONE: Dropdown selected transaction type
            var transTypeList = EnumHelper.GetSelectList(typeof(TransType));

            if (selectedTransType == 4)
            {
                // all 
            }
            else
            {
                foreach (SelectListItem type in transTypeList)
                {
                    if (transTypeList.IndexOf(type) == selectedTransType)
                    {
                        query = query.Where(t => t.TransType.Equals(type));
                    }
                }
            }

            //DONE: Radio buttons selected range
            switch (selectedRange)
            {
                case Range.One:
                    query = query.Where(t => t.Amount > 0 && t.Amount <= 100);
                    break;
                case Range.Two:
                    query = query.Where(t => t.Amount > 100 && t.Amount <= 200);
                    break;
                case Range.Three:
                    query = query.Where(t => t.Amount > 200 && t.Amount <= 300);
                    break;
                case Range.Four:
                    query = query.Where(t => t.Amount > 300);
                    break;
                case Range.Custom:
                    // query
                    //TODO: how to handle custom input?
                    break;
            }

            //DONE: Transaction number textbox search
            if (transactionNumber != null && transactionNumber != "")
            {
                Int32 intTransactionNum;
                try
                {
                    intTransactionNum = Convert.ToInt32(transactionNumber);
                }

                catch
                {
                    return View();
                }

                query = query.Where(t => t.TransactionNum == intTransactionNum);
            }

            //TODO: Radio buttons selected date
            switch (selectedDate)
            {
                case Date.One:
                    // query
                    break;
                case Date.Two:
                    // query
                    break;
                case Date.Three:
                    // query
                    break;
                case Date.All:
                    // query
                    break;
                case Date.Custom:
                    //query
                    //TODO: how to handle custom paramater
                    break;
            }


            //TODO: query orderby

            List<Transaction> SelectedTransactions = query.ToList();
            return View("Index", SelectedTransactions);

        } // end of SearchTransaction
    }
}