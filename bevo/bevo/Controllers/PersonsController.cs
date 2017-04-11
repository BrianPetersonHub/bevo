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
        public ActionResult Create([Bind(Include = "Id,FirstName,MiddleInitial,LastName,Street,City,State,ZipCode,Email,PhoneNumber,Birthday,Password")] AppUser user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("ChooseAccount");
            }
            return View(user);
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