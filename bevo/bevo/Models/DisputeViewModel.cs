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
        public String CustNum { get; set; }
        public Decimal? TransAmount { get; set; }
        public Int32? TransName { get; set; }
        public Decimal? CorrectAmount { get; set; }
        public String Message { get; set; }
        public String ManEmail { get; set; }


    }
}