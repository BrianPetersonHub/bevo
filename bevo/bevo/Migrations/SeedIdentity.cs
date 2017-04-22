using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using bevo.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.VisualBasic;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Validation;

namespace bevo.Migrations
{
    public class SeedIdentity
    {
        public static AppDbContext db = new AppDbContext();
        public static void SeedManager(AppDbContext db)
        {
            //create a user manager to add users to databases
            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));

            //create a role manager 
            AppRoleManager roleManager = new AppRoleManager(new RoleStore<AppRole>(db));

            //create an admin role 
            String roleName = "Manager";

            //check if role exists 
            if (roleManager.RoleExists(roleName) == false) //role doesn't exist
            {
                roleManager.Create(new AppRole(roleName));
            }

            //create user
            String strEmail = "admin2@example.com";
            AppUser user = new AppUser()
            {
                //TODO: Replace this with useful information. Might have to read from a csv 
                UserName = strEmail,
                Email = strEmail,
                FirstName = "Admin",
                MiddleInitial = "Q",
                LastName = "Example",
                PhoneNumber = "5125551234",
                State = "TX",
                Street = "123 Admin Street",
                City = "Austin",
                ZipCode = "12345",
                Birthday = "1/1/1901"
                //Major = Major.MIS,
                //OKToText = true
            };

            //see if user is already there 
            AppUser userToAdd = userManager.FindByName(strEmail);
            if (userToAdd == null) //this user doesn't exist yet
            {
                userManager.Create(user, "Password123!");
                userToAdd = userManager.FindByName(strEmail);

                //add user to the role 
                if (userManager.IsInRole(userToAdd.Id, roleName) == false) //the user isn't in the role
                {
                    userManager.AddToRole(userToAdd.Id, roleName);
                }
            }


        }



        public static void SeedPerson(AppDbContext db, String[] seedPerson)
        {
                //create a user manager to add users to databases
                UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));

                //create a role manager 
                AppRoleManager roleManager = new AppRoleManager(new RoleStore<AppRole>(db));

                String roleName = seedPerson[11];

                //check if role exists 
                if (roleManager.RoleExists(roleName) == false) //role doesn't exist
                {
                    roleManager.Create(new AppRole(roleName));
                }

                //Manually hash the password 
                //string Password = seedPerson[4].ToString();
                //var passwordHash = new PasswordHasher();
                //string hashedPassword = passwordHash.HashPassword(Password);

                string password = seedPerson[4];
                string strUserName = seedPerson[0];

            //var user = userManager.Find(strUserName, password);

            //if (user != null)
            //{
            //    return;
            //}


            var user = new AppUser()
                {
                    //Id = Guid.NewGuid().ToString(),
                    UserName = seedPerson[0].ToString(),
                    Email = seedPerson[0],
                    FirstName = seedPerson[1],
                    MiddleInitial = seedPerson[2],
                    LastName = seedPerson[3],
                    Street = seedPerson[5],
                    City = seedPerson[6],
                    State = seedPerson[7],
                    ZipCode = seedPerson[8],
                    Birthday = seedPerson[10],
                    PhoneNumber = seedPerson[9]
                    //PasswordHash = hashedPassword
                };

            //var result = userManager.Create(user, password);

            //if (result.Succeeded)
            //{
            //    userManager.AddToRole(user.Id, roleName);
            //}
            //else
            //{
            //    var e = new Exception("Could not add default account.");

            //    throw e;
            //}

            //see if user is already there 
            AppUser userToAdd = userManager.FindByEmail(strUserName);
            if (userToAdd == null) //this user doesn't exist yet
            {
                userManager.Create(user, password);
                userToAdd = userManager.FindByEmail(strUserName);
                string identification = user.Id;

                userManager.AddToRole(identification, roleName);
            }


        }


        public static void ReadPersonCSV()
        {
            //call manager seeding to seed in the one manager who is in charge of all this nonsense 
            SeedManager(db);

            //look through the csv and add in users for each other people in there 
            string[] allLines = File.ReadAllLines(@"C:\Users\James Abbott\Desktop\MIS 333 Seed Data\Person.csv");
            foreach (String line in allLines)
            {
                String[] record = line.Split(',');
                SeedPerson(db, record);
            }
        }

        public static void AddDebugger()
        {
            if (System.Diagnostics.Debugger.IsAttached == false)
            {

                System.Diagnostics.Debugger.Launch();

            }
        }
    }
}
