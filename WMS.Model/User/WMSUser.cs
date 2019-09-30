using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Model.AccessControl;

namespace WMS.Model.User
{
    public class RecoverPassword
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
    public class WMSActiveUser
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Telephone { get; set; }
        public bool IsActive { get; set; }
    }
    public class WMSAddress
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public WMSCountry Country { get; set; }
    }

    public class WMSActiveCustomer
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }

    }
    public class WMSCountry
    {
        public int CountryId { get; set; }
        public string CountryPhoneCode { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Code2 { get; set; }
        public int TimezoneId { get; set; }
        public int Rank { get; set; }
    }

    public class WMSChangePassword
    {
        public string UserName { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

    }
    public class TrackUser
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int TakeRows { get; set; }
        public int CurrentPage { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string keyword { get; set; }
        public string StyleCode { get; set; }
    }

    public class RecoveryEmail
    {
        public string UserName { get; set; }
    }
    public class WMSInternalUser : WMSUser
    {
        public WMSAddress Address { get; set; }
         public WMSCustomer Additional { get; set; }
    }
    public class WMSUserGrid
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string RoleDisplay { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string ManagerName { get; set; }
        public string ManagerEmail { get; set; }
        public string StyleCode { get; set; }
    }

    public class WMSUserLoginDetail
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string Email { get; set; }
        public string PhotoUrl { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public List<UserAccessLevelMenu> Modules { get; set; }
    }

    public class WMSUserCredential
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class WMSUser
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string CompanyName { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string TelephoneNo { get; set; }
        public string MobileNo { get; set; }
        public string FaxNumber { get; set; }
        public TimeZoneModel Timezone { get; set; }
        public string ShortName { get; set; }
        public string Position { get; set; }
        public int CustomerId { get; set; }
        public string Skype { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreatedOnUtc { get; set; }
        public int CreatedBy { get; set; }
        public string ProfileImage { get; set; }
        public string UserCode { get; set; }
        public string AccountNumber { get; set; }
    }
    public class TimeZoneModel
    {
        public int TimezoneId { get; set; }
        public string Name { get; set; }
    }

    public class WMSUserAdditionalModel
    {
        public List<WMSUser> SalesCoOrdinatorUser { get; set; }
        public List<WMSUser> MechandiseUser { get; set; }
        public List<WMSUser> WarehouseUser { get; set; }
        public List<WMSUser> SalesRepresentative { get; set; }
    }
}
