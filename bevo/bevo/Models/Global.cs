using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bevo.Models
{
    //make a static class called Global. This class has one property in it which is called AccountID
    //this property has a default value of 1000000000, which is the first account ID number that we 
    //will have. Every time we call a constructor on any of the account classes, we will use the set
    //method on the property to increment it up by one and then we will assign that value to that 
    //controller's AccountID property. 
    public static class Global
    {
        public static Int32 AccountID { get; set; } = 1000000000;
    }
}