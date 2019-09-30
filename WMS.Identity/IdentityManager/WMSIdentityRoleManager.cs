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

namespace WMS.Identity.IdentityManager
{
    public class WMSIdentityRoleManager : RoleManager<WMSIdentityRole, int>
    {
        public WMSIdentityRoleManager(IRoleStore<WMSIdentityRole, int> store) : base(store)
        {

        }
        public static WMSIdentityRoleManager Create(IdentityFactoryOptions<WMSIdentityRoleManager> options, IOwinContext context)
        {
            var manager = new WMSIdentityRoleManager(new RoleStore<WMSIdentityRole, int, WMSIdentityUserRole>(context.Get<WMSIdentityDbContext>()));
            return manager;
        }

    }
}
