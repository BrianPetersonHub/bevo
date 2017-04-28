﻿using System;
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
                Int32? fromAccountNum = transaction.FromAccount;
                Int32? toAccountNum = transaction.ToAccount;
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

                    //if user is over 70, they cannot transfer into ira
                    if (UnderAgeLimt() == false)
                    {
                        return RedirectToAction("TransferAgeError", "IRAccount");
                    }

                    //if total contributions > 5000, they cannot deposit this amount
                    Decimal iraContributionTotal = TotalContributions(transaction.Amount);
                    if (iraContributionTotal > 5000)
                    {
                        Decimal maxTransferAmount = 5000 - (iraContributionTotal - transaction.Amount);
                        transaction.Amount = maxTransferAmount;
                        ViewBag.MaxTransferAmount = maxTransferAmount.ToString();
                        ViewBag.Transaction = transaction;
                        return RedirectToAction("TransferLimitError", "IRAccount");
                    }

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

                    //checks if over 65, if so return error page
                    if (OverAgeLimt() == false)
                    {
                        if (transaction.Amount > 3000)
                        {
                            return RedirectToAction("TransferAgeAmountError", "IRAccount");
                        }
                        else
                        {
                            fromAccount.Transactions.Add(transaction);
                            Transaction feeTransaction = new Transaction();
                            feeTransaction.TransType = TransType.Fee;
                            feeTransaction.Date = DateTime.Now;
                            feeTransaction.FromAccount = transaction.FromAccount;
                            feeTransaction.Amount = 30;
                            feeTransaction.Description = "Fee for transfering funds out of IRA when under 65 years old";
                            fromAccount.Transactions.Add(feeTransaction);
                        }
                    }
                    else
                    {
                         fromAccount.Transactions.Add(transaction);
                         fromAccount.Balance = fromAccount.Balance - transaction.Amount;
                    }
                   
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
        public String GetAccountType(Int32? accountNum)
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

        //method returns true if user is under age max
        public Boolean UnderAgeLimt()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());

            String strToday = DateTime.Now.ToString("M/d/yyyy");
            String strBirthday = user.Birthday;

            String strTodayYear = strToday.Substring(strToday.Length - 4);
            String strBirthdayYear = strBirthday.Substring(strBirthday.Length - 4);

            Int32 intTodayYear = Convert.ToInt32(strTodayYear);
            Int32 intBirthdayYear = Convert.ToInt32(strBirthdayYear);

            ////////////////////////////////////////////////////
            Boolean foundFirstDash = false;
            Int32 firstDashIndex = -1;

            while (foundFirstDash == false)
            {
                firstDashIndex = firstDashIndex + 1;
                if (strToday.Substring(firstDashIndex, 1) == "/")
                {
                    foundFirstDash = true;
                }
            }
            foundFirstDash = false;
            Int32 secondDashIndex = firstDashIndex;
            while (foundFirstDash == false)
            {
                secondDashIndex = secondDashIndex + 1;
                if (strToday.Substring(secondDashIndex, 1) == "/")
                {
                    foundFirstDash = true;
                }
            }

            String strTodayDay = strToday.Substring(firstDashIndex + 1, secondDashIndex - firstDashIndex - 1);
            Int32 intTodayDay = Convert.ToInt32(strTodayDay);
            ////////////////////////////////////////////////////

            String strTodayMonth = strToday.Substring(0, firstDashIndex);
            Int32 intTodayMonth = Convert.ToInt32(strTodayMonth);

            ////////////////////////////////////////////////////
            foundFirstDash = false;
            firstDashIndex = -1;
            while (foundFirstDash == false)
            {
                firstDashIndex = firstDashIndex + 1;
                if (strBirthday.Substring(firstDashIndex, 1) == "/")
                {
                    foundFirstDash = true;
                }
            }
            foundFirstDash = false;
            secondDashIndex = firstDashIndex;
            while (foundFirstDash == false)
            {
                secondDashIndex = secondDashIndex + 1;
                if (strBirthday.Substring(secondDashIndex, 1) == "/")
                {
                    foundFirstDash = true;
                }
            }

            String strBirthdayDay = strBirthday.Substring(firstDashIndex + 1, secondDashIndex - firstDashIndex - 1);
            Int32 intBirthdayDay = Convert.ToInt32(strBirthdayDay);
            ////////////////////////////////////////////////////

            String strBirthdayMonth = strBirthday.Substring(0, firstDashIndex);
            Int32 intBirthdayMonth = Convert.ToInt32(strBirthdayMonth);

            ////////////////////////////////////////////////////


            // Calculate the age.
            Int32 age = intTodayYear - intBirthdayYear;
            if (intBirthdayMonth > intTodayMonth)
            {
                age = age - 1;
            }
            else if ((intBirthdayMonth == intTodayMonth) && (intBirthdayDay > intTodayDay))
            {
                age = age - 1;
            }

            //true or false: is age > 70?
            if (age > 70)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        //method returns the users total deposits after this transaction
        public Decimal TotalContributions(Decimal transactionAmount)
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            IRAccount irAccount = user.IRAccount;
            Decimal sumDeposits = 0;
            foreach (var t in irAccount.Transactions)
            {
                if ((t.TransType == TransType.Deposit) ||
                   ((t.TransType == TransType.Transfer) && (t.ToAccount == irAccount.AccountNum)))
                {
                    sumDeposits = sumDeposits + (t.Amount);
                }
            }
            sumDeposits = sumDeposits + transactionAmount;

            return sumDeposits;

        }

        //returns false if not over 65
        public Boolean OverAgeLimt()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());

            String strToday = DateTime.Now.ToString("M/d/yyyy");
            String strBirthday = user.Birthday;

            String strTodayYear = strToday.Substring(strToday.Length - 4);
            String strBirthdayYear = strBirthday.Substring(strBirthday.Length - 4);

            Int32 intTodayYear = Convert.ToInt32(strTodayYear);
            Int32 intBirthdayYear = Convert.ToInt32(strBirthdayYear);

            ////////////////////////////////////////////////////
            Boolean foundFirstDash = false;
            Int32 firstDashIndex = -1;

            while (foundFirstDash == false)
            {
                firstDashIndex = firstDashIndex + 1;
                if (strToday.Substring(firstDashIndex, 1) == "/")
                {
                    foundFirstDash = true;
                }
            }
            foundFirstDash = false;
            Int32 secondDashIndex = firstDashIndex;
            while (foundFirstDash == false)
            {
                secondDashIndex = secondDashIndex + 1;
                if (strToday.Substring(secondDashIndex, 1) == "/")
                {
                    foundFirstDash = true;
                }
            }

            String strTodayDay = strToday.Substring(firstDashIndex + 1, secondDashIndex - firstDashIndex - 1);
            Int32 intTodayDay = Convert.ToInt32(strTodayDay);
            ////////////////////////////////////////////////////

            String strTodayMonth = strToday.Substring(0, firstDashIndex);
            Int32 intTodayMonth = Convert.ToInt32(strTodayMonth);

            ////////////////////////////////////////////////////
            foundFirstDash = false;
            firstDashIndex = -1;
            while (foundFirstDash == false)
            {
                firstDashIndex = firstDashIndex + 1;
                if (strBirthday.Substring(firstDashIndex, 1) == "/")
                {
                    foundFirstDash = true;
                }
            }
            foundFirstDash = false;
            secondDashIndex = firstDashIndex;
            while (foundFirstDash == false)
            {
                secondDashIndex = secondDashIndex + 1;
                if (strBirthday.Substring(secondDashIndex, 1) == "/")
                {
                    foundFirstDash = true;
                }
            }

            String strBirthdayDay = strBirthday.Substring(firstDashIndex + 1, secondDashIndex - firstDashIndex - 1);
            Int32 intBirthdayDay = Convert.ToInt32(strBirthdayDay);
            ////////////////////////////////////////////////////

            String strBirthdayMonth = strBirthday.Substring(0, firstDashIndex);
            Int32 intBirthdayMonth = Convert.ToInt32(strBirthdayMonth);

            ////////////////////////////////////////////////////


            // Calculate the age.
            Int32 age = intTodayYear - intBirthdayYear;
            if (intBirthdayMonth > intTodayMonth)
            {
                age = age - 1;
            }
            else if ((intBirthdayMonth == intTodayMonth) && (intBirthdayDay > intTodayDay))
            {
                age = age - 1;
            }

            //true or false: is age > 65?
            if (age < 65)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

    }
}