using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Model.User
{
    public class Currency
    {
        public string CurrencyCode { get; set; }
        public string CurrencyDescription { get; set; }
    }

    public class WMSCustomer : WMSUser
    {
        public WMSAddress Address { get; set; }
        public associateUserGroup associateUserGroup { get; set; }
        public CustomerAssociatedUser AccountUser { get; set; }
        public CustomerAssociatedUser SalesCoOrdinatorUser { get; set; }
        public CustomerAssociatedUser MechandiseUser { get; set; }
        public CustomerAssociatedUser WarehouseUser { get; set; }
        public CustomerAssociatedUser SalesRepresentative { get; set; }
    }
    public class associateUserGroup
    {
        public int salesUser { get; set; }
        public int salesCoOrdinator { get; set; }
        public int merchandiseuser { get; set; }
        public int warehouseUser { get; set; }
    }
    public class CustomerAssociatedUser
    {
        public int UserId { get; set; }
        public string AssociateType { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string TelephoneNo { get; set; }
        public string CompanyName { get; set; }
        public string Role { get; set; }
        public string RoleDisplay { get; set; }
    }

    public class WMSCustomreGrid
    {
        public int UserId { get; set; }
        public string AccountNo { get; set; }
        public string Email { get; set; }
        public string TelephoneNo { get; set; }
        public string CompanyName { get; set; }
        public string Country { get; set; }
        public string ContactName { get; set; }
        public string SalesReprestative { get; set; } 
        public string Merchandiser { get; set; }
    }

}
