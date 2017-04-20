using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bevo.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        public ActionResult Home()
        {
            //TODO: make sure this is returning the Customer/Home view
            return View();
        }
    }
}