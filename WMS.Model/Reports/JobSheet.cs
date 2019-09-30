using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Model.Reports
{
    #region

    public class JobSheetDYOOrder
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderNumber { get; set; }
        public string PONumber { get; set; }
        public string OrderName { get; set; }
        public string Customer { get; set; }
        public string DeliveryType { get; set; }
        public List<JobSheetOrderDesign> OrderDesigns { get; set; }
    }

    public class JobSheetOrderDesign
    {
        public int OrderDesignId { get; set; }
        public string DesignName { get; set; }
        public string DesignNumber { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string ProductImageURL { get; set; }
        public string ProductCode { get; set; }
        public string GarmentColor { get; set; }
        public string GarmentColorCode { get; set; }
        public bool IsCollapsed { get; set; }
        public bool IsDefaultLogo { get; set; }
        public List<OrderEmblishmnetDetail> OrderEmblishmentDetails { get; set; }
        public List<OrderDesignDetailReportModelDetail> OrderDesignDetail { get; set; }
    }

    public class OrderEmblishmnetDetail
    {
        public int OrderEmblishmnetId { get; set; }
        public string EmblishmentType { get; set; }
        public int OrderDesignId { get; set; }
        public string Color { get; set; }
        public string PrintMethod { get; set; }
        public string Font { get; set; }
        public string LogoWidth { get; set; }
        public string LogoHeight { get; set; }
        public string DimensionUnit { get; set; }
        public string LogoPath { get; set; }
    }

    #endregion

    public class MainDataSource
    {
        public string ReportType { get; set; }
        public string OrderName { get; set; }
        public string PONumber { get; set; }
        public string DesignNumber { get; set; }
        public string OrderNumber { get; set; }
        public string Customer { get; set; }
        public string StyleCode { get; set; }
        public string GarmentType { get; set; }
        public string Sizenumber { get; set; }
        public string FabricNumber { get; set; }
        public string Artist { get; set; }
        public DateTime Date { get; set; }
        public string GarmentColor { get; set; }
        public string GarmentColor2 { get; set; }
        public string GarmentColor3 { get; set; }
        public string GarmentColor4 { get; set; }
        public string GarmentColor5 { get; set; }
        public string GarmentColor6 { get; set; }
        public string GarmentColorCode { get; set; }
        public string DesignImagePath { get; set; }
        public MainDataSourceHeader HeaderInformation { get; set; }
        public List<ParentDataSource> Parent { get; set; }
        public List<OrderDesignDetailReportModel> OrderDesigns { get; set; }
    }

    public class MainDataSourceHeader
    {
        public string Logo { get; set; }
        public string Customer { get; set; }
        public string PONumber { get; set; }
        public string OrderDate { get; set; }
        public string FactoryDate { get; set; }
        public string CustomerAddress { get; set; }           
        public string DeliveryCustomerName { get; set; }
        public string DeliveryContactName { get; set; }
        public string DeliveryPhoneNumber { get; set; }
        public string DeliveryEmailAddress { get; set; }
        public string DeliveryAddressStreet { get; set; }
        public string DeliveryAddressSuburb { get; set; }
        public string DeliveryAddressState { get; set; }
        public string DeliveryPostCode { get; set; }
        public string DeliveryCountry { get; set; }
        public string DeliveryPort { get; set; }
        public string SalesOrderNumber { get; set; }
        public string SalesRepEmailAddress { get; set; }
        public string SalesRepCode { get; set; }
        public string SalesCustomerReference { get; set; }
        public string SalesProductDeliveryDate { get; set; }
        public string SalesEventDate { get; set; }
        public string SalesDeliveryTimelines { get; set; }
    }

    public class OrderDesignDetailReportModel
    {
        public string GarmentType { get; set; }
        public string StyleCode { get; set; }
        public string NameHeader { get; set; }
        public ReportColorModel Color1 { get; set; }
        public ReportColorModel Color2 { get; set; }
        public ReportColorModel Color3 { get; set; }
        public ReportColorModel Color4 { get; set; }
        public List<OrderDesignDetailReportModelDetail> OrderDesignDetails { get; set; }
    }

    public class OrderDesignDetailReportModelDetail
    {

        public string PlayerName { get; set; }
        public string PlayerNumber { get; set; }
        public string size { get; set; }
        public int Quantity { get; set; }
    }

    public class ReportColorModel
    {
        public string Color { get; set; }
        public string ColorCode { get; set; }
    }

    public class ParentDataSource
    {
        public ChildDataSource Child1 { get; set; }
        public ChildDataSource Child2 { get; set; }
        public ChildDataSource Child3 { get; set; }
        public ChildDataSource Child4 { get; set; }
    }

    public class ChildDataSource
    {
        public string ImagePath { get; set; }
        public string Position { get; set; }
        public string Size { get; set; }
        public string Font { get; set; }
        public string Printing { get; set; }
        public string Color1 { get; set; }
        public string Color2 { get; set; }
        public string Color3 { get; set; }
        public string Color4 { get; set; }
        public string Color5 { get; set; }
        public string Color6 { get; set; }
        public string Color7 { get; set; }
        public string Color8 { get; set; }
        public string Color9 { get; set; }
        public string Color10 { get; set; }
        public string Color11 { get; set; }
        public string Color12 { get; set; }
        public string Color13 { get; set; }
        public string Color14 { get; set; }
    }
}
