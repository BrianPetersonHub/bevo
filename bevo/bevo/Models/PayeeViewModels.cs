using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using bevo.Models;

namespace bevo.Models
{
    public class PayeeViewModel
    {
        public Int32 PayeeID { get; set; }
        public String PayeeName { get; set; }
        public PayeeType Type { get; set; }
    }
}