using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Identity.IdentityModels
{
    public class WMSIdentityRole : IdentityRole<int, WMSIdentityUserRole>
    {
        #region properties 
        public string DisplayName { get; set; }
        #endregion 
    }
}
