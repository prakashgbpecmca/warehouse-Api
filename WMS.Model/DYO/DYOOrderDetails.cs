using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Model.DYO
{
    public class DYOOrderDetails
    {
        public int OrderId { get; set; }
        public string OrderName { get; set; }
        public string OrderNo { get; set; }
        public string PONumber { get; set; }
        public string DeliveryType { get; set; }
        public string CreatedOn { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public int SalesCordinator { get; set; }
        public int Merchandiser { get; set; }
        public string CreatedBy { get; set; }
        public string Status { get; set; }
        public int? OrderStatusId { get; set; }
        public string OrderNotes { get; set; }
        public string OrderTypeRange { get; set; }
        public bool ShowAcceptReject { get; set; }
        public bool IsModifyShow { get; set; }
        public int ModifyDays { get; set; }
        public string RequestedDeliveryDate { get; set; }
        public string RequestedExFactoryDate { get; set; }
        public string ConfirmedExFactoryDate { get; set; }
        public DateTime? ARequestedDeliveryDate { get; set; }
        public DateTime? ARequestedExFactoryDate { get; set; }
        public DateTime? AConfirmedExFactoryDate { get; set; }
        public OrderBillingAddress _OrderBillingAddress { get; set; }
        public OrderDeliveryInformation _OrderDeliveryInformation { get; set; }
        public List<ProductInformation> _ProductInformation { get; set; }
    }
    public class OrderBillingAddress
    {
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
    }
    public class OrderDeliveryInformation
    {
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string Address { get; set; }
        public string DeliveryMethod { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
    }
    public class ProductInformation
    {
        public int OrderDesignId { get; set; }
        public bool IsModify { get; set; }
        public int ProductCatagoryId { get; set; }
        public string ProductName { get; set; }
        public string DesignNumber { get; set; }
        public string ProductCode { get; set; }
        public string Image { get; set; }
        public string OrderDesignName { get; set; }
        public int OrderDesignStatusId { get; set; }
        public bool ShowAcceptReject { get; set; }
        public List<ProductDetails> _ProductDetails { get; set; }
    }
    public class ProductDetails
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Size { get; set; }
        public string Quantity { get; set; }
        public int totalqty { get; set; }

    }
    //
    public class StrikeOffModel
    {
        public int RevisionNo { get; set; }
        public List<StrikeOffListModel> StrikeOfflist { get; set; }
        public Boolean IsOrderDesignComplete { get; set; }
        public int OrderStatusId { get; set; }
    }
    public class StrikeOffListModel
    {
        public string EmblishmentType { get; set; }
        public int ID { get; set; }
        public int? OrderID { get; set; }
        public int OrderEmblishmentID { get; set; }
        public string LogoImage { get; set; }
        public string LogoImageName { get; set; }
        public string PrintMethod { get; set; }
        public string Status { get; set; }
        public string SampleLogo { get; set; }
        public string SampleLogoName { get; set; }
        public int SampleLogoID { get; set; }
        public string SampleLogoStatus { get; set; }
        public bool? IsFinished { get; set; }
        public int OrderStatusId { get; set; }


    }
    public class StrikeApprovalModel
    {
        public List<CommunicationLogModel> commLogList { get; set; }
        public OrderRevisionModel revHistory { get; set; }
        public List<StrikeOffSampleLogoModel> sampleLogo { get; set; }
    }
    public class OrderRevisionModel
    {
        public int roleid { get; set; }
        public int orderid { get; set; }
        public List<oEmblishment> oEmblishmentlist { get; set; }
    }
    public class oEmblishment
    {
        public int RevisionHistoryid { get; set; }
        public int oemblishmentid { get; set; }
        public byte statusid { get; set; }
    }

    public class CommunicationLogModel
    {
        public int ID { get; set; }
        public Nullable<int> OrderID { get; set; }
        public Nullable<int> OrderEmblishmentID { get; set; }
        public Nullable<int> UserID { get; set; }
        public string Reason { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public bool IsPublic { get; set; }
        public string UserName { get; set; }
    }
    public class StrikeOffSampleLogo
    {
        public int OrderID { get; set; }
        public int OrderEmblishmentID { get; set; }
        public string FileName { get; set; }

    }
    public class StrikeOffSampleLogoModel
    {
        public int SampleLogoID { get; set; }
        public byte statusid { get; set; }
        public int orderid { get; set; }
        public int userid { get; set; }

    }

    public class StrrikeModel
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string Reason { get; set; }
        public bool IsPublic { get; set; }
    }
}
