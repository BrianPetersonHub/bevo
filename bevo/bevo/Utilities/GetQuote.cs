using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using bevo.Models;
using System.Net;
using System.IO;

namespace bevo.Utilities
{
    public static class GetQuote
    {
        // Get stock quote by passing symbol
        public static StockQuote GetStock(string symbol)
        {
            //s -> symbol
            //g -> days low
            //h -> days high
            //c -> change
            //o -> open
            //p -> previous close
            //v -> volume
            //l1 -> last trade (price only) -> close
            //n -> name
            string baseURL = "http://finance.yahoo.com/d/quotes.csv?s={0}&f=npl1v";
            string url = string.Format(baseURL, symbol);

            //Get page showing the table with the chosen indices
            System.Net.HttpWebRequest request = null;

            //csv content
            string docText = string.Empty;
            StockQuote stock = null;
            try
            {
                request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
                request.Timeout = 300000;

                using (var response = (HttpWebResponse)request.GetResponse())
                using (StreamReader stReader = new StreamReader(response.GetResponseStream()))
                {
                    string output = stReader.ReadLine();
                    //"\"Apple Inc.\",587.44,572.98,36820544"

                    string[] sa = output.Split(new char[] { ',' });

                    stock = new StockQuote();
                    stock.Symbol = symbol;
                    stock.Name = sa[0];
                    stock.PreviousClose = decimal.Parse(sa[1]);
                    stock.LastTradePrice = decimal.Parse(sa[2]);
                    stock.Volume = decimal.Parse(sa[3]);
                }
            }
            catch (Exception e)
            {
                //Throw some exception here.
            }
            return stock;
        }

        public static Decimal HistoricalStockPrice(String ticker, DateTime date)
        {
            String monthNum = DateNumbers(date)[0].ToString();
            String dayNum = DateNumbers(date)[1].ToString();
            String yearNum = DateNumbers(date)[2].ToString();


            String urlToPullFrom = "http://ichart.yahoo.com/table.csv?s=" + ticker + "&a=" + monthNum + "&b=" + dayNum +
                                   "&c=" + yearNum + "&d=" + monthNum + "&e=" + dayNum + "&f=" + yearNum + "&g=d&ignore=.csv";


            //Get page showing the table with the chosen indices
            System.Net.HttpWebRequest request = null;

            //csv content
            string docText = string.Empty;
            Decimal closePrice = new Decimal();
            try
            {
                request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(urlToPullFrom));
                request.Timeout = 300000;

                using (var response = (HttpWebResponse)request.GetResponse())
                using (StreamReader stReader = new StreamReader(response.GetResponseStream()))
                {
                    stReader.ReadLine();
                    string output = stReader.ReadLine();
                    //"\"Apple Inc.\",587.44,572.98,36820544"

                    string[] sa = output.Split(new char[] { ',' });

                    foreach(string i in sa)
                    {
                        String TestVar = i.ToString();
                    }

                    closePrice = Decimal.Parse(sa[1]);
                }
            }
            catch
            {
                //Don't have a catch block
            }

            return closePrice;

        }



        public static List<String> DateNumbers(DateTime date)
        {
            List<String> returnList = new List<String>(); 

            if(date.Month == 1)
            {
                returnList.Add("0");
            }
            else if(date.Month == 2)
            {
                returnList.Add("1");
            }
            else if(date.Month ==3)
            {
                returnList.Add("2");
            }
            else if(date.Month == 4)
            {
                returnList.Add("3");
            }
            else if(date.Month == 5)
            {
                returnList.Add("4");
            }
            else if(date.Month == 6)
            {
                returnList.Add("5");
            }
            else if(date.Month == 7)
            {
                returnList.Add("6");
            }
            else if(date.Month == 8)
            {
                returnList.Add("7");
            }
            else if(date.Month == 9)
            {
                returnList.Add("8");
            }
            else if(date.Month == 10)
            {
                returnList.Add("9");
            }
            else if(date.Month == 11)
            {
                returnList.Add("10");
            }
            else if(date.Month ==12)
            {
                returnList.Add("11");
            }

            returnList.Add(date.Day.ToString());

            returnList.Add(date.Year.ToString());

            return returnList;
        }


    }
}