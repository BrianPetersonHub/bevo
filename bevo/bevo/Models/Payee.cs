using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace team1_longhornBank.Models
{
    public enum PayeeType
    {
        Credit_Card,
        Utilities,
        Rent,
        Mortgage,
        Other
    }
    public class Payee
    {
        public Int32 PayeeID { get; set; }

        public String Name { get; set; }

        public String Street { get; set; }

        public String City { get; set; }

        public String Zipcode { get; set; }

        public String PhoneNumber { get; set; }

        public PayeeType PayeeType { get; set; }
    }
}