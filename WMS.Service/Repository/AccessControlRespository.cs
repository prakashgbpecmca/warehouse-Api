using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Model.AccessControl;
using WMS.Model.Common;
using WMS.Model.User;
using WMS.Service.AllRepository;
using WMS.Service.DataAccess;
using WMS.Service.GenericRepository;

namespace WMS.Service.Repository
{
    public class AccessControlRespository
    {
        //public AccessControl GetAllUser(int userId)
        //{
        //    AccessControl access = new AccessControl();
        //    List<WMSAccessModuleUser> userDetail = new List<WMSAccessModuleUser>();
        //    List<AccessControlModule> module = new List<AccessControlModule>();
        //    var interfaceObj = new AllRepository<DataAccess.WMSUser>();
        //    var result = interfaceObj.GetAll();
        //    var interModule = new AllRepository<DataAccess.ModuleScreen>().GetAll();

        //    if (result != null)
        //    {
        //        foreach (var user in result.Where(k => k.IsActive))
        //        {
        //            WMSAccessModuleUser obj = new WMSAccessModuleUser()
        //            {
        //                UserId = user.Id,
        //                Name = user.ContactFirstName + " " + user.ContactLastName,
        //                UserName = user.UserName,
        //            };
        //            userDetail.Add(obj);
        //        }
        //    }
        //    if (interModule != null)
        //    {
        //        foreach (var model in interModule)
        //        {
        //            AccessControlModule obj = new AccessControlModule()
        //            {
        //                ModuleId = model.Id,
        //                ModuleName = model.Name,
        //                DisplayName = model.DisplayName,
        //                ModuleClass = model.Class,
        //                OrderNumber = model.OrderNumber.HasValue ? model.OrderNumber.Value : 0,
        //                Ichecked = false,
        //            };
        //            module.Add(obj);
        //        }
        //    }
        //    access.UserId = userId;
        //    access.WMSAccessModuleUser = userDetail;
        //    access.AccessControlModule = module;
        //    if (userId > 0)
        //    {
        //        using (var dbContext = new WMSEntities())
        //        {
        //            var entity = dbContext.UserModuleScreens.Where(k => k.UserId == userId).ToList();

        //            if (entity != null)
        //            {
        //                foreach (var moduel in entity)
        //                {
        //                    access.AccessControlModule.FirstOrDefault(k => k.ModuleId == moduel.ModuleScreenId).Ichecked = moduel.IsAllowed;
        //                }
        //            }
        //        }
        //    }
        //    return access;
        //}

        //public AccessControl SaveData(AccessControl obj)
        //{
        //    List<UserModuleScreen> usermodeul = new List<UserModuleScreen>();
        //    var interfaceObj = new AllRepository<DataAccess.UserModuleScreen>();
        //    foreach (var item in obj.AccessControlModule)
        //    {
        //        UserModuleScreen module = new UserModuleScreen()
        //        {
        //            UserId = obj.UserId,
        //            ModuleScreenId = item.ModuleId,
        //            ModuleScreenDetailId = 0,
        //            IsAllowed = item.Ichecked,
        //            CreatedBy = 1.ToString(),
        //            CreatedOn = DateTime.UtcNow,
        //        };
        //        usermodeul.Add(module);
        //    }
        //    // Delete already Assign Module
        //    using (var dbContext = new WMSEntities())
        //    {
        //        var entity = dbContext.UserModuleScreens.Where(k => k.UserId == obj.UserId).ToList();
        //        if (entity != null)
        //        {
        //            dbContext.UserModuleScreens.RemoveRange(entity);
        //            dbContext.SaveChanges();
        //        }
        //    }
        //    //Assign New Module
        //    foreach (var module in usermodeul.Where(k => k.IsAllowed))
        //    {
        //        interfaceObj.InsertModel(module);
        //        interfaceObj.Save();
        //    }
        //    return obj;
        //}

        public List<AccessModuleRole> GetRoles()
        {
            List<AccessModuleRole> _role = new List<AccessModuleRole>();
            using (var dbContext = new WMSEntities())
            {
                _role = (from r in dbContext.WMSRoles
                         select new AccessModuleRole
                         {
                             RoleId = r.Id,
                             RoleName = r.Name,
                             RoleNameDisplay = r.DisplayName
                         }).ToList();
            }

            return _role;
        }

        public List<AccessModuleUsers> GetUsers(int RoleId)
        {
            List<AccessModuleUsers> _user = new List<AccessModuleUsers>();
            using (var dbContext = new WMSEntities())
            {
                _user = (from r in dbContext.WMSRoles
                         join ur in dbContext.WMSUserRoles on r.Id equals ur.RoleId
                         join u in dbContext.WMSUsers on ur.UserId equals u.Id
                         where ur.RoleId == RoleId
                               && u.IsActive == true
                         select new AccessModuleUsers
                         {
                             UserId = u.Id,
                             FirstName = u.ContactFirstName,
                             LastName = u.ContactLastName
                         }).ToList();
            }

            return _user;
        }

