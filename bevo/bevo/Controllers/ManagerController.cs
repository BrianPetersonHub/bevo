using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using bevo.Models;
using System.Collections.Generic;
using System.Net;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using bevo.Utilities;

namespace bevo.Controllers
{
    public class ManagerController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Manager
        public ActionResult Home()
        {
            //Put useful information in the viewbag
            ViewBag.TransactionMasterList = GetTrMasterList();
            ViewBag.TransactionToApprove = GetTrToApprove();
            ViewBag.UnresolvedDisputes = GetUnresolvedDisputes();
            ViewBag.AllDisputes = GetAllDisputes();
            ViewBag.PendingTransactions = GetPendingTransactions();

            return View();
        }

        // GET: Manager/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUser appUser = db.Users.Find(id);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            return View(appUser);
        }

        // GET: Manager/Create
        public ActionResult Create()
        {
            ViewBag.Id = new SelectList(db.IRAccounts, "IRAccountID", "AccountName");
            ViewBag.Id = new SelectList(db.StockPortfolios, "StockPortfolioID", "AccountName");
            return View();
        }

        // POST: Manager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Enabled,FirstName,MiddleInitial,LastName,Street,City,State,ZipCode,Birthday,Active,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(appUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id = new SelectList(db.IRAccounts, "IRAccountID", "AccountName", appUser.Id);
            ViewBag.Id = new SelectList(db.StockPortfolios, "StockPortfolioID", "AccountName", appUser.Id);
            return View(appUser);
        }

        // GET: Manager/Edit/5
        public ActionResult Edit(string id)
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            EditEmployeeViewModel eevm = new EditEmployeeViewModel();
            eevm.City = user.City;
            eevm.Email = user.Email;
            eevm.PhoneNumber = user.PhoneNumber;
            eevm.State = user.State;
            eevm.Street = user.Street;
            eevm.ZipCode = user.ZipCode;

            return View(eevm);
        }

        // POST: Manager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Street,City,State,ZipCode,Email,PhoneNumber")] EditEmployeeViewModel eevm)
        {
            if (ModelState.IsValid)
            {
                //Get the db record for the user who is logged in 
                AppUser user = db.Users.Find(User.Identity.GetUserId());

                user.Street = eevm.Street;
                user.City = eevm.City;
                user.State = eevm.State;
                user.ZipCode = eevm.ZipCode;
                user.Email = eevm.Email;
                user.PhoneNumber = eevm.PhoneNumber;

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Home");
            }
            return View(eevm);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }








        //TODO: finsh this
        //Get method for seeing account initial deposit approvals
        public ActionResult ViewPendingTransactions()
        {
            ViewBag.PendingTransactions = GetPendingTransactions();
            return View();
        }

        public ActionResult ApprovePendingTransactions(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transToChange = db.Transactions.Find(id);
            if (transToChange == null)
            {
                return HttpNotFound();
            }

            //for transfers, have to chagne two accounts balances
            if (transToChange.TransType == TransType.Transfer || transToChange.TransType == TransType.Deposit || transToChange.TransType == TransType.Sell_Stock || transToChange.TransType == TransType.Bonus)
            {
                //to accounts
                if (GetAccountType(transToChange.ToAccount) == "CHECKING")
                {
                    var query = from account in db.CheckingAccounts
                                where account.AccountNum == transToChange.ToAccount
                                select account.CheckingAccountID;
                    //gets first (only) thing from query list
                    Int32 accountID = query.First();
                    CheckingAccount a = db.CheckingAccounts.Find(accountID);
                    a.Balance = a.Balance + transToChange.Amount;
                }

                else if (GetAccountType(transToChange.ToAccount) == "SAVING")
                {
                    var query = from account in db.SavingAccounts
                                where account.AccountNum == transToChange.ToAccount
                                select account.SavingAccountID;
                    //gets first (only) thing from query list
                    Int32 accountID = query.First();
                    SavingAccount a = db.SavingAccounts.Find(accountID);
                    a.Balance = a.Balance + transToChange.Amount;
                }

                else if (GetAccountType(transToChange.ToAccount) == "IRA")
                {
                    var query = from account in db.IRAccounts
                                where account.AccountNum == transToChange.ToAccount
                                select account.IRAccountID;
                    //gets first (only) thing from query list
                    String accountID = query.First();
                    IRAccount a = db.IRAccounts.Find(accountID);
                    a.Balance = a.Balance + transToChange.Amount;
                }

                else if (GetAccountType(transToChange.ToAccount) == "STOCKPORTFOLIO")
                {
                    var query = from account in db.StockPortfolios
                                where account.AccountNum == transToChange.ToAccount
                                select account.StockPortfolioID;
                    //gets first (only) thing from query list
                    String accountID = query.First();
                    StockPortfolio a = db.StockPortfolios.Find(accountID);
                    a.Balance = a.Balance + transToChange.Amount;
                }
            }
            else
            {
                //from accounts
                if (GetAccountType(transToChange.FromAccount) == "CHECKING")
                {
                    var query = from account in db.CheckingAccounts
                                where account.AccountNum == transToChange.FromAccount
                                select account.CheckingAccountID;
                    //gets first (only) thing from query list
                    Int32 accountID = query.First();
                    CheckingAccount a = db.CheckingAccounts.Find(accountID);
                    a.Balance = a.Balance - transToChange.Amount;
                }

                else if (GetAccountType(transToChange.FromAccount) == "SAVING")
                {
                    var query = from account in db.SavingAccounts
                                where account.AccountNum == transToChange.FromAccount
                                select account.SavingAccountID;
                    //gets first (only) thing from query list
                    Int32 accountID = query.First();
                    SavingAccount a = db.SavingAccounts.Find(accountID);
                    a.Balance = a.Balance - transToChange.Amount;
                }

                else if (GetAccountType(transToChange.FromAccount) == "IRA")
                {
                    var query = from account in db.IRAccounts
                                where account.AccountNum == transToChange.FromAccount
                                select account.IRAccountID;
                    //gets first (only) thing from query list
                    String accountID = query.First();
                    IRAccount a = db.IRAccounts.Find(accountID);
                    a.Balance = a.Balance - transToChange.Amount;
                }

                else if (GetAccountType(transToChange.FromAccount) == "STOCKPORTFOLIO")
                {
                    var query = from account in db.StockPortfolios
                                where account.AccountNum == transToChange.FromAccount
                                select account.StockPortfolioID;
                    //gets first (only) thing from query list
                    String accountID = query.First();
                    StockPortfolio a = db.StockPortfolios.Find(accountID);
                    a.Balance = a.Balance - transToChange.Amount;
                }
            }
            transToChange.NeedsApproval = false;
            db.SaveChanges();

            AppUser user = db.Users.Find(User.Identity.GetUserId());
            String bodyForEmail = null;
            bodyForEmail = "Your pending transaction, transaction number " + transToChange.TransactionID + ", has been accepted.";
            bevo.Messaging.EmailMessaging.SendEmail(user.Email, "Transaction Accepted", bodyForEmail);

            return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Successfully accepted pending transaction!'); window.location='../Home';</script>");
        }

        public ActionResult RejectPendingTransactions(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }

            String toAccountType = GetAccountType(transaction.ToAccount);
            String fromAccountType = GetAccountType(transaction.FromAccount);

            AppUser user = new AppUser();
            if (GetAccountType(transaction.ToAccount) == "CHECKING" || GetAccountType(transaction.FromAccount) == "CHECKING")
            {
                var query = from account in db.CheckingAccounts
                            where account.AccountNum == transaction.ToAccount || account.AccountNum == transaction.FromAccount
                            select account.CheckingAccountID;
                //gets first (only) thing from query list
                Int32 accountID = query.First();
                CheckingAccount a = db.CheckingAccounts.Find(accountID);
                user = a.AppUser;
            }
            if (GetAccountType(transaction.ToAccount) == "SAVING" || GetAccountType(transaction.FromAccount) == "SAVING")
            {
                var query = from account in db.SavingAccounts
                            where account.AccountNum == transaction.ToAccount || account.AccountNum == transaction.FromAccount
                            select account.SavingAccountID;
                //gets first (only) thing from query list
                Int32 accountID = query.First();
                SavingAccount a = db.SavingAccounts.Find(accountID);
                user = a.AppUser;
            }
            else if (GetAccountType(transaction.ToAccount) == "IRA" || GetAccountType(transaction.FromAccount) == "IRA")
            {
                var query = from account in db.IRAccounts
                            where account.AccountNum == transaction.ToAccount || account.AccountNum == transaction.FromAccount
                            select account.IRAccountID;
                //gets first (only) thing from query list
                String accountID = query.First();
                IRAccount a = db.IRAccounts.Find(accountID);
                user = a.AppUser;
            }
            else if (GetAccountType(transaction.ToAccount) == "STOCKPORTFOLIO" || GetAccountType(transaction.FromAccount) == "STOCKPORTFOLIO")
            {
                var query = from account in db.StockPortfolios
                            where account.AccountNum == transaction.ToAccount || account.AccountNum == transaction.FromAccount
                            select account.StockPortfolioID;
                //gets first (only) thing from query list
                String accountID = query.First();
                StockPortfolio a = db.StockPortfolios.Find(accountID);
                user = a.AppUser;
            }
            transaction.NeedsApproval = false;
            db.SaveChanges();
            //Send an email to the user in question 
            String bodyForEmail = null;

            bodyForEmail = "Your pending transaction, transaction number " + transaction.TransactionID + ", has been rejected.";

            bevo.Messaging.EmailMessaging.SendEmail(user.Email, "Dispute Rejected", bodyForEmail);

            return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Successfully rejected pending transactions!'); window.location='../Home';</script>");
        }


        public List<TransViewModel> GetPendingTransactions()
        {
            List<TransViewModel> tvms = new List<TransViewModel>();
            foreach (CheckingAccount a in db.CheckingAccounts)
            {
                AppUser user = a.AppUser;
                foreach (Transaction t in a.Transactions)
                {
                    if (t.NeedsApproval == true)
                    {
                        TransViewModel tvm = new TransViewModel();
                        tvm.TransactionID = t.TransactionID;
                        tvm.TransactionNum = t.TransactionNum;
                        tvm.TransType = t.TransType;
                        tvm.Amount = t.Amount;
                        tvm.toAccount = t.ToAccount;
                        tvm.fromAccount = t.FromAccount;
                        tvm.Date = t.Date;
                        tvm.Description = t.Description;
                        tvm.FirstName = user.FirstName;
                        tvm.LastName = user.LastName;

                        tvms.Add(tvm);
                    }
                }
            }
            foreach (SavingAccount a in db.SavingAccounts)
            {
                AppUser user = a.AppUser;
                foreach (Transaction t in a.Transactions)
                {
                    if (t.NeedsApproval == true)
                    {
                        TransViewModel tvm = new TransViewModel();
                        tvm.TransactionID = t.TransactionID;
                        tvm.TransactionNum = t.TransactionNum;
                        tvm.TransType = t.TransType;
                        tvm.Amount = t.Amount;
                        tvm.toAccount = t.ToAccount;
                        tvm.fromAccount = t.FromAccount;
                        tvm.Date = t.Date;
                        tvm.Description = t.Description;
                        tvm.FirstName = user.FirstName;
                        tvm.LastName = user.LastName;

                        tvms.Add(tvm);
                    }
                }
            }
            foreach (IRAccount a in db.IRAccounts)
            {
                AppUser user = a.AppUser;
                foreach (Transaction t in a.Transactions)
                {
                    if (t.NeedsApproval == true)
                    {
                        TransViewModel tvm = new TransViewModel();
                        tvm.TransactionID = t.TransactionID;
                        tvm.TransactionNum = t.TransactionNum;
                        tvm.TransType = t.TransType;
                        tvm.Amount = t.Amount;
                        tvm.toAccount = t.ToAccount;
                        tvm.fromAccount = t.FromAccount;
                        tvm.Date = t.Date;
                        tvm.Description = t.Description;
                        tvm.FirstName = user.FirstName;
                        tvm.LastName = user.LastName;

                        tvms.Add(tvm);
                    }
                }
            }
            foreach (StockPortfolio a in db.StockPortfolios)
            {
                AppUser user = a.AppUser;
                foreach (Transaction t in a.Transactions)
                {
                    if (t.NeedsApproval == true)
                    {
                        TransViewModel tvm = new TransViewModel();
                        tvm.TransactionID = t.TransactionID;
                        tvm.TransactionNum = t.TransactionNum;
                        tvm.TransType = t.TransType;
                        tvm.Amount = t.Amount;
                        tvm.toAccount = t.ToAccount;
                        tvm.fromAccount = t.FromAccount;
                        tvm.Date = t.Date;
                        tvm.Description = t.Description;
                        tvm.FirstName = user.FirstName;
                        tvm.LastName = user.LastName;

                        tvms.Add(tvm);
                    }
                }
            }

            return tvms;
        }


        //Get method for chanign employee to a manager 
        public ActionResult PromoteEmployee()
        {
            ViewBag.AllEmployees = GetEmployees();
            return View();
        }

        //Post method for changing employee to a manager 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PromoteEmployee(String id)
        {
            AppDbContext db = new AppDbContext();

            //Get the user we want 
            var query = from user in db.Users
                        select user;
            query = query.Where(user => user.Id == id);
            List<AppUser> queryList = query.ToList();
            AppUser userInQuestion = queryList[0];

            //Remove user from employee role and add them to manager role 
            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            userManager.RemoveFromRole(userInQuestion.Id, "Employee");
            userManager.AddToRole(userInQuestion.Id, "Manager");

            //Save Changes
            db.Entry(userInQuestion).State = EntityState.Modified;
            db.SaveChanges();

            return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Successfully promoted employee!'); window.location='../Manager/Home';</script>");
        }


        //Make a method to get a list of all the employees and puts in in the viewbag for the fire employees view
        public ActionResult FireEmployee()
        {
            ViewBag.AllEmployees = GetEmployees();
            ViewBag.SelectEmployee = SelectEmployee();
            List<AppUser> employees = GetEmployees();
            return View(employees);
        }

        //Make the post edit method to fire the employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FireEmployee(String id)
        {
            AppDbContext db = new AppDbContext();

            //Get the user we want 
            var query = from user in db.Users
                        select user;
            query = query.Where(user => user.Id == id);
            List<AppUser> queryList = query.ToList();
            AppUser userInQuestion = queryList[0];

            //change the user's disabled variable to true 
            userInQuestion.Disabled = true;

            //Save Changes
            db.Entry(userInQuestion).State = EntityState.Modified;
            db.SaveChanges();

            return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Successfully terminated employee!'); window.location='../Manager/Home';</script>");
        }

        //Make a method to get a list of all the customers and puts it in the viewbag for the freeze customer view
        public ActionResult FreezeCustomer()
        {
            ViewBag.SelectCustomers = SelectCustomer();
            ViewBag.AllCustomers = GetCustomers();
            List<AppUser> customers = GetCustomers();
            return View(customers);
        }

        //Make the post edit method to fire the employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FreezeCustomer(String id)
        {
            AppDbContext db = new AppDbContext();

            //Get the user we want 
            var query = from user in db.Users
                        select user;
            query = query.Where(user => user.Id == id);
            List<AppUser> queryList = query.ToList();
            AppUser userInQuestion = queryList[0];

            //change the user's disabled variable to true 
            userInQuestion.Disabled = true;

            //Save Changes
            db.Entry(userInQuestion).State = EntityState.Modified;
            db.SaveChanges();

            return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Successfully froze customer account!'); window.location='../Manager/Home';</script>");
        }

        public ActionResult ReactivateCustomer()
        {
            ViewBag.SelectCustomers = SelectDisabledCustomer();
            ViewBag.AllCustomers = GetDisabledCustomers();
            List<AppUser> customers = GetDisabledCustomers();
            return View(customers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReactivateCustomer(String id)
        {
            AppDbContext db = new AppDbContext();

            //Get the user we want 
            var query = from user in db.Users
                        select user;
            query = query.Where(user => user.Id == id);
            List<AppUser> queryList = query.ToList();
            AppUser userInQuestion = queryList[0];

            //change the user's disabled variable to true 
            userInQuestion.Disabled = false;

            //Save Changes
            db.Entry(userInQuestion).State = EntityState.Modified;
            db.SaveChanges();

            return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Successfully reactivated customer account!'); window.location='../Manager/Home';</script>");

        }

        //Make a get method for editing the dispute
        //The view for this method should be bound to the disputeviewmodel class 
        public ActionResult EditDispute(int? id)
        {
            AppDbContext db = new AppDbContext();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Get the dispute object we're concerned with 
            var query = from d in db.Disputes
                        select d;
            query = query.Where(d => d.DisputeID == id);

            if (query == null)
            {
                return HttpNotFound();
            }

            else
            {
                Dispute theDispute = query.ToList()[0];
                //Apply this dispute's information to a dispute viewmodel which will then be passed
                //to the edit object view 

                DisputeViewModel dvm = new DisputeViewModel();
                dvm.CorrectAmount = theDispute.DisputedAmount;
                dvm.CustEmail = theDispute.AppUser.Email;
                dvm.FirstName = theDispute.AppUser.FirstName;
                dvm.LastName = theDispute.AppUser.LastName;
                dvm.Message = theDispute.Message;
                dvm.Status = theDispute.DisputeStatus;
                dvm.TransName = theDispute.Transaction.TransactionID;
                dvm.TransAmount = theDispute.Transaction.Amount;
                dvm.DisputeID = theDispute.DisputeID;
                ViewBag.DisputeDetails = dvm;

                EditDisputeViewModel edvm = new EditDisputeViewModel();
                edvm.DisputeID = dvm.DisputeID;
                return View(edvm);
            }
        }

        //Make a post method for editing disputes 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDispute([Bind(Include = "DisputeID,AdjustedAmount,Comment,Status")] EditDisputeViewModel edvm)
        {
            AppDbContext db = new AppDbContext();

            if (ModelState.IsValid)
            {
                Dispute disToChange = db.Disputes.Find(edvm.DisputeID);
                Transaction transToChange = db.Transactions.Find(disToChange.Transaction.TransactionID);

                var query1 = from d in db.Disputes
                             select d;
                query1 = query1.Where(d => d.DisputeID == edvm.DisputeID);

                Dispute theDispute = query1.ToList()[0];
                //Apply this dispute's information to a dispute viewmodel 

                DisputeViewModel dvm = new DisputeViewModel();
                dvm.CorrectAmount = theDispute.DisputedAmount;
                dvm.CustEmail = theDispute.AppUser.Email;
                dvm.FirstName = theDispute.AppUser.FirstName;
                dvm.LastName = theDispute.AppUser.LastName;
                dvm.Message = theDispute.Message;
                dvm.Status = theDispute.DisputeStatus;
                dvm.TransName = theDispute.Transaction.TransactionID;
                dvm.TransAmount = theDispute.Transaction.Amount;
                dvm.DisputeID = theDispute.DisputeID;


                //get user currently logged in 
                UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
                var user = userManager.FindById(User.Identity.GetUserId());

                //Make appropriate changes to the dispute and transaction in quetion 
                //based on whether the dispute was accepted, rejected, or adjusted
                if (edvm.Status == DisputeStatus.Accepted)
                {
                    if (transToChange.TransType == TransType.Transfer || transToChange.TransType == TransType.Deposit || transToChange.TransType == TransType.Sell_Stock || transToChange.TransType == TransType.Bonus)
                    {
                        //to accounts
                        if (GetAccountType(transToChange.ToAccount) == "CHECKING")
                        {
                            var query = from account in db.CheckingAccounts
                                        where account.AccountNum == transToChange.ToAccount
                                        select account.CheckingAccountID;
                            //gets first (only) thing from query list
                            Int32 accountID = query.First();
                            CheckingAccount a = db.CheckingAccounts.Find(accountID);
                            a.Balance = a.Balance + dvm.CorrectAmount - transToChange.Amount;
                        }

                        else if (GetAccountType(transToChange.ToAccount) == "SAVING")
                        {
                            var query = from account in db.SavingAccounts
                                        where account.AccountNum == transToChange.ToAccount
                                        select account.SavingAccountID;
                            //gets first (only) thing from query list
                            Int32 accountID = query.First();
                            SavingAccount a = db.SavingAccounts.Find(accountID);
                            a.Balance = a.Balance + dvm.CorrectAmount - transToChange.Amount;
                        }

                        else if (GetAccountType(transToChange.ToAccount) == "IRA")
                        {
                            var query = from account in db.IRAccounts
                                        where account.AccountNum == transToChange.ToAccount
                                        select account.IRAccountID;
                            //gets first (only) thing from query list
                            String accountID = query.First();
                            IRAccount a = db.IRAccounts.Find(accountID);
                            a.Balance = a.Balance + dvm.CorrectAmount - transToChange.Amount;
                        }

                        else if (GetAccountType(transToChange.ToAccount) == "STOCKPORTFOLIO")
                        {
                            var query = from account in db.StockPortfolios
                                        where account.AccountNum == transToChange.ToAccount
                                        select account.StockPortfolioID;
                            //gets first (only) thing from query list
                            String accountID = query.First();
                            StockPortfolio a = db.StockPortfolios.Find(accountID);
                            a.Balance = a.Balance + dvm.CorrectAmount - transToChange.Amount;
                        }
                    }
                    else
                    {
                        //from accounts
                        if (GetAccountType(transToChange.FromAccount) == "CHECKING")
                        {
                            var query = from account in db.CheckingAccounts
                                        where account.AccountNum == transToChange.FromAccount
                                        select account.CheckingAccountID;
                            //gets first (only) thing from query list
                            Int32 accountID = query.First();
                            CheckingAccount a = db.CheckingAccounts.Find(accountID);
                            a.Balance = a.Balance - dvm.CorrectAmount + transToChange.Amount;
                        }

                        else if (GetAccountType(transToChange.FromAccount) == "SAVING")
                        {
                            var query = from account in db.SavingAccounts
                                        where account.AccountNum == transToChange.FromAccount
                                        select account.SavingAccountID;
                            //gets first (only) thing from query list
                            Int32 accountID = query.First();
                            SavingAccount a = db.SavingAccounts.Find(accountID);
                            a.Balance = a.Balance - dvm.CorrectAmount + transToChange.Amount;
                        }

                        else if (GetAccountType(transToChange.FromAccount) == "IRA")
                        {
                            var query = from account in db.IRAccounts
                                        where account.AccountNum == transToChange.FromAccount
                                        select account.IRAccountID;
                            //gets first (only) thing from query list
                            String accountID = query.First();
                            IRAccount a = db.IRAccounts.Find(accountID);
                            a.Balance = a.Balance - dvm.CorrectAmount + transToChange.Amount;
                        }

                        else if (GetAccountType(transToChange.FromAccount) == "STOCKPORTFOLIO")
                        {
                            var query = from account in db.StockPortfolios
                                        where account.AccountNum == transToChange.FromAccount
                                        select account.StockPortfolioID;
                            //gets first (only) thing from query list
                            String accountID = query.First();
                            StockPortfolio a = db.StockPortfolios.Find(accountID);
                            a.Balance = a.Balance - dvm.CorrectAmount + transToChange.Amount;
                        }
                    }

                    disToChange.DisputeStatus = edvm.Status;
                    if (edvm.Comment != null)
                    {
                        disToChange.Message = disToChange.Message + "\n" + edvm.Comment;
                    }
                    transToChange.Amount = dvm.CorrectAmount;
                    transToChange.Description = "Dispute [Accepted] " + transToChange.Description;
                    disToChange.ManResolvedEmail = user.Email;



                    db.Entry(disToChange).State = EntityState.Modified;
                    db.Entry(transToChange).State = EntityState.Modified;
                    db.SaveChanges();

                    return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: You have successfully accepted the dispute!'); window.location='../../Manager/Home';</script>");

                }
                if (edvm.Status == DisputeStatus.Rejected)
                {
                    //Send an email to the user in question 
                    String bodyForEmail = null;
                    if (edvm.Comment != null)
                    {
                        bodyForEmail = "Your dispute on transaction number " + transToChange.TransactionID + " has been rejected."
                       + "Additional messages from the manager who resolved your dispute: " + "\n" + edvm.Comment;
                    }
                    else
                    {
                        bodyForEmail = "Your dispute on transaction number " + transToChange.TransactionID + " has been rejected.";
                    }

                    bevo.Messaging.EmailMessaging.SendEmail(disToChange.AppUser.Email, "Dispute Rejected", bodyForEmail);

                    //Change the appropriate information and save it to the DB
                    disToChange.DisputeStatus = edvm.Status;
                    transToChange.Description = "Dispute [Rejected] " + transToChange.Description + edvm.Comment;
                    db.Entry(disToChange).State = EntityState.Modified;
                    db.Entry(transToChange).State = EntityState.Modified;
                    db.SaveChanges();

                    return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: You have successfuly rejected the dispute!'); window.location='../../Manager/Home';</script>");
                }
                if (edvm.Status == DisputeStatus.Adjusted)
                {
                    //for transfers, have to chagne two accounts balances
                    if (transToChange.TransType == TransType.Transfer || transToChange.TransType == TransType.Deposit || transToChange.TransType == TransType.Sell_Stock || transToChange.TransType == TransType.Bonus)
                    {
                        //to accounts
                        if (GetAccountType(transToChange.ToAccount) == "CHECKING")
                        {
                            var query = from account in db.CheckingAccounts
                                        where account.AccountNum == transToChange.ToAccount
                                        select account.CheckingAccountID;
                            //gets first (only) thing from query list
                            Int32 accountID = query.First();
                            CheckingAccount a = db.CheckingAccounts.Find(accountID);
                            a.Balance = a.Balance + dvm.CorrectAmount - transToChange.Amount;
                        }

                        else if (GetAccountType(transToChange.ToAccount) == "SAVING")
                        {
                            var query = from account in db.SavingAccounts
                                        where account.AccountNum == transToChange.ToAccount
                                        select account.SavingAccountID;
                            //gets first (only) thing from query list
                            Int32 accountID = query.First();
                            SavingAccount a = db.SavingAccounts.Find(accountID);
                            a.Balance = a.Balance + dvm.CorrectAmount - transToChange.Amount;
                        }

                        else if (GetAccountType(transToChange.ToAccount) == "IRA")
                        {
                            var query = from account in db.IRAccounts
                                        where account.AccountNum == transToChange.ToAccount
                                        select account.IRAccountID;
                            //gets first (only) thing from query list
                            String accountID = query.First();
                            IRAccount a = db.IRAccounts.Find(accountID);
                            a.Balance = a.Balance + dvm.CorrectAmount - transToChange.Amount;
                        }

                        else if (GetAccountType(transToChange.ToAccount) == "STOCKPORTFOLIO")
                        {
                            var query = from account in db.StockPortfolios
                                        where account.AccountNum == transToChange.ToAccount
                                        select account.StockPortfolioID;
                            //gets first (only) thing from query list
                            String accountID = query.First();
                            StockPortfolio a = db.StockPortfolios.Find(accountID);
                            a.Balance = a.Balance + dvm.CorrectAmount - transToChange.Amount;
                        }
                    }
                    else
                    {
                        //from accounts
                        if (GetAccountType(transToChange.FromAccount) == "CHECKING")
                        {
                            var query = from account in db.CheckingAccounts
                                        where account.AccountNum == transToChange.FromAccount
                                        select account.CheckingAccountID;
                            //gets first (only) thing from query list
                            Int32 accountID = query.First();
                            CheckingAccount a = db.CheckingAccounts.Find(accountID);
                            a.Balance = a.Balance - dvm.CorrectAmount + transToChange.Amount;
                        }

                        else if (GetAccountType(transToChange.FromAccount) == "SAVING")
                        {
                            var query = from account in db.SavingAccounts
                                        where account.AccountNum == transToChange.FromAccount
                                        select account.SavingAccountID;
                            //gets first (only) thing from query list
                            Int32 accountID = query.First();
                            SavingAccount a = db.SavingAccounts.Find(accountID);
                            a.Balance = a.Balance - dvm.CorrectAmount + transToChange.Amount;
                        }

                        else if (GetAccountType(transToChange.FromAccount) == "IRA")
                        {
                            var query = from account in db.IRAccounts
                                        where account.AccountNum == transToChange.FromAccount
                                        select account.IRAccountID;
                            //gets first (only) thing from query list
                            String accountID = query.First();
                            IRAccount a = db.IRAccounts.Find(accountID);
                            a.Balance = a.Balance - dvm.CorrectAmount + transToChange.Amount;
                        }

                        else if (GetAccountType(transToChange.FromAccount) == "STOCKPORTFOLIO")
                        {
                            var query = from account in db.StockPortfolios
                                        where account.AccountNum == transToChange.FromAccount
                                        select account.StockPortfolioID;
                            //gets first (only) thing from query list
                            String accountID = query.First();
                            StockPortfolio a = db.StockPortfolios.Find(accountID);
                            a.Balance = a.Balance - dvm.CorrectAmount + transToChange.Amount;
                        }
                    }


                    disToChange.DisputeStatus = edvm.Status;
                    if (edvm.Comment != null)
                    {
                        disToChange.Message = disToChange.Message + "\n" + edvm.Comment;
                    }
                    transToChange.Amount = (decimal)edvm.AdjustedAmount;
                    transToChange.Description = "Dispute [Adjusted] " + transToChange.Description + edvm.Comment;

                    disToChange.ManResolvedEmail = user.Email;

                    db.Entry(disToChange).State = EntityState.Modified;
                    db.Entry(transToChange).State = EntityState.Modified;
                    db.SaveChanges();

                    return Content("<script language'javascript' type = 'text/javascript'> alert('You have successfuly adjusted the dispute!'); window.location='../../Manager/Home';</script>");
                }


            }

            var dis = from d in db.Disputes
                      select d;
            dis = dis.Where(d => d.DisputeID == edvm.DisputeID);

            Dispute dispute = dis.ToList()[0];
            //Apply this dispute's information to a dispute viewmodel 

            DisputeViewModel dvm2 = new DisputeViewModel();
            dvm2.CorrectAmount = dispute.DisputedAmount;
            dvm2.CustEmail = dispute.AppUser.Email;
            dvm2.FirstName = dispute.AppUser.FirstName;
            dvm2.LastName = dispute.AppUser.LastName;
            dvm2.Message = dispute.Message;
            dvm2.Status = dispute.DisputeStatus;
            dvm2.TransName = dispute.Transaction.TransactionID;
            dvm2.TransAmount = dispute.Transaction.Amount;
            dvm2.DisputeID = dispute.DisputeID;
            ViewBag.DisputeDetails = dvm2;
            return View(edvm);
        }

        public ActionResult ProcessBalancedPortfolios()
        {
            List<StockPortfolio> stockPortfolios = GetStockPortfolios();
            foreach (var sp in stockPortfolios)
            {
                if (BalanceCheck(sp) == true)
                {
                    Decimal value = sp.Balance;
                    //add bonus transaction 
                    foreach (StockDetail sd in sp.StockDetails)
                    {
                        //get current price of stock
                        StockQuote quote = GetQuote.GetStock(sd.Stock.StockTicker);
                        value = value + (quote.LastTradePrice * sd.Quantity);
                    }
                    foreach (Transaction trans in sp.Transactions)
                    {
                        if (trans.TransType == TransType.Fee)
                        {
                            value = value - trans.Amount;
                        }
                    }

                    //make new bonus transaction
                    Transaction t = new Transaction();
                    t.TransType = TransType.Bonus;
                    t.Amount = value * .1m;
                    t.Date = DateTime.Today;
                    t.ToAccount = sp.AccountNum;
                    t.Description = "Balanced Portfolio Bonus";
                    db.Transactions.Add(t);

                    var query = from a in db.StockPortfolios
                                where a.AccountNum == t.ToAccount
                                select a.StockPortfolioID;
                    //gets first (only) thing from query list
                    String accountID = query.First();
                    StockPortfolio account = db.StockPortfolios.Find(accountID);
                    account.Transactions.Add(t);
                    account.Balance = account.Balance + t.Amount;
                }
            }
            //ViewBag.TotalBonuses
            db.SaveChanges();
            return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Successfully added bonuses to Customers with balanced stock portfolios!'); window.location='../Manager/Home';</script>");
        }

        public ActionResult CreateStock()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StockName,StockTicker,TypeOfStock,FeeAmount")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                List<Stock> stocks = db.Stocks.ToList();
                foreach (Stock s in stocks)
                {
                    if (s.StockTicker == stock.StockTicker)
                    {
                        return Content("<script language'javascript' type = 'text/javascript'> alert('Error: You cannot add two of the same stock tickers'); window.location='../Manager/Home';</script>");
                    }
                }

                db.Stocks.Add(stock);
                db.SaveChanges();
                return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Successfully added a new stock!'); window.location='../Manager/Home';</script>");
            }
            return View(stock);
        }

        //Go to page to edit a customer's account 
        public ActionResult ChangeCustomerInfo()
        {
            ViewBag.AllCustomers = GetCustomers();
            ViewBag.SelectCustomer = SelectCustomer();

            return View();
        }

        //Post method for editing the customer's account 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeCustomerInfo([Bind(Include = "FirstName,MiddleInitial,LastName,Street,City,State,ZipCode,Birthday,Email,PhoneNumber")] EditUserViewModel evm, String id)
        {
            AppUser user = db.Users.Find(id);

            if (ModelState.IsValid)
            {
                user.Birthday = evm.Birthday;
                user.City = evm.City;
                user.Email = evm.Email;
                user.FirstName = evm.FirstName;
                user.LastName = evm.LastName;
                user.MiddleInitial = evm.MiddleInitial;
                user.PhoneNumber = evm.PhoneNumber;
                user.State = evm.State;
                user.Street = evm.Street;
                user.ZipCode = evm.ZipCode;

                db.SaveChanges();
                return Content("<script language'javascript' type = 'text/javascript'> alert('Successfully updated customer info!'); window.location='../Manager/Home';</script>");

            }

            return View(evm);

        }

        //Go to the view for selecting which customer you want to change the password for 
        public ActionResult ChangeCustomerPassword()
        {
            ViewBag.AllCustomers = GetCustomers();
            ViewBag.SelectCustomer = SelectCustomer();

            return View();
        }

        //Post method for changing a customer's password 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeCustomerPassword(String id, String newPassword)
        {
            AppDbContext db = new AppDbContext();
            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));

            //Get the user we want 
            var query = from user in db.Users
                        select user;
            query = query.Where(user => user.Id == id);
            List<AppUser> queryList = query.ToList();
            AppUser userInQuestion = queryList[0];

            String resetToken = userManager.GeneratePasswordResetToken(id);
            userManager.ResetPassword(id, resetToken, newPassword);

            db.Entry(userInQuestion).State = EntityState.Modified;
            db.SaveChanges();

            return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Successfully changed customer password!'); window.location='../Manager/Home';</script>");
        }

        //Go to page to edit a customer's account 
        public ActionResult ChangeEmployeeInfo()
        {
            ViewBag.AllEmployees = GetEmployees();
            ViewBag.SelectEmployee = SelectEmployee();

            return View();
        }

        //Post method for editing the customer's account 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeEmployeeInfo([Bind(Include = "FirstName,MiddleInitial,LastName,Street,City,State,ZipCode,Birthday,Email,PhoneNumber")] EditUserViewModel evm, String id)
        {
            AppUser user = db.Users.Find(id);

            if (ModelState.IsValid)
            {
                user.Birthday = evm.Birthday;
                user.City = evm.City;
                user.Email = evm.Email;
                user.FirstName = evm.FirstName;
                user.LastName = evm.LastName;
                user.MiddleInitial = evm.MiddleInitial;
                user.PhoneNumber = evm.PhoneNumber;
                user.State = evm.State;
                user.Street = evm.Street;
                user.ZipCode = evm.ZipCode;

                db.SaveChanges();
                return Content("<script language'javascript' type = 'text/javascript'> alert('Successfully updated employee info!'); window.location='../Manager/Home';</script>");

            }

            return View(evm);

        }

        //Go to the view for selecting which customer you want to change the password for 
        public ActionResult ChangeEmployeePassword()
        {
            ViewBag.AllEmployees = GetEmployees();
            ViewBag.SelectEmployee = SelectEmployee();

            return View();
        }

        //Post method for changing a customer's password 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeEmployeePassword(String id, String newPassword)
        {
            AppDbContext db = new AppDbContext();
            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));

            //Get the user we want 
            var query = from user in db.Users
                        select user;
            query = query.Where(user => user.Id == id);
            List<AppUser> queryList = query.ToList();
            AppUser userInQuestion = queryList[0];

            String resetToken = userManager.GeneratePasswordResetToken(id);
            userManager.ResetPassword(id, resetToken, newPassword);

            db.Entry(userInQuestion).State = EntityState.Modified;
            db.SaveChanges();

            return Content("<script language'javascript' type = 'text/javascript'> alert('Confirmation: Successfully changed employee password!'); window.location='../Manager/Home';</script>");
        }

        //Approve a stock portfolio for trading 
        public ActionResult ApprovePortfolio()
        {
            ViewBag.PortfoliosToApprove = PortfoliosToApprove();
            ViewBag.SelectPortfolios = MultiSelectStockPortfolios();

            return View();
        }
        //Post Method for approving portfolios
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApprovePortfolio(String[] selectedPortfolios)
        {
            foreach(String p in selectedPortfolios)
            {
                StockPortfolio portfolioInQuestion = db.StockPortfolios.Find(p);
                portfolioInQuestion.Disabled = true;

                db.SaveChanges();
            }

            return View();
        }














        //Get a list of all transactions 
        public List<Transaction> GetTrMasterList()
        {
            AppDbContext db = new AppDbContext();

            List<Transaction> returnList = db.Transactions.ToList();

            return returnList;
        }

        //Get a list of all transactions requiring manager approval
        public List<Transaction> GetTrToApprove()
        {
            AppDbContext db = new AppDbContext();

            List<Transaction> returnList = new List<Transaction>();
            var query = from t in db.Transactions
                        select t;
            query = query.Where(t => t.NeedsApproval == true);
            returnList = query.ToList();

            return returnList;
        }

        public ActionResult CurrentDisputes()
        {
            List<DisputeViewModel> currentDisputes = GetUnresolvedDisputes();
            return View(currentDisputes);
        }

        public List<DisputeViewModel> GetUnresolvedDisputes()
        {
            AppDbContext db = new AppDbContext();

            List<Dispute> disputeList = new List<Dispute>();
            var query = from d in db.Disputes
                        select d;
            query = query.Where(d => d.DisputeStatus == DisputeStatus.Submitted);
            disputeList = query.ToList();

            List<DisputeViewModel> dvmList = new List<DisputeViewModel>();
            foreach (Dispute d in disputeList)
            {
                DisputeViewModel dvm = new DisputeViewModel();
                dvm.CorrectAmount = d.DisputedAmount;
                dvm.FirstName = d.AppUser.FirstName;
                dvm.LastName = d.AppUser.LastName;
                dvm.TransAmount = d.Transaction.Amount;
                dvm.Message = d.Message;
                dvm.CustEmail = d.AppUser.Email;
                dvm.TransName = d.Transaction.TransactionID;
                dvm.DisputeID = d.DisputeID;
                dvm.Status = DisputeStatus.Submitted;
                dvmList.Add(dvm);
            }


            return dvmList;
        }

        public ActionResult AllDisputes()
        {
            List<DisputeViewModel> allDisputes = GetAllDisputes();
            return View(allDisputes);
        }

        public List<DisputeViewModel> GetAllDisputes()
        {
            AppDbContext db = new AppDbContext();

            List<Dispute> disputeList = db.Disputes.ToList();

            List<DisputeViewModel> dvmList = new List<DisputeViewModel>();
            foreach (Dispute d in disputeList)
            {
                DisputeViewModel dvm = new DisputeViewModel();
                dvm.CorrectAmount = d.DisputedAmount;
                dvm.FirstName = d.AppUser.FirstName;
                dvm.LastName = d.AppUser.LastName;
                dvm.TransAmount = d.Transaction.Amount;
                dvm.Message = d.Message;
                dvm.CustEmail = d.AppUser.Email;
                dvm.TransName = d.Transaction.TransactionID;
                dvm.DisputeID = d.DisputeID;
                dvm.Status = d.DisputeStatus;

                dvmList.Add(dvm);
            }

            return dvmList;
        }
                

        //get a list of all the user objects for employees 
        public List<AppUser> GetEmployees()
        {
            AppDbContext db = new AppDbContext();

            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            List<AppUser> employeeList = new List<AppUser>();

            foreach(AppUser user in db.Users)
            {
                if(userManager.GetRoles(user.Id).Contains("Employee"))
                {
                    employeeList.Add(user);
                }
            }

            
            return employeeList;
        }

        //Make a select list for all of the employees a manager could choose to promote or fire
        public IEnumerable<SelectListItem> SelectEmployee()
        {
            List<AppUser> employees = GetEmployees();
            SelectList selectEmployee = new SelectList(employees.OrderBy(e => e.Email), "Id", "Email");
            return selectEmployee;
        }


        //Get a list of all the customers in the db 
        public List<AppUser> GetCustomers()
        {
            AppDbContext db = new AppDbContext();

            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            List<AppUser> customerList = new List<AppUser>();

            foreach (AppUser user in db.Users)
            {
                if (userManager.GetRoles(user.Id).Contains("Customer"))
                {
                    customerList.Add(user);
                }
            }


            return customerList;
        }

        //Make a select list for all of the customeres a manager could choose to disable 
        public IEnumerable<SelectListItem> SelectCustomer()
        {
            List<AppUser> customers = GetCustomers();
            SelectList selectCustomer = new SelectList(customers.OrderBy(c => c.Email), "Id", "Email");
            return selectCustomer;
        }

        public List<AppUser> GetDisabledCustomers()
        {
            AppDbContext db = new AppDbContext();

            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            List<AppUser> customerList = new List<AppUser>();

            foreach (AppUser user in db.Users)
            {
                if (userManager.GetRoles(user.Id).Contains("Customer") && user.Disabled == true)
                {
                    customerList.Add(user);
                }
            }


            return customerList;
        }

        public IEnumerable<SelectListItem> SelectDisabledCustomer()
        {
            List<AppUser> customers = GetDisabledCustomers();
            SelectList selectCustomer = new SelectList(customers, "Id", "Email");
            return selectCustomer;
        }

        //get a list of all stock portfolios
        public List<StockPortfolio> GetStockPortfolios()
        {
            List<StockPortfolio> stockPortfolios = db.StockPortfolios.ToList();
            return stockPortfolios;
        }

        //returns true if portfolio is balanced
        public Boolean BalanceCheck(StockPortfolio sp)
        {
            //Get a list of all the stocks in the account 
            List<Stock> stockList = new List<Stock>();

            foreach (StockDetail s in sp.StockDetails)
            {
                stockList.Add(s.Stock);
            }

            //Counts to keep track of each stock type in the account 
            Int32 numOrdinary = new Int32();
            Int32 numIndex = new Int32();
            Int32 numMutual = new Int32();

            foreach (Stock stock in stockList)
            {
                if (stock.TypeOfStock == StockType.Ordinary)
                {
                    numOrdinary += 1;
                }
                else if (stock.TypeOfStock == StockType.Index_Fund)
                {
                    numIndex += 1;
                }
                else if (stock.TypeOfStock == StockType.Mutual_Fund)
                {
                    numMutual += 1;
                }
            }

            //Check if the account qualifies 
            if (numOrdinary >= 2 && numIndex >= 1 && numMutual >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public String GetAccountType(Int32? accountNum)
        {
            String accountType;

            List<CheckingAccount> checkingAccounts = db.CheckingAccounts.ToList();
            foreach (var c in checkingAccounts)
            {
                if (accountNum == c.AccountNum)
                {
                    accountType = "CHECKING";
                    return accountType;
                }
            }

            List<SavingAccount> savingAccounts = db.SavingAccounts.ToList();
            foreach (var s in savingAccounts)
            {
                if (accountNum == s.AccountNum)
                {
                    accountType = "SAVING";
                    return accountType;
                }
            }

            List<IRAccount> iraAccounts = db.IRAccounts.ToList();
            if (iraAccounts != null)
            {
                foreach (var i in iraAccounts)
                {
                  if (accountNum == i.AccountNum)
                    {
                        accountType = "IRA";
                        return accountType;
                    }
                }
            }


            List<StockPortfolio> stockPortfolios = db.StockPortfolios.ToList();
            if (iraAccounts != null)
            {
                foreach (var s in stockPortfolios)
                {
                    if (accountNum == s.AccountNum)
                    {
                        accountType = "STOCKPORTFOLIO";
                        return accountType;
                    }
                }
            }

            return "NOT FOUND";
        }

        public List<StockViewModel> PortfolioSnapshot()
        {
            //Get the ID of the user who is currently logged in
            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            var user = userManager.FindById(User.Identity.GetUserId());

            //Look at each StockDetail in the person's account and make a StockViewModel to campture information about it 
            //Get a list of all the stocks in the account 
            List<StockDetail> stockDetailList = new List<StockDetail>();
            foreach (StockDetail s in user.StockPortfolio.StockDetails)
            {
                stockDetailList.Add(s);
            }

            //Make list to hold all the stockviewmodel objects
            List<StockViewModel> listToReturn = new List<StockViewModel>();

            //Add information from each stock record into the viewbag 
            foreach (StockDetail detail in stockDetailList)
            {
                StockViewModel model = new StockViewModel();
                model.Name = detail.Stock.StockName;
                model.NumInAccount = detail.Quantity;
                model.Ticker = detail.Stock.StockTicker;

                StockQuote quote = bevo.Utilities.GetQuote.GetStock(detail.Stock.StockTicker);

                model.CurrentPrice = quote.LastTradePrice;

                //Add the model to the list of models already in the viewbag
                listToReturn.Add(model);
            }

            return listToReturn;
        }

        public List<StockPortfolio> PortfoliosToApprove()
        {
            List<StockPortfolio> returnList = db.StockPortfolios.Where(s => s.Disabled == true).ToList();

            return returnList;
        }

        public IEnumerable<SelectListItem> MultiSelectStockPortfolios()
        {
            List<StockPortfolio> portfolios = PortfoliosToApprove();

            SelectList selectPortfolioList = new SelectList(portfolios, "StockPortfolioID", "AccountName");

            return selectPortfolioList;
        }

    }
}
