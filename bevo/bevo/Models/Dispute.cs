using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bevo.Models
{
    public enum DisputeStatus
    {
        Submitted,
        Accepted,
        Rejected,
        Adjusted
    }
    public class Dispute
    {
        [Key, ForeignKey("Transaction")]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Int32 DisputeId { get; set; }

        public DisputeStatus DisputeStatus { get; set; }

        public String Message { get; set; }

        [Display(Name = "Correct amount")]
        public Decimal DisputedAmount { get; set; }



        //Nav properties 
        public virtual Transaction Transaction { get; set; }
    }
}