using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Model.User;

namespace WMS.Model.Email
{
    public class EmailGenericModel
    {
        public string Name { get; set; }

        public string To { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }

        public string ImageHeader { get; set; }
        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string FromTelephone { get; set; }
        public string FromSiteAddress { get; set; }

    }
    public class EmailE1 : EmailGenericModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string SiteLink { get; set; }
        public string RecoveryLink { get; set; }
    }
    public class EmailE4 : EmailGenericModel
    {
        public string CustomerName { get; set; }
        public string DesignName { get; set; }
        public string DesignNumber { get; set; }
        public string Date { get; set; }
        public string OrderNumber { get; set; }
        public string PONumber { get; set; }
        public string StyleNumber { get; set; }
        public string Color { get; set; }
        public int Quantity { get; set; }
        public string SiteLink { get; set; }
    }
    public class Email42 : EmailGenericModel
    {
        public string CustomerName { get; set; }
        public string DesignName { get; set; }
        public string DesignNumber { get; set; }
        public string Date { get; set; }
        public string PONumber { get; set; }
        public string OrderNumber { get; set; }
        public string StyleNumber { get; set; }
        public string Color { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; }
        public string SiteLink { get; set; }
    }
    public class Email44 : EmailGenericModel
    {
        public string CustomerName { get; set; }
        public string DesignName { get; set; }
        public string DesignNumber { get; set; }
        public string Date { get; set; }
        public string PONumber { get; set; }
        public string OrderNumber { get; set; }
        public string AcceptedBy { get; set; }
        public string StyleNumber { get; set; }
        public string Color { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; }
        public string SiteLink { get; set; }
    }
    public class EmailE1_2 : EmailGenericModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string RecoveryLink { get; set; }
        public string SiteLink { get; set; }
    }

    public class EmailE_2 : EmailGenericModel
    {
        public int CreatedBy { get; set; }
        public int OrderId { get; set; }
        public string PONumber { get; set; }
        public string OrderNumber { get; set; }
        public string OrderName { get; set; }
        public string DeliveryType { get; set; }
        public string RequestedDeliveryDate { get; set; }
        public DateTime? RequestedDeliveryDateTime { get; set; }
        public DateTime TargetDeliveryDate { get; set; }
        public string TargetDeliveryDisplayDate { get; set; }
        public int TotalQuantity { get; set; }
        public OrderAddress DeliveryAddress { get; set; }
        public string CustomerName { get; set; }
        public List<E_2ProductDetail> ProductDetail { get; set; }
        public string RejectLink { get; set; }
        public string AcceptLink { get; set; }
        public string SiteAddress { get; set; }
        public string Reason { get; set; }
        public DateTime OrderPlacedDate { get; set; }
        public string OrderPlacedDisplayDate { get; set; }
        public int SalesCoordinatorId { get; set; }
        public string CoordinatorName { get; set; }
        public string MerchandiserName { get; set; }
        public DateTime? MerchandiserDelDate { get; set; }
        public string MerchandiserDelDateInEmail { get; set; }
        public string MerchandiserDisplayDelDate { get; set; }
        public int? coordinatorid { get; set; }
        public int? merchdiserid { get; set; }
        public string custmail { get; set; }
        public string coordmail { get; set; }
        public string merchmail { get; set; }

        public string LogoAcceptLink { get; set; }
        public string LogoRejectLink { get; set; }
    }

    public class E_2ProductDetail
    {
        public int SerialNo { get; set; }
        public string DesignNumber { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string SKU { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public short Quantity { get; set; }
    }

    public class OrderAddress
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public int UserId { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public WMSCountry Country { get; set; }
    }


    public class EmailE_3 : EmailGenericModel
    {
        public int OrderId { get; set; }
        public string PONumber { get; set; }
        public string OrderNumber { get; set; }
        public string OrderName { get; set; }
        public string DeliveryType { get; set; }
        public DateTime TargetDeliveryDate { get; set; }
        public string TargetDeliveryDisplayDate { get; set; }
        public int TotalQuantity { get; set; }
        public OrderAddress DeliveryAddress { get; set; }
        public string CustomerName { get; set; }
        public string WarehouseUserName { get; set; }
        public List<E_2ProductDetail> ProductDetail { get; set; }
        public DateTime OrderPlacedDate { get; set; }
        public string OrderPlacedDisplayDate { get; set; }
        public string MerchandiserDelDateInEmail { get; set; }
    }
}
