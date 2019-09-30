using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Model.User;

namespace WMS.Model.Common
{
    public class MasterModel
    {
        public List<WMSCountry> Countries { get; set; }
        public List<WMSCountryPostCode> CountryPostCodes { get; set; }
        public List<WMSActiveUser> Users { get; set; }
        public List<WMSUserRoleType> Roles { get; set; }
        public List<WMSCustomreGrid> Customers { get; set; }
    }

    public class WMSCountryPostCode
    {
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public string PhoneCode { get; set; }
    }
    public class WMSUserRoleType
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }
}
