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
            // for the dropbox
            List<AccountsViewModel> allAccounts = GetAccounts();
            SelectList selectAccounts = new SelectList(allAccounts.OrderBy(q => q.AccountName), "AccountNum", "AccountName");
            ViewBag.allAccounts = selectAccounts;
            return View();
        }

        //POST: Create/Transfer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TransactionID,TransactionNum,Date,FromAccount,ToAccount,TransType,Amount,Description,Dispute")] Transaction transaction, int? toAccount1, int? fromAccount1)
        {

            if (fromAccount1 != null)
            {
                transaction.FromAccount = fromAccount1;
            }

            transaction.ToAccount = toAccount1;
            transaction.FromAccount = fromAccount1;
            if (ModelState.IsValid)
            {
                Int32? fromAccountNum = transaction.FromAccount;
                Int32? toAccountNum = transaction.ToAccount;
                String fromAccountType = GetAccountType(fromAccountNum);
                String toAccountType = GetAccountType(toAccountNum);
                transaction.TransType = TransType.Transfer;
                transaction.Description = "Transfer " + transaction.Amount.ToString() + " from " + transaction.FromAccount.ToString().Substring(transaction.FromAccount.ToString().Length - 4) + " to " + transaction.ToAccount.ToString().Substring(transaction.ToAccount.ToString().Length - 4);

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
                        return Content("<script language'javascript' type = 'text/javascript'> alert('Error: Cannot contribute to IRA if you are older than 70.'); window.location='../Customer/Home';</script>");
                    }

                    //if total contributions > 5000, they cannot deposit this amount
                    Decimal iraContributionTotal = TotalContributions(transaction.Amount);
                    if (iraContributionTotal > 5000)
                    {
                        Decimal maxTransferAmount = 5000 - (iraContributionTotal - transaction.Amount);
                        transaction.Amount = maxTransferAmount;
                        ViewBag.MaxTransferAmount = maxTransferAmount.ToString();
                        ViewBag.Transaction = transaction;
                        return Content("<script language'javascript' type = 'text/javascript'> alert('Error: You have reached your contribution limit of $5000 for this year.'); window.location='../Customer/Home';</script>");
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
                    //check if account is already overdrafted
                    if (fromAccount.Balance <= -50)
                    {
                        return Content("<script language'javascript' type = 'text/javascript'> alert('Error: You have already overdrafted the maximum amount.'); window.location='../Customer/Home';</script>");
                    }
                    //check for overdraft strarts. if transaction will make balance <-50, return new view with error and autofilled max transaction
                    if (fromAccount.Balance - transaction.Amount < -50)
                    {
                        transaction.Amount = 50 + fromAccount.Balance;
                        ModelState.Clear();
                        return View("CreateAutoCorrect", transaction);
                    }
                    //if transaction makes balance between 0 and -50, add transaction, make new fee transaction of $30 on top of overdraft
                    else if (fromAccount.Balance - transaction.Amount < 0 && fromAccount.Balance - transaction.Amount >= -50)
                    {
                        fromAccount.Transactions.Add(transaction);

                        Transaction feeTransaction = new Transaction();
                        feeTransaction.TransType = TransType.Fee;
                        feeTransaction.Amount = 30;
                        feeTransaction.Date = DateTime.Today;
                        feeTransaction.FromAccount = fromAccount.AccountNum;
                        feeTransaction.Description = "$30 fee from overdrafting";

                        fromAccount.Transactions.Add(feeTransaction);

                        fromAccount.Balance = fromAccount.Balance - transaction.Amount - feeTransaction.Amount;

                        //send email

                        string userId = User.Identity.GetUserId();
                        AppUser user = db.Users.Find(userId);
                        string userEmail = user.Email;
                        CheckingAccount account = db.CheckingAccounts.Find(transaction.FromAccount);

                        Messaging.EmailMessaging.SendEmail(userEmail, "Overdraft on " + account.AccountName, account.AccountName + " is overdrawn and a $30.00 fee was added to your account. Your current balance on the account is -$" + (account.Balance * -1).ToString() + ".");
                    }
                    else
                    {
                        fromAccount.Transactions.Add(transaction);
                        fromAccount.Balance = fromAccount.Balance - transaction.Amount;
                    }
                }
                else if (fromAccountType == "SAVING")
                {
                    var query = from a in db.SavingAccounts
                                where a.AccountNum == fromAccountNum
                                select a.SavingAccountID;
                    //gets first (only) thing from query list
                    Int32 accountID = query.First();
                    SavingAccount fromAccount = db.SavingAccounts.Find(accountID);
                    //check if account is already overdrafted
                    if (fromAccount.Balance <= -50)
                    {
                        return Content("<script language'javascript' type = 'text/javascript'> alert('Error: You have already overdrafted the maximum amount.'); window.location='../Customer/Home';</script>");
                    }
                    //check for overdraft strarts. if transaction will make balance <-50, return new view with error and autofilled max transaction
                    if (fromAccount.Balance - transaction.Amount < -50)
                    {
                        transaction.Amount = 50 + fromAccount.Balance;
                        ModelState.Clear();
                        return View("CreateAutoCorrect", transaction);
                    }
                    //if transaction makes balance between 0 and -50, add transaction, make new fee transaction of $30 on top of overdraft
                    else if (fromAccount.Balance - transaction.Amount < 0 && fromAccount.Balance - transaction.Amount >= -50)
                    {
                        fromAccount.Transactions.Add(transaction);

                        Transaction feeTransaction = new Transaction();
                        feeTransaction.TransType = TransType.Fee;
                        feeTransaction.Amount = 30;
                        feeTransaction.Date = DateTime.Today;
                        feeTransaction.FromAccount = fromAccount.AccountNum;
                        feeTransaction.Description = "$30 fee from overdrafting";

                        fromAccount.Transactions.Add(feeTransaction);

                        fromAccount.Balance = fromAccount.Balance - transaction.Amount - feeTransaction.Amount;

                        //send email

                        string userId = User.Identity.GetUserId();
                        AppUser user = db.Users.Find(userId);
                        string userEmail = user.Email;
                        SavingAccount account = db.SavingAccounts.Find(transaction.FromAccount);

                        Messaging.EmailMessaging.SendEmail(userEmail, "Overdraft on " + account.AccountName, account.AccountName + " is overdrawn and a $30.00 fee was added to your account. Your current balance on the account is -$" + (account.Balance * -1).ToString() + ".");
                    }
                    else
                    {
                        fromAccount.Transactions.Add(transaction);
                        fromAccount.Balance = fromAccount.Balance - transaction.Amount;
                    }
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
                            return Content("<script language'javascript' type = 'text/javascript'> alert('Error: You cannot take out more $3000 from your IRA if you are under 65 years old.'); window.location='../Customer/Home';</script>");
                        }
                        else
                        {
                            if (fromAccount.Balance <= -50)
                            {
                                return Content("<script language'javascript' type = 'text/javascript'> alert('Error: You have already overdrafted the maximum amount.'); window.location='../Customer/Home';</script>");
                            }
                            if (fromAccount.Balance - transaction.Amount < -50)
                            {
                                transaction.Amount = 50 + fromAccount.Balance;
                                ModelState.Clear();
                                return View("CreateAutoCorrect", transaction);
                            }
                            else if (fromAccount.Balance - transaction.Amount < 0 && fromAccount.Balance - transaction.Amount >= -50)
                            {
                                fromAccount.Transactions.Add(transaction);

                                Transaction ODfeeTransaction = new Transaction();
                                ODfeeTransaction.TransType = TransType.Fee;
                                ODfeeTransaction.Amount = 30;
                                ODfeeTransaction.Date = DateTime.Today;
                                ODfeeTransaction.FromAccount = fromAccount.AccountNum;
                                ODfeeTransaction.Description = "$30 fee from overdrafting";
                                fromAccount.Transactions.Add(ODfeeTransaction);

                                //send email

                                string userId = User.Identity.GetUserId();
                                AppUser user = db.Users.Find(userId);
                                string userEmail = user.Email;
                                IRAccount account = db.IRAccounts.Find(transaction.FromAccount);

                                Messaging.EmailMessaging.SendEmail(userEmail, "Overdraft on " + account.AccountName, account.AccountName + " is overdrawn and a $30.00 fee was added to your account. Your current balance on the account is -$" + (account.Balance * -1).ToString() + ".");

                                Transaction feeTransaction = new Transaction();
                                feeTransaction.TransType = TransType.Fee;
                                feeTransaction.Date = DateTime.Now;
                                feeTransaction.FromAccount = transaction.FromAccount;
                                feeTransaction.Amount = 30;
                                feeTransaction.Description = "$30 Fee for transfering funds out of IRA when under 65 years old";
                                fromAccount.Transactions.Add(feeTransaction);

                                fromAccount.Balance = fromAccount.Balance - transaction.Amount - ODfeeTransaction.Amount - feeTransaction.Amount;
                            }
                        }
                    }
                    else
                    {
                        if (fromAccount.Balance <= -50)
                        {
                            return Content("<script language'javascript' type = 'text/javascript'> alert('Error: You have already overdrafted the maximum amount.'); window.location='../Customer/Home';</script>");
                        }
                        if (fromAccount.Balance - transaction.Amount <= -50)
                        {
                            transaction.Amount = 50 + fromAccount.Balance;
                            ModelState.Clear();
                            return View("CreateAutoCorrect", transaction);
                        }
                        else if (fromAccount.Balance - transaction.Amount < 0 && fromAccount.Balance - transaction.Amount >= -50)
                        {
                            fromAccount.Transactions.Add(transaction);

                            Transaction feeTransaction = new Transaction();
                            feeTransaction.TransType = TransType.Fee;
                            feeTransaction.Amount = 30;
                            feeTransaction.Date = DateTime.Today;
                            feeTransaction.FromAccount = fromAccount.AccountNum;
                            feeTransaction.Description = "$30 fee from overdrafting";

                            fromAccount.Transactions.Add(feeTransaction);

                            fromAccount.Balance = fromAccount.Balance - transaction.Amount - feeTransaction.Amount;

                            //send email

                            string userId = User.Identity.GetUserId();
                            AppUser user = db.Users.Find(userId);
                            string userEmail = user.Email;
                            IRAccount account = db.IRAccounts.Find(transaction.FromAccount);

                            Messaging.EmailMessaging.SendEmail(userEmail, "Overdraft on " + account.AccountName, account.AccountName + " is overdrawn and a $30.00 fee was added to your account. Your current balance on the account is -$" + (account.Balance * -1).ToString() + ".");
                        }
                        else
                        {
                            fromAccount.Transactions.Add(transaction);
                            fromAccount.Balance = fromAccount.Balance - transaction.Amount;
                        }

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
                    //check if account is already overdrafted
                    if (fromAccount.Balance <= -50)
                    {
                        return Content("<script language'javascript' type = 'text/javascript'> alert('Error: You have already overdrafted the maximum amount.'); window.location='../Customer/Home';</script>");
                    }
                    //check for overdraft strarts. if transaction will make balance <-50, return new view with error and autofilled max transaction
                    if (fromAccount.Balance - transaction.Amount < -50)
                    {
                        transaction.Amount = 50 + fromAccount.Balance;
                        ModelState.Clear();
                        return View("CreateAutoCorrect", transaction);
                    }
                    //if transaction makes balance between 0 and -50, add transaction, make new fee transaction of $30 on top of overdraft
                    else if (fromAccount.Balance - transaction.Amount < 0 && fromAccount.Balance - transaction.Amount >= -50)
                    {
                        fromAccount.Transactions.Add(transaction);

                        Transaction feeTransaction = new Transaction();
                        feeTransaction.TransType = TransType.Fee;
                        feeTransaction.Amount = 30;
                        feeTransaction.Date = DateTime.Today;
                        feeTransaction.FromAccount = fromAccount.AccountNum;
                        feeTransaction.Description = "$30 fee from overdrafting";

                        fromAccount.Transactions.Add(feeTransaction);

                        fromAccount.Balance = fromAccount.Balance - transaction.Amount - feeTransaction.Amount;

                        //send email

                        string userId = User.Identity.GetUserId();
                        AppUser user = db.Users.Find(userId);
                        string userEmail = user.Email;
                        StockPortfolio account = db.StockPortfolios.Find(transaction.FromAccount);

                        Messaging.EmailMessaging.SendEmail(userEmail, "Overdraft on " + account.AccountName, account.AccountName + " is overdrawn and a $30.00 fee was added to your account. Your current balance on the account is -$" + (account.Balance * -1).ToString() + ".");
                    }
                    else
                    {
                        fromAccount.Transactions.Add(transaction);
                        fromAccount.Balance = fromAccount.Balance - transaction.Amount;
                    }
                }

                db.SaveChanges();

                return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Transfer successfull!'); window.location='../Customer/Home';</script>");
            }

            List<AccountsViewModel> allAccounts = GetAccounts();
            SelectList selectAccounts = new SelectList(allAccounts.OrderBy(q => q.AccountName), "AccountNum", "AccountName");
            ViewBag.allAccounts = selectAccounts;
            return View(transaction);
        }


        public ActionResult CreateAutoCorrect()
        {
            return View();
        }

        public ActionResult MaxOverdrafError()
        {
            return View();
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

        //find all accounts a user owns
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