using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bevo.Models;
using bevo.DAL;

namespace bevo.Controllers
{
    public class SavingAccountsController : Controller
    {
        private AppDbContext db = new AppDbContext();
        public SavingAccountsController()
        {
            SavingAccount SavingAccount = new SavingAccount();
            SavingAccount.AccountNum = CountAccounts() + 1;
            SavingAccount.AccountName = "Longhorn Savings";
            SavingAccount.Balance = 0;
        }

        public SavingAccountsController(String accountName)
        {
            SavingAccount SavingAccount = new SavingAccount();
            SavingAccount.AccountNum = CountAccounts() + 1;
            SavingAccount.AccountName = accountName;
            SavingAccount.Balance = 0;
        }

        public Int32 CountAccounts()
        {
            Int32 intCount = 1000000000;
            intCount += db.CheckingAccounts.Count();
            intCount += db.SavingAccounts.Count();
            intCount += db.IRAccounts.Count();
            intCount += db.StockPortfolio.Count();
            return intCount;
        }

        // GET: SavingAccounts
        public ActionResult Index()
        {
            return View();
        }
    }
}