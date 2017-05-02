using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bevo.Models
{
    public class TransactionSearchViewModel
    {
        public String Description { get; set; }

        public TransType? Type { get; set; }

        public Decimal? Amount { get; set; }

        public Int32? TransNum { get; set; }

        public DateTime? Date { get; set; }
    }
}