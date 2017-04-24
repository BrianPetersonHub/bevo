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

            return View(transaction);
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

        //GET: Transaction/NewTransaction
        public ActionResult ChooseTransaction()
        {
            return View();
        }

        //GET: Deposit/Transaction
        public ActionResult Deposit()
        {
            return View();
        }

        //POST: Deposit/Transaction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deposit([Bind(Include = "TransactionID,TransactionNum,Date,ToAccount,TransType,Amount,Description,Dispute")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                Int32 accountNum = transaction.ToAccount;
                String accountType = GetAccountType(accountNum);

                if (accountType == "CHECKING")
                {
                    var query = from a in db.CheckingAccounts
                                where a.AccountNum == accountNum
                                select a.CheckingAccountID;
                    //gets first (only) thing from query list
                    Int32 accountID = query.First();
                    CheckingAccount account = db.CheckingAccounts.Find(accountID);
                    account.Transactions.Add(transaction);
                    account.Balance = account.Balance + transaction.Amount;
                }

                else if (accountType == "SAVING")
                {
                    var query = from a in db.SavingAccounts
                                where a.AccountNum == accountNum
                                select a.SavingAccountID;
                    //gets first (only) thing from query list
                    Int32 accountID = query.First();
                    SavingAccount account = db.SavingAccounts.Find(accountID);
                    account.Transactions.Add(transaction);
                    account.Balance = account.Balance + transaction.Amount;
                }

                else if (accountType == "IRA")
                {
                    var query = from a in db.IRAccounts
                                where a.AccountNum == accountNum
                                select a.IRAccountID;
                    //gets first (only) thing from query list
                    String accountID = query.First();
                    IRAccount account = db.IRAccounts.Find(accountID);
                    account.Transactions.Add(transaction);
                    account.Balance = account.Balance + transaction.Amount;
                }

                else if (accountType == "STOCKPORTFOLIO")
                {
                    var query = from a in db.StockPortfolios
                                where a.AccountNum == accountNum
                                select a.StockPortfolioID;
                    //gets first (only) thing from query list
                    String accountID = query.First();
                    StockPortfolio account = db.StockPortfolios.Find(accountID);
                    account.Transactions.Add(transaction);
                    account.Balance = account.Balance + transaction.Amount;
                }

                db.SaveChanges(); 

                return RedirectToAction("Home", "Customer");
            }

            return View(transaction);
        }


        //method returns string (CHECKING, SAVING, IRA, STOCK PORTFOLIO) depending on what type of account 
        public String GetAccountType (Int32 accountNum)
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            String accountType;

            List<CheckingAccount> checkingAccounts = user.CheckingAccounts;
            foreach (var c in checkingAccounts)
            {
                if (accountNum == c.AccountNum)
                {
                    accountType = "CHECKING";
                    return accountType;
                }
            }

            List<SavingAccount> savingAccounts = user.SavingAccounts;
            foreach (var s in savingAccounts)
            {
                if (accountNum == s.AccountNum)
                {
                    accountType = "SAVING";
                    return accountType;
                }
            }

            IRAccount iraAccount = user.IRAccount;
            if (accountNum == iraAccount.AccountNum)
            {
                accountType = "IRA";
                return accountType;
            }

            StockPortfolio stockPortfolio = user.StockPortfolio;
            if (accountNum == stockPortfolio.AccountNum)
            {
                accountType = "STOCKPORTFOLIO";
                return accountType;
            }

            return "NOT FOUND";
        }
    }
}