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
    public class WithdrawController : Controller
    {
        AppDbContext db = new AppDbContext();

        // GET: Withdraw
        [Authorize]
        public ActionResult Create()
        {
            if (db.Users.Find(User.Identity.GetUserId()).Disabled == true)
            {
                return Content("<script language'javascript' type = 'text/javascript'> alert('Access Denied: Your account has been disabled. You are in a view-only mode.'); window.location='../Customer/Home';</script>");
            }

            // pass account numbers, names, and balances for dropdown list
            List<AccountsViewModel> allAccounts = GetAccounts();
            SelectList selectAccounts = new SelectList(allAccounts.OrderBy(q => q.AccountName), "AccountNum", "AccountName");
            ViewBag.allAccounts = selectAccounts;
            return View();
        }

        //POST: Create/Withdraw
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TransactionID,TransactionNum,Date,FromAccount,ToAccount,TransType,Amount,Description,Dispute")] Transaction transaction, int? selectedAccount)
        {
            if (selectedAccount != null)
            {
                transaction.FromAccount = selectedAccount;
            }

            if (ModelState.IsValid)
            {
                Int32? accountNum = transaction.FromAccount;
                String accountType = GetAccountType(accountNum);
                transaction.TransType = TransType.Withdrawal;
                transaction.Description = "Withdraw $" + transaction.Amount.ToString() + " from " + accountNum.ToString().Substring(accountNum.ToString().Length - 4);

                //FOR CHECKING
                if (accountType == "CHECKING")
                {
                    var query = from a in db.CheckingAccounts
                                where a.AccountNum == accountNum
                                select a.CheckingAccountID;
                    //gets first (only) thing from query list
                    Int32 accountID = query.First();
                    CheckingAccount account = db.CheckingAccounts.Find(accountID);
                    //check if account is already overdrafted
                    if (account.Balance - transaction.Amount < 0)
                    {
                        return Content("<script language'javascript' type = 'text/javascript'> alert('Error: You cannot withdraw more than is in your account.'); window.location='../Customer/Home';</script>");
                    }

                    else
                    {
                        account.Transactions.Add(transaction);
                        account.Balance = account.Balance - transaction.Amount;

                        db.SaveChanges();
                        return Content("<script language'javascript' type = 'text/javascript'> alert('Withdrawal Success!'); window.location='../Customer/Home';</script>");

                    }

                }

                //FOR SAVING
                else if (accountType == "SAVING")
                {
                    var query = from a in db.SavingAccounts
                                where a.AccountNum == accountNum
                                select a.SavingAccountID;
                    //gets first (only) thing from query list
                    Int32 accountID = query.First();
                    SavingAccount account = db.SavingAccounts.Find(accountID);

                    //check if account is already overdrafted
                    if (account.Balance - transaction.Amount < 0)
                    {
                        return Content("<script language'javascript' type = 'text/javascript'> alert('Error: You cannot withdraw more than is in your account.'); window.location='../Customer/Home';</script>");
                    }
                    else
                    {
                        account.Transactions.Add(transaction);
                        account.Balance = account.Balance - transaction.Amount;

                        db.SaveChanges();
                        return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Withdraw successfull.'); window.location='../Customer/Home';</script>");

                    }

                }

                else if (accountType == "IRA")
                {
                    var query = from a in db.IRAccounts
                                where a.AccountNum == accountNum
                                select a.IRAccountID;
                    //gets first (only) thing from query list
                    String accountID = query.First();
                    IRAccount account = db.IRAccounts.Find(accountID);

                    //checks if over 65, if so return error page
                    if (OverAgeLimt() == false)
                    {
                        if (transaction.Amount > 3000)
                        {
                            return Content("<script language'javascript' type = 'text/javascript'> alert('Error: You cannot withdraw more than $3000 from IRA if you are under 65.'); window.location='../Customer/Home';</script>");
                        }
                        else
                        {
                            if (account.Balance - transaction.Amount < 0)
                            {
                                return Content("<script language'javascript' type = 'text/javascript'> alert('Error: You cannot withdraw more than is in your account.'); window.location='../Customer/Home';</script>");
                            }
                            else
                            {
                                Transaction feeTransaction = new Transaction();
                                feeTransaction.TransType = TransType.Fee;
                                feeTransaction.Date = DateTime.Now;
                                feeTransaction.FromAccount = transaction.FromAccount;
                                feeTransaction.Amount = 30;
                                feeTransaction.Description = "$30 Fee for transfering funds out of IRA when under 65 years old";
                                account.Transactions.Add(feeTransaction);
                                account.Transactions.Add(transaction);
                                account.Balance = account.Balance - transaction.Amount - feeTransaction.Amount;

                                db.SaveChanges();
                                return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: You withdraw was successful with a fee.'); window.location='../Customer/Home';</script>");
                            }
                        }
                    }
                    else
                    {
                        if (account.Balance - transaction.Amount <= -50)
                        {
                            return Content("<script language'javascript' type = 'text/javascript'> alert('Error: You cannot withdraw more than is in your account.'); window.location='../Customer/Home';</script>");
                        }

                        else
                        {
                            account.Transactions.Add(transaction);
                            account.Balance = account.Balance - transaction.Amount;

                            db.SaveChanges();
                            return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: You withdraw was successful.'); window.location='../Customer/Home';</script>");

                        }
                    }
                
                }

                else if (accountType == "STOCKPORTFOLIO")
                {
                    var query = from a in db.StockPortfolios
                                where a.AccountNum == accountNum
                                select a.StockPortfolioID;
                    //gets first (only) thing from query list
                    String accountID = query.First();
                    StockPortfolio account = db.StockPortfolios.Find(accountID);
                    if (account.Balance - transaction.Amount < 0)
                    {
                        return Content("<script language'javascript' type = 'text/javascript'> alert('Error: You cannot overdraft a withdraw.'); window.location='../Customer/Home';</script>");
                    }
                    else
                    {
                        account.Transactions.Add(transaction);
                        account.Balance = account.Balance - transaction.Amount;

                        db.SaveChanges();
                        return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Withdraw successfull!'); window.location='../Customer/Home';</script>");

                    }
                }
                
                db.SaveChanges();
            }

            List<AccountsViewModel> allAccounts = GetAccounts();
            SelectList selectAccounts = new SelectList(allAccounts.OrderBy(q => q.AccountName), "AccountNum", "AccountName");
            ViewBag.allAccounts = selectAccounts;
            return View(transaction);
        }


        [Authorize]
        public ActionResult MaxOverdraftError()
        {
            return View();
        }

        //method returns string (CHECKING, SAVING, IRA, STOCK PORTFOLIO) depending on what type of account 
        [Authorize]
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
            if (iraAccount != null)
            {
                if (accountNum == iraAccount.AccountNum)
                {
                    accountType = "IRA";
                    return accountType;
                }
            }


            StockPortfolio stockPortfolio = user.StockPortfolio;
            if (stockPortfolio != null)
            {
                if (accountNum == stockPortfolio.AccountNum)
                {
                    accountType = "STOCKPORTFOLIO";
                    return accountType;
                }
            }

            return "NOT FOUND";
        }

        //returns false if not over 65
        [Authorize]
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

        //find all accounts a user owns
        [Authorize]
        public List<AccountsViewModel> GetAccounts()
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            List<AccountsViewModel> allAccounts = new List<AccountsViewModel>();
            if (user.CheckingAccounts != null)
            {
                List<CheckingAccount> checkingAccounts = user.CheckingAccounts.ToList<CheckingAccount>();
                // get checkings
                foreach (var c in checkingAccounts)
                {
                    AccountsViewModel accountToAdd = new AccountsViewModel();
                    accountToAdd.AccountNum = c.AccountNum;
                    accountToAdd.AccountName = c.AccountName;
                    accountToAdd.Balance = c.Balance;
                    allAccounts.Add(accountToAdd);
                }
            }
            if (user.SavingAccounts != null)
            {
                List<SavingAccount> savingAccounts = user.SavingAccounts.ToList<SavingAccount>();
                // get savings
                foreach (var s in savingAccounts)
                {
                    AccountsViewModel accountToAdd = new AccountsViewModel();
                    accountToAdd.AccountNum = s.AccountNum;
                    accountToAdd.AccountName = s.AccountName;
                    accountToAdd.Balance = s.Balance;
                    allAccounts.Add(accountToAdd);
                }
            }
            if (user.StockPortfolio != null)
            {
                StockPortfolio stockPortfolio = user.StockPortfolio;
                // get cash portion stock portfolio
                AccountsViewModel p = new AccountsViewModel();
                p.AccountNum = stockPortfolio.AccountNum;
                p.AccountName = stockPortfolio.AccountName;
                p.Balance = stockPortfolio.Balance;
                allAccounts.Add(p);
            }

            if (user.IRAccount != null)
            {
                IRAccount iraAccount = user.IRAccount;
                // get cash portion stock portfolio
                AccountsViewModel p = new AccountsViewModel();
                p.AccountNum = iraAccount.AccountNum;
                p.AccountName = iraAccount.AccountName;
                p.Balance = iraAccount.Balance;
                allAccounts.Add(p);
            }
            return allAccounts;
        }
    }
}