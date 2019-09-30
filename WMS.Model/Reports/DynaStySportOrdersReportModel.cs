using System;

namespace WMS.Model.Reports
{
    public class DynaStySportOrdersReportModel
    {
        public int SalesCoordinator { get; set; }
        public int Merchandiser { get; set; }
        public int WarehouseUser { get; set; }
        public string OrderNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public string DesingCollection { get; set; }
        public string OrderType { get; set; }
        public int Quantity { get; set; }
        public string OrderStatus { get; set; }
        public string DesignName { get; set; }
    }
}
