using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Model.User;

namespace WMS.Model.Settings
{
    public class WarehouseGrid
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string WarehouseUser { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public bool IsActive { get; set; }
    }
    public class WMSWarehouse
    {
        public int WarehouseId { get; set; }
        public int WarehouseUserId { get; set; }
        public string WarehouseName { get; set; }
        public int CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public WMSWarehouseAddress Address { get; set; }

    }
    public class WMSWarehouseAddress
    {
        public int CountryId { get; set; } 
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Lattitude { get; set; }
        public string Lognitude { get; set; }

    }
}
