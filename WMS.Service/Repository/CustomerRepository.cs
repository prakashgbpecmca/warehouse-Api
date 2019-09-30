using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Model.User;
using WMS.Service.DataAccess;

namespace WMS.Service.Repository
{
    public class CustomerRepository
    {
        WMSEntities dbContext = new WMSEntities();

      public List<WMSCustomreGrid>  GetCustomerCompanyName(TrackUser model)
        {
            List<WMSCustomreGrid> list = new List<WMSCustomreGrid>();
            var data = (from r in dbContext.WMSUsers
                        join ur in dbContext.WMSUserRoles on r.Id equals ur.UserId
                        where r.IsActive == true && ur.RoleId == 3

                        select new WMSCustomreGrid
                        {   UserId =r.Id,
                            CompanyName = r.CompanyName
                        });
            list = data.OrderBy(x => x.CompanyName).ToList();
            return list;

        }

        public List<WMSCustomreGrid> GetCustomers(TrackUser model) // 4th in stock allocation
        {
            List<WMSCustomreGrid> list = new List<WMSCustomreGrid>();

            var data = (from r in dbContext.WMSUsers
                        join ua in dbContext.WMSUserAddresses on r.Id equals ua.UserId into add
                        from ua in add.DefaultIfEmpty()
                        join c in dbContext.Countries on ua.CountryId equals c.CountryId into cntry
                        from c in cntry.DefaultIfEmpty()
                        join uadd in dbContext.WMSUserAdditionals on r.Id equals uadd.UserId into temp
                        from uadd in temp.DefaultIfEmpty()
                        join u in dbContext.WMSUsers on uadd.MerchandiseUserId equals u.Id into tempCordi
                        from u in tempCordi.DefaultIfEmpty()
                        join ur in dbContext.WMSUserRoles on r.Id equals ur.UserId
                        where r.IsActive == true && ur.RoleId==3
                        select new WMSCustomreGrid
                        {
                            AccountNo = "",
                            CompanyName = r.CompanyName,
                            ContactName = r.ContactFirstName + " " + r.ContactLastName,
                            Country = c.CountryName,
                            Email = r.Email,
                            TelephoneNo = "(+ " + c.CountryPhoneCode + " )" + r.TelephoneNo,
                            UserId = r.Id,
                            Merchandiser = u.ContactFirstName + " " + u.ContactLastName
                            //SalesReprestative = u.ContactFirstName + " " + u.ContactLastName
                        });
            if (model.keyword != "" && model.keyword != null)
            {
                data = data.Where(x => x.ContactName.ToLower().Contains(model.keyword.ToLower())
                || x.CompanyName.ToLower().Contains(model.keyword.ToLower())
                || x.Country.ToLower().Contains(model.keyword.ToLower())
                || x.Merchandiser.ToLower().Contains(model.keyword.ToLower())
                || x.Email.ToLower().Contains(model.keyword.ToLower()));
            }
            list = data.OrderByDescending(x => x.UserId).ToList();
            return list;
        }

