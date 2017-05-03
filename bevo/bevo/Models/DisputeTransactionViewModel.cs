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
        [Required(ErrorMessage = "Message is a Required field.")]
        public String Message { get; set; }
        [Required(ErrorMessage = "Disputed amount is a Required field.")]
        [Display(Name = "Correct amount")]
        public Decimal DisputedAmount { get; set; }
        public Int32 TransactionID { get; set; }

    }
}