using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Identity.IdentityModels;

namespace WMS.Identity.IdentityManager
{
    public class WMSIdentitySignInManager : SignInManager<WMSIdentityUser, int>
    {
        public WMSIdentitySignInManager(WMSIdentityUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public static WMSIdentitySignInManager Create(IdentityFactoryOptions<WMSIdentitySignInManager> options, IOwinContext context)
        {
            return new WMSIdentitySignInManager(context.GetUserManager<WMSIdentityUserManager>(), context.Authentication);
        }
    }
}
