using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bevo.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace bevo.Controllers
{
    public class PayBillsController : Controller
    {
        //Define the appdbcontext
        private AppDbContext db = new AppDbContext();


        // GET: PayBills
        public ActionResult Index()
        {
            ViewBag.CustomerPayees = GetCustomerPayees();
            ViewBag.AllAccounts = GetAccounts();
            ViewBag.SelectAccount = SelectAccount();
            ViewBag.SelectPayee = SelectPayee();
            return View();
        }

        // Go to Add Payee page 
        public ActionResult AddPayeeIndex()
        {
            ViewBag.AvailablePayees = GetPayees();
            ViewBag.CustomerPayees = GetCustomerPayees();
            ViewBag.SelectPayee = SelectAvailablePayees();
            return View();
        }

        //Go to confimration page
        public ActionResult Confirmation()
        {
            return View();
        }

        //Go to error page
        public ActionResult Error()
        {
            return View();
        }
        // Add payee to customer from list of payees 
        public ActionResult AddPayee(Int32 selectedPayee)
        {
            Payee payeeToAdd = new Payee();
            //Get the curent user
            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            var user = userManager.FindById(User.Identity.GetUserId());

            var query = from p in db.Payees
                        select p;
            query = query.Where(p => p.PayeeID == selectedPayee);

            List<Payee> qList = query.ToList();

            foreach(Payee p in qList)
            {
                user.Payees.Add(p);
                db.SaveChanges();
            }

            return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Succussfully added new payee!'); window.location='../PayBills/Index';</script>");
        }

        //Get ALL payees in db
        public List<PayeeViewModel> GetPayees()
        {
            List<PayeeViewModel> allPayees = new List<PayeeViewModel>();
            List<PayeeViewModel> customerPayees = GetCustomerPayees();
            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            var user = userManager.FindById(User.Identity.GetUserId());

            foreach (Payee p in db.Payees)
            {
                if (user.Payees.Contains(p))
                { }
                else
                {
                    PayeeViewModel payee = new PayeeViewModel();
                    payee.PayeeName = p.Name;
                    payee.PayeeID = p.PayeeID;
                    payee.Type = p.PayeeType;
                    allPayees.Add(payee);
                }
            }

            return allPayees;
        }

        //Get ONLY payees that the customer has
        public List<PayeeViewModel> GetCustomerPayees()
        {
            //Get the curent user
            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            var user = userManager.FindById(User.Identity.GetUserId());

            List<PayeeViewModel> customerPayees = new List<PayeeViewModel>();
            var query = from p in db.Payees
                        select p;
            List<Payee> qList = query.ToList();

            foreach(Payee p in qList)
            {
                if(user.Payees.Contains(p))
                {
                    PayeeViewModel payee = new PayeeViewModel();
                    payee.PayeeName = p.Name;
                    payee.PayeeID = p.PayeeID;
                    payee.Type = p.PayeeType;
                    customerPayees.Add(payee);
                }
            }
            
            return customerPayees;
        }
        
        // SelectList to make drop down for select payee
        public IEnumerable<SelectListItem> SelectPayee()
        {
            List<PayeeViewModel> allPayees = GetCustomerPayees();
            SelectList selectPayee = new SelectList(allPayees.OrderBy(p => p.PayeeID), "PayeeID", "PayeeName");
            return selectPayee;
        }

        public IEnumerable<SelectListItem> SelectAvailablePayees()
        {
            List<PayeeViewModel> allPayees = GetPayees();
            SelectList selectPayee = new SelectList(allPayees.OrderBy(p => p.PayeeID), "PayeeID", "PayeeName");
            return selectPayee;
        }
        // Get all checkings and savings accounts
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
            return allAccounts;
        }

        // Get select list of all accounts
        public IEnumerable<SelectListItem> SelectAccount()
        {
            List<AccountsViewModel> allAccounts = GetAccounts();
            SelectList selectAccount = new SelectList(allAccounts.OrderBy(p => p.AccountNum), "AccountNum", "AccountName");
            return selectAccount;
        }


        // POST: Pay bill, make payment 
        public ActionResult PayBill(Int32 selectedPayee, Int32 selectedAccount, Decimal paymentAmount, DateTime dateEntered)
        {

            //Make a transaction to store this info
            Transaction trans = new Transaction();
            if (paymentAmount >= 0)
            {
                trans.Amount = paymentAmount;
            }
            trans.TransType = TransType.Pay_Payee;
            trans.Date = dateEntered;


            // find account type and query for type
            String accountType = GetAccountType(selectedAccount);

            if (accountType == "CHECKING")
            {
                var query = from a in db.CheckingAccounts
                            where a.AccountNum == selectedAccount
                            select a.CheckingAccountID;
                Int32 accountID = query.First();
                CheckingAccount fromAccount = db.CheckingAccounts.Find(accountID);

                if (query != null)
                {
                    // if payment causes overdraft, 
                    if (fromAccount.Balance - paymentAmount < -50)
                    {
                        return Content("<script language'javascript' type = 'text/javascript'> alert('Error: You cannot overdraft more than $50!'); window.location='Index';</script>");
                    }
                    else if (fromAccount.Balance - paymentAmount < 0 && fromAccount.Balance - paymentAmount >= -50)
                    {
                        trans.FromAccount = fromAccount.AccountNum;

                        String payeeName;
                        //find payee name
                        List<PayeeViewModel> allPayees = GetPayees();
                        foreach (var payee in allPayees)
                        {
                            if (payee.PayeeID == selectedPayee)
                            {
                                payeeName = payee.PayeeName;
                                trans.Description = "Payment of " + "$" + paymentAmount + " to " + payeeName;
                            }
                        }
                        
                        fromAccount.Transactions.Add(trans);

                        

                        Transaction feeTransaction = new Transaction();
                        feeTransaction.TransType = TransType.Fee;
                        feeTransaction.Amount = 30;
                        feeTransaction.Date = DateTime.Today;
                        feeTransaction.FromAccount = fromAccount.AccountNum;
                        feeTransaction.Description = "$30 fee from overdrafting";

                        fromAccount.Transactions.Add(feeTransaction);

                        fromAccount.Balance = fromAccount.Balance - trans.Amount - feeTransaction.Amount;
                        db.Transactions.Add(trans);
                        db.Transactions.Add(feeTransaction);
                        db.SaveChanges();

                    }
                    else
                    {
                        fromAccount.Transactions.Add(trans);
                        //Assign the from account as that account's AccountNum
                        fromAccount.Transactions.Add(trans);
                        fromAccount.Balance = fromAccount.Balance - trans.Amount;

                        String payeeName;
                        //find payee name
                        List<PayeeViewModel> allPayees = GetPayees();
                        foreach (var payee in allPayees)
                        {
                            if (payee.PayeeID == selectedPayee)
                            {
                                payeeName = payee.PayeeName;
                                trans.Description = "Payment of " + "$" + paymentAmount + " to " + payeeName;
                            }
                        }

                        db.Transactions.Add(trans);
                        db.SaveChanges();
                    }

                    return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Successfully paid bill!'); window.location='../Customer/Home';</script>");
                }
            }

            else if (accountType == "SAVING")
            {
                var query = from a in db.SavingAccounts
                            where a.AccountNum == selectedAccount
                            select a.SavingAccountID;
                Int32 accountID = query.First();
                SavingAccount fromAccount = db.SavingAccounts.Find(accountID);

                if (query != null)
                {
                    // if payment causes overdraft, 
                    if (fromAccount.Balance - paymentAmount < -50)
                    {
                        return Content("<script language'javascript' type = 'text/javascript'> alert('Error: You cannot overdraft more than $50!'); window.location='Index';</script>"); // to go to other controller method, ../Customer/Home
                    }
                    else if (fromAccount.Balance - paymentAmount <= 0 && fromAccount.Balance - paymentAmount >= -50)
                    {
                        fromAccount.Transactions.Add(trans);

                        String payeeName;
                        //find payee name
                        List<PayeeViewModel> allPayees = GetPayees();
                        foreach (var payee in allPayees)
                        {
                            if (payee.PayeeID == selectedPayee)
                            {
                                payeeName = payee.PayeeName;
                                trans.Description = "Payment of" + paymentAmount + "to" + payeeName;
                            }
                        }

                        Transaction feeTransaction = new Transaction();
                        feeTransaction.TransType = TransType.Fee;
                        feeTransaction.Amount = 30;
                        feeTransaction.Date = DateTime.Today;
                        feeTransaction.FromAccount = fromAccount.AccountNum;
                        feeTransaction.Description = "$30 fee from overdrafting";

                        fromAccount.Transactions.Add(feeTransaction);

                        fromAccount.Balance = fromAccount.Balance - trans.Amount - feeTransaction.Amount;
                        db.Transactions.Add(trans);
                        db.Transactions.Add(feeTransaction);
                        db.SaveChanges();

                    }
                    else
                    {
                        fromAccount.Transactions.Add(trans);

                        String payeeName;
                        //find payee name
                        List<PayeeViewModel> allPayees = GetPayees();
                        foreach (var payee in allPayees)
                        {
                            if (payee.PayeeID == selectedPayee)
                            {
                                payeeName = payee.PayeeName;
                                trans.Description = "Payment of" + paymentAmount + "to" + payeeName;
                            }
                        }

                        //Assign the from account as that account's AccountNum
                        trans.FromAccount = fromAccount.AccountNum;
                        if (paymentAmount >= 0)
                        {
                            trans.Amount = paymentAmount;
                        }
                        trans.TransType = TransType.Pay_Payee;
                        trans.Date = dateEntered;

                        fromAccount.Transactions.Add(trans);
                        fromAccount.Balance = fromAccount.Balance - trans.Amount;

                        db.Transactions.Add(trans);
                        db.SaveChanges();
                    }

                    return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Successfully paid bill!'); window.location='../Customer/Home';</script>");
                }
            }
            return RedirectToAction("Index");

        }

        // get account type
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


    }
}