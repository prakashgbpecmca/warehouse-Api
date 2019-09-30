using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WMS.Model.ApiResponse;
using WMS.Model.User;
using WMS.Service.DataAccess;

namespace WMS.Service.Repository
{
    public class WMSUserRepository
    {
        WMSEntities dbContext = new WMSEntities();

        #region User
        public bool SaveAddress(WMSAddress address)
        {
            WMSUserAddress dbAddress;
            try
            {
                if (address.Id == 0)
                {
                    dbAddress = new WMSUserAddress();
                    dbAddress.Address = address.Address;
                    dbAddress.Address2 = address.Address2;
                    dbAddress.Address3 = address.Address3;
                    dbAddress.City = address.City;
                    dbAddress.CountryId = address.Country.CountryId;
                    dbAddress.PostCode = address.PostCode;
                    dbAddress.State = address.State;
                    dbAddress.City = address.City;
                    dbAddress.Suburb = address.Area;
                    dbAddress.UserId = address.UserId;
                    dbContext.WMSUserAddresses.Add(dbAddress);
                    dbContext.SaveChanges();
                }
                else
                {
                    dbAddress = dbContext.WMSUserAddresses.Find(address.Id);
                    if (dbAddress != null)
                    {
                        dbAddress.Address = address.Address;
                        dbAddress.Address2 = address.Address2;
                        dbAddress.Address3 = address.Address3;
                        dbAddress.City = address.City;
                        dbAddress.CountryId = address.Country.CountryId;
                        dbAddress.PostCode = address.PostCode;
                        dbAddress.State = address.State;
                        dbAddress.City = address.City;
                        dbAddress.Suburb = address.Area;
                        dbContext.SaveChanges();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public WMSUserProfile ProfileDetail(int userId)
        {

            WMSUserProfile profile = (from r in dbContext.WMSUsers
                                      join ua in dbContext.WMSUserAddresses on r.Id equals ua.UserId
                                      join c in dbContext.Countries on ua.CountryId equals c.CountryId
                                      where r.Id == userId
                                      select new WMSUserProfile
                                      {
                                          Name = r.ContactFirstName + " " + r.ContactLastName,
                                          CompanyName = r.CompanyName,
                                          imageUrl = r.ProfileImage,
                                          Address = new WMSAddress
                                          {
                                              Address = ua.Address,
                                              Address2 = ua.Address2,
                                              City = ua.City,
                                              Area = ua.Suburb,
                                              Country = new WMSCountry
                                              {
                                                  Code = c.CountryCode,
                                                  Code2 = c.CountryCode2,
                                                  CountryId = c.CountryId,
                                                  Name = c.CountryName
                                              },
                                              Phone = "(+" + c.CountryPhoneCode + ") " + r.TelephoneNo,
                                              PostCode = ua.PostCode,
                                              State = ua.State
                                          },
                                          Email = r.Email,
                                          UserId = r.Id
                                      }).FirstOrDefault();

            return profile;

        }

        public int GetCountryTimeZone(int countryId)
        {
            var country = dbContext.Countries.Find(countryId);
            if (country != null)
            {
                return country.CountryId;
            }
            return 0;
        }

        public List<WMSActiveUser> ActiveUsers(string term)
        {
            var users = (from r in dbContext.WMSUsers
                         join ua in dbContext.WMSUserAddresses on r.Id equals ua.UserId
                         join c in dbContext.Countries on ua.CountryId equals c.CountryId
                         where r.ContactFirstName.Contains(term) || r.ContactLastName.Contains(term)
                         select new WMSActiveUser
                         {
                             Email = r.Email,
                             Name = r.ContactFirstName + " " + r.ContactLastName,
                             Telephone = "(+ " + c.CountryPhoneCode + ")" + r.TelephoneNo,
                             UserId = r.Id
                         }
                         ).ToList();

            return users;
        }

        public WMSInternalUser GetUser(int userId)
        {
            WMSInternalUser user = new WMSInternalUser();


            //Step 1: User detail 
            var dbUser = dbContext.WMSUsers.Find(userId);

            if (dbUser != null)
            {
                user.UserId = dbUser.Id;
                user.UserName = dbUser.Email;
                user.ContactFirstName = dbUser.ContactFirstName;
                user.ContactLastName = dbUser.ContactLastName;
                user.CompanyName = dbUser.CompanyName;
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

            var dbUserAdditional = dbContext.WMSUserAdditionals.Where(p => p.UserId == userId).FirstOrDefault();
            if (dbUserAdditional != null)
            {
                user.UserCode = dbUserAdditional.UserCode;
                user.CustomerId = dbUserAdditional.CustomerId.HasValue ? dbUserAdditional.CustomerId.Value : 0;
            }

            return user;
        }

        public List<WMSUserGrid> GetUsers(TrackUser model)
        {
            try
            {
                var data = (from u in dbContext.WMSUsers
                            join ur in dbContext.WMSUserRoles on u.Id equals ur.UserId
                            join add in dbContext.WMSUserAddresses on u.Id equals add.UserId
                            join c in dbContext.Countries on add.CountryId equals c.CountryId
                            join r in dbContext.WMSRoles on ur.RoleId equals r.Id
                            //join ua in dbContext.WMSUserAdditionals on u.Id equals ua.UserId
                            //join mu in dbContext.WMSUsers on ua.ManageUserId equals mu.Id
                            where u.IsActive == true && ur.RoleId != 3 && (model.RoleId == 0 || r.Id == model.RoleId)

                            select new WMSUserGrid
                            {
                                UserId = u.Id,
                                Email = u.Email,
                                //  ManagerName = mu.ContactFirstName + " " + mu.ContactLastName,
                                //  ManagerEmail = mu.Email,
                                Name = u.ContactFirstName + " " + u.ContactLastName,
                                Role = r.Name,
                                RoleDisplay = r.DisplayName,
                                Telephone = "(+" + c.CountryPhoneCode + ") " + u.TelephoneNo
                            });
                if (model.RoleId > 0 && model.keyword != "")
                {
                    data = data.Where(x => x.Name.ToLower().Contains(model.keyword.ToLower())
                     || x.RoleDisplay.ToLower().Contains(model.keyword.ToLower())
                     || x.Email.ToLower().Contains(model.keyword.ToLower()));
                }

                List<WMSUserGrid> list = data.ToList();

                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public ApiResponse DeleteUser(int userId)
        {
            ApiResponse result = new ApiResponse();

            try
            {
                var user = dbContext.WMSUsers.Find(userId);
                if (user != null)
                {
                    user.IsActive = false;
                    dbContext.SaveChanges();
                    result.Status = true;
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        public bool SaveUserAddtional(WMSCustomer model)
        {
            try
            {
                WMSUserAdditional dbAdditional;
                dbAdditional = dbContext.WMSUserAdditionals.Where(p => p.UserId == model.UserId).FirstOrDefault();
                if (dbAdditional != null)
                {
                    dbAdditional.SalesCoOrdinatorId = model.associateUserGroup.salesCoOrdinator;
                    dbAdditional.SalesUserId = model.associateUserGroup.salesUser;
                    dbAdditional.MerchandiseUserId = model.associateUserGroup.merchandiseuser;
                    dbAdditional.WarehouseUserId = model.associateUserGroup.warehouseUser;
                    dbAdditional.UserId = model.UserId;

                    dbContext.SaveChanges();
                }
                else
                {
                    string acc = ISexistAccountNo();
                    dbAdditional = new WMSUserAdditional();
                    dbAdditional.SalesCoOrdinatorId = model.associateUserGroup.salesCoOrdinator;
                    dbAdditional.SalesUserId = model.associateUserGroup.salesUser;
                    dbAdditional.MerchandiseUserId = model.associateUserGroup.merchandiseuser;
                    dbAdditional.WarehouseUserId = model.associateUserGroup.warehouseUser;
                    dbAdditional.UserId = model.UserId;
                    dbAdditional.PaymentCurrency = "INR";
                    dbAdditional.CreditLimit = 1000;
                    dbAdditional.AccountNumber = acc;

                    dbContext.WMSUserAdditionals.Add(dbAdditional);
                    dbContext.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool SaveUserAddtional(WMSInternalUser model)
        {
            try
            {
                WMSUserAdditional dbAdditional;
                dbAdditional = dbContext.WMSUserAdditionals.Where(p => p.UserId == model.UserId).FirstOrDefault();
                if (dbAdditional != null)
                {
                    dbAdditional.CustomerId = model.CustomerId;
                    dbAdditional.UserId = model.UserId;
                    dbAdditional.UserCode = model.UserCode;
                    dbContext.SaveChanges();
                }
                else
                {
                    dbAdditional = new WMSUserAdditional();
                    dbAdditional.CustomerId = model.CustomerId;
                    dbAdditional.UserId = model.UserId;
                    dbAdditional.PaymentCurrency = "INR";
                    dbAdditional.CreditLimit = 1000;
                    dbAdditional.AccountNumber = "";
                    dbAdditional.UserCode = model.UserCode;
                    dbContext.WMSUserAdditionals.Add(dbAdditional);
                    dbContext.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int GetUserRole(int id)
        {
            var role = dbContext.WMSUserRoles.Where(p => p.UserId == id).FirstOrDefault();
            if (role != null)
            {
                return role.RoleId;
            }
            return 0;
        }
        public WMSUserLoginDetail LoginUserDetail(int id)
        {
            var user = (from u in dbContext.WMSUsers
                        join ur in dbContext.WMSUserRoles on u.Id equals ur.UserId
                        where u.Id == id
                        select new WMSUserLoginDetail
                        {
                            UserId = u.Id,
                            Email = u.Email,
                            Name = u.ContactFirstName + " " + u.ContactLastName,
                            PhotoUrl = "",
                            UserName = u.UserName,
                            RoleId = ur.RoleId
                        }
                        ).FirstOrDefault();

            return user;
        }

        public string uploadProfileImage(int userid, string filename)
        {
            try
            {
                var data = dbContext.WMSUsers.Where(x => x.Id == userid).FirstOrDefault();
                if (data != null)
                {
                    data.ProfileImage = filename;
                    dbContext.SaveChanges();
                }
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        public List<Model.User.WMSUser> GetUsersByRoleID(int roleID)
        {
            try
            {
                List<Model.User.WMSUser> list = (from u in dbContext.WMSUsers
                                                 join ur in dbContext.WMSUserRoles on u.Id equals ur.UserId
                                                 where u.IsActive == true && ur.RoleId == roleID
                                                 select new Model.User.WMSUser
                                                 {
                                                     UserId = u.Id,
                                                     Email = u.Email,
                                                     ContactFirstName = u.ContactFirstName,
                                                     ContactLastName = u.ContactLastName
                                                 }).ToList();
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public WMSUserAdditionalModel UserAdditonalInitials()
        {
            WMSUserAdditionalModel model = new WMSUserAdditionalModel();

            model.SalesRepresentative = GetUsersByRoleID(2);
            model.SalesCoOrdinatorUser = GetUsersByRoleID(4);
            model.MechandiseUser = GetUsersByRoleID(5);
            model.WarehouseUser = GetUsersByRoleID(6);
            return model;
        }

        public string ISexistAccountNo()
        {
            Random rnd = new Random();
            string acc = rnd.Next(100000000, 999999999).ToString();
            var data = dbContext.WMSUserAdditionals.Where(x => x.AccountNumber == acc).FirstOrDefault();
            if (data != null)
            {
                return ISexistAccountNo();
            }
            else { return acc; }
        }
    }
}
