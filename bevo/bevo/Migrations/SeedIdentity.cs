using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using bevo.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.VisualBasic;
using Microsoft.AspNet.Identity;

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

            AppUser user = new AppUser()
            {
                UserName = seedPerson[0].ToString(),
                Email = seedPerson[0].ToString(),
                FirstName = seedPerson[1].ToString(),
                MiddleInitial = seedPerson[2].ToString(),
                LastName = seedPerson[3].ToString(),
                Street = seedPerson[5].ToString(),
                City = seedPerson[6].ToString(),
                State = seedPerson[7].ToString(),
                ZipCode = seedPerson[8].ToString(),
                Birthday = seedPerson[10].ToString(),
                PhoneNumber = seedPerson[9].ToString()
            };

            string strUserName = seedPerson[0].ToString();

            //see if user is already there 
            AppUser userToAdd = userManager.FindByName(strUserName);
            if (userToAdd == null) //this user doesn't exist yet
            {
                string Password = seedPerson[4].ToString();
                userManager.Create(user, Password);
                userToAdd = userManager.FindByName(strUserName);

                //add user to the role 
                if (userManager.IsInRole(userToAdd.Id, roleName) == false) //the user isn't in the role
                {
                    userManager.AddToRole(userToAdd.Id, roleName);
                }
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
