using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public Int32 DisputeID { get; set; }

        public DisputeStatus DisputeStatus { get; set; }

        public String Message { get; set; }

        public Decimal DisputedAmount { get; set; }
    }
}