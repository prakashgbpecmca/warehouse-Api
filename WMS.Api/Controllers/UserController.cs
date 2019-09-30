using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WMS.Model.User;
using WMS.Service.Repository;

namespace WMS.Api.Controllers
{
    public class UserController : ApiController
    {
        [Route("User/GetActiveUsers")]
        [HttpPost]
        public List<WMSActiveUser> GetActiveUsers(string term)
        {
            return new WMSUserRepository().ActiveUsers(term);
        }

        [Route("User/ProfileDetail")]
        [HttpGet]
        public WMSUserProfile ProfileDetail(int userId)
        {
            WMSUserProfile profile = new WMSUserRepository().ProfileDetail(userId);
            return profile;
        }
        [Route("User/Customers")]
        [HttpGet]
        public WMSUserProfile Customers(int userId)
        {
            WMSUserProfile profile = new WMSUserRepository().ProfileDetail(userId);
            return profile;
        }
        [HttpPost]
        public IHttpActionResult uploadProfileImage()
        {
            try
            {
                string filename = "";
                var httpRequest = HttpContext.Current.Request;
                string userid = httpRequest.Form[0];
                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        if (postedFile != null && postedFile.ContentLength > 0)
                        {
                            var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                            filename = Guid.NewGuid() + ext.ToLower();
                            var filePath = HttpContext.Current.Server.MapPath("~/Content/Uploads/Users/" + filename);

                            postedFile.SaveAs(filePath);
                        }
                    }
                    string img = new WMSUserRepository().uploadProfileImage(Convert.ToInt32(userid), filename);
                    return Ok("Success_" + filename);
                }
                else { return Ok("Please upload image!"); }
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
    }
}
