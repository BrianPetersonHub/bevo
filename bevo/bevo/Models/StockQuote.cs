using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bevo.Models
{
    public class StockQuote
    {
            public String Symbol { get; set; }
            public String Name { get; set; }
            public Decimal PreviousClose { get; set; }
            public Decimal LastTradePrice { get; set; }
            public Decimal Volume { get; set; }
    }
}
