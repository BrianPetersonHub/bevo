﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bevo.Models
{
    public class CheckingAccount
    {
        public int32 AccountID { get; set; }
        public String AccountName { get; set; }
        public Decimal Balance { get; set; }
    }
}