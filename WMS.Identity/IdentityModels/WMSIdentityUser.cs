using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WMS.Identity.IdentityManager;

namespace WMS.Identity.IdentityModels
{
    public class WMSIdentityUser : IdentityUser<int, WMSIdentityUserLogin, WMSIdentityUserRole, WMSIdentityUserClaim>
    {
        #region properties   
        public string CompanyName { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; } 
        public string TelephoneNo { get; set; }
        public string MobileNo { get; set; }
        public int TimezoneId { get; set; }
        public string ShortName { get; set; }
        public string Position { get; set; }
        public string Skype { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
        public int? UpdatedBy { get; set; } 
        public DateTime? LastLoginOnUtc { get; set; }
        public DateTime? LastPasswordChangedOnUtc { get; set; }
        public string ProfileImage { get; set; }
        #endregion
        public async Task<ClaimsIdentity> GenerateUserIdentity(WMSIdentityUserManager userManager, string authenticationType)
        {
            // Add custom user claims here
            return await userManager.CreateIdentityAsync(this, authenticationType);
        }
    }

}
