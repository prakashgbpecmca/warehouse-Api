using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Model.Common;
using WMS.Model.Settings;
using WMS.Service.DataAccess;

namespace WMS.Service.Repository
{
    public class SettingRepository
    {
        WMSEntities dbContext = new WMSEntities();

        #region Warehouse 
        public MasterModel Initials(int userId)
        {
            MasterModel model = new MasterModel();
            model.Countries = new MasterRepository().Countries();
            model.CountryPostCodes = new MasterRepository().GetCountryPhoneCodeList();
            model.Users = new MasterRepository().GetWarehouseUsers();
            model.Roles = new MasterRepository().GetSystemRole();
            return model;
        }

        public List<WarehouseGrid> GetWarehouseList(int userId, string searchText)
        {
            var userRole = (from r in dbContext.WMSUserRoles
                            where r.UserId == userId
                            select new
                            {
                                RoleId = r.RoleId
                            }).FirstOrDefault();
            var colection = (from r in dbContext.Warehouses
                             join u in dbContext.WMSUsers on r.WarehouseUserId equals u.Id
                             join c in dbContext.Countries on r.CountryId equals c.CountryId
                             select new WarehouseGrid
                             {
                                 WarehouseId = r.WarehouseId,
                                 City = r.City == null ? "" : r.City,
                                 State = r.State == null ? "" : r.State,
                                 Country = c.CountryName == null ? "" : c.CountryName,
                                 WarehouseUser = u.ContactFirstName + " " + u.ContactLastName,
                                 WarehouseName = r.Name == null ? "": r.Name,
                                 IsActive = r.IsActive
                             }).ToList();

            if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrWhiteSpace(searchText))
            {
                colection = colection.Where(x => x.City.ToLower().Contains(searchText.ToLower())
                                                || x.State.ToLower().Contains(searchText.ToLower())
                                                || x.Country.ToLower().Contains(searchText.ToLower())
                                                || x.WarehouseUser.ToLower().Contains(searchText.ToLower())
                                                || x.WarehouseName.ToLower().Contains(searchText.ToLower())

               ).ToList();
            }
            return colection;
        }

        public bool SaveWarehouse(WMSWarehouse warehouse)
        {
            bool status = false;
            if (warehouse != null)
            {
                Warehouse dbWarehouse;
                if (warehouse.WarehouseId == 0)
                {
                    dbWarehouse = new Warehouse();

                    dbWarehouse.Name = warehouse.WarehouseName;
                    dbWarehouse.WarehouseUserId = warehouse.WarehouseUserId;
                    dbWarehouse.TelephoneNo = warehouse.Address.Phone;
                    dbWarehouse.CreatedBy = warehouse.CreatedBy;
                    dbWarehouse.IsActive = warehouse.IsActive;
                    dbWarehouse.LocationLatitude = warehouse.Address.Lattitude;
                    dbWarehouse.LocationLongitude = warehouse.Address.Lognitude;
                    dbWarehouse.Address = warehouse.Address.Address;
                    dbWarehouse.Address2 = warehouse.Address.Address2;
                    dbWarehouse.Address3 = warehouse.Address.Address3;
                    dbWarehouse.City = warehouse.Address.City;
                    dbWarehouse.State = warehouse.Address.State;
                    dbWarehouse.Zip = warehouse.Address.Zip;
                    dbWarehouse.CountryId = warehouse.Address.CountryId;
                    dbWarehouse.Email = warehouse.Address.Email;

                    dbContext.Warehouses.Add(dbWarehouse);
                    dbContext.SaveChanges();
                    status = true;

                }
                else
                {
                    dbWarehouse = dbContext.Warehouses.Find(warehouse.WarehouseId);
                    if (dbWarehouse != null)
                    {
                        dbWarehouse.Name = warehouse.WarehouseName;
                        dbWarehouse.WarehouseUserId = warehouse.WarehouseUserId;
                        dbWarehouse.TelephoneNo = warehouse.Address.Phone;
                        dbWarehouse.CreatedBy = warehouse.CreatedBy;
                        dbWarehouse.IsActive = warehouse.IsActive;
                        dbWarehouse.LocationLatitude = warehouse.Address.Lattitude;
                        dbWarehouse.LocationLongitude = warehouse.Address.Lognitude;
                        dbWarehouse.Address = warehouse.Address.Address;
                        dbWarehouse.Address2 = warehouse.Address.Address2;
                        dbWarehouse.Address3 = warehouse.Address.Address3;
                        dbWarehouse.City = warehouse.Address.City;
                        dbWarehouse.State = warehouse.Address.State;
                        dbWarehouse.Zip = warehouse.Address.Zip;
                        dbWarehouse.CountryId = warehouse.Address.CountryId;
                        dbWarehouse.Email = warehouse.Address.Email;
                        dbContext.SaveChanges();
                        status = true;
                    }
                    else
                    {
                        status = false;
                    }
                }
                return status;
            }
            else
            {
                return status;
            }
        }

        public bool ChangeWarehouseStatus(int warehouseId, bool status)
        {
            var dbWarehouse = dbContext.Warehouses.Find(warehouseId);
            if (dbWarehouse != null)
            {
                dbWarehouse.IsActive = status;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public WMSWarehouse GetWarehouseDetail(int warehouseId)
        {
            WMSWarehouse detail = new WMSWarehouse();
            var dbWarehouse = dbContext.Warehouses.Find(warehouseId);
            if (dbWarehouse != null)
            {
                detail.WarehouseId = dbWarehouse.WarehouseId;
                detail.WarehouseName = dbWarehouse.Name;
                detail.WarehouseUserId = dbWarehouse.WarehouseUserId;
                detail.IsActive = true;

                detail.Address = new WMSWarehouseAddress();
                detail.Address.CountryId = dbWarehouse.CountryId;
                detail.Address.Address = dbWarehouse.Address;
                detail.Address.Address2 = dbWarehouse.Address2;
                detail.Address.City = dbWarehouse.City;
                detail.Address.State = dbWarehouse.State;
                detail.Address.Area = dbWarehouse.Address3;
                detail.Address.Zip = dbWarehouse.Zip;
                detail.Address.Phone = dbWarehouse.TelephoneNo;
                detail.Address.Email = dbWarehouse.Email;
                detail.Address.Lattitude = dbWarehouse.LocationLatitude;
                detail.Address.Lognitude = dbWarehouse.LocationLongitude;

            }
            return detail;
        }
        #endregion

    }
}
