using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WMS.Model.Settings;
using WMS.Service.Repository;

namespace WMS.Api.Controllers
{
    public class SettingsController : ApiController
    {
        #region Warehouse 
        [HttpGet]
        [Route("Settings/GetWarehouseInitials")]
        public IHttpActionResult GetWarehouseInitials(int userId)
        {
            try
            {
                var data = new SettingRepository().Initials(userId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest();
            }
        }
        [HttpGet]
        [Route("Settings/GetWarehouseList")]
        public IHttpActionResult GetWarehouseList(int userId, string searchText)
        {

            try
            {
                List<WarehouseGrid> list = new SettingRepository().GetWarehouseList(userId, searchText);
                return Ok(list);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest();
            }
        }
        [HttpGet]
        [Route("Settings/GetWarehouseDetail")]
        public IHttpActionResult GetWarehouseDetail(int wasrehouseId)
        {
            try
            {
                WMSWarehouse detail = new SettingRepository().GetWarehouseDetail(wasrehouseId);
                return Ok(detail);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest();
            }
        }
        [HttpGet]
        [Route("Settings/WarehouseStatus")]
        public IHttpActionResult WarehouseStatus(int warehouseId, bool status)
        {
            try
            {
                bool detail = new SettingRepository().ChangeWarehouseStatus(warehouseId, status);
                return Ok(detail);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("Settings/SaveWarehouse")]
        public IHttpActionResult SaveWarehouse(WMSWarehouse warehouse)
        {
            try
            {
                bool result = new SettingRepository().SaveWarehouse(warehouse);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest();
            }
        }
        #endregion
    }
}
