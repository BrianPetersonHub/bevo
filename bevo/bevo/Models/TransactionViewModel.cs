using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace bevo.Models
{
    public class TransactionListViewModel
    {
        public Int32 TransactionID { get; set; }
        public String Description { get; set; }
        public Decimal Amount { get; set; }

        [Display(Name = "Date (MM/DD/YYYY)")]
        public DateTime Date { get; set; }

        public Int32 TransactionNum { get; set; }

        public TransType TransType { get; set; }

    }
}