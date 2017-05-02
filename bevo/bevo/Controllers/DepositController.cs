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
    public class DepositController : Controller
    {
        AppDbContext db = new AppDbContext();

        //GET: Create/Deposit
        public ActionResult Create()
        {
            // pass account numbers, names, and balances for dropdown list
            List<AccountsViewModel> allAccounts = GetAccounts();
            SelectList selectAccounts = new SelectList(allAccounts.OrderBy(q => q.AccountName), "AccountNum", "AccountName");
            ViewBag.allAccounts = selectAccounts;
            return View();
        }

        //POST: Create/Deposit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TransactionID,TransactionNum,Date,FromAccount,ToAccount,TransType,Amount,Description")] Transaction transaction, int? toAccount)
        {
            if (toAccount != null)
            {
                transaction.ToAccount = toAccount;
            }
            if (transaction.Amount < 0)
            {
                ViewBag.Error = "putting in positive values for what you want to deposit";
                return View("Error");
            }
            if (ModelState.IsValid)
            {
                Int32? accountNum = transaction.ToAccount;
                String accountType = GetAccountType(accountNum);
                transaction.TransType = TransType.Deposit;
                transaction.Description = "Deposit" + transaction.Amount.ToString() + "into" + accountNum.ToString();

                if (accountType == "CHECKING")
                {
                    var query = from a in db.CheckingAccounts
                                where a.AccountNum == accountNum
                                select a.CheckingAccountID;
                    //gets first (only) thing from query list
                    Int32 accountID = query.First();
                    CheckingAccount account = db.CheckingAccounts.Find(accountID);
                    account.Transactions.Add(transaction);
                    //Add the deposit to the account if it's less than 5000
                    //Otherwise, mark it for approval from a manager 
                    if (transaction.Amount >= 5000)
                    {
                        transaction.NeedsApproval = true;
                    }
                    else
                    {
                        account.Balance = account.Balance + transaction.Amount;
                    }
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
                    //Check if transaction needs approval 
                    if(transaction.Amount >= 5000)
                    {
                        transaction.NeedsApproval = true;
                    }
                    else
                    {
                        account.Balance = account.Balance + transaction.Amount;
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

                    //if user is over 70, they cannot deposit
                    if (UnderAgeLimt() == false)
                    {
                        return RedirectToAction("DepositAgeError", "IRAccount");
                    }

                    //if total contributions > 5000, they cannot deposit this amount
                    Decimal iraContributionTotal = TotalContributions(transaction.Amount);
                    if (iraContributionTotal > 5000)
                    {
                        Decimal maxDepositAmount = 5000 - (iraContributionTotal - transaction.Amount);

                        if (maxDepositAmount > 0)
                        {
                            transaction.Amount = maxDepositAmount;
                            ModelState.Clear();
                            return View("CreateAutoCorrect", transaction);
                        }
                        else
                        {
                            return View("MaxDepositError", "IRAccount");
                        }

                    }

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
                    if(transaction.Amount >= 5000)
                    {
                        transaction.NeedsApproval = true;
                    }
                    else
                    {
                        account.Balance = account.Balance + transaction.Amount;
                    }
                }

                db.SaveChanges();

                return RedirectToAction("Home", "Customer");
            }

            List<AccountsViewModel> allAccounts = GetAccounts();
            SelectList selectAccounts = new SelectList(allAccounts.OrderBy(q => q.AccountName), "AccountNum", "AccountName");
            ViewBag.allAccounts = selectAccounts;
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
        // get all accounts
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