using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Identity.IdentityModels
{
    public class WMSIdentityUserLogin : IdentityUserLogin<int>
    {
        #region Properties 
        public int? UserLoginId { get; set; } 
        public bool? IsActive { get; set; }
        public bool? IsLocked { get; set; }
       
        #endregion
    }
}
