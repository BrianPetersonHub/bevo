//This is a test 
//I'm going to change some stuff around here. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace team1_longhornBank.Models
{
    public enum AccountType
    {
        Checkings,
        Savings,
        IRA,
        Stock_Portfolio
    }

    public class Account
    {
        public Int32 AccountID { get; set; }

        public AccountType AccountType { get; set; }

        public String AccountName { get; set; }

        public Decimal Balance { get; set; }


    }
}