using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Model.AccessControl
{

    public class UserAccessLevel
    {

    }

    public class UserAccessLevelMenu
    {
        public int ModuleScreenId { get; set; }
        public string Menu { get; set; }
        public string MenuDisplay { get; set; }
        public string Route { get; set; }
        public string RouteParams { get; set; }
        public string Class { get; set; }
        public List<UserAccessLevelMenuDetail> ChildTabs { get; set; }
    }
    public class UserAccessLevelMenuDetail
    {
        public int ModuleScreenDetailId { get; set; }
        public string Menu { get; set; }
        public string MenuDispaly { get; set; }
        public string Route { get; set; }
        public string RouteParams { get; set; }
    }

    public class AccessControl
    {
        public int UserId { get; set; }
        public List<WMSAccessModuleUser> WMSAccessModuleUser { get; set; }
        public List<AccessControlModule> AccessControlModule { get; set; }
    }

    public class WMSAccessModuleUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
    }

    public class AccessControlModule
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string DisplayName { get; set; }
        public string ModuleClass { get; set; }
        public int OrderNumber { get; set; }
        public bool Ichecked { get; set; }
    }

    public class AccessModuleRole
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleNameDisplay { get; set; }
    }

    public class AccessModuleUsers
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class AccessModuleScreens
    {
        public int UserModuleScreenId { get; set; }
        public bool IsAllow { get; set; }
        public int ModuleScreenId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public DateTime CreatredOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModuleClass { get; set; }
    }
}
