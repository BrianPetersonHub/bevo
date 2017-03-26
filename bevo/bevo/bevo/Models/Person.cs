using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace team1_longhornBank.Models
{
    public enum PersonType
    {
        Manager,
        Employee,
        Customer
    }

    public class Person
    {
        public Int32 PersonID { get; set; }

        public PersonType PersonType { get; set; }

        public bool Enabled { get; set; }

        public String FirstName { get; set; }

        public Char MiddleInitial { get; set; }

        public String LastName { get; set; }

        public String Street { get; set; }

        public String City { get; set; }

        public String State { get; set; }

        public String ZipCode { get; set; }

        public String Email { get; set; }

        public String Password { get; set; }

        public String PhoneNumber { get; set; }

        public String Birthday { get; set; }

    }
}