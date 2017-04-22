using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bevo.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc.Html;

namespace bevo.Controllers
{
    public enum Range
    {
        [Display(Name = "0-100")]
        One,

        [Display(Name = "100-200")]
        Two,

        [Display(Name = "200-300")]
        Three,

        [Display(Name = "300+")]
        Four,

        [Display(Name = "Custom")]
        Custom
    }

    public enum Date
    {
        [Display(Name = "Last 15 days")]
        One,

        [Display(Name = "Last 30 days")]
        Two,

        [Display(Name = "Last 60 days")]
        Three,

        All,

        Custom
    }
    public class TransactionsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: All transactions
        // For employees and managers 
        public ActionResult Index()
        {
            return View();
        }

        // GET: Transactions/Index/#
        public ActionResult Index(int? id)
        {
            //TODO: only get transactions associated with customer id
            return View();
        }

        // Search Results
        public ActionResult SearchTransactions( String description,
                                                int selectedTransType,
                                                Range selectedRange,
                                                String transactionNumber,
                                                Date selectedDate )
        {

            //TODO: start query
            var query = from t in db.Transactions
                        select t;

            //TODO: Description textbox search
            if (description != null && description != "")
            {
                query = query.Where(t => t.Description.Contains(description));
            }

            //TODO: Dropdown selected transaction type
            var transTypeList = EnumHelper.GetSelectList(typeof(TransType));
            if (selectedTransType == 0)
            {
                // query all 
            }
            else
            {
                // query selected transaction type
            }

            //TODO: Radio buttons selected range
            switch (selectedRange)
            {
                case Range.One:
                    // query
                    break;
                case Range.Two:
                    // query
                    break;
                case Range.Three:
                    // query
                    break;
                case Range.Four:
                    // query
                    break;
                case Range.Custom:
                    // query
                    //TODO: how to handle custom input?
                    break;
            }

            //TODO: Transaction number textbox search
            if (transactionNumber != null && transactionNumber != "")
            {
                Int32 intTransactionNumber;
                try
                {
                    intTransactionNumber = Convert.ToInt32(transactionNumber);
                }

                catch
                {

                }
                // query
            }

            //TODO: Radio buttons selected date
            switch (selectedDate)
            {
                case Date.One:
                    // query
                    break;
                case Date.Two:
                    // query
                    break;
                case Date.Three:
                    // query
                    break;
                case Date.All:
                    // query
                    break;
                case Date.Custom:
                    //query
                    //TODO: how to handle custom paramater
                    break;
            }

        }

    }
}