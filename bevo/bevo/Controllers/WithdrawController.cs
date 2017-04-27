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
    public class WithdrawController : Controller
    {
        AppDbContext db = new AppDbContext();

        // GET: Withdraw
        public ActionResult Create()
        {
            return View();
        }

        //POST: Create/Withdraw
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TransactionID,TransactionNum,Date,FromAccount,ToAccount,TransType,Amount,Description,Dispute")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                Int32 accountNum = transaction.FromAccount;
                String accountType = GetAccountType(accountNum);
                transaction.TransType = TransType.Withdrawal;

                if (accountType == "CHECKING")
                {
                    var query = from a in db.CheckingAccounts
                                where a.AccountNum == accountNum
                                select a.CheckingAccountID;
                    //gets first (only) thing from query list
                    Int32 accountID = query.First();
                    CheckingAccount account = db.CheckingAccounts.Find(accountID);
                    account.Transactions.Add(transaction);
                    account.Balance = account.Balance - transaction.Amount;
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
                    account.Balance = account.Balance - transaction.Amount;
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
                            return RedirectToAction("WithdrawAgeAmountError", "IRAccount");
                        }
                        else
                        {
                            account.Transactions.Add(transaction);
                            Transaction feeTransaction = new Transaction();
                            feeTransaction.TransType = TransType.Fee;
                            feeTransaction.Date = DateTime.Now;
                            feeTransaction.FromAccount = transaction.FromAccount;
                            feeTransaction.Amount = 30;
                            feeTransaction.Description = "Fee for transfering funds out of IRA when under 65 years old";
                            account.Transactions.Add(feeTransaction);
                        }
                    }
                    else
                    {
                        account.Transactions.Add(transaction);
                        account.Balance = account.Balance - transaction.Amount;
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
                    account.Transactions.Add(transaction);
                    account.Balance = account.Balance - transaction.Amount;
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