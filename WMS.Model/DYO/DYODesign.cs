using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Model.DYO
{
    #region Order 
    public class OrderInitials
    {
        public int CustomerId { get; set; }
        public int Address { get; set; }
    }

    public class DYOorder
    {
        public int OrderId { get; set; }
        public string OrderName { get; set; }
        public int CustomerId { get; set; }
        public int CreatedBy { get; set; }
        public string OrderNumber { get; set; }

    }
    #endregion

    public class SKUQuantity
    {
        public int ProductSKUId { get; set; }
        public int Quantity { get; set; }
    }

    #region design initials 
    public class EmblishMethod
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class EmblishCustomColor
    {
        public string ColorType { get; set; }
        public List<EmblishMethod> ColorTypes { get; set; }
        public string CustomColor { get; set; }
    }
    public class EmblishmentOption
    {
        public string EmplishmentType { get; set; }
        public string EmplishmentTypeDisplay { get; set; }
        public bool IsDefaultLogoShow { get; set; }
        public bool IsPlayer { get; set; }
        public bool IsTextOriented { get; set; }
        public string LogoImage { get; set; }
        public string PrintMethod { get; set; }
        public List<EmblishMethod> PrintingMethods { get; set; }
        public List<EmblishCustomColor> ColorTypes { get; set; }
        //public string ColorType { get; set; }
        //public List<EmblishMethod> CustomColorTypes { get; set; }
        //public string CustomColor { get; set; }
        public int? LogoWidth { get; set; }
        public List<int> WidthRange { get; set; }

        public int LogoMaxWidth { get; set; }
        public int LogoMinWidth { get; set; }

        public int LogoMaxHeight { get; set; }
        public int LogoMinHeight { get; set; }

        public bool IsAspectRatio { get; set; }
        public int? LogoHeight { get; set; }
        public List<int> HeightRange { get; set; }

        public string DimensionUnit { get; set; }
        public string LogoColor { get; set; }
        public List<WMSColor> LogoColors { get; set; }

        public string FontFamily { get; set; }
        public List<EmblishMethod> FontFamilies { get; set; }

        public string FontColor { get; set; }
        public List<WMSColor> FontColors { get; set; }
    }
    public class WMSColor
    {
        public string Color { get; set; }
        public string ColorCode { get; set; }
        public bool IsActive { get; set; }
    }

    public class DYODesignProductEmblishment
    {
        public bool IsUpperArmRight { get; set; }
        public bool IsLowerArmRight { get; set; }
        public bool IsUpperArmLeft { get; set; }
        public bool IsLowerArmLeft { get; set; }
        public bool IsChestCenterTop { get; set; }
        public bool IsRightChest { get; set; }
        public bool IsLeftChest { get; set; }
        public bool IsFrontMain { get; set; }
        public bool IsBackNeck { get; set; }
        public bool IsPlayerName { get; set; }
        public bool IsPlayerNumber { get; set; }
        public bool IsMidBack { get; set; }
        public bool IsLowerBack { get; set; }
        public bool IsLowerSleeveRight { get; set; }
        public bool IsLowerSleeveLeft { get; set; }
        public bool IsThighRight { get; set; }
        public bool IsThighLeft { get; set; }
        public bool IsBackThighLeft { get; set; }
        public bool IsBackThighRight { get; set; }
        public bool IsSockFrontRight { get; set; }
        public bool IsSockFrontLeft { get; set; }
        public bool IsSockCalfLeft { get; set; }
        public bool IsSockCalfRight { get; set; }


    }
    public class DYODesignProductStyle
    {
        public int ProductStyleId { get; set; }
        public string ProductStyleCode { get; set; }
        public string ProductStyleThumbnail { get; set; }
        public string ProductStyleThumbnailURL { get; set; }
        public string ProductStyle3DModelName { get; set; }
        public string ProductStyle3DModelURL { get; set; }
        public byte NumberOfSection { get; set; }
        public bool IsUploadLogo { get; set; }
        public bool IsText { get; set; }
    }
    public class ProductDesignInitials
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int ProductId { get; set; }
        public string ProductType { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string Product3DModelName { get; set; }
        public string Product3DModelURL { get; set; }
        //  public Byte[] Product3DModelByte { get; set; }
        public List<EmblishmentOption> EmblishmentOptions { get; set; }
        //   public DYODesignProductEmblishment Emblishment { get; set; }
        public List<DYODesignProductStyle> ProductStyles { get; set; }
        public List<WMSColor> ProductColors { get; set; }
        public byte NumberOfSection { get; set; }
        public List<SockRangeSection> Sections { get; set; }
        public bool IsDefaultLogo { get; set; }
    }

    public class SockRangeSection
    {
        public bool IsActive { get; set; }
        public string SectionNumber { get; set; }
        public List<WMSColor> SectionColors { get; set; }
    }
    #endregion

    #region  Product Design

    public class DYOOrderCollection
    {
        public int OrderId { get; set; }
        public string OrderName { get; set; }
        public int CustomerId { get; set; }
        public int CreatedBy { get; set; }
        public string OrderNumber { get; set; }
        public string JobsheetPath { get; set; }
        public string JobSheetName { get; set; }
        public string DesignImagePath { get; set; }
        public string DesignImageName { get; set; }
        public List<DYOProductDesign> OrderDesigns { get; set; }
    }

    public class DYOProductDesign
    {
        public int OrderId { get; set; }
        public bool IsRawGarment { get; set; }
        public int OrderDesignId { get; set; }
        public int OrderDesignStatusId { get; set; }
        public string OrderDesignName { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public int CreatedBy { get; set; }
        public string ProductType { get; set; }
        public string ProductColor { get; set; }
        public string ProductTwoDImageName { get; set; }
        public string ProductTwoDFrontImageBase64 { get; set; }
        public string ProductTwoDBackImageBase64 { get; set; }
        public string ProductTwoDImageBase64 { get; set; }
        public bool IsDefaultLogo { get; set; }
        public bool IsMailSendToSalesCordinator { get; set; }
        public bool IsMailSendToMerchandiser { get; set; }
        public List<ProductSection> ProductSections { get; set; }
        public List<DYOProductDesignEmblishment> Embelishments { get; set; }
    }
    public class DYOProductDesignEmblishment
    {
        public int OrderEmblishmentId { get; set; }
        public int OrderDesignId { get; set; }
        public string EmblishmentType { get; set; }
        public string PrintMethod { get; set; }
        public LogoDimension LogoDimension { get; set; }
        public string LogoColor { get; set; }
        public List<DYOOrderEmblishmentDetail> CustomColors { get; set; }
        public string LogoName { get; set; }
        public string LogoImageBase64 { get; set; }
        public string PlayerName { get; set; }
        public string PlayerNumber { get; set; }
        public string FontFamily { get; set; }
        public string FontColor { get; set; }
        public bool IsActive { get; set; }
    }
    public class DYOOrderEmblishmentDetail
    {
        public int OrderEmblishmentDetailId { get; set; }
        public int OrderEmblishmentId { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
    }
    public class LogoDimension
    {
        public string DimensionUnit { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public bool IsAspectRatio { get; set; }
    }
    public class ProductSection
    {
        public int ProductSectionId { get; set; }
        public string SectionNumber { get; set; }
        public string SectionColor { get; set; }
    }
    #endregion

    #region DesignStock

    public class DYODesignCatagory
    {
        public int CatagoryId { get; set; }
        public string Catagory { get; set; }
        public string CatagoryDispaly { get; set; }
        public bool IsDisable { get; set; }

        public int PendingOrders { get; set; }


        public List<DYODesignStock> Products { get; set; }
    }

    public class DYODesignStock
    {
        public List<DYODesignSizeStock> ProductSizes { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string ProductThumbnailURL { get; set; }
        public int Quantity { get; set; }
    }

    public class DYODesignSizeStock
    {
        public int SizeId { get; set; }
        public string SizeName { get; set; }
        public string SizeDisplayName { get; set; }
        public int ColorId { get; set; }
        public string Color { get; set; }
        public string ColorCode { get; set; }
        public int Quantity { get; set; }
        public int OrderNumber { get; set; }
    }
    #endregion 

    public class AcceptRejectDesignResultModel
    {
        public bool Status { get; set; }
        public int OrderDesignId { get; set; }
        public int DesignStatusId { get; set; }

    }
    public class AcceptRejectDesign
    {
        public int OrderId { get; set; }
        public int OrderStatusId { get; set; }
        public string ActionType { get; set; }
        public int OrderDesignId { get; set; }
        public int DesignStatusId { get; set; }
        public int UserId { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CraetedOn { get; set; }
        public string Reason { get; set; }
    }

    public class SchedulerRejectDesign
    {
        public int OrderId { get; set; }
        public int OrderStatusId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        //public string ActionType { get; set; }
        public int OrderDesignId { get; set; }
        public int DesignStatusId { get; set; }
        //public int UserId { get; set; }
        //public bool IsPublic { get; set; }
        //public DateTime CraetedOn { get; set; }
        public string Reason { get; set; }
        public string DesignName { get; set; }
        public string DesignNumber { get; set; }
    }


    public class OrderDesignModel
    {
        public int orderid { get; set; }
        public byte statusid { get; set; }
        public int userid { get; set; }
        public string reason { get; set; }
        public Boolean ispublic { get; set; }
        public int roleid { get; set; }
        public DateTime delDate { get; set; }

    }

    public class OrderStrickOffModel
    {
        public int orderid { get; set; }
        public byte statusid { get; set; }
        public int userid { get; set; }
        public string reason { get; set; }
        public Boolean ispublic { get; set; }
        public int samplelogoid { get; set; }
    }
}
