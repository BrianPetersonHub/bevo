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
    public class TransferController : Controller
    {
        AppDbContext db = new AppDbContext();

        //GET: Create/Transfer
        public ActionResult Create()
        {
            return View();
        }

        //POST: Create/Transfer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TransactionID,TransactionNum,Date,FromAccount,ToAccount,TransType,Amount,Description,Dispute")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                Int32 fromAccountNum = transaction.FromAccount;
                Int32 toAccountNum = transaction.ToAccount;
                String fromAccountType = GetAccountType(fromAccountNum);
                String toAccountType = GetAccountType(toAccountNum);
                transaction.TransType = TransType.Transfer;

                if (toAccountType == "CHECKING")
                {
                    var query = from a in db.CheckingAccounts
                                where a.AccountNum == toAccountNum
                                select a.CheckingAccountID;
                    //gets first (only) thing from query list
                    Int32 accountID = query.First();
                    CheckingAccount toAccount = db.CheckingAccounts.Find(accountID);
                    toAccount.Transactions.Add(transaction);
                    toAccount.Balance = toAccount.Balance + transaction.Amount;
                }
                else if (toAccountType == "SAVING")
                {
                    var query = from a in db.SavingAccounts
                                where a.AccountNum == toAccountNum
                                select a.SavingAccountID;
                    //gets first (only) thing from query list
                    Int32 accountID = query.First();
                    SavingAccount toAccount = db.SavingAccounts.Find(accountID);
                    toAccount.Transactions.Add(transaction);
                    toAccount.Balance = toAccount.Balance + transaction.Amount;
                }
                else if (toAccountType == "IRA")
                {
                    var query = from a in db.IRAccounts
                                where a.AccountNum == toAccountNum
                                select a.IRAccountID;
                    //gets first (only) thing from query list
                    String accountID = query.First();
                    IRAccount toAccount = db.IRAccounts.Find(accountID);
                    toAccount.Transactions.Add(transaction);
                    toAccount.Balance = toAccount.Balance + transaction.Amount;
                }
                else if (toAccountType == "STOCKPORTFOLIO")
                {
                    var query = from a in db.StockPortfolios
                                where a.AccountNum == toAccountNum
                                select a.StockPortfolioID;
                    //gets first (only) thing from query list
                    String accountID = query.First();
                    StockPortfolio toAccount = db.StockPortfolios.Find(accountID);
                    toAccount.Transactions.Add(transaction);
                    toAccount.Balance = toAccount.Balance + transaction.Amount;
                }

                if (fromAccountType == "CHECKING")
                {
                    var query = from a in db.CheckingAccounts
                                where a.AccountNum == fromAccountNum
                                select a.CheckingAccountID;
                    //gets first (only) thing from query list
                    Int32 accountID = query.First();
                    CheckingAccount fromAccount = db.CheckingAccounts.Find(accountID);
                    fromAccount.Transactions.Add(transaction);
                    fromAccount.Balance = fromAccount.Balance - transaction.Amount;
                }
                else if (fromAccountType == "SAVING")
                {
                    var query = from a in db.SavingAccounts
                                where a.AccountNum == fromAccountNum
                                select a.SavingAccountID;
                    //gets first (only) thing from query list
                    Int32 accountID = query.First();
                    SavingAccount fromAccount = db.SavingAccounts.Find(accountID);
                    fromAccount.Transactions.Add(transaction);
                    fromAccount.Balance = fromAccount.Balance - transaction.Amount;
                }
                else if (fromAccountType == "IRA")
                {
                    var query = from a in db.IRAccounts
                                where a.AccountNum == fromAccountNum
                                select a.IRAccountID;
                    //gets first (only) thing from query list
                    String accountID = query.First();
                    IRAccount fromAccount = db.IRAccounts.Find(accountID);
                    fromAccount.Transactions.Add(transaction);
                    fromAccount.Balance = fromAccount.Balance - transaction.Amount;
                }
                else if (fromAccountType == "STOCKPORTFOLIO")
                {
                    var query = from a in db.StockPortfolios
                                where a.AccountNum == fromAccountNum
                                select a.StockPortfolioID;
                    //gets first (only) thing from query list
                    String accountID = query.First();
                    StockPortfolio fromAccount = db.StockPortfolios.Find(accountID);
                    fromAccount.Transactions.Add(transaction);
                    fromAccount.Balance = fromAccount.Balance - transaction.Amount;
                }

                db.SaveChanges();

                return RedirectToAction("Home", "Customer");
            }

            return View(transaction);
        }


        //method returns string (CHECKING, SAVING, IRA, STOCK PORTFOLIO) depending on what type of account 
        public String GetAccountType(Int32 accountNum)
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