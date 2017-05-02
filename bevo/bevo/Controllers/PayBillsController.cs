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
            ViewBag.SelectPayee = SelectPayee();
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
            }

            return RedirectToAction("Index");
        }

        //Get ALL payees in db
        public List<PayeeViewModel> GetPayees()
        {
            List<PayeeViewModel> allPayees = new List<PayeeViewModel>();
            List<PayeeViewModel> customerPayees = GetCustomerPayees();
            foreach(Payee p in db.Payees)
            {
                PayeeViewModel payee = new PayeeViewModel();
                payee.PayeeName = p.Name;
                payee.PayeeID = p.PayeeID;
                payee.Type = p.PayeeType;

                if (!customerPayees.Contains(payee))
                {
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

            // create transaction with type Pay_Payee

            // logic for overdrafting rules
            // if paymentAmount causes overdraft
                // if payment would cause selectedAccount > $50 under
                    // error message
                // else
                    // $30 transaction fee Type: "Fee", Description: "Overdraft Fee"
            // return RedirectToAction("Confirmation");
            return RedirectToAction("Index");

        }






    }
}