using System;

namespace WMS.Model.Reports
{
    public class DailyDispatchReportModel
    {
        public string OrderNumber { get; set; }
        public DateTime? OrderCreatedOn { get; set; }
        public string DeliveryType { get; set; }
        public string ShippingMethod { get; set; }
        public decimal TotalNetWeight { get; set; }
        public decimal TotalGrossWeight { get; set; }
        public decimal Weight { get; set; }
        public int? ActualQuantity { get; set; }
        public int Quantity { get; set; }    
        public int Pieces { get; set; }
        public string Unit { get; set; }
    }

    public class TotalDailyDispatchReportModel
    {
        public int Salescordinator { get; set; }
        public int Merchandiser { get; set; }
        public int WarehouseUser { get; set; }
        public string CustomerName { get; set; }
        public string AccountNumber { get; set; }
        public string OrderNumber { get; set; }
        public DateTime? OrderCreatedOn { get; set; }
        public string DeliveryType { get; set; }
        public string ShippingMethod { get; set; }
        public decimal TotalNetWeight { get; set; }
        public decimal TotalGrossWeight { get; set; }
        public decimal Weight { get; set; }
        public int? ActualQuantity { get; set; }
        public int Quantity { get; set; }
        public int Pieces { get; set; }
        public string Unit { get; set; }
    }

    public class OrderPendingDispatchReportModel
    {
        public int SalesCordinator { get; set; }
        public int Merchandiser { get; set; }
        public int WarehouseUser { get; set; }
        public string CustomerName { get; set; }
        public string AccountNumber { get; set; }
        public string OrderNumber { get; set; }
        public DateTime? OrderCreatedOn { get; set; }
        public string DeliveryType { get; set; }
        public string ShippingMethod { get; set; }
        public decimal TotalNetWeight { get; set; }
        public decimal TotalGrossWeight { get; set; }
        public decimal Weight { get; set; }
        public int? ActualQuantity { get; set; }
        public int Quantity { get; set; }
        public int Pieces { get; set; }
        public string Unit { get; set; }
    }

    public class PendingPickRequestDispatchReportModel
    {
        public int SalesCordinator { get; set; }
        public int Merchandiser { get; set; }
        public int WarehouseUser { get; set; }
        public string CustomerName { get; set; }
        public string AccountNumber { get; set; }
        public string OrderNumber { get; set; }
        public DateTime? OrderCreatedOn { get; set; }        
        public DateTime? PickupRequestedOn { get; set; }           
        public int Pieces { get; set; }
        public int Quantity { get; set; }
        
    }
}
