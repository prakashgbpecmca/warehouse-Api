using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Identity.IdentityDataAccess;
using WMS.Identity.IdentityModels;
using WMS.Identity.IdentityService;

namespace WMS.Identity.IdentityManager
{
    public class WMSIdentityUserManager : UserManager<WMSIdentityUser, int>
    {
        #region constructors and destructors
        public WMSIdentityUserManager(IUserStore<WMSIdentityUser, int> store) : base(store)
        {
        }
        #endregion

        #region methods
        public static WMSIdentityUserManager Create(IdentityFactoryOptions<WMSIdentityUserManager> options, IOwinContext context)
        {
            var manager = new WMSIdentityUserManager(new UserStore<WMSIdentityUser, WMSIdentityRole, int, WMSIdentityUserLogin, WMSIdentityUserRole, WMSIdentityUserClaim>(context.Get<WMSIdentityDbContext>()));

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<WMSIdentityUser, int>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false

            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                RequireDigit = false,
                RequireLowercase = false,
                RequireNonLetterOrDigit = false,
                RequireUppercase = false,

            };
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider(
                "PhoneCode",
                new PhoneNumberTokenProvider<WMSIdentityUser, int>
                {
                    MessageFormat = "Your security code is: {0}"
                });
            manager.RegisterTwoFactorProvider(
                "EmailCode",
                new EmailTokenProvider<WMSIdentityUser, int>
                {
                    Subject = "Security Code",
                    BodyFormat = "Your security code is: {0}"
                });

            manager.EmailService = new WMSIdentityEmailService();
            manager.SmsService = new WMSIdentitySmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<WMSIdentityUser, int>(dataProtectionProvider.Create("ASP.NET Identity"));
            }


            return manager;
        }


        #endregion
    }
}
