using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bevo.Models;

namespace bevo.Controllers
{
    public class PayBillsController : Controller
    {
        // GET: PayBills
        public ActionResult Index()
        {
            ViewBag.CustomerPayees = GetCustomerPayees();
            ViewBag.AllAccounts = GetAccounts();
            return View();
        }

        // Go to Add Payee page 
        public ActionResult AddPayeeIndex()
        {
            ViewBag.AllPayees = GetPayees();
            ViewBag.SelectPayee = SelectPayee();
            return View();
        }

        // Add payee to customer from list of payees 
        public ActionResult AddPayee(Int32 selectedPayee)
        {
            Payee payeeToAdd = new Payee();
            // get user
            // query to get payee that matches selectedPayee PayeeID
            // add payee to customer's list of payees

            return RedirectToAction("Index");
        }

        // Get ALL payees in db
        public List<PayeeViewModels> GetPayees()
        {
            List<PayeeViewModels> allPayees = new List<PayeeViewModels>();
            // logic to gett ALL payees in db
            return allPayees;
        }

        // Get ONLY payees that the customer has
        public List<PayeeViewModels> GetCustomerPayees()
        {
            List<PayeeViewModels> customerPayees = new List<PayeeViewModels>();
            // logic to gett only payees that customer has added
            return customerPayees;
        }
        
        // SelectList to make drop down for select payee
        public IEnumerable<SelectListItem> SelectPayee()
        {
            List<PayeeViewModels> allPayees = GetCustomerPayees();
            SelectList selectPayee = new SelectList(allPayees.OrderBy(p => p.PayeeID), "PayeeID", "PayeeName");
            return selectPayee;
        }

        // Get all checkings and savings accounts
        public List<AccountsViewModel> GetAccounts()
        {
            List<AccountsViewModel> allAccounts = new List<AccountsViewModel>();
            // hass properties AccountNum, AccountName, Balance
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