        public List<AccessModuleScreens> SaveInitial(int UserId, string LoggedInUserId)
        {
            List<AccessModuleScreens> _screen = new List<AccessModuleScreens>();

            try
            {
                using (var dbContext = new WMSEntities())
                {
                    _screen = (from um in dbContext.UserModuleScreens
                               join ms in dbContext.ModuleScreens on um.ModuleScreenId equals ms.Id
                               where um.UserId == UserId
                               select new AccessModuleScreens
                               {
                                   UserModuleScreenId = um.Id,
                                   UserId = um.UserId,
                                   ModuleScreenId = um.ModuleScreenId,
                                   IsAllow = um.IsAllowed,
                                   Name = ms.Name,
                                   DisplayName = ms.DisplayName,
                                   CreatedBy = um.CreatedBy,
                                   CreatredOn = um.CreatedOn,
                                   ModuleClass = ms.Class
                               }).ToList();

                    if (_screen.Count > 0)
                    {
                        return _screen;
                    }
                    else
                    {
                        var parent = dbContext.ModuleScreens.ToList();
                        if (parent.Count > 0)
                        {
                            foreach (ModuleScreen ms in parent)
                            {
                                UserModuleScreen ums;
                                var child = dbContext.ModuleScreenDetails.Where(p => p.ModuleScreenId == ms.Id).ToList();
                                if (child.Count > 0)
                                {
                                    foreach (ModuleScreenDetail msd in child)
                                    {
                                        ums = new UserModuleScreen();
                                        ums.UserId = UserId;
                                        ums.ModuleScreenId = ms.Id;
                                        ums.ModuleScreenDetailId = msd.Id;
                                        ums.IsAllowed = false;
                                        ums.CreatedOn = DateTime.Now;
                                        ums.CreatedBy = LoggedInUserId;
                                        ums.UpdatedOn = null;
                                        ums.UpdateBy = null;
                                        dbContext.UserModuleScreens.Add(ums);
                                        dbContext.SaveChanges();
                                    }
                                }
                                else
                                {
                                    ums = new UserModuleScreen();
                                    ums.UserId = UserId;
                                    ums.ModuleScreenId = ms.Id;
                                    ums.ModuleScreenDetailId = 0;
                                    ums.IsAllowed = false;
                                    ums.CreatedOn = DateTime.Now;
                                    ums.CreatedBy = LoggedInUserId;
                                    ums.UpdatedOn = null;
                                    ums.UpdateBy = null;
                                    dbContext.UserModuleScreens.Add(ums);
                                    dbContext.SaveChanges();
                                }
                            }
                        }

                        _screen = (from um in dbContext.UserModuleScreens
                                   join ms in dbContext.ModuleScreens on um.ModuleScreenId equals ms.Id
                                   where um.UserId == UserId
                                   select new AccessModuleScreens
                                   {
                                       UserModuleScreenId = um.Id,
                                       UserId = um.UserId,
                                       ModuleScreenId = um.ModuleScreenId,
                                       IsAllow = um.IsAllowed,
                                       Name = ms.Name,
                                       DisplayName = ms.DisplayName,
                                       CreatedBy = um.CreatedBy,
                                       CreatredOn = um.CreatedOn,
                                       ModuleClass = ms.Class
                                   }).ToList();

                        return _screen;
                    }
                }
            }
            catch
            {
                return _screen;
            }
        }

        public bool AssignAccessModule(AccessModuleScreens module)
        {
            try
            {
                using (var dbContext = new WMSEntities())
                {
                    var mm = dbContext.UserModuleScreens.Find(module.UserModuleScreenId);
                    if (mm != null && mm.Id > 0)
                    {
                        mm.IsAllowed = module.IsAllow;
                        mm.UpdatedOn = null;
                        mm.UpdateBy = null;
                        dbContext.Entry(mm).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public List<UserAccessLevelMenu> UserScreens(int id)
        {
            using (var dbContext = new WMSEntities())
            {

                var userRole = dbContext.WMSUserRoles.Where(p => p.UserId == id).FirstOrDefault();

                var collection = (from r in dbContext.UserModuleScreens
                                  join ms in dbContext.ModuleScreens on r.ModuleScreenId equals ms.Id
                                  where r.UserId == id && r.IsAllowed == true
                                  select new
                                  {
                                      ModuleScreenId = r.ModuleScreenId,
                                      UsreId = r.UserId,
                                      Menu = ms.Name,
                                      MenuDispaly = ms.DisplayName,
                                      Route = ms.Route,
                                      RouteParams = "",
                                      Class = ms.Class

                                  }).ToList();

                List<UserAccessLevelMenu> list = collection.Select(p => new UserAccessLevelMenu
                {
                    Menu = p.Menu,
                    Route = p.Route,
                    MenuDisplay = p.MenuDispaly,
                    ModuleScreenId = p.ModuleScreenId,
                    RouteParams = p.RouteParams,
                    Class = p.Class
                }).ToList();

                foreach (var item in list)
                {
                    if(userRole.RoleId == (byte)WMSUserRoleEnum.Warehouse)
                    {
                         if (item.Menu == "Orders")
                        {
                            item.MenuDisplay = "Pick Requests";
                            item.Class = "fa-registered";
                        }
                    }
                }
                return list;
            }
        }
    }
}
