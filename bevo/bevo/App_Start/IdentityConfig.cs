﻿using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using bevo.Models;
using Microsoft.Owin.Security.DataProtection;

namespace bevo
{

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class AppUserManager : UserManager<AppUser>
    {
        public AppUserManager(IUserStore<AppUser> store)
            : base(store)
        {
        }


        //DONE:  Change any settings related to your user password rules, etc.
        public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options, IOwinContext context) 
        {
            var manager = new AppUserManager(new UserStore<AppUser>(context.Get<AppDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<AppUser>(manager)
            {
                //DONE: If you want to use email as the username, you need to make sure usernames can have @ 
                //and that each person has a unique email
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            //NOTE: Stan added this
            var provider = new DpapiDataProtectionProvider("bevo");
            manager.UserTokenProvider = new DataProtectorTokenProvider<AppUser, string>(provider.Create("UserToken"))
                as IUserTokenProvider<AppUser, string>;

            // DONE: Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 3,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<AppUser, string>
    {
        public ApplicationSignInManager(AppUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(AppUser user)
        {
            return user.GenerateUserIdentityAsync((AppUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<AppUserManager>(), context.Authentication);
        }
    }
}
