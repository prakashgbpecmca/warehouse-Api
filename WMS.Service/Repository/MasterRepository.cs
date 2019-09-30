using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Model.Common;
using WMS.Model.User;
using WMS.Service.DataAccess;

namespace WMS.Service.Repository
{
    public class MasterRepository
    {
        WMSEntities dbContext = new WMSEntities();

        public MasterModel Initials()
        {
            MasterModel model = new MasterModel();

            model.Countries = Countries();
            model.CountryPostCodes = GetCountryPhoneCodeList();
            model.Roles = GetSystemRole();
            model.Customers = new StockRespository().GetStockRangeCustomers(1);
            return model;
        }

        internal List<WMSActiveUser> GetWarehouseUsers()
        {
            var users = (from r in dbContext.WMSUsers
                         join ur in dbContext.WMSUserRoles on r.Id equals ur.UserId
                         where ur.RoleId == (int)WMSUserRoleEnum.Warehouse && r.IsActive == true
                         select new WMSActiveUser
                         {
                             UserId = r.Id,
                             IsActive = r.IsActive,
                             Email = r.Email,
                             Name = r.ContactFirstName + " " + r.ContactLastName,
                         }).ToList();

            return users;
        }

        public List<WMSUserRoleType> GetSystemRole()
        {
            List<WMSUserRoleType> list = new List<WMSUserRoleType>();
            var roles = dbContext.WMSRoles.ToList();

            if (roles.Count > 0)
            {
                WMSUserRoleType role;

                foreach (var item in roles)
                {
                    role = new WMSUserRoleType();
                    role.RoleId = item.Id;
                    role.Name = item.Name;
                    role.DisplayName = item.DisplayName;

                    list.Add(role);

                }
            }
            return list;
        }

        public List<WMSCountryPostCode> GetCountryPhoneCodeList()
        {
            var lstCountry = dbContext.Countries.ToList();

            List<WMSCountryPostCode> lstCountryPhoneCodes = new List<WMSCountryPostCode>();

            foreach (Country country in lstCountry)
            {
                WMSCountryPostCode countryPhoneCode = new WMSCountryPostCode();

                countryPhoneCode.Name = country.CountryName;
                countryPhoneCode.PhoneCode = country.CountryPhoneCode;
                countryPhoneCode.CountryCode = country.CountryCode;
                lstCountryPhoneCodes.Add(countryPhoneCode);
            }

            return lstCountryPhoneCodes;
        }
        public List<WMSCountry> Countries()
        {
            List<WMSCountry> li = new List<WMSCountry>();
            var lstCountry = (from r in dbContext.Countries
                              where r.Rank == 1
                              select new WMSCountry
                              {
                                  Name = r.CountryName,
                                  Code = r.CountryCode,
                                  Code2 = r.CountryCode2,
                                  CountryId = r.CountryId,
                                  Rank = r.Rank
                              }).OrderBy(x => x.Rank).ThenBy(x => x.Name).ToList();
            li.AddRange(lstCountry);
            li.Add(new WMSCountry
            {
                Name = "--------------------------------------------------------------------------------",
                Code = "",
                Code2 = "",
                CountryId = -1,
                Rank = 0
            });
            lstCountry = (from r in dbContext.Countries
                          where r.Rank == 2
                          select new WMSCountry
                          {
                              Name = r.CountryName,
                              Code = r.CountryCode,
                              Code2 = r.CountryCode2,
                              CountryId = r.CountryId,
                              Rank = r.Rank
                          }).OrderBy(x => x.Rank).ThenBy(x => x.Name).ToList();
            li.AddRange(lstCountry);
            return li;
        }
    }
}
