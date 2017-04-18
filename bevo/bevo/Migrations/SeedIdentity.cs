using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using bevo.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.VisualBasic;


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
                LastName = "Example",
                PhoneNumber = "(512)555Â1234",
                Major = Major.MIS,
                OKToText = true
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

            String roleName = seedPerson[12];

            //check if role exists 
            if (roleManager.RoleExists(roleName) == false) //role doesn't exist
            {
                roleManager.Create(new AppRole(roleName));
            }

            AppUser user = new AppUser()
            {
                UserName = seedPerson[1],
                Email = seedPerson[1],
                FirstName = seedPerson[2],
                MiddleInitial = seedPerson[3],
                LastName = seedPerson[4],
                Street = seedPerson[6],
                City = seedPerson[7],
                State = seedPerson[8],
                ZipCode = seedPerson[9],
                Birthday = seedPerson[11],
                PhoneNumber = seedPerson[10],
            };

            //see if user is already there 
            AppUser userToAdd = userManager.FindByName(strEmail);
            if (userToAdd == null) //this user doesn't exist yet
            {
                userManager.Create(user, seedPerson[5]);
                userToAdd = userManager.FindByName(seedPerson[1]);

                //add user to the role 
                if (userManager.IsInRole(userToAdd.Id, roleName) == false) //the user isn't in the role
                {
                    userManager.AddToRole(userToAdd.Id, roleName);
                }
            }

        }


        public static void ReadPersonCSV()
        {
            string allLines = File.ReadAllLines(@"C:\Users\James Abbott\Desktop\MIS 333 Seed Data\Person.csv");
            foreach (String line in allLines)
            {
                record = line.Split(",");
                SeedPerson(db, record);
            }
        }
    }
}
