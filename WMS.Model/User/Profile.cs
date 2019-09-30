using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Model.User
{
    public class WMSUserProfile
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public WMSAddress Address { get; set; } 
        public string imageUrl { get; set; }
    }

    public class Credentials
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

}
