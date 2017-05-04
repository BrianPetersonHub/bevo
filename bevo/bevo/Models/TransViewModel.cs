using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bevo.Models
{
    public class TransViewModel
    {
        public Int32 TransactionID { get; set; }
        public Int32 TransactionNum { get; set; }
        public TransType TransType { get; set; }
        public Decimal Amount { get; set; }
        public Int32? toAccount { get; set; }
        public Int32? fromAccount { get; set; }
        public DateTime Date { get; set; }
        public String Description { get; set; }

        public String FirstName { get; set; }
        public String LastName { get; set; }
    }
}