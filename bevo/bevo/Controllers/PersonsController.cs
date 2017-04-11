using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bevo.Models;
namespace bevo.Controllers
{
    public class PersonsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Persons
        public ActionResult Index()
        {
            return View(); 
        }

        //GET: Persons/Create
        public ActionResult Create()
        {
            return View();
        }

        //POST: Persons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PersonID,FirstName,MiddleInitial,LastName,Street,City,State,ZipCode,Email,PhoneNumber,BirthDay,Password")] Person person)
        {
            if (ModelState.IsValid)
            {
                db.Persons.Add(person);
                db.SaveChanges();
                return RedirectToAction("ChooseAccount");
            }
            return View(person);
        }

        public ActionResult ChooseAccount()
        {
            return View();
        }

        public ActionResult CustomerHome()
        {
            return View();
        }
    }
}