using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bevo.Models
{
    public class DisputeTransactionViewModel
    {
        [Key, ForeignKey("Transaction")]
        public Int32 DisputeID { get; set; }

        public DisputeStatus DisputeStatus { get; set; }

        public String Message { get; set; }

        [Display(Name = "Correct amount")]
        public Decimal DisputedAmount { get; set; }
        public Int32 TransactionID { get; set; }

    }
}