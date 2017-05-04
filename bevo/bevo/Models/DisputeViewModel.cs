using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bevo.Models
{
    public class DisputeViewModel
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String CustEmail { get; set; }
        public Decimal? TransAmount { get; set; }
        public Int32? TransName { get; set; }
        public Decimal CorrectAmount { get; set; }
        public String Message { get; set; }
        public String ManEmail { get; set; }
        public DisputeStatus Status { get; set; }
        public Int32? DisputeID { get; set; }

    }

    public class EditDisputeViewModel
    {
        public Int32? DisputeID { get; set; }
        public DisputeStatus Status { get; set; }
        public String Comment { get; set; }
        public Decimal? AdjustedAmount { get; set; }
    }
}