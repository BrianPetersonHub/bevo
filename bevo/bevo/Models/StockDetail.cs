﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace bevo.Models
{
    public class StockDetail
    {
        public Int32 StockDetailID { get; set; }
        public Int32 Quantity { get; set; }
    }
}