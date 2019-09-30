using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WMS.Model.AccessControl;
using WMS.Service.Repository;

namespace WMS.Api.Controllers
{
    public class AccessControlController : ApiController
    {
        //[HttpPost]
        //public IHttpActionResult AssigneModule(AccessControl boj)
        //{
        //    var result = new AccessControlRespository().SaveData(boj);
        //    return Ok(result);
        //}

        //[HttpGet]
        //public IHttpActionResult GetAllUser(int userId)
        //{
        //    var result = new AccessControlRespository().GetAllUser(userId);
        //    return Ok(result);
        //}


         

        [HttpGet]
        [Route("AccessControl/GetAllRoles")]
        public List<AccessModuleRole> GetAllRoles()
        {
            return new AccessControlRespository().GetRoles();
        }

        [HttpGet]
        [Route("AccessControl/GetAllUsers")]
        public List<AccessModuleUsers> GetAllUsers(int RoleId)
        {
            return new AccessControlRespository().GetUsers(RoleId);
        }

        [HttpGet]
        [Route("AccessControl/SaveInitial")]
        public IHttpActionResult SaveInitial(int UserId, string LoggedInUserId)
        {
            return Ok(new AccessControlRespository().SaveInitial(UserId, LoggedInUserId));
        }

        [HttpPost]
        [Route("AccessControl/AssignModule")]
        public IHttpActionResult AssignModule(AccessModuleScreens module)
        {
            return Ok(new AccessControlRespository().AssignAccessModule(module));
        }
    }
}