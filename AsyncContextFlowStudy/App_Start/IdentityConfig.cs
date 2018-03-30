using System.Security.Claims;
using System.Threading.Tasks;
using AsyncContextFlowStudy.App_Start;
using AsyncContextFlowStudy.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace AsyncContextFlowStudy
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new HardcodedUserStore(new[]
            {
                new ApplicationUser
                {
                    Id = "herp",
                    Email = "herp@example.com",
                    PasswordHash = HashPassword("sekret1"),
                    UserName = "Herp",
                },
                new ApplicationUser
                {
                    Id = "derp",
                    Email = "derp@example.com",
                    PasswordHash = HashPassword("sekret1"),
                    UserName = "Derp",
                },
            }));

            return manager;
        }

        static string HashPassword(string password)
        {
            var hasher = new PasswordHasher();
            return hasher.HashPassword(password);
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
