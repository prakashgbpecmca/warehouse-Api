using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WMS.Model.Common;
using WMS.Service.Repository;

namespace WMS.Api.Controllers
{
    public class MasterController : ApiController
    {
        [HttpGet]
        public MasterModel Initials()
        {
            return new MasterRepository().Initials();
        }

        [HttpGet]
        [Route("Master/Roles")]
        public List<WMSUserRoleType> Roles()
        {
            return new MasterRepository().GetSystemRole();
        }

        [HttpPost]
        public IHttpActionResult UploadBatteryForms()
        {
            try
            {
                int id = 0;
                var httpRequest = HttpContext.Current.Request;

                HttpFileCollection files = httpRequest.Files;
             
                var docfiles = new List<string>();


                string filePathToSave = "";
                string fileFullPath = "";
                filePathToSave = HttpContext.Current.Server.MapPath(filePathToSave);

                if (!System.IO.Directory.Exists(filePathToSave))
                    System.IO.Directory.CreateDirectory(filePathToSave);

                //This code will execute only when user will upload the document
                if (files.Count > 0)
                {
                    HttpPostedFile file = files[0];
                    if (!string.IsNullOrEmpty(file.FileName))
                    {
                        if (!File.Exists(filePathToSave + file.FileName))
                        {
                            //Save in server folder
                            fileFullPath = filePathToSave + file.FileName;
                            file.SaveAs(fileFullPath);

                            //Save file name and other information in DB

                            return BadRequest("BatteryForm");
                        }
                        else
                        {
                            return BadRequest("BatteryForm");
                        }
                    }
                }

                if (id > 0)
                {
                    return Ok(id);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
