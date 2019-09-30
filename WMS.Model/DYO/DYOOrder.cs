using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Model.User;

namespace WMS.Model.DYO
{
    public class DYOOrdePickup
    {
        public int ID { get; set; }
        public string Line { get; set; }
        public string Rack { get; set; }
        public string Row { get; set; }
        public string Location { get; set; }
        public int RequiredQty { get; set; }
    }

    public class DYOOrderComunicationLog
    {
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public bool Ispublic { get; set; }
        public string Reason { get; set; }
    }

    public class ReportResult
    {
        public string CustomerCompanyName { get; set; }
        public string CustomerName { get; set; }
        public string JobSheetNumber { get; set; }
    }
    #region my cart

    public class CollectionSaveModel
    {
        public int OrderId { get; set; }
        public string CollectionName { get; set; }
    }
    public class CollectionDYOOrder
    {
        public int TotalRows { get; set; }
        public int OrderId { get; set; }
        public string OrderName { get; set; }
        public string OrderNumber { get; set; }
        public string OrderDescripton { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string JobSheetName { get; set; }
        public string JobSheetPath { get; set; }
        public List<CollectionOrderDesign> OrderDesigns { get; set; }
    }

    public class CollectionOrderDesign
    {
        public int OrderDesignId { get; set; }
        public string OrderDesignName { get; set; }
        public string ProductImageURL { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string ProductColor { get; set; }
        public string ProductColorCode { get; set; }
    }

    #endregion

    public class TimeZoneModal
    {
        public int TimezoneId { get; set; }
        public string Name { get; set; }
        public string Offset { get; set; }
        public string OffsetShort { get; set; }
    }
    #region order detail
    public class DYOOrderDetail
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string CreatedOnDateTime { get; set; }
        public string OrderNumber { get; set; }
        public string OrderName { get; set; }
        public string OrderStatus { get; set; }
        public String CreatedBy { get; set; }
        public string OrderNote { get; set; }
        public string OrderType { get; set; }
        public DYOOrderAddress BillingAddress { get; set; }
        public DYOOrderAddress OrderAddress { get; set; }
        public List<DYOOrderDesign> OrderDesigns { get; set; }
    }
    #endregion
    public class DYOOrderGrid
    {
        public int TotalRows { get; set; }
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string StatusImage { get; set; }
        public string OrderStatus { get; set; }
        public string OrderNumber { get; set; }
        public string OrderName { get; set; }
        public string Customer { get; set; }
        public string SalesCoodinator { get; set; }
        public string DeliveryType { get; set; }
        public string OrderRangeType { get; set; }
        public string DeliveryMethod { get; set; }
        public string PONumber { get; set; }
        public bool IsCollapsed { get; set; }
        public string jobSheetName { get; set; }
        public string jobSheetUrl { get; set; }
        public string jobSheetShiipingFileName { get; set; }
        public string jobSheetShippingFileUrl { get; set; }
        public string PickNoteName { get; set; }
        public string PickNoteUrl { get; set; }
        public string UpdatedPickNoteName { get; set; }
        public string UpdatedPickNoteUrl { get; set; }
        public bool IsCartonLabel { get; set; }
        public string DispatchNoteName { get; set; }
        public string DispatchNoteUrl { get; set; }
        public int OrderStatusId { get; set; }
        public List<OrderDesignGrid> OrderDesigns { get; set; }
    }

    public class OrderDesignGrid
    {
        public int OrderDesignId { get; set; }
        public string DesignName { get; set; }
        public string DesignNumber { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string ProductImageURL { get; set; }
        public string ProductCode { get; set; }
        public bool IsCollapsed { get; set; }
        public List<OrderDesignDetailGrid> OrderDesignDetails { get; set; }
    }

    public class OrderDesignDetailGrid
    {
        public int OrderDesignDetailId { get; set; }
        public int OrderDesignId { get; set; }
        public byte SizeId { get; set; }
        public string SizeName { get; set; }
        public byte ColorId { get; set; }
        public string Color { get; set; }
        public string PlayerName { get; set; }
        public string PlayerNumber { get; set; }
        public short Quantity { get; set; }
    }

    public class OrderTrackAndTraceInitials
    {
        public List<WMSActiveCustomer> Customers { get; set; }
        public List<WMSOrderStatus> Status { get; set; }
    }
    public class WMSOrderStatus
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public string StatusDisplay { get; set; }
        public string Image { get; set; }
    }
 
    public class MyOrderTrackAndTrace
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int OrderStatusId { get; set; }
        public string OrderNumber { get; set; }
        public string PONumber { get; set; }
        public int CurrentPage { get; set; }
        public int TakeRows { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; }
    }
    public class DeliveryDateModel
    {
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public DateTime DeliveryDate { get; set; }
    }
    public class DYOOrder
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string OrderNumber { get; set; }
        public string OrderName { get; set; }
        public string PONumber { get; set; }
        public byte OrderStatusId { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public string OrderNote { get; set; }
        public string OrderType { get; set; }
        public byte DelivieryMethodId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DYOOrderAddress OrderAddress { get; set; }
        public List<DYOOrderDesign> OrderDesigns { get; set; }
    }
    public class DYOOrderAddress
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public WMSCountry Country { get; set; }

    }

    public class DYOPrderCollectionInitials
    {
        public List<WMSDeliveryMethod> DiliveryMethods { get; set; }
        public List<DYOProductSpec> ProductSizes { get; set; }
        public List<DYOProductSpec> ProductColors { get; set; }
        public List<string> ProductStyles { get; set; }
    }
    public class WMSDeliveryMethod
    {
        public int DelivieryMethodId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Code { get; set; }
    }
    public class DYOProductSpec
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderNumber { get; set; }

    }
    public class DYOOrderDesign
    {
        public int OrderDesignId { get; set; }
        public int ProductId { get; set; }
        public string ProductType { get; set; } //  for sock range 
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductCode { get; set; }
        public string ProductImage { get; set; }
        public string ProductImageURL { get; set; }
        public string DesignName { get; set; }
        public string DesignNumber { get; set; }
        public bool IsPlayerName { get; set; }
        public bool IsPlayerNumber { get; set; }
        public bool IsInserText { get; set; }
        public List<DYOProductSpec> ProductSizes { get; set; }
        public List<DYOOrderDesignDetail> OrderDesignDetails { get; set; }
    }
    public class WMSFile
    {
        public string CartonName { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
    public class DYOOrderDesignDetail
    {
        public int OrderDesignDetailId { get; set; }
        public int OrderDesignId { get; set; }
        public string StyleCode { get; set; }
        public int ProductId { get; set; }
        public byte SizeId { get; set; }
        public byte ColorId { get; set; }
        public string Color { get; set; }
        public bool IsInserText { get; set; }
        public string InserText { get; set; }
        public string PlayerName { get; set; }
        public bool IsPlayerName { get; set; }
        public bool IsPlayerNumber { get; set; }
        public string PlayerNumber { get; set; }
        public short Quantity { get; set; }
        public int ActualQuantity { get; set; }
    }

}
