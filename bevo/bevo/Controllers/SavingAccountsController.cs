using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bevo.Models;

namespace bevo.Controllers
{
    public class SavingAccountsController : Controller
    {
        public SavingAccountsController()
        {
            SavingAccount SavingAccount = new SavingAccount();
            SavingAccount.AccountNum = 123;
            SavingAccount.AccountName = "Longhorn Savings";
            SavingAccount.Balance = 0;
        }

        public SavingAccountsController(Decimal balance)
        {

        }

        public SavingAccountsController(String accountName)
        {

        }

        // GET: SavingAccounts
        public ActionResult Index()
        {
            return View();
        }
    }
}