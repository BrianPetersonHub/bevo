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

    //TRANSACTION CONTROLLER
    public class TransactionsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Transactions/Home  (this has a simple search box for search by description)
        // For employees and managers 
        public ActionResult Home(String SearchString)
        {
            ViewBag.CountAllTransactions = GetAllTransactions().Count();

            if (SearchString == null)
            {
                return View(GetAllTransactions());
            }
            else
            {
                return View(GetSelectedTransactions(SearchString));
            }
        }

        public ActionResult DetailedSearch()
        {
            ViewBag.TransTypeSelectList = GetAllTransTypesSL();

            return View();
        }


        // Search Results
        public ActionResult SearchResults (String description,
                                            int? selectedTransType,
                                            Range selectedRange,
                                            int? transactionNumber,
                                            Date selectedDate )
        {
            var query = from t in db.Transactions
                        select t;
            
            //DONE: description textbox search
            if (description != null && description != "")
            {
                query = query.Where(t => t.Description.Contains(description));
            }

            //DONE: Dropdown selected transaction type
            var transTypeList = EnumHelper.GetSelectList(typeof(TransType));

            if (selectedTransType == 8)
            {
                // all
                ; 
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
            if (transactionNumber != null)
            {
                //Int32 intTransactionNum;
                //try
                //{
                //    intTransactionNum = Convert.ToInt32(transactionNumber);
                //}

                //catch
                //{
                //    return Content("<script language'javascript' type = 'text/javascript'> alert('Error: Please enter a number for transaction number.'); window.location='../Transaction/DetailedSearch';</script>");
                //}

                query = query.Where(t => t.TransactionNum == transactionNumber);
            }

            //TODO: Radio buttons selected date
            switch (selectedDate)
            {
                case Date.One:
                    query = query.Where(t => t.Date >= t.Date.AddDays(-15));
                    break;
                case Date.Two:
                    query = query.Where(t => t.Date >= t.Date.AddDays(-30));
                    break;
                case Date.Three:
                    query = query.Where(t => t.Date >= t.Date.AddDays(-60));
                    break;
                case Date.All:
                    break;
                case Date.Custom:
                    //query
                    //TODO: how to handle custom paramater
                    break;
            }


            //TODO: query orderby

            List<Transaction> SelectedTransactions = query.ToList();
            ViewBag.TransTypeSelectList = GetAllTransTypesSL();
            return View("SearchResults", SelectedTransactions);

        } // end of SearchTransaction



        //returns select list of drop down options for trans type
        public SelectList GetAllTransTypesSL()
        {

            List<SelectListItem> allTransTypes = 
            Enum.GetValues(typeof(TransType)).Cast<TransType>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();

            SelectList listToReturn = new SelectList(allTransTypes, "Value", "Text");


            return listToReturn;
         }

    // Get list of all transactions to view
    public List<Transaction> GetAllTransactions()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            //initialize list and add all transactions in the user's IRA
            List<Transaction> allTransactions = user.IRAccount.Transactions;

            //add all checking account transactions
            foreach (CheckingAccount c in user.CheckingAccounts)
            {
                foreach (Transaction t in c.Transactions)
                {
                    allTransactions.Add(t);
                }
            }

            //add all saving account transactions
            foreach (SavingAccount s in user.SavingAccounts)
            {
                foreach (Transaction t in s.Transactions)
                {
                    allTransactions.Add(t);
                }
            }

            //add all stock portfolio transactions
            foreach (Transaction t in user.StockPortfolio.Transactions)
            {
                allTransactions.Add(t);
            }

            return allTransactions;
        }

        //Get list of selcted transactions to view
        public List<Transaction> GetSelectedTransactions(String SearchString)
        {
            List<Transaction> allTransactions = GetAllTransactions();
            var query = from t in allTransactions
                        where t.Description != null
                        where t.Description.Contains(SearchString)
                        select t;

            List<Transaction> selectedTransactions = query.ToList();
            return selectedTransactions;
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
    }
}