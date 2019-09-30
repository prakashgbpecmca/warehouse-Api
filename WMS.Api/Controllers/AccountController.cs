using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using WMS.Api.Models;
using WMS.Api.Providers;
using WMS.Api.Results;
using WMS.Identity.IdentityManager;
using WMS.Model.User;
using WMS.Identity.IdentityModels;
using WMS.Services.Utility;
using WMS.Service.Repository;
using WMS.Model.Email;
using WMS.Model.ApiResponse;
using System.Net;

namespace WMS.Api.Controllers
{
    public class AccountController : ApiController
    {

        #region -- Member Vars --

        private WMSIdentityUserManager _userManager;
        private WMSIdentityRoleManager _roleManager;
        private WMSIdentitySignInManager _signInManager;

        #endregion

        #region -- Properties --

        public WMSIdentitySignInManager SignInManager
        {
            get
            {
                return _signInManager ?? Request.GetOwinContext().Get<WMSIdentitySignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public WMSIdentityUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<WMSIdentityUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public WMSIdentityRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? Request.GetOwinContext().Get<WMSIdentityRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return Request.GetOwinContext().Authentication;
            }
        }

        #endregion

        #region -- Constructors and Destructors --

        public AccountController()
        {
        }

        #endregion

        #region User 
        [Route("Account/RegisterUser")]
        [HttpPost]
        public async Task<IHttpActionResult> RegisterUser(WMSInternalUser model)
        {
            WMSIdentityUser user;
            try
            {
                int timeZoneId = 0;

                if (model.Address != null && model.Address.Country != null)
                {
                    timeZoneId = new WMSUserRepository().GetCountryTimeZone(model.Address.Country.CountryId);
                }
                if (model.UserId == 0)
                {
                    user = new WMSIdentityUser();
                    user.UserName = model.Email;
                    user.ContactFirstName = model.ContactFirstName;
                    user.ContactLastName = model.ContactLastName;
                    user.CompanyName = model.CompanyName;


                    user.TimezoneId = timeZoneId;
                    user.MobileNo = model.MobileNo;
                    user.PhoneNumber = model.TelephoneNo;
                    user.Position = model.Position;
                    user.ProfileImage = "staff.png";
                    user.TelephoneNo = model.TelephoneNo;
                    user.Skype = model.Skype;
                    user.ShortName = model.ShortName;
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    user.CreatedOnUtc = DateTime.UtcNow;
                    user.CreatedBy = model.CreatedBy;
                    user.IsActive = true;
                    Random rnd = new Random();
                    string paasword = rnd.Next(10000000, 99999999).ToString();
                    var result = await UserManager.CreateAsync(user, paasword);

                    if (result.Succeeded)
                    {
                        //  identity user role 
                        if ((UserManager.AddToRole(user.Id, (await RoleManager.FindByIdAsync(model.RoleId)).Name)).Succeeded)
                        {
                            model.UserId = user.Id;
                            model.Address.UserId = user.Id;

                            new WMSUserRepository().SaveAddress(model.Address);

                            // user addityiobnal for sales representative

                            new WMSUserRepository().SaveUserAddtional(model);

                            //  Credential email to user  
                            new EmailRepository().Email_E1(new EmailE1 { UserName = user.UserName, Email = user.UserName, UserId = user.Id, Password = paasword });
                        }
                    }
                }
                else
                {
                    user = await UserManager.FindByIdAsync(model.UserId);
                    if (user != null)
                    {
                        user.UserName = model.Email;
                        user.ContactFirstName = model.ContactFirstName;
                        user.ContactLastName = model.ContactLastName;
                        user.CompanyName = model.CompanyName;

                        user.TimezoneId = timeZoneId;

                        //  user.FaxNumber = model.FaxNumber;
                        user.MobileNo = model.MobileNo;
                        user.PhoneNumber = model.TelephoneNo;
                        user.Position = model.Position;
                        user.ProfileImage = "staff.png";
                        user.TelephoneNo = model.TelephoneNo;
                        user.Skype = model.Skype;
                        user.ShortName = model.ShortName;
                        user.Email = model.Email;
                        user.UserName = model.UserName;
                        user.UpdatedOnUtc = DateTime.UtcNow;
                        user.UpdatedBy = model.CreatedBy;
                        user.IsActive = true;
                        var UpdateResult = await UserManager.UpdateAsync(user);
                        if (UpdateResult.Succeeded)
                        {
                            var role = await RoleManager.FindByIdAsync(model.RoleId);
                            if (!(await UserManager.IsInRoleAsync(user.Id, role.Name)))
                            {
                                var roles = await UserManager.GetRolesAsync(user.Id);
                                if (roles != null)
                                {
                                    foreach (var userRole in roles)
                                    {
                                        await UserManager.RemoveFromRoleAsync(user.Id, userRole);
                                    }
                                }
                                await UserManager.AddToRoleAsync(user.Id, role.Name);
                            }
                            new WMSUserRepository().SaveAddress(model.Address);
                            new WMSUserRepository().SaveUserAddtional(model);
                            model.Address.UserId = user.Id;


                        }
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        //[HttpGet]
        //public IHttpActionResult GetUser(string UserName, string Password)
        //{
        //    if (!string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password))
        //    {
        //        if (UserName.ToLower() == "customer" && Password.ToLower() == "customer@1234")
        //        {
        //            ValidateUser.UserName = "customer";
        //            return Ok(true);
        //        }
        //        else
        //        {
        //            ValidateUser.UserName = null;
        //            return Ok(false);
        //        }
        //    }
        //    else
        //    {
        //        ValidateUser.UserName = null;
        //        return Ok(false);
        //    }
        //}

        [Route("Account/GetUser")]
        [HttpGet]
        public WMSInternalUser GetUser(int userId)
        {
            WMSInternalUser user;

            user = new WMSUserRepository().GetUser(userId);

            return user;
        }
        [Route("Account/GetUsers")]
        [HttpPost]
        public List<WMSUserGrid> GetUsers(TrackUser model)
        {

            List<WMSUserGrid> list = new WMSUserRepository().GetUsers(model);
            return list;
        }

        [HttpDelete]
        public IHttpActionResult DeleteUser(int userId)
        {
            ApiResponse result = new WMSUserRepository().DeleteUser(userId);

            return Ok(result);
        }

        #endregion

        #region Customer
        [Route("Account/RegisterCustomer")]
        [HttpPost]
        public async Task<IHttpActionResult> RegisterCustomer(WMSCustomer model)
        {
            WMSIdentityUser user;
            try
            {
                int timeZoneId = 0;
                if (model.Address != null && model.Address.Country != null)
                {
                    timeZoneId = new WMSUserRepository().GetCountryTimeZone(model.Address.Country.CountryId);
                }

                model.RoleId = 3;
                if (model.UserId == 0)
                {
                    user = new WMSIdentityUser();
                    user.UserName = model.Email;
                    user.ContactFirstName = model.ContactFirstName;
                    user.ContactLastName = model.ContactLastName;
                    user.CompanyName = model.CompanyName;
                    user.TimezoneId = timeZoneId;// model.Timezone.TimezoneId;
                    user.MobileNo = model.MobileNo;
                    user.PhoneNumber = model.TelephoneNo;
                    user.Position = model.Position;
                    user.ProfileImage = "staff.png";
                    user.TelephoneNo = model.TelephoneNo;
                    user.Skype = model.Skype;
                    user.ShortName = model.ShortName;
                    user.Email = model.Email;
                    //  user.UserName = model.UserName;
                    user.CreatedOnUtc = DateTime.UtcNow;
                    user.CreatedBy = model.CreatedBy;
                    user.IsActive = true;
                    Random rnd = new Random();
                    string paasword = rnd.Next(10000000, 99999999).ToString();
                    var result = await UserManager.CreateAsync(user, paasword);

                    if (result.Succeeded)
                    {
                        //  identity user role 
                        if ((UserManager.AddToRole(user.Id, (await RoleManager.FindByIdAsync(model.RoleId)).Name)).Succeeded)
                        {
                            model.Address.UserId = user.Id;
                            new WMSUserRepository().SaveAddress(model.Address);

                            //user additional
                            model.UserId = user.Id;
                            bool d = new WMSUserRepository().SaveUserAddtional(model);

                            //  Credential email to user  
                            new EmailRepository().Email_E1(new EmailE1 { UserName = user.UserName, Email = user.UserName, UserId = user.Id, Password = paasword });
                        }
                    }
                }
                else
                {
                    user = await UserManager.FindByIdAsync(model.UserId);
                    if (user != null)
                    {
                        user.UserName = model.Email;
                        user.ContactFirstName = model.ContactFirstName;
                        user.ContactLastName = model.ContactLastName;
                        user.CompanyName = model.CompanyName;

                        user.TimezoneId = timeZoneId;//model.Timezone.TimezoneId;
                        //  user.FaxNumber = model.FaxNumber;
                        user.MobileNo = model.MobileNo;
                        user.PhoneNumber = model.TelephoneNo;
                        user.Position = model.Position;
                        //   user.ProfileImage = "staff.png";
                        user.TelephoneNo = model.TelephoneNo;
                        user.Skype = model.Skype;
                        user.ShortName = model.ShortName;
                        user.Email = model.Email;
                        //  user.UserName = model.UserName;
                        user.UpdatedOnUtc = DateTime.UtcNow;
                        user.UpdatedBy = model.CreatedBy;
                        user.IsActive = true;
                        var UpdateResult = await UserManager.UpdateAsync(user);
                        if (UpdateResult.Succeeded)
                        {
                            var role = await RoleManager.FindByIdAsync(model.RoleId);
                            if (!(await UserManager.IsInRoleAsync(user.Id, role.Name)))
                            {
                                var roles = await UserManager.GetRolesAsync(user.Id);
                                if (roles != null)
                                {
                                    foreach (var userRole in roles)
                                    {
                                        await UserManager.RemoveFromRoleAsync(user.Id, userRole);
                                    }
                                }
                                await UserManager.AddToRoleAsync(user.Id, role.Name);
                            }
                            model.Address.UserId = user.Id;
                            new WMSUserRepository().SaveAddress(model.Address);

                            // User Additional
                            model.UserId = user.Id;
                            new WMSUserRepository().SaveUserAddtional(model);

                        }
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [Route("Account/GetCustomer")]
        [HttpGet]
        public WMSCustomer GetCustomer(int userId)
        {
            WMSCustomer user = new CustomerRepository().GetUser(userId);

            return user;
        }
        [Route("Account/GetCustomers")]
        [HttpPost]
        public List<WMSCustomreGrid> GetCustomers(TrackUser model)
        {
            List<WMSCustomreGrid> list = new CustomerRepository().GetCustomers(model);
            return list;
        }
       [Route("Account/GetCustomerCompanyName")]
       [HttpGet]
        public List<WMSCustomreGrid> GetCustomerCompanyName(TrackUser model) 
        {
            List<WMSCustomreGrid> list = new CustomerRepository().GetCustomerCompanyName(model);
            return list;
        }

        #endregion

        #region login user 
        [Route("Account/LoginUser")]
        [HttpPost]
        public async Task<IHttpActionResult> LoginUser(WMSUserCredential userCrednetial)
        {
            WMSUserLoginDetail loginDetail = new WMSUserLoginDetail();
            string msg = "";
            try
            {
                // get user detail using identity library
                var user = await UserManager.FindByNameAsync(userCrednetial.UserName);
                // Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("user")));
                if (user != null)
                {

                    //  Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("user found")));
                    var roleId = new WMSUserRepository().GetUserRole(user.Id);
                    loginDetail = new WMSUserRepository().LoginUserDetail(user.Id);
                    loginDetail.Modules = new AccessControlRespository().UserScreens(user.Id);
                     
                    if (loginDetail != null)
                    {
                        if (string.IsNullOrEmpty(loginDetail.PhotoUrl))
                            loginDetail.PhotoUrl = ""; //AppSettings.ProfileImagePath + "avtar.jpg";
                        else
                            loginDetail.PhotoUrl = "";// AppSettings.WebApiPath + "UploadFiles/ProfilePhoto/" + loginDetail.PhotoUrl;

                        msg = "Success";
                    }

                }
                else
                {
                    msg = "Username is invalid!";
                    // Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("user not found")));
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                // Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
            return Ok(new WebAPIResponse { Message = msg, Result = loginDetail });//Ok(loginDetail);
        }

        #endregion
         
        #region forget passord
        /// <summary>
        /// RecoverPassword will set a new paasword for user if token is valid for user
        /// </summary>

        [Route("Account/RecoverPassword")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> RecoverPassword(RecoverPassword recoverPassword)
        {
            try
            {
                if (recoverPassword.Token == null || recoverPassword.UserId == 0)
                {
                    return BadRequest("Invalid token.");
                }
                var myuser = await UserManager.FindByIdAsync(recoverPassword.UserId);
                var userRole = new WMSUserRepository().GetUserRole(myuser.Id);
                if (myuser != null)
                {
                    var df = await UserManager.UserTokenProvider.ValidateAsync("ResetPassword", recoverPassword.Token, UserManager, myuser);
                    if (await UserManager.UserTokenProvider.ValidateAsync("ResetPassword", recoverPassword.Token, UserManager, myuser))
                    {
                        var result = await UserManager.ResetPasswordAsync(recoverPassword.UserId, recoverPassword.Token, recoverPassword.NewPassword);
                        if (result.Succeeded)
                        {
                            // return Ok(userRole);
                            return Ok(new WebAPIResponse { Message = "Success", Result = userRole });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            //  return BadRequest("Invalid token.");
            return Ok(new WebAPIResponse { Message = "Invalid token." });
        }

        /// <summary>
        /// ForgetPassword will send mail to user with a secure token 
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [Route("Account/ForgetPassword")]
        public async Task<IHttpActionResult> ForgetPassword(RecoveryEmail model)
        {
            var user = await UserManager.FindByNameAsync(model.UserName);
            if (user != null)
            {
                int RoleId = new WMSUserRepository().GetUserRole(user.Id);

                bool status = new EmailRepository().SendForgetPasswordEmail(user.Id, WebUtility.UrlEncode(await UserManager.GeneratePasswordResetTokenAsync(user.Id)));
                var res = new ApiResponse
                {
                    Status = status,
                    Email = user.Email
                };
                return Ok(new WebAPIResponse { Message = "Success", Result = res });
            }
            //   return BadRequest("User_Not_Found");
            return Ok(new WebAPIResponse { Message = "User not found" });
        }
        #endregion

        #region -- Change Password --


        [HttpPost]
        [Route("Account/ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(WMSChangePassword changePasswordDetail)
        {
            try
            {
                var user = await UserManager.FindAsync(changePasswordDetail.UserName, changePasswordDetail.CurrentPassword);
                //  var user = await UserManager.FindByNameAsync(changePasswordDetail.UserName);
                if (user != null)
                {
                    if ((await UserManager.RemovePasswordAsync(user.Id)).Succeeded)
                    {
                        if ((await UserManager.AddPasswordAsync(user.Id, changePasswordDetail.NewPassword)).Succeeded)
                        {
                            user.LastLoginOnUtc = DateTime.UtcNow;
                            user.LastPasswordChangedOnUtc = DateTime.UtcNow;
                            if ((await UserManager.UpdateAsync(user)).Succeeded)
                            {
                                return Ok("Success");
                            }
                        }
                    }
                }
                else { return Ok("Current Password does not match!"); }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
            return BadRequest();
        }

        #endregion

        #region user validation 
        [HttpGet]
        public async Task<IHttpActionResult> IsEmailExist(string email)
        {
            ApiResponse result = new ApiResponse();
            try
            {
                if (!string.IsNullOrEmpty(email))
                {
                    WMSIdentityUser user = await UserManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        result.Status = false;
                    }
                    else
                    {
                        result.Status = true;
                    }
                }
                else
                {
                    result.Status = false;
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                result.Status = false;
            }
            return Ok(result);
        }
        [HttpGet]
        public async Task<IHttpActionResult> IsUserNameExist(string username)
        {
            ApiResponse result = new ApiResponse();
            try
            {
                if (!string.IsNullOrEmpty(username))
                {
                    WMSIdentityUser user = await UserManager.FindByEmailAsync(username);
                    if (user == null)
                    {
                        result.Status = false;
                    }
                    else
                    {
                        result.Status = true;
                    }
                }
                else
                {
                    result.Status = false;
                }
            }
            catch (Exception ex)
            {
                // Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                result.Status = false;
            }
            return Ok(result);
        }
        #endregion

        #region GEt user additonal intial
        [HttpGet]
        public IHttpActionResult UserAdditonalInitials()
        {
            var result = new WMSUserRepository().UserAdditonalInitials();
            return Ok(result);
        }
        #endregion
    }
}