        public WMSCustomer GetUser(int userId)
        {
            WMSCustomer user = new WMSCustomer();

            // User detail 
            var dbUser = dbContext.WMSUsers.Find(userId);
            if (dbUser != null)
            {
                user.UserId = dbUser.Id;
                user.UserName = dbUser.Email;
                user.ContactFirstName = dbUser.ContactFirstName;
                user.ContactLastName = dbUser.ContactLastName;
                user.CompanyName = dbUser.CompanyName ;
                user.Timezone = new TimeZoneModel();
                user.Timezone.TimezoneId = dbUser.TimezoneId;
                user.MobileNo = dbUser.MobileNo;
                user.PhoneNumber = dbUser.TelephoneNo;
                user.Position = dbUser.Position;
                user.ProfileImage = "staff.png";
                user.TelephoneNo = dbUser.TelephoneNo;
                user.Skype = dbUser.Skype;
                user.ShortName = dbUser.ShortName;
                user.Email = dbUser.Email;
                user.UserName = dbUser.UserName;
                user.CreatedOnUtc = DateTime.UtcNow;
                user.CreatedBy = dbUser.CreatedBy;
                user.IsActive = true;
            }

            // User Role
            var userRole = dbContext.WMSUserRoles.Where(p => p.UserId == userId).FirstOrDefault();
            if (userRole != null)
            {
                user.RoleId = userRole.RoleId;
            }

            // User Address
            user.Address = new WMSAddress();
            var dbAddress = dbContext.WMSUserAddresses.Where(p => p.UserId == userId).FirstOrDefault();
            if (dbAddress != null)
            {
                user.Address.UserId = dbAddress.UserId;
                user.Address.Address = dbAddress.Address;
                user.Address.Address2 = dbAddress.Address2;
                user.Address.Address3 = dbAddress.Address3;
                user.Address.Area = dbAddress.Suburb;
                user.Address.City = dbAddress.City;
                user.Address.Country = new WMSCountry();

                var dbCountry = dbContext.Countries.Find(dbAddress.CountryId);
                if (dbCountry != null)
                {
                    user.Address.Country.CountryId = dbCountry.CountryId;
                    user.Address.Country.Name = dbCountry.CountryName;
                    user.Address.Country.Code = dbCountry.CountryCode;
                    user.Address.Country.Code2 = dbCountry.CountryCode2;

                }
                user.Address.Id = dbAddress.Id;
                user.Address.PostCode = dbAddress.PostCode;
                user.Address.State = dbAddress.State;
            }

            // User Additional
            var userAdditional = dbContext.WMSUserAdditionals.Where(p => p.UserId == userId).FirstOrDefault();
            user.AccountNumber = userAdditional.AccountNumber;
            if (userAdditional != null)
            {
                // Sales representative  
                user.SalesRepresentative = (from r in dbContext.WMSUsers
                                            join a in dbContext.WMSUserAddresses on r.Id equals a.UserId
                                            join c in dbContext.Countries on a.CountryId equals c.CountryId
                                            join ur in dbContext.WMSUserRoles on r.Id equals ur.UserId
                                            join rr in dbContext.WMSRoles on ur.RoleId equals rr.Id
                                            where r.Id == userAdditional.SalesUserId
                                            select new CustomerAssociatedUser
                                            {
                                                CompanyName = r.CompanyName,
                                                ContactName = r.ContactFirstName + " " + r.ContactLastName,
                                                Email = r.Email,
                                                Role = rr.Name,
                                                RoleDisplay = rr.DisplayName,
                                                TelephoneNo = "(+ " + c.CountryPhoneCode + " )" + r.TelephoneNo,
                                                UserId = r.Id,

                                            }
                                     ).FirstOrDefault();

                // Sales Co-ordinator
                user.SalesCoOrdinatorUser = (from r in dbContext.WMSUsers
                                             join a in dbContext.WMSUserAddresses on r.Id equals a.UserId
                                             join c in dbContext.Countries on a.CountryId equals c.CountryId
                                             join ur in dbContext.WMSUserRoles on r.Id equals ur.UserId
                                             join rr in dbContext.WMSRoles on ur.RoleId equals rr.Id
                                             where r.Id == userAdditional.SalesCoOrdinatorId
                                             select new CustomerAssociatedUser
                                             {
                                                 CompanyName = r.CompanyName,
                                                 ContactName = r.ContactFirstName + " " + r.ContactLastName,
                                                 Email = r.Email,
                                                 Role = rr.Name,
                                                 RoleDisplay = rr.DisplayName,
                                                 TelephoneNo = "(+ " + c.CountryPhoneCode + " )" + r.TelephoneNo,
                                                 UserId = r.Id
                                             }
                                   ).FirstOrDefault();

                // Merchandise user
                user.MechandiseUser = (from r in dbContext.WMSUsers
                                       join a in dbContext.WMSUserAddresses on r.Id equals a.UserId
                                       join c in dbContext.Countries on a.CountryId equals c.CountryId
                                       join ur in dbContext.WMSUserRoles on r.Id equals ur.UserId
                                       join rr in dbContext.WMSRoles on ur.RoleId equals rr.Id
                                       where r.Id == userAdditional.MerchandiseUserId
                                       select new CustomerAssociatedUser
                                       {
                                           CompanyName = r.CompanyName,
                                           ContactName = r.ContactFirstName + " " + r.ContactLastName,
                                           Email = r.Email,
                                           Role = rr.Name,
                                           RoleDisplay = rr.DisplayName,
                                           TelephoneNo = "(+ " + c.CountryPhoneCode + " )" + r.TelephoneNo,
                                           UserId = r.Id
                                       }
                                   ).FirstOrDefault();

                // Waewhouse user
                user.WarehouseUser = (from r in dbContext.WMSUsers
                                      join a in dbContext.WMSUserAddresses on r.Id equals a.UserId
                                      join c in dbContext.Countries on a.CountryId equals c.CountryId
                                      join ur in dbContext.WMSUserRoles on r.Id equals ur.UserId
                                      join rr in dbContext.WMSRoles on ur.RoleId equals rr.Id
                                      where r.Id == userAdditional.WarehouseUserId
                                      select new CustomerAssociatedUser
                                      {
                                          CompanyName = r.CompanyName,
                                          ContactName = r.ContactFirstName + " " + r.ContactLastName,
                                          Email = r.Email,
                                          Role = rr.Name,
                                          RoleDisplay = rr.DisplayName,
                                          TelephoneNo = "(+ " + c.CountryPhoneCode + " )" + r.TelephoneNo,
                                          UserId = r.Id
                                      }
                                   ).FirstOrDefault();

            }

            return user;
        }
    }
}
