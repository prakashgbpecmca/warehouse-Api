using System;
using System.Collections.Generic;
//using Excel = Microsoft.Office.Interop.Excel;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using WMS.Model.Common;
using WMS.Model.DYO;
using WMS.Model.Reports;
using WMS.Service.DataAccess;

namespace WMS.Service.Repository
{
    public class ReportRepository
    {
        WMSEntities dbContext = new WMSEntities();

        public TimeZoneModal UserTimeZoneDetail(int userId)
        {
            var TimeZone = (from uu in dbContext.WMSUsers
                            join tz in dbContext.Timezones on uu.TimezoneId equals tz.TimezoneId
                            where uu.Id == userId
                            select tz).FirstOrDefault();
            TimeZoneModal TZ = new TimeZoneModal();
            if (TimeZone != null)
            {
                TZ.Name = TimeZone.Name;
                TZ.Offset = TimeZone.Offset;
                TZ.OffsetShort = TimeZone.OffsetShort;
                TZ.TimezoneId = TimeZone.TimezoneId;
            }
            return TZ;
        }
        public TimeZoneModal TimeZoneDetail(int CountryId)
        {
            var TimeZone = (from uu in dbContext.Countries
                            join tz in dbContext.Timezones on uu.TimeZoneId equals tz.TimezoneId
                            where uu.CountryId == CountryId
                            select tz).FirstOrDefault();
            TimeZoneModal TZ = new TimeZoneModal();
            if (TimeZone != null)
            {
                TZ.Name = TimeZone.Name;
                TZ.Offset = TimeZone.Offset;
                TZ.OffsetShort = TimeZone.OffsetShort;
                TZ.TimezoneId = TimeZone.TimezoneId;
            }
            return TZ;
        }
        public List<MainDataSource> GetJobSheetImage(int orderId, string type)
        {
            List<MainDataSource> dataSource = new List<MainDataSource>();
            MainDataSource obj;
            ParentDataSource emblishment;
            OrderDesignDetailReportModel designDetail;
            if (string.IsNullOrEmpty(type))
            {
                var collection = (from r in dbContext.Orders
                                  join od in dbContext.OrderDesigns on r.Id equals od.OrderId
                                  join ods in dbContext.OrderDesignStyles on od.Id equals ods.OrderDesignId into leftJoinTempStyle
                                  from sdsd in leftJoinTempStyle.DefaultIfEmpty()
                                  join pm in dbContext.ProductMasters on od.ProductId equals pm.Id
                                  join oemb in dbContext.OrderEmblishments on od.Id equals oemb.OrderDesignId into lestorderEmb
                                  from tempOrderEmd in lestorderEmb.DefaultIfEmpty()
                                  join u in dbContext.WMSUsers on r.CustomerId equals u.Id
                                  where r.Id == orderId
                                  select new
                                  {
                                      OrderId = r.Id,
                                      Customer = u.ContactFirstName + " " + u.ContactLastName,
                                      OrderDate = DateTime.UtcNow,
                                      OrderName = r.OrderName,
                                      OrderNumber = r.OrderNumber,
                                      PONumber = r.PONumber,
                                      DeliveryType = r.OrderType,
                                      CustomerName = u.CompanyName,

                                      OrderDesignId = od.Id,
                                      ProductImage = od.ProductDesignImage,
                                      ProductImageURL = "~/Files/Orders/" + od.OrderId + "/" + od.Id + "/" + od.ProductDesignImage,
                                      DesignName = od.DesignName,
                                      DesignNumber = od.DesignNumber,
                                      IsDefaultLogo = od.IsDeafultLogo,                                      
                                      ProductId = pm.Id,
                                      ProductName = pm.ProductName,
                                      ProductCode = pm.ProductCode,
                                      GarmentColor = od.ProductColor,
                                      OrderEmblishmentId = tempOrderEmd == null ? 0 : tempOrderEmd.Id,
                                      EmblishmentType = tempOrderEmd == null ? "" : tempOrderEmd.EmblishmentType,
                                      Colors = tempOrderEmd == null ? "" : tempOrderEmd.Color,
                                      LogoPath = "~/Files/Orders/" + od.OrderId + "/" + od.Id + "/" + (tempOrderEmd == null ? "" : tempOrderEmd.LogoImage),
                                      PrintMethod = tempOrderEmd == null ? "" : tempOrderEmd.PrintMethod,
                                      FontFamily = tempOrderEmd == null ? "" : tempOrderEmd.FontFamily,
                                      LogoWidth = tempOrderEmd == null ? "" : tempOrderEmd.LogoWidth.ToString(),
                                      LogoHeight = tempOrderEmd == null ? "" : tempOrderEmd.LogoHeight.ToString(),
                                      DimensionUnit = tempOrderEmd == null ? "" : tempOrderEmd.DimensionUnit
                                  }).ToList();


                List<JobSheetDYOOrder> orders = collection.GroupBy(p => p.OrderId)
                                     .Select(group => new JobSheetDYOOrder
                                     {
                                         OrderId = group.FirstOrDefault().OrderId,
                                         Customer = group.FirstOrDefault().CustomerName,
                                         DeliveryType = group.FirstOrDefault().DeliveryType,
                                         OrderDate = group.FirstOrDefault().OrderDate,
                                         OrderName = group.FirstOrDefault().OrderName,
                                         OrderNumber = group.FirstOrDefault().OrderNumber,
                                         PONumber = group.FirstOrDefault().PONumber,
                                         OrderDesigns = group.GroupBy(zx => new { zx.OrderId, zx.OrderDesignId }).Select(p => new JobSheetOrderDesign
                                         {
                                             DesignName = p.FirstOrDefault().DesignName,
                                             DesignNumber = p.FirstOrDefault().DesignNumber,
                                             OrderDesignId = p.FirstOrDefault().OrderDesignId,
                                             ProductCode = p.FirstOrDefault().ProductCode,
                                             ProductImageURL = p.FirstOrDefault().ProductImageURL,
                                             ProductId = p.FirstOrDefault().ProductId,
                                             ProductName = p.FirstOrDefault().ProductName,
                                             GarmentColor = p.FirstOrDefault().GarmentColor,
                                             IsDefaultLogo = p.FirstOrDefault().IsDefaultLogo,

                                             IsCollapsed = false,
                                             OrderEmblishmentDetails = p.GroupBy(xy => new { xy.OrderId, xy.OrderDesignId, xy.OrderEmblishmentId })
                                                 .Select(q => new OrderEmblishmnetDetail
                                                 {
                                                     OrderEmblishmnetId = q.FirstOrDefault().OrderEmblishmentId,
                                                     Color = q.FirstOrDefault().Colors,
                                                     Font = q.FirstOrDefault().FontFamily,
                                                     PrintMethod = q.FirstOrDefault().PrintMethod,
                                                     LogoPath = q.FirstOrDefault().LogoPath,
                                                     EmblishmentType = q.FirstOrDefault().EmblishmentType,
                                                     OrderDesignId = q.FirstOrDefault().OrderDesignId,
                                                     DimensionUnit = q.FirstOrDefault().DimensionUnit,
                                                     LogoHeight = q.FirstOrDefault().LogoHeight == "0.00" ? "" : q.FirstOrDefault().LogoHeight,
                                                     LogoWidth = q.FirstOrDefault().LogoWidth == "0.00" ? "" : q.FirstOrDefault().LogoWidth
                                                 }).ToList()

                                         }).ToList()

                                     }).OrderByDescending(p => p.OrderDate).ToList();

                foreach (var item in orders)
                {

                    foreach (var design in item.OrderDesigns)
                    {
                        design.ProductImageURL = HttpContext.Current.Server.MapPath(design.ProductImageURL);
                    }
                    if (item.OrderDesigns.Count == 1)
                    {
                        item.OrderName = item.OrderDesigns[0].DesignName;
                    }
                }

                if (orders.Count > 0)
                {
                    foreach (var item in orders)
                    {
                        #region "Report Header"
                        var reportHeader = (from r in dbContext.Orders
                                            join uc in dbContext.WMSUsers on r.CustomerId equals uc.Id
                                            join uadd in dbContext.WMSUserAddresses on r.CustomerId equals uadd.UserId
                                            join uaadc in dbContext.Countries on uadd.CountryId equals uaadc.CountryId
                                            join u in dbContext.WMSUserAdditionals on r.CustomerId equals u.UserId
                                            join us in dbContext.WMSUsers on r.CreatedBy equals us.Id
                                            join oa in dbContext.OrderAddresses on r.OrderAddressId equals oa.Id into left
                                            from temp in left.DefaultIfEmpty()
                                            let ciD = temp == null ? 0 : temp.CountryId
                                            join oadC in dbContext.Countries on ciD equals oadC.CountryId into letC
                                            from temC in letC.DefaultIfEmpty()
                                            where r.Id == item.OrderId
                                            select new
                                            {
                                                OrderNumber = r.OrderNumber,
                                                PONumber = r.PONumber,
                                                CustomerRef = u.AccountNumber,
                                                ValidatedExFactoryDate = r.ConfirmedExFactoryDate,
                                                OrderDate = r.CreatedOnUtc,
                                                SalesRepEmail = us.Email,

                                                DeliveryCompany = temp == null ? "" : temp.CompanyName,
                                                DeliveryContact = temp == null ? "" : temp.ContactFirstName + " " + temp.ContactLastName,
                                                DeliveryPhone = temC == null ? "" : "(+" + temC.CountryPhoneCode + ")" + temp.PhoneNumber,
                                                DeliveryEmail = temp == null ? "" : temp.Email,
                                                DeliveryAddress1 = temp == null ? "" : temp.Address,
                                                DeliveryAddress2 = temp == null ? "" : temp.Address2,
                                                DeliveryCity = temp == null ? "" : temp.City,
                                                DeliveryState = temp == null ? "" : temp.State,
                                                DeliveryPostCode = temp == null ? "" : temp.PostCode,
                                                DeliverySuberb = temp == null ? "" : temp.Suburb,
                                                DeliveryCounty = temC == null ? "" : temC.CountryName,


                                                CustomerCompany = uc.CompanyName,
                                                CustomerContact = uc.ContactFirstName + " " + uc.ContactLastName,
                                                CustomerPhone = "(+" + uaadc.CountryPhoneCode + ") " + uc.TelephoneNo,
                                                CustomerEmail = uc.Email,
                                                CustomerAddress1 = uadd.Address,
                                                CustomerAddress2 = uadd.Address2,
                                                CustomerCity = uadd.City,
                                                CustomerState = uadd.State,
                                                CustomerPostCode = uadd.PostCode,
                                                CustomerSuberb = uadd.Suburb,
                                                CustomerCounty = uaadc.CountryName,

                                            }
                                             ).FirstOrDefault();

                        #endregion

                        if (item.OrderDesigns.Count > 0)
                        {
                            foreach (var design in item.OrderDesigns)
                            {

                                obj = new MainDataSource();

                                obj.HeaderInformation = new MainDataSourceHeader();

                                #region "Address"
                                if (reportHeader != null)
                                {
                                    string Address = "";
                                    if (!string.IsNullOrEmpty(reportHeader.CustomerCompany))
                                    {
                                        Address = Address + reportHeader.CustomerCompany + Environment.NewLine;
                                    }
                                    else
                                    {
                                        Address = Address + reportHeader.CustomerContact + Environment.NewLine;
                                    }
                                    Address += "Address: ";
                                    if (!string.IsNullOrEmpty(reportHeader.CustomerAddress1))
                                    {
                                        Address = Address + reportHeader.CustomerAddress1 + Environment.NewLine;
                                    }
                                    if (!string.IsNullOrEmpty(reportHeader.CustomerAddress2))
                                    {
                                        Address = Address + reportHeader.CustomerAddress2 + Environment.NewLine;
                                    }
                                    if (!string.IsNullOrEmpty(reportHeader.CustomerCity))
                                    {
                                        Address = Address + reportHeader.CustomerCity;
                                    }
                                    if (!string.IsNullOrEmpty(reportHeader.CustomerPostCode))
                                    {
                                        Address = Address + " - " + reportHeader.CustomerPostCode + Environment.NewLine;
                                    }
                                    if (!string.IsNullOrEmpty(reportHeader.CustomerState))
                                    {
                                        Address = Address + reportHeader.CustomerState + Environment.NewLine; ;
                                    }
                                    if (reportHeader.CustomerCounty != null && !string.IsNullOrEmpty(reportHeader.CustomerCounty))
                                    {
                                        Address = Address + reportHeader.CustomerCounty + Environment.NewLine;
                                    }
                                    Address += "Phone: ";
                                    if (reportHeader.CustomerPhone != null)
                                    {
                                        Address += reportHeader.CustomerPhone + Environment.NewLine;
                                    }
                                    Address += "Email: ";

                                    if (!string.IsNullOrEmpty(reportHeader.CustomerEmail))
                                    {
                                        Address += reportHeader.CustomerEmail + Environment.NewLine;
                                    }


                                    obj.HeaderInformation.CustomerAddress = Address;

                                    obj.HeaderInformation.DeliveryCustomerName = reportHeader.DeliveryCompany;
                                    obj.HeaderInformation.DeliveryContactName = reportHeader.DeliveryContact;
                                    obj.HeaderInformation.DeliveryPhoneNumber = reportHeader.DeliveryPhone;
                                    obj.HeaderInformation.DeliveryEmailAddress = reportHeader.DeliveryEmail;
                                    obj.HeaderInformation.DeliveryAddressStreet = reportHeader.DeliveryAddress1;
                                    obj.HeaderInformation.DeliveryAddressSuburb = reportHeader.DeliveryAddress2;
                                    obj.HeaderInformation.DeliveryAddressState = reportHeader.DeliveryState;
                                    obj.HeaderInformation.DeliveryPostCode = reportHeader.DeliveryPostCode;
                                    obj.HeaderInformation.DeliveryCountry = reportHeader.DeliveryCounty;
                                    obj.HeaderInformation.DeliveryPort = string.Empty;
                                    obj.HeaderInformation.OrderDate = reportHeader.OrderDate.ToString("dd-MMM-yyyy");
                                    obj.HeaderInformation.SalesOrderNumber = reportHeader.OrderNumber;
                                    obj.HeaderInformation.SalesRepEmailAddress = reportHeader.SalesRepEmail;
                                    obj.HeaderInformation.SalesRepCode = string.Empty;
                                    obj.HeaderInformation.SalesCustomerReference = reportHeader.CustomerRef;
                                    obj.HeaderInformation.FactoryDate = reportHeader.ValidatedExFactoryDate.HasValue ? reportHeader.ValidatedExFactoryDate.Value.ToString("dd-MMM-yyyy") : "";
                                    obj.HeaderInformation.Logo = HttpContext.Current.Server.MapPath("~/Files/Images/brand-logo.png");
                                    obj.HeaderInformation.Customer = "DYNASTY STOCK ORDER DETAILS - CHINA";
                                    obj.HeaderInformation.SalesDeliveryTimelines = "DELIVERY TIMELINES - APPROXIMATE - 2.5 weeks";
                                }
                                #endregion

                                obj.ReportType = "OrderGraphic";
                                obj.Customer = item.Customer;
                                obj.DesignImagePath = design.ProductImageURL;
                                obj.GarmentColorCode = design.GarmentColor;

                                obj.GarmentType = design.ProductName;
                                obj.FabricNumber = "";
                                obj.StyleCode = design.ProductCode;
                                obj.Sizenumber = "";
                                //obj.PONumber = item.OrderNumber;
                                obj.PONumber = item.PONumber;
                                obj.OrderNumber = item.OrderNumber;
                                obj.DesignNumber = design.DesignNumber;

                                obj.Artist = "";
                                obj.Date = item.OrderDate;
                                obj.OrderName = item.OrderName;

                                // For Sock Range
                                var orderDesignStyles = dbContext.OrderDesignStyles.Where(p => p.OrderDesignId == design.OrderDesignId).ToList();
                                if (orderDesignStyles.Count > 0)
                                {
                                    if (orderDesignStyles.Count == 1)
                                    {
                                        var colour = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[0].Color).FirstOrDefault();
                                        obj.GarmentColor = orderDesignStyles[0].Color + "~" + colour.Color;
                                    }
                                    if (orderDesignStyles.Count == 2)
                                    {

                                        var colour1 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[0].Color).FirstOrDefault();
                                        obj.GarmentColor = orderDesignStyles[0].Color + "~" + colour1.Color;

                                        var colour2 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[1].Color).FirstOrDefault();
                                        obj.GarmentColor2 = orderDesignStyles[1].Color + "~" + colour2.Color;

                                    }
                                    if (orderDesignStyles.Count == 3)
                                    {
                                        var colour1 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[0].Color).FirstOrDefault();
                                        obj.GarmentColor = orderDesignStyles[0].Color + "~" + colour1.Color;

                                        var colour2 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[1].Color).FirstOrDefault();
                                        obj.GarmentColor2 = orderDesignStyles[1].Color + "~" + colour2.Color;

                                        var colour3 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[2].Color).FirstOrDefault();
                                        obj.GarmentColor3 = orderDesignStyles[2].Color + "~" + colour3.Color;
                                    }
                                    if (orderDesignStyles.Count == 4)
                                    {
                                        var colour1 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[0].Color).FirstOrDefault();
                                        obj.GarmentColor = orderDesignStyles[0].Color + "~" + colour1.Color;

                                        var colour2 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[1].Color).FirstOrDefault();
                                        obj.GarmentColor2 = orderDesignStyles[1].Color + "~" + colour2.Color;

                                        var colour3 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[2].Color).FirstOrDefault();
                                        obj.GarmentColor3 = orderDesignStyles[2].Color + "~" + colour3.Color;

                                        var colour4 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[3].Color).FirstOrDefault();
                                        obj.GarmentColor4 = orderDesignStyles[3].Color + "~" + colour4.Color;
                                    }
                                    if (orderDesignStyles.Count == 5)
                                    {
                                        var colour1 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[0].Color).FirstOrDefault();
                                        obj.GarmentColor = orderDesignStyles[0].Color + "~" + colour1.Color;

                                        var colour2 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[1].Color).FirstOrDefault();
                                        obj.GarmentColor2 = orderDesignStyles[1].Color + "~" + colour2.Color;

                                        var colour3 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[2].Color).FirstOrDefault();
                                        obj.GarmentColor3 = orderDesignStyles[2].Color + "~" + colour3.Color;

                                        var colour4 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[3].Color).FirstOrDefault();
                                        obj.GarmentColor4 = orderDesignStyles[3].Color + "~" + colour4.Color;

                                        var colour5 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[4].Color).FirstOrDefault();
                                        obj.GarmentColor5 = orderDesignStyles[4].Color + "~" + colour5.Color;
                                    }
                                    if (orderDesignStyles.Count == 6)
                                    {
                                        var colour1 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[0].Color).FirstOrDefault();
                                        obj.GarmentColor = orderDesignStyles[0].Color + "~" + colour1.Color;

                                        var colour2 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[1].Color).FirstOrDefault();
                                        obj.GarmentColor2 = orderDesignStyles[1].Color + "~" + colour2.Color;

                                        var colour3 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[2].Color).FirstOrDefault();
                                        obj.GarmentColor3 = orderDesignStyles[2].Color + "~" + colour3.Color;

                                        var colour4 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[3].Color).FirstOrDefault();
                                        obj.GarmentColor4 = orderDesignStyles[3].Color + "~" + colour4.Color;

                                        var colour5 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[4].Color).FirstOrDefault();
                                        obj.GarmentColor5 = orderDesignStyles[4].Color + "~" + colour5.Color;

                                        var colour6 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[5].Color).FirstOrDefault();
                                        obj.GarmentColor6 = orderDesignStyles[5].Color + "~" + colour6.Color;
                                    }
                                }
                                else
                                {
                                    // For Stock Range 
                                    var color = dbContext.Colors.Where(p => p.code == design.GarmentColor).FirstOrDefault();
                                    if (color != null)
                                    {
                                        obj.GarmentColor = design.GarmentColor + "~" + color.color1;
                                    }
                                }


                                // Add extra emblishment for default logo 
                                if (design.IsDefaultLogo)
                                {
                                    var emb1 = new OrderEmblishmnetDetail
                                    {
                                        OrderEmblishmnetId = 0,
                                        Color = "",
                                        Font = "",
                                        PrintMethod = "",
                                        LogoPath = "~/Files/Images/brand-logo.png",
                                        EmblishmentType = "DefaultLogo",
                                        OrderDesignId = 0,
                                        DimensionUnit = "",
                                        LogoHeight = "",
                                        LogoWidth = ""
                                    };
                                    design.OrderEmblishmentDetails.Add(emb1);
                                }


                                obj.Parent = new List<ParentDataSource>();
                                for (int i = 0; i < design.OrderEmblishmentDetails.Count; i++)
                                {
                                    design.OrderEmblishmentDetails[i].Color = design.OrderEmblishmentDetails[i].Color.Replace(',', ';');
                                    int val = design.OrderEmblishmentDetails[i].OrderEmblishmnetId;
                                    var ong = dbContext.OrderEmblishmentDetails.Where(p => p.OrderEmblishmentId == val).ToList();
                                    if (ong != null && ong.Count > 0)
                                    {
                                        foreach (var item1 in ong)
                                        {
                                            design.OrderEmblishmentDetails[i].Color += ";" + item1.Color;
                                        }
                                    }
                                }

                                for (int i = 0; i <= design.OrderEmblishmentDetails.Count; i = i + 4)
                                {
                                    if (design.OrderEmblishmentDetails.Count >= i)
                                    {
                                        emblishment = new ParentDataSource();

                                        if (design.OrderEmblishmentDetails.Count > i)
                                        {

                                            var colors = design.OrderEmblishmentDetails[i].Color.Split(';');

                                            emblishment.Child1 = new ChildDataSource();


                                            // emblishment.Child1.Position = design.OrderEmblishmentDetails[i].EmblishmentType;

                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.PlayerNameType)
                                            {
                                                var print = new DYORepository().getPrintingMethods("").Where(p => p.Key == design.OrderEmblishmentDetails[i].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {

                                                    emblishment.Child1.Printing = print.Value;

                                                }
                                                else
                                                {
                                                    emblishment.Child1.Printing = design.OrderEmblishmentDetails[i].PrintMethod;
                                                }
                                                emblishment.Child1.Size = string.Empty;
                                                emblishment.Child1.ImagePath = HttpContext.Current.Server.MapPath("~/Files/Images/PlayerName.png");
                                            }
                                            else if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.PlayerNumberType)
                                            {
                                                var print = new DYORepository().getPrintingMethods("").Where(p => p.Key == design.OrderEmblishmentDetails[i].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {

                                                    emblishment.Child1.Printing = print.Value;

                                                }
                                                else
                                                {
                                                    emblishment.Child1.Printing = design.OrderEmblishmentDetails[i].PrintMethod;
                                                }
                                                emblishment.Child1.Size = string.Empty;
                                                emblishment.Child1.ImagePath = HttpContext.Current.Server.MapPath("~/Files/Images/PlayerNumber.png");
                                            }
                                            else
                                            {
                                                var print = new DYORepository().getPrintingMethods("PrintMethods").Where(p => p.Key == design.OrderEmblishmentDetails[i].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {

                                                    emblishment.Child1.Printing = print.Value;

                                                }
                                                else
                                                {
                                                    emblishment.Child1.Printing = design.OrderEmblishmentDetails[i].PrintMethod;
                                                }

                                                if (!string.IsNullOrEmpty(design.OrderEmblishmentDetails[i].LogoWidth) && !string.IsNullOrEmpty(design.OrderEmblishmentDetails[i].LogoHeight))
                                                {
                                                    emblishment.Child1.Size = design.OrderEmblishmentDetails[i].LogoWidth + "(w)" + " x " + design.OrderEmblishmentDetails[i].LogoHeight + "(h)";
                                                }
                                                else if (string.IsNullOrEmpty(design.OrderEmblishmentDetails[i].LogoWidth) && string.IsNullOrEmpty(design.OrderEmblishmentDetails[i].LogoHeight))
                                                {
                                                    emblishment.Child1.Size = string.Empty;
                                                }
                                                else
                                                {
                                                    if (!string.IsNullOrEmpty(design.OrderEmblishmentDetails[i].LogoWidth) && string.IsNullOrEmpty(design.OrderEmblishmentDetails[i].LogoHeight))
                                                    {
                                                        emblishment.Child1.Size = design.OrderEmblishmentDetails[i].LogoWidth + "(w) " + "[AR]";
                                                    }
                                                    if (string.IsNullOrEmpty(design.OrderEmblishmentDetails[i].LogoWidth) && !string.IsNullOrEmpty(design.OrderEmblishmentDetails[i].LogoHeight))
                                                    {
                                                        emblishment.Child1.Size = design.OrderEmblishmentDetails[i].LogoHeight + "(h) " + "[AR]";
                                                    }
                                                }
                                                emblishment.Child1.ImagePath = HttpContext.Current.Server.MapPath(design.OrderEmblishmentDetails[i].LogoPath);
                                            }

                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.PlayerNameType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.PlayerNameDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.PlayerNumberType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.PlayerNumberDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.InsertTextType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.InsertTextDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.BackNeckType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.BackNeckDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.BackThighLeftType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.BackThighLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.BackThighRightType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.BackThighRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.ChestCenterTopType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.ChestCenterTopDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.FrontMainType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.FrontMainDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.LeftChestType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.LeftChestDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.LeftShoulder)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.LeftShoulderDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.LowerArmLeftType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.LowerArmLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.LowerArmRightType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.LowerArmRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.LowerBackType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.LowerBackDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.LowerSleeveLeftType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.LowerSleeveLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.LowerSleeveRightType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.LowerSleeveRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.MidBackType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.MidBackDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.RightChestType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.RightChestDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.RightShoulder)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.RightShoulderDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.SockCalfLeftType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.SockCalfLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.SockCalfRightType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.SockCalfRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.SockFrontLeftType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.SockFrontLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.SockFrontRightType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.SockFrontRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.ThighLeftType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.ThighLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.ThighRightType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.ThighRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.UploadLogoType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.UplaodLogoDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.UpperArmLeftType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.UpperArmLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.UpperArmRightType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.UpperArmRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == "DefaultLogo")
                                            {
                                                emblishment.Child1.Position = "Default Logo";
                                            }



                                            emblishment.Child1.Font = design.OrderEmblishmentDetails[i].Font;
                                            if (colors.Length > 0)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[0]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child1.Color1 = colors[0] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color1 = colors[0];
                                                }
                                            }
                                            if (colors.Length > 1)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[1]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {

                                                    emblishment.Child1.Color2 = colors[1] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color2 = colors[1];
                                                }
                                                //emblishment.Child1.Color2 = colors[1];
                                            }
                                            if (colors.Length > 2)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[2]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child1.Color3 = colors[2] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color3 = colors[2];
                                                }
                                                //   emblishment.Child1.Color3 = colors[2];
                                            }
                                            if (colors.Length > 3)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[3]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child1.Color4 = colors[3] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color4 = colors[3];
                                                }
                                                //  emblishment.Child1.Color4 = colors[3];
                                            }
                                            if (colors.Length > 4)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[4]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child1.Color5 = colors[4] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color5 = colors[4];
                                                }
                                                //    emblishment.Child1.Color5 = colors[4];
                                            }
                                            if (colors.Length > 5)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[5]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child1.Color6 = colors[5] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color6 = colors[5];
                                                }
                                                // emblishment.Child1.Color6 = colors[5];
                                            }
                                            if (colors.Length > 6)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[6]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child1.Color7 = colors[6] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color7 = colors[6];
                                                }
                                                // emblishment.Child1.Color7 = colors[6];
                                            }
                                            if (colors.Length > 7)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[7]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child1.Color8 = colors[7] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color8 = colors[7];
                                                }
                                                //  emblishment.Child1.Color8 = colors[7];
                                            }
                                            if (colors.Length > 8)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[8]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child1.Color9 = colors[8] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color9 = colors[8];
                                                }
                                                //emblishment.Child1.Color9 = colors[8];
                                            }
                                            if (colors.Length > 9)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[9]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child1.Color10 = colors[9] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color10 = colors[9];
                                                }
                                                //  emblishment.Child1.Color10 = colors[9];
                                            }
                                            if (colors.Length > 10)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[10]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child1.Color11 = colors[10] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color11 = colors[10];
                                                }
                                                //    emblishment.Child1.Color11 = colors[10];
                                            }
                                            if (colors.Length > 11)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[11]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child1.Color12 = colors[11] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color12 = colors[11];
                                                }
                                                //  emblishment.Child1.Color12 = colors[11];
                                            }
                                        }

                                        if (design.OrderEmblishmentDetails.Count > i + 1)
                                        {
                                            var colors = design.OrderEmblishmentDetails[i + 1].Color.Split(';');
                                            emblishment.Child2 = new ChildDataSource();
                                            //     emblishment.Child2.Printing = design.OrderEmblishmentDetails[i + 1].PrintMethod;
                                            //emblishment.Child2.Position = design.OrderEmblishmentDetails[i + 1].EmblishmentType;
                                            //emblishment.Child2.Size = design.OrderEmblishmentDetails[i + 1].LogoWidth + "x" + design.OrderEmblishmentDetails[i + 1].LogoHeight;
                                            //emblishment.Child2.ImagePath = design.OrderEmblishmentDetails[i + 1].LogoPath;

                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.PlayerNameType)
                                            {
                                                emblishment.Child2.Size = string.Empty;
                                                emblishment.Child2.ImagePath = HttpContext.Current.Server.MapPath("~/Files/Images/PlayerName.png");
                                                var print = new DYORepository().getPrintingMethods("").Where(p => p.Key == design.OrderEmblishmentDetails[i + 1].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {

                                                    emblishment.Child2.Printing = print.Value;

                                                }
                                                else
                                                {
                                                    emblishment.Child2.Printing = design.OrderEmblishmentDetails[i + 1].PrintMethod;
                                                }

                                            }
                                            else if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.PlayerNumberType)
                                            {
                                                emblishment.Child2.Size = string.Empty;
                                                emblishment.Child2.ImagePath = HttpContext.Current.Server.MapPath("~/Files/Images/PlayerNumber.png");
                                                var print = new DYORepository().getPrintingMethods("").Where(p => p.Key == design.OrderEmblishmentDetails[i + 1].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {

                                                    emblishment.Child2.Printing = print.Value;

                                                }
                                                else
                                                {
                                                    emblishment.Child2.Printing = design.OrderEmblishmentDetails[i + 1].PrintMethod;
                                                }
                                            }
                                            else
                                            {

                                                if (!string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 1].LogoWidth) && !string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 1].LogoHeight))
                                                {
                                                    emblishment.Child2.Size = design.OrderEmblishmentDetails[i + 1].LogoWidth + "(w)" + " x " + design.OrderEmblishmentDetails[i + 1].LogoHeight + "(h)";
                                                }
                                                else if (string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 1].LogoWidth) && string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 1].LogoHeight))
                                                {
                                                    emblishment.Child2.Size = string.Empty;
                                                }
                                                else
                                                {
                                                    if (!string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 1].LogoWidth) && string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 1].LogoHeight))
                                                    {
                                                        emblishment.Child2.Size = design.OrderEmblishmentDetails[i + 1].LogoWidth + "(w) " + "[AR]";
                                                    }
                                                    if (string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 1].LogoWidth) && !string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 1].LogoHeight))
                                                    {
                                                        emblishment.Child2.Size = design.OrderEmblishmentDetails[i + 1].LogoHeight + "(h) " + "[AR]";
                                                    }
                                                }



                                                //  emblishment.Child2.Size = design.OrderEmblishmentDetails[i + 1].LogoWidth + " x " + design.OrderEmblishmentDetails[i + 1].LogoHeight;



                                                emblishment.Child2.ImagePath = HttpContext.Current.Server.MapPath(design.OrderEmblishmentDetails[i + 1].LogoPath);
                                                var print = new DYORepository().getPrintingMethods("PrintMethods").Where(p => p.Key == design.OrderEmblishmentDetails[i + 1].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {

                                                    emblishment.Child2.Printing = print.Value;

                                                }
                                                else
                                                {
                                                    emblishment.Child2.Printing = design.OrderEmblishmentDetails[i + 1].PrintMethod;
                                                }
                                            }

                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.PlayerNameType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.PlayerNameDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.PlayerNumberType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.PlayerNumberDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.InsertTextType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.InsertTextDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.BackNeckType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.BackNeckDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.BackThighLeftType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.BackThighLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.BackThighRightType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.BackThighRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.ChestCenterTopType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.ChestCenterTopDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.FrontMainType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.FrontMainDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.LeftChestType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.LeftChestDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.LeftShoulder)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.LeftShoulderDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.LowerArmLeftType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.LowerArmLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.LowerArmRightType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.LowerArmRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.LowerBackType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.LowerBackDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.LowerSleeveLeftType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.LowerSleeveLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.LowerSleeveRightType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.LowerSleeveRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.MidBackType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.MidBackDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.RightChestType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.RightChestDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.RightShoulder)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.RightShoulderDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.SockCalfLeftType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.SockCalfLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.SockCalfRightType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.SockCalfRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.SockFrontLeftType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.SockFrontLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.SockFrontRightType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.SockFrontRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.ThighLeftType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.ThighLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.ThighRightType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.ThighRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.UploadLogoType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.UplaodLogoDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.UpperArmLeftType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.UpperArmLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.UpperArmRightType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.UpperArmRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == "DefaultLogo")
                                            {
                                                emblishment.Child2.Position = "Default Logo";
                                            }

                                            emblishment.Child2.Font = design.OrderEmblishmentDetails[i + 1].Font;
                                            if (colors.Length > 0)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[0]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color1 = colors[0] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color1 = colors[0];
                                                }

                                                // emblishment.Child2.Color1 = colors[0];
                                            }
                                            if (colors.Length > 1)
                                            {

                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[1]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color2 = colors[1] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color2 = colors[1];
                                                }

                                                // emblishment.Child2.Color2 = colors[1];
                                            }
                                            if (colors.Length > 2)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[2]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color3 = colors[2] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color3 = colors[2];
                                                }
                                                //  emblishment.Child2.Color3 = colors[2];
                                            }
                                            if (colors.Length > 3)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[3]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color4 = colors[3] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color4 = colors[3];
                                                }
                                                // emblishment.Child2.Color4 = colors[3];
                                            }
                                            if (colors.Length > 4)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[4]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color5 = colors[4] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color5 = colors[4];
                                                }
                                                //    emblishment.Child2.Color5 = colors[4];
                                            }
                                            if (colors.Length > 5)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[5]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color6 = colors[5] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color6 = colors[5];
                                                }
                                                //  emblishment.Child2.Color6 = colors[5];
                                            }
                                            if (colors.Length > 6)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[6]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color7 = colors[6] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color7 = colors[6];
                                                }
                                                // emblishment.Child2.Color7 = colors[6];
                                            }
                                            if (colors.Length > 7)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[7]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color8 = colors[7] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color8 = colors[7];
                                                }
                                                //  emblishment.Child2.Color8 = colors[7];
                                            }
                                            if (colors.Length > 8)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[8]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color9 = colors[8] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color9 = colors[8];
                                                }
                                                //  emblishment.Child2.Color9 = colors[8];
                                            }
                                            if (colors.Length > 9)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[9]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color10 = colors[9] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color10 = colors[9];
                                                }
                                                //  emblishment.Child1.Color10 = colors[9];
                                            }
                                            if (colors.Length > 10)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[10]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color11 = colors[10] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color11 = colors[10];
                                                }
                                                // emblishment.Child2.Color11 = colors[10];
                                            }
                                            if (colors.Length > 11)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[11]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color12 = colors[11] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color12 = colors[11];
                                                }
                                                // emblishment.Child2.Color12 = colors[11];
                                            }

                                        }
                                        if (design.OrderEmblishmentDetails.Count > i + 2)
                                        {
                                            var colors = design.OrderEmblishmentDetails[i + 2].Color.Split(';');

                                            emblishment.Child3 = new ChildDataSource();
                                            emblishment.Child3 = new ChildDataSource();
                                            // emblishment.Child3.Printing = design.OrderEmblishmentDetails[i + 2].PrintMethod;
                                            //emblishment.Child3.Position = design.OrderEmblishmentDetails[i + 2].EmblishmentType;
                                            //emblishment.Child3.Size = design.OrderEmblishmentDetails[i + 2].LogoWidth + "x" + design.OrderEmblishmentDetails[i + 2].LogoHeight;
                                            //emblishment.Child3.ImagePath = design.OrderEmblishmentDetails[i + 2].LogoPath;

                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.PlayerNameType)
                                            {
                                                emblishment.Child3.Size = string.Empty;
                                                emblishment.Child3.ImagePath = HttpContext.Current.Server.MapPath("~/Files/Images/PlayerName.png");
                                                var print = new DYORepository().getPrintingMethods("").Where(p => p.Key == design.OrderEmblishmentDetails[i + 2].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {

                                                    emblishment.Child3.Printing = print.Value;

                                                }
                                                else
                                                {
                                                    emblishment.Child3.Printing = design.OrderEmblishmentDetails[i + 2].PrintMethod;
                                                }
                                            }
                                            else if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.PlayerNumberType)
                                            {
                                                emblishment.Child3.Size = string.Empty;
                                                emblishment.Child3.ImagePath = HttpContext.Current.Server.MapPath("~/Files/Images/PlayerNumber.png");
                                                var print = new DYORepository().getPrintingMethods("").Where(p => p.Key == design.OrderEmblishmentDetails[i + 2].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {

                                                    emblishment.Child3.Printing = print.Value;

                                                }
                                                else
                                                {
                                                    emblishment.Child3.Printing = design.OrderEmblishmentDetails[i + 2].PrintMethod;
                                                }
                                            }
                                            else
                                            {

                                                if (!string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 2].LogoWidth) && !string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 2].LogoHeight))
                                                {
                                                    emblishment.Child3.Size = design.OrderEmblishmentDetails[i + 2].LogoWidth + "(w)" + " x " + design.OrderEmblishmentDetails[i + 2].LogoHeight + "(h)";
                                                }
                                                else if (string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 2].LogoWidth) && string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 2].LogoHeight))
                                                {
                                                    emblishment.Child3.Size = string.Empty;
                                                }
                                                else
                                                {
                                                    if (!string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 2].LogoWidth) && string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 2].LogoHeight))
                                                    {
                                                        emblishment.Child3.Size = design.OrderEmblishmentDetails[i + 2].LogoWidth + "(w) " + "[AR]";
                                                    }
                                                    if (string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 2].LogoWidth) && !string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 2].LogoHeight))
                                                    {
                                                        emblishment.Child3.Size = design.OrderEmblishmentDetails[i + 2].LogoHeight + "(h) " + "[AR]";
                                                    }
                                                }



                                                //   emblishment.Child3.Size = design.OrderEmblishmentDetails[i + 2].LogoWidth + " x " + design.OrderEmblishmentDetails[i + 2].LogoHeight;
                                                emblishment.Child3.ImagePath = HttpContext.Current.Server.MapPath(design.OrderEmblishmentDetails[i + 2].LogoPath);
                                                var print = new DYORepository().getPrintingMethods("PrintMethods").Where(p => p.Key == design.OrderEmblishmentDetails[i + 2].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {
                                                    emblishment.Child3.Printing = print.Value;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Printing = design.OrderEmblishmentDetails[i + 2].PrintMethod;
                                                }
                                            }

                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.PlayerNameType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.PlayerNameDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.PlayerNumberType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.PlayerNumberDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.InsertTextType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.InsertTextDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.BackNeckType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.BackNeckDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.BackThighLeftType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.BackThighLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.BackThighRightType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.BackThighRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.ChestCenterTopType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.ChestCenterTopDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.FrontMainType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.FrontMainDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.LeftChestType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.LeftChestDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.LeftShoulder)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.LeftShoulderDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.LowerArmLeftType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.LowerArmLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.LowerArmRightType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.LowerArmRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.LowerBackType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.LowerBackDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.LowerSleeveLeftType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.LowerSleeveLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.LowerSleeveRightType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.LowerSleeveRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.MidBackType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.MidBackDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.RightChestType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.RightChestDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.RightShoulder)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.RightShoulderDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.SockCalfLeftType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.SockCalfLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.SockCalfRightType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.SockCalfRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.SockFrontLeftType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.SockFrontLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.SockFrontRightType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.SockFrontRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.ThighLeftType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.ThighLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.ThighRightType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.ThighRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.UploadLogoType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.UplaodLogoDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.UpperArmLeftType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.UpperArmLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.UpperArmRightType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.UpperArmRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == "DefaultLogo")
                                            {
                                                emblishment.Child3.Position = "Default Logo";
                                            }

                                            emblishment.Child3.Font = design.OrderEmblishmentDetails[i + 2].Font;
                                            if (colors.Length > 0)
                                            {

                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[0]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color1 = colors[0] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color1 = colors[0];
                                                }
                                                // emblishment.Child3.Color1 = colors[0];
                                            }
                                            if (colors.Length > 1)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[1]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color2 = colors[1] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color2 = colors[1];
                                                }
                                                // emblishment.Child3.Color2 = colors[1];
                                            }
                                            if (colors.Length > 2)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[2]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color3 = colors[2] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color3 = colors[2];
                                                }
                                                // emblishment.Child3.Color3 = colors[2];
                                            }
                                            if (colors.Length > 3)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[3]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color4 = colors[3] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color4 = colors[3];
                                                }
                                                //  emblishment.Child3.Color4 = colors[3];
                                            }
                                            if (colors.Length > 4)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[4]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color5 = colors[4] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color5 = colors[4];
                                                }
                                                // emblishment.Child3.Color5 = colors[4];
                                            }
                                            if (colors.Length > 5)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[5]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color6 = colors[5] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color6 = colors[5];
                                                }
                                                //  emblishment.Child3.Color6 = colors[5];
                                            }
                                            if (colors.Length > 6)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[6]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color7 = colors[6] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color7 = colors[6];
                                                }
                                                //    emblishment.Child3.Color7 = colors[6];
                                            }
                                            if (colors.Length > 7)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[7]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color8 = colors[7] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color8 = colors[7];
                                                }

                                                emblishment.Child3.Color8 = colors[7];
                                            }
                                            if (colors.Length > 8)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[8]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color9 = colors[8] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color9 = colors[8];
                                                }

                                                //  emblishment.Child3.Color9 = colors[8];
                                            }
                                            if (colors.Length > 9)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[9]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color10 = colors[9] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color10 = colors[9];
                                                }
                                                //     emblishment.Child3.Color10 = colors[9];
                                            }
                                            if (colors.Length > 10)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[10]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color11 = colors[10] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color11 = colors[10];
                                                }
                                                //   emblishment.Child3.Color11 = colors[10];
                                            }
                                            if (colors.Length > 11)
                                            {

                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[11]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color12 = colors[11] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color12 = colors[11];
                                                }
                                                // emblishment.Child3.Color12 = colors[11];
                                            }

                                        }
                                        if (design.OrderEmblishmentDetails.Count > i + 3)
                                        {
                                            emblishment.Child4 = new ChildDataSource();
                                            var colors = design.OrderEmblishmentDetails[i + 3].Color.Split(';');
                                            //  emblishment.Child4.Printing = design.OrderEmblishmentDetails[i + 3].PrintMethod;
                                            //emblishment.Child4.Position = design.OrderEmblishmentDetails[i + 3].EmblishmentType;
                                            //emblishment.Child4.Size = design.OrderEmblishmentDetails[i + 3].LogoWidth + "x" + design.OrderEmblishmentDetails[i + 3].LogoHeight;
                                            //emblishment.Child4.ImagePath = design.OrderEmblishmentDetails[i + 3].LogoPath;


                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.PlayerNameType)
                                            {
                                                emblishment.Child4.Size = string.Empty;
                                                emblishment.Child4.ImagePath = HttpContext.Current.Server.MapPath("~/Files/Images/PlayerName.png");
                                                var print = new DYORepository().getPrintingMethods("").Where(p => p.Key == design.OrderEmblishmentDetails[i + 3].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {

                                                    emblishment.Child4.Printing = print.Value;

                                                }
                                                else
                                                {
                                                    emblishment.Child4.Printing = design.OrderEmblishmentDetails[i = 3].PrintMethod;
                                                }
                                            }
                                            else if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.PlayerNumberType)
                                            {
                                                emblishment.Child4.Size = string.Empty;
                                                emblishment.Child4.ImagePath = HttpContext.Current.Server.MapPath("~/Files/Images/PlayerNumber.png");
                                                var print = new DYORepository().getPrintingMethods("").Where(p => p.Key == design.OrderEmblishmentDetails[i + 3].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {

                                                    emblishment.Child4.Printing = print.Value;

                                                }
                                                else
                                                {
                                                    emblishment.Child4.Printing = design.OrderEmblishmentDetails[i + 3].PrintMethod;
                                                }
                                            }
                                            else
                                            {

                                                if (!string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 3].LogoWidth) && !string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 3].LogoHeight))
                                                {
                                                    emblishment.Child4.Size = design.OrderEmblishmentDetails[i + 3].LogoWidth + "(w)" + " x " + design.OrderEmblishmentDetails[i + 3].LogoHeight + "(h)";
                                                }
                                                else if (string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 3].LogoWidth) && string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 3].LogoHeight))
                                                {
                                                    emblishment.Child4.Size = string.Empty;
                                                }
                                                else
                                                {
                                                    if (!string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 3].LogoWidth) && string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 3].LogoHeight))
                                                    {
                                                        emblishment.Child4.Size = design.OrderEmblishmentDetails[i + 3].LogoWidth + "(w) " + "[AR]";
                                                    }
                                                    if (string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 3].LogoWidth) && !string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 3].LogoHeight))
                                                    {
                                                        emblishment.Child4.Size = design.OrderEmblishmentDetails[i + 3].LogoHeight + "(h) " + "[AR]";
                                                    }
                                                }

                                                //  emblishment.Child4.Size = design.OrderEmblishmentDetails[i + 3].LogoWidth + " x " + design.OrderEmblishmentDetails[i + 3].LogoHeight;
                                                emblishment.Child4.ImagePath = HttpContext.Current.Server.MapPath(design.OrderEmblishmentDetails[i + 3].LogoPath);
                                                //   emblishment.Child3.Size = design.OrderEmblishmentDetails[i + 3].LogoWidth + " x " + design.OrderEmblishmentDetails[i + 3].LogoHeight;
                                                emblishment.Child4.ImagePath = HttpContext.Current.Server.MapPath(design.OrderEmblishmentDetails[i + 3].LogoPath);
                                                var print = new DYORepository().getPrintingMethods("PrintMethods").Where(p => p.Key == design.OrderEmblishmentDetails[i + 3].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {

                                                    emblishment.Child4.Printing = print.Value;

                                                }
                                                else
                                                {
                                                    emblishment.Child4.Printing = design.OrderEmblishmentDetails[i + 3].PrintMethod;
                                                }
                                            }

                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.PlayerNameType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.PlayerNameDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.PlayerNumberType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.PlayerNumberDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.InsertTextType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.InsertTextDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.BackNeckType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.BackNeckDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.BackThighLeftType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.BackThighLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.BackThighRightType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.BackThighRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.ChestCenterTopType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.ChestCenterTopDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.FrontMainType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.FrontMainDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.LeftChestType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.LeftChestDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.LeftShoulder)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.LeftShoulderDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.LowerArmLeftType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.LowerArmLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.LowerArmRightType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.LowerArmRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.LowerBackType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.LowerBackDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.LowerSleeveLeftType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.LowerSleeveLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.LowerSleeveRightType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.LowerSleeveRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.MidBackType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.MidBackDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.RightChestType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.RightChestDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.RightShoulder)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.RightShoulderDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.SockCalfLeftType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.SockCalfLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.SockCalfRightType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.SockCalfRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.SockFrontLeftType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.SockFrontLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.SockFrontRightType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.SockFrontRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.ThighLeftType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.ThighLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.ThighRightType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.ThighRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.UploadLogoType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.UplaodLogoDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.UpperArmLeftType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.UpperArmLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.UpperArmRightType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.UpperArmRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == "DefaultLogo")
                                            {
                                                emblishment.Child4.Position = "Default Logo";
                                            }

                                            emblishment.Child4.Font = design.OrderEmblishmentDetails[i + 3].Font;
                                            if (colors.Length > 0)
                                            {

                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[0]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color1 = colors[0] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color1 = colors[0];
                                                }
                                                // emblishment.Child4.Color1 = colors[0];
                                            }
                                            if (colors.Length > 1)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[1]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color2 = colors[1] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color2 = colors[1];
                                                }
                                                // emblishment.Child4.Color2 = colors[1];
                                            }
                                            if (colors.Length > 2)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[2]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color3 = colors[2] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color3 = colors[2];
                                                }
                                                // emblishment.Child4.Color3 = colors[2];
                                            }
                                            if (colors.Length > 3)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[3]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color4 = colors[3] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color4 = colors[3];
                                                }
                                                //  emblishment.Child4.Color4 = colors[3];
                                            }
                                            if (colors.Length > 4)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[4]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color5 = colors[4] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color5 = colors[4];
                                                }
                                                // emblishment.Child3.Color5 = colors[4];
                                            }
                                            if (colors.Length > 5)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[5]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color6 = colors[5] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color6 = colors[5];
                                                }
                                                //  emblishment.Child4.Color6 = colors[5];
                                            }
                                            if (colors.Length > 6)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[6]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color7 = colors[6] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color7 = colors[6];
                                                }
                                                //    emblishment.Child3.Color7 = colors[6];
                                            }
                                            if (colors.Length > 7)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[7]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color8 = colors[7] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color8 = colors[7];
                                                }

                                                //    emblishment.Child4.Color8 = colors[7];
                                            }
                                            if (colors.Length > 8)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[8]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color9 = colors[8] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color9 = colors[8];
                                                }

                                                //  emblishment.Child3.Color9 = colors[8];
                                            }
                                            if (colors.Length > 9)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[9]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color10 = colors[9] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color10 = colors[9];
                                                }
                                                //     emblishment.Child3.Color10 = colors[9];
                                            }
                                            if (colors.Length > 10)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[10]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color11 = colors[10] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color11 = colors[10];
                                                }
                                                //   emblishment.Child3.Color11 = colors[10];
                                            }
                                            if (colors.Length > 11)
                                            {

                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[11]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color12 = colors[11] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color12 = colors[11];
                                                }
                                                // emblishment.Child3.Color12 = colors[11];
                                            }
                                        }
                                        if (emblishment.Child1 == null && emblishment.Child2 == null && emblishment.Child3 == null && emblishment.Child4 == null)
                                        {

                                        }
                                        else
                                        {
                                            obj.Parent.Add(emblishment);
                                        }

                                    }
                                }
                                dataSource.Add(obj);
                            }
                        }
                    }
                }
            }
            else
            {
                var collection = (from r in dbContext.Orders
                                  join od in dbContext.OrderDesigns on r.Id equals od.OrderId
                                  join pm in dbContext.ProductMasters on od.ProductId equals pm.Id
                                  join oemb in dbContext.OrderEmblishments on od.Id equals oemb.OrderDesignId into tempOrderEmb
                                  from ordEmb in tempOrderEmb.DefaultIfEmpty()
                                  let tmpIdForOrderEmblishment = (ordEmb == null ? 0 : ordEmb.Id)
                                  join odd in dbContext.OrderDesignDetails on od.Id equals odd.OrderDesignId
                                  join psku in dbContext.ProductSKUs on odd.ProductSKUId equals psku.Id
                                  join size in dbContext.Sizes on psku.SizeId equals size.ID
                                  join u in dbContext.WMSUsers on r.CustomerId equals u.Id
                                  join oembd in dbContext.OrderEmblishmentDetails on tmpIdForOrderEmblishment equals oembd.OrderEmblishmentId into tempstock
                                  from oembd in tempstock.DefaultIfEmpty()
                                  where r.Id == orderId

                                  select new
                                  {
                                      OrderId = r.Id,
                                      Customer = u.ContactFirstName + " " + u.ContactLastName,
                                      OrderDate = DateTime.UtcNow,
                                      OrderName = r.OrderName,
                                      OrderNumber = r.OrderNumber,
                                      PONumber = r.PONumber,
                                      DeliveryType = r.OrderType,
                                      CustomerName = u.CompanyName,

                                      OrderDesignId = od.Id,
                                      ProductImage = od.ProductDesignImage,
                                      ProductImageURL = "~/Files/Orders/" + od.OrderId + "/" + od.Id + "/" + od.ProductDesignImage,
                                      DesignName = od.DesignName,
                                      DesignNumber = od.DesignNumber,
                                      ProductId = pm.Id,
                                      ProductName = pm.ProductName,
                                      ProductCode = pm.ProductCode,
                                      GarmentColor = od.ProductColor,
                                      IsDefaultLogo = od.IsDeafultLogo,
                                      OrderEmblishmentId = ordEmb == null ? 0 : ordEmb.Id,
                                      EmblishmentType = ordEmb == null ? "" : ordEmb.EmblishmentType,
                                      Colors = ordEmb == null ? "" : ordEmb.Color,
                                      LogoPath = "~/Files/Orders/" + od.OrderId + "/" + od.Id + "/" + (ordEmb == null ? "" : ordEmb.LogoImage),
                                      PrintMethod = ordEmb == null ? "" : ordEmb.PrintMethod,
                                      FontFamily = ordEmb == null ? "" : ordEmb.FontFamily,
                                      LogoWidth = ordEmb == null ? "" : ordEmb.LogoWidth.ToString(),
                                      LogoHeight = ordEmb == null ? "" : ordEmb.LogoHeight.ToString(),
                                      DimensionUnit = ordEmb == null ? "" : ordEmb.DimensionUnit,


                                      OrderDesignDetailId = odd.Id,
                                      PlayerName = odd.PlayerName,
                                      PlayerNumber = odd.PlayerNumber,
                                      Quanity = odd.Quantity,

                                      size = size.size1

                                  }).ToList();


                List<JobSheetDYOOrder> orders = collection.GroupBy(p => p.OrderId)
                                     .Select(group => new JobSheetDYOOrder
                                     {
                                         OrderId = group.FirstOrDefault().OrderId,
                                         Customer = group.FirstOrDefault().CustomerName,
                                         DeliveryType = group.FirstOrDefault().DeliveryType,
                                         OrderDate = group.FirstOrDefault().OrderDate,
                                         OrderName = group.FirstOrDefault().OrderName,
                                         OrderNumber = group.FirstOrDefault().OrderNumber,
                                         PONumber = group.FirstOrDefault().PONumber,
                                         OrderDesigns = group.GroupBy(zx => new { zx.OrderId, zx.OrderDesignId }).Select(p => new JobSheetOrderDesign
                                         {
                                             DesignName = p.FirstOrDefault().DesignName,
                                             DesignNumber=p.FirstOrDefault().DesignNumber,
                                             OrderDesignId = p.FirstOrDefault().OrderDesignId,
                                             ProductCode = p.FirstOrDefault().ProductCode,
                                             ProductImageURL = p.FirstOrDefault().ProductImageURL,
                                             ProductId = p.FirstOrDefault().ProductId,
                                             ProductName = p.FirstOrDefault().ProductName,
                                             GarmentColor = p.FirstOrDefault().GarmentColor,
                                             IsDefaultLogo = p.FirstOrDefault().IsDefaultLogo,
                                             IsCollapsed = false,
                                             OrderEmblishmentDetails = p.GroupBy(xy => new { xy.OrderId, xy.OrderDesignId, xy.OrderEmblishmentId })
                                                 .Select(q => new OrderEmblishmnetDetail
                                                 {
                                                     OrderEmblishmnetId = q.FirstOrDefault().OrderEmblishmentId,
                                                     Color = q.FirstOrDefault().Colors,
                                                     Font = q.FirstOrDefault().FontFamily,
                                                     PrintMethod = q.FirstOrDefault().PrintMethod,
                                                     LogoPath = q.FirstOrDefault().LogoPath,
                                                     EmblishmentType = q.FirstOrDefault().EmblishmentType,
                                                     OrderDesignId = q.FirstOrDefault().OrderDesignId,
                                                     DimensionUnit = q.FirstOrDefault().DimensionUnit,
                                                     LogoHeight = q.FirstOrDefault().LogoHeight == "0.00" ? "" : q.FirstOrDefault().LogoHeight,
                                                     LogoWidth = q.FirstOrDefault().LogoWidth == "0.00" ? "" : q.FirstOrDefault().LogoWidth
                                                 }).ToList(),
                                             OrderDesignDetail = p.GroupBy(zx => new { zx.OrderId, zx.OrderDesignId, zx.OrderDesignDetailId })
                                                                   .Select(ba => new OrderDesignDetailReportModelDetail
                                                                   {
                                                                       PlayerName = ba.FirstOrDefault().PlayerName,
                                                                       PlayerNumber = ba.FirstOrDefault().PlayerNumber,
                                                                       Quantity = ba.FirstOrDefault().Quanity,
                                                                       size = ba.FirstOrDefault().size
                                                                   }).ToList()

                                         }).ToList()

                                     }).OrderByDescending(p => p.OrderDate).ToList();

                foreach (var item in orders)
                {

                    foreach (var design in item.OrderDesigns)
                    {
                        design.ProductImageURL = HttpContext.Current.Server.MapPath(design.ProductImageURL);
                    }
                    if (item.OrderDesigns.Count == 1)
                    {
                        item.OrderName = item.OrderDesigns[0].DesignName;
                    }
                }

                if (orders.Count > 0)
                {
                    foreach (var item in orders)
                    {
                        #region "Report Header"

                        var reportHeader = (from r in dbContext.Orders
                                            join uc in dbContext.WMSUsers on r.CustomerId equals uc.Id
                                            join uadd in dbContext.WMSUserAddresses on r.CustomerId equals uadd.UserId
                                            join uaadc in dbContext.Countries on uadd.CountryId equals uaadc.CountryId
                                            join u in dbContext.WMSUserAdditionals on r.CustomerId equals u.UserId
                                            join us in dbContext.WMSUsers on r.CreatedBy equals us.Id
                                            join oa in dbContext.OrderAddresses on r.OrderAddressId equals oa.Id
                                            join oadC in dbContext.Countries on oa.CountryId equals oadC.CountryId

                                            where r.Id == item.OrderId
                                            select new
                                            {
                                                OrderNumber = r.OrderNumber,
                                                PONumber = r.PONumber,
                                                CustomerRef = u.AccountNumber,
                                                ValidatedExFactoryDate = r.ConfirmedExFactoryDate,
                                                OrderDate = r.CreatedOnUtc,
                                                SalesRepEmail = us.Email,

                                                DeliveryCompany = oa.CompanyName,
                                                DeliveryContact = oa.ContactFirstName + " " + oa.ContactLastName,
                                                DeliveryPhone = "(+" + oadC.CountryPhoneCode + ")" + oa.PhoneNumber,
                                                DeliveryEmail = oa.Email,
                                                DeliveryAddress1 = oa.Address,
                                                DeliveryAddress2 = oa.Address2,
                                                DeliveryCity = oa.City,
                                                DeliveryState = oa.State,
                                                DeliveryPostCode = oa.PostCode,
                                                DeliverySuberb = oa.Suburb,
                                                DeliveryCounty = oadC.CountryName,


                                                CustomerCompany = uc.CompanyName,
                                                CustomerContact = uc.ContactFirstName + " " + uc.ContactLastName,
                                                CustomerPhone = "(+" + uaadc.CountryPhoneCode + ") " + uc.TelephoneNo,
                                                CustomerEmail = uc.Email,
                                                CustomerAddress1 = uadd.Address,
                                                CustomerAddress2 = uadd.Address2,
                                                CustomerCity = uadd.City,
                                                CustomerState = uadd.State,
                                                CustomerPostCode = uadd.PostCode,
                                                CustomerSuberb = uadd.Suburb,
                                                CustomerCounty = uaadc.CountryName,

                                            }
                                            ).FirstOrDefault();
                        #endregion

                        if (item.OrderDesigns.Count > 0)
                        {
                            #region "Order Design"

                            foreach (var design in item.OrderDesigns)
                            {
                                obj = new MainDataSource();

                                obj.HeaderInformation = new MainDataSourceHeader();
                                if (reportHeader != null)
                                {
                                    string Address = "";
                                    if (!string.IsNullOrEmpty(reportHeader.CustomerCompany))
                                    {
                                        Address = Address + reportHeader.CustomerCompany + Environment.NewLine;
                                    }
                                    else
                                    {
                                        Address = Address + reportHeader.CustomerContact + Environment.NewLine;
                                    }
                                    Address += "Address: ";
                                    if (!string.IsNullOrEmpty(reportHeader.CustomerAddress1))
                                    {
                                        Address = Address + reportHeader.CustomerAddress1 + Environment.NewLine;
                                    }
                                    if (!string.IsNullOrEmpty(reportHeader.CustomerAddress2))
                                    {
                                        Address = Address + reportHeader.CustomerAddress2 + Environment.NewLine;
                                    }
                                    if (!string.IsNullOrEmpty(reportHeader.CustomerCity))
                                    {
                                        Address = Address + reportHeader.CustomerCity;
                                    }
                                    if (!string.IsNullOrEmpty(reportHeader.CustomerPostCode))
                                    {
                                        Address = Address + " - " + reportHeader.CustomerPostCode + Environment.NewLine;
                                    }
                                    if (!string.IsNullOrEmpty(reportHeader.CustomerState))
                                    {
                                        Address = Address + reportHeader.CustomerState + Environment.NewLine; ;
                                    }
                                    if (reportHeader.CustomerCounty != null && !string.IsNullOrEmpty(reportHeader.CustomerCounty))
                                    {
                                        Address = Address + reportHeader.CustomerCounty + Environment.NewLine;
                                    }
                                    Address += "Phone: ";
                                    if (reportHeader.CustomerPhone != null)
                                    {
                                        Address += reportHeader.CustomerPhone + Environment.NewLine;
                                    }
                                    Address += "Email: ";

                                    if (!string.IsNullOrEmpty(reportHeader.CustomerEmail))
                                    {
                                        Address += reportHeader.CustomerEmail + Environment.NewLine;
                                    }


                                    obj.HeaderInformation.CustomerAddress = Address;

                                    obj.HeaderInformation.DeliveryCustomerName = reportHeader.DeliveryCompany;
                                    obj.HeaderInformation.DeliveryContactName = reportHeader.DeliveryContact;
                                    obj.HeaderInformation.DeliveryPhoneNumber = reportHeader.DeliveryPhone;
                                    obj.HeaderInformation.DeliveryEmailAddress = reportHeader.DeliveryEmail;
                                    obj.HeaderInformation.DeliveryAddressStreet = reportHeader.DeliveryAddress1;
                                    obj.HeaderInformation.DeliveryAddressSuburb = reportHeader.DeliveryAddress2;
                                    obj.HeaderInformation.DeliveryAddressState = reportHeader.DeliveryState;
                                    obj.HeaderInformation.DeliveryPostCode = reportHeader.DeliveryPostCode;
                                    obj.HeaderInformation.DeliveryCountry = reportHeader.DeliveryCounty;
                                    obj.HeaderInformation.DeliveryPort = string.Empty;
                                    obj.HeaderInformation.OrderDate = reportHeader.OrderDate.ToString("dd-MMM-yyyy");
                                    obj.HeaderInformation.SalesOrderNumber = reportHeader.OrderNumber;
                                    obj.HeaderInformation.SalesRepEmailAddress = reportHeader.SalesRepEmail;
                                    obj.HeaderInformation.SalesRepCode = string.Empty;
                                    obj.HeaderInformation.SalesCustomerReference = reportHeader.CustomerRef;
                                    obj.HeaderInformation.FactoryDate = reportHeader.ValidatedExFactoryDate.HasValue ? reportHeader.ValidatedExFactoryDate.Value.ToString("dd-MMM-yyyy") : "";
                                    // obj.HeaderInformation.Logo = "~/Files/Images/brand-logo.png";
                                    obj.HeaderInformation.Logo = HttpContext.Current.Server.MapPath("~/Files/Images/brand-logo.png"); ;
                                    obj.HeaderInformation.Customer = "DYNASTY STOCK ORDER DETAILS - CHINA";
                                    obj.HeaderInformation.SalesDeliveryTimelines = "DELIVERY TIMELINES - APPROXIMATE - 2.5 weeks";
                                }


                                obj.Customer = item.Customer;
                                obj.DesignImagePath = design.ProductImageURL;
                                obj.GarmentColorCode = design.GarmentColor;

                                obj.GarmentType = design.ProductName;
                                obj.FabricNumber = "";
                                obj.StyleCode = design.ProductCode;
                                obj.Sizenumber = "";
                                obj.OrderNumber = item.OrderNumber;
                                obj.PONumber = item.PONumber;
                                obj.DesignNumber = design.DesignNumber;
                                obj.Artist = "";
                                obj.Date = item.OrderDate;

                                #region "Order Design Styles For Sock Range"

                                var orderDesignStyles = dbContext.OrderDesignStyles.Where(p => p.OrderDesignId == design.OrderDesignId).ToList();
                                if (orderDesignStyles.Count > 0)
                                {
                                    if (orderDesignStyles.Count == 1)
                                    {
                                        var colour = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[0].Color).FirstOrDefault();
                                        obj.GarmentColor = orderDesignStyles[0].Color + "~" + colour.Color;
                                    }
                                    if (orderDesignStyles.Count == 2)
                                    {

                                        var colour1 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[0].Color).FirstOrDefault();
                                        obj.GarmentColor = orderDesignStyles[0].Color + "~" + colour1.Color;

                                        var colour2 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[1].Color).FirstOrDefault();
                                        obj.GarmentColor2 = orderDesignStyles[1].Color + "~" + colour2.Color;

                                    }
                                    if (orderDesignStyles.Count == 3)
                                    {
                                        var colour1 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[0].Color).FirstOrDefault();
                                        obj.GarmentColor = orderDesignStyles[0].Color + "~" + colour1.Color;

                                        var colour2 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[1].Color).FirstOrDefault();
                                        obj.GarmentColor2 = orderDesignStyles[1].Color + "~" + colour2.Color;

                                        var colour3 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[2].Color).FirstOrDefault();
                                        obj.GarmentColor3 = orderDesignStyles[2].Color + "~" + colour3.Color;
                                    }
                                    if (orderDesignStyles.Count == 4)
                                    {
                                        var colour1 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[0].Color).FirstOrDefault();
                                        obj.GarmentColor = orderDesignStyles[0].Color + "~" + colour1.Color;

                                        var colour2 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[1].Color).FirstOrDefault();
                                        obj.GarmentColor2 = orderDesignStyles[1].Color + "~" + colour2.Color;

                                        var colour3 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[2].Color).FirstOrDefault();
                                        obj.GarmentColor3 = orderDesignStyles[2].Color + "~" + colour3.Color;

                                        var colour4 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[3].Color).FirstOrDefault();
                                        obj.GarmentColor4 = orderDesignStyles[3].Color + "~" + colour4.Color;
                                    }
                                    if (orderDesignStyles.Count == 5)
                                    {
                                        var colour1 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[0].Color).FirstOrDefault();
                                        obj.GarmentColor = orderDesignStyles[0].Color + "~" + colour1.Color;

                                        var colour2 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[1].Color).FirstOrDefault();
                                        obj.GarmentColor2 = orderDesignStyles[1].Color + "~" + colour2.Color;

                                        var colour3 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[2].Color).FirstOrDefault();
                                        obj.GarmentColor3 = orderDesignStyles[2].Color + "~" + colour3.Color;

                                        var colour4 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[3].Color).FirstOrDefault();
                                        obj.GarmentColor4 = orderDesignStyles[3].Color + "~" + colour4.Color;

                                        var colour5 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[4].Color).FirstOrDefault();
                                        obj.GarmentColor5 = orderDesignStyles[4].Color + "~" + colour5.Color;
                                    }
                                    if (orderDesignStyles.Count == 6)
                                    {
                                        var colour1 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[0].Color).FirstOrDefault();
                                        obj.GarmentColor = orderDesignStyles[0].Color + "~" + colour1.Color;

                                        var colour2 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[1].Color).FirstOrDefault();
                                        obj.GarmentColor2 = orderDesignStyles[1].Color + "~" + colour2.Color;

                                        var colour3 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[2].Color).FirstOrDefault();
                                        obj.GarmentColor3 = orderDesignStyles[2].Color + "~" + colour3.Color;

                                        var colour4 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[3].Color).FirstOrDefault();
                                        obj.GarmentColor4 = orderDesignStyles[3].Color + "~" + colour4.Color;

                                        var colour5 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[4].Color).FirstOrDefault();
                                        obj.GarmentColor5 = orderDesignStyles[4].Color + "~" + colour5.Color;

                                        var colour6 = new DYORepository().getColors("").Where(p => p.ColorCode == orderDesignStyles[5].Color).FirstOrDefault();
                                        obj.GarmentColor6 = orderDesignStyles[5].Color + "~" + colour6.Color;
                                    }
                                }
                                else
                                {
                                    // For Stock Range 
                                    var color = dbContext.Colors.Where(p => p.code == design.GarmentColor).FirstOrDefault();
                                    if (color != null)
                                    {
                                        obj.GarmentColor = design.GarmentColor + "~" + color.color1;
                                    }
                                }

                                #endregion

                                // Add extra emblishment for default logo 
                                if (design.IsDefaultLogo)
                                {
                                    var emb1 = new OrderEmblishmnetDetail
                                    {
                                        OrderEmblishmnetId = 0,
                                        Color = "",
                                        Font = "",
                                        PrintMethod = "",
                                        LogoPath = "~/Files/Images/brand-logo.png",
                                        EmblishmentType = "DefaultLogo",
                                        OrderDesignId = 0,
                                        DimensionUnit = "",
                                        LogoHeight = "",
                                        LogoWidth = ""
                                    };
                                    design.OrderEmblishmentDetails.Add(emb1);
                                }
                                obj.Parent = new List<ParentDataSource>();

                                for (int i = 0; i < design.OrderEmblishmentDetails.Count; i++)
                                {
                                    design.OrderEmblishmentDetails[i].Color = design.OrderEmblishmentDetails[i].Color.Replace(',', ';');
                                    int val = design.OrderEmblishmentDetails[i].OrderEmblishmnetId;
                                    var ong = dbContext.OrderEmblishmentDetails.Where(p => p.OrderEmblishmentId == val).ToList();
                                    if (ong != null && ong.Count > 0)
                                    {
                                        foreach (var item1 in ong)
                                        {
                                            design.OrderEmblishmentDetails[i].Color += ";" + item1.Color;
                                        }
                                    }
                                }
                                #region 
                                for (int i = 0; i <= design.OrderEmblishmentDetails.Count; i = i + 4)
                                {
                                    if (design.OrderEmblishmentDetails.Count >= i)
                                    {
                                        emblishment = new ParentDataSource();

                                        if (design.OrderEmblishmentDetails.Count > i)
                                        {

                                            var colors = design.OrderEmblishmentDetails[i].Color.Split(';');

                                            emblishment.Child1 = new ChildDataSource();
                                            // emblishment.Child1.Printing = design.OrderEmblishmentDetails[i].PrintMethod;
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.PlayerNameType)
                                            {
                                                emblishment.Child1.Size = string.Empty;
                                                emblishment.Child1.ImagePath = HttpContext.Current.Server.MapPath("~/Files/Images/PlayerName.png");
                                                var print = new DYORepository().getPrintingMethods("").Where(p => p.Key == design.OrderEmblishmentDetails[i].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {

                                                    emblishment.Child1.Printing = print.Value;

                                                }
                                                else
                                                {
                                                    emblishment.Child1.Printing = design.OrderEmblishmentDetails[i].PrintMethod;
                                                }
                                            }
                                            else if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.PlayerNumberType)
                                            {
                                                emblishment.Child1.Size = string.Empty;
                                                emblishment.Child1.ImagePath = HttpContext.Current.Server.MapPath("~/Files/Images/PlayerNumber.png");
                                                var print = new DYORepository().getPrintingMethods("").Where(p => p.Key == design.OrderEmblishmentDetails[i].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {

                                                    emblishment.Child1.Printing = print.Value;

                                                }
                                                else
                                                {
                                                    emblishment.Child1.Printing = design.OrderEmblishmentDetails[i].PrintMethod;
                                                }
                                            }
                                            else
                                            {

                                                if (!string.IsNullOrEmpty(design.OrderEmblishmentDetails[i].LogoWidth) && !string.IsNullOrEmpty(design.OrderEmblishmentDetails[i].LogoHeight))
                                                {
                                                    emblishment.Child1.Size = design.OrderEmblishmentDetails[i].LogoWidth + "(w)" + " x " + design.OrderEmblishmentDetails[i].LogoHeight + "(h)";
                                                }
                                                else if (string.IsNullOrEmpty(design.OrderEmblishmentDetails[i].LogoWidth) && string.IsNullOrEmpty(design.OrderEmblishmentDetails[i].LogoHeight))
                                                {
                                                    emblishment.Child1.Size = string.Empty;
                                                }
                                                else
                                                {
                                                    if (!string.IsNullOrEmpty(design.OrderEmblishmentDetails[i].LogoWidth) && string.IsNullOrEmpty(design.OrderEmblishmentDetails[i].LogoHeight))
                                                    {
                                                        emblishment.Child1.Size = design.OrderEmblishmentDetails[i].LogoWidth + "(w) " + "[AR]";
                                                    }
                                                    if (string.IsNullOrEmpty(design.OrderEmblishmentDetails[i].LogoWidth) && !string.IsNullOrEmpty(design.OrderEmblishmentDetails[i].LogoHeight))
                                                    {
                                                        emblishment.Child1.Size = design.OrderEmblishmentDetails[i].LogoHeight + "(h) " + "[AR]";
                                                    }
                                                }

                                                //   emblishment.Child1.Size = design.OrderEmblishmentDetails[i].LogoWidth + " x " + design.OrderEmblishmentDetails[i].LogoHeight;
                                                emblishment.Child1.ImagePath = HttpContext.Current.Server.MapPath(design.OrderEmblishmentDetails[i].LogoPath);
                                                var print = new DYORepository().getPrintingMethods("PrintMethods").Where(p => p.Key == design.OrderEmblishmentDetails[i].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {

                                                    emblishment.Child1.Printing = print.Value;

                                                }
                                                else
                                                {
                                                    emblishment.Child1.Printing = design.OrderEmblishmentDetails[i].PrintMethod;
                                                }
                                            }

                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.PlayerNameType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.PlayerNameDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.PlayerNumberType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.PlayerNumberDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.InsertTextType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.InsertTextDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.BackNeckType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.BackNeckDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.BackThighLeftType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.BackThighLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.BackThighRightType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.BackThighRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.ChestCenterTopType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.ChestCenterTopDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.FrontMainType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.FrontMainDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.LeftChestType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.LeftChestDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.LeftShoulder)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.LeftShoulderDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.LowerArmLeftType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.LowerArmLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.LowerArmRightType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.LowerArmRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.LowerBackType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.LowerBackDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.LowerSleeveLeftType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.LowerSleeveLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.LowerSleeveRightType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.LowerSleeveRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.MidBackType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.MidBackDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.RightChestType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.RightChestDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.RightShoulder)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.RightShoulderDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.SockCalfLeftType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.SockCalfLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.SockCalfRightType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.SockCalfRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.SockFrontLeftType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.SockFrontLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.SockFrontRightType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.SockFrontRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.ThighLeftType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.ThighLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.ThighRightType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.ThighRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.UploadLogoType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.UplaodLogoDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.UpperArmLeftType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.UpperArmLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == EmblishmentOptionEnum.UpperArmRightType)
                                            {
                                                emblishment.Child1.Position = EmblishmentOptionEnum.UpperArmRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == "DefaultLogo")
                                            {
                                                emblishment.Child1.Position = "Default Logo";
                                            }

                                            emblishment.Child1.Font = design.OrderEmblishmentDetails[i].Font;
                                            if (colors.Length > 0)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[0]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child1.Color1 = colors[0] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color1 = colors[0];
                                                }
                                            }
                                            if (colors.Length > 1)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[1]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {

                                                    emblishment.Child1.Color2 = colors[1] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color2 = colors[1];
                                                }
                                                //emblishment.Child1.Color2 = colors[1];
                                            }
                                            if (colors.Length > 2)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[2]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child1.Color3 = colors[2] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color3 = colors[2];
                                                }
                                                //   emblishment.Child1.Color3 = colors[2];
                                            }
                                            if (colors.Length > 3)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[3]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child1.Color4 = colors[3] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color4 = colors[3];
                                                }
                                                //  emblishment.Child1.Color4 = colors[3];
                                            }
                                            if (colors.Length > 4)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[4]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child1.Color5 = colors[4] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color5 = colors[4];
                                                }
                                                //    emblishment.Child1.Color5 = colors[4];
                                            }
                                            if (colors.Length > 5)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[5]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child1.Color6 = colors[5] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color6 = colors[5];
                                                }
                                                // emblishment.Child1.Color6 = colors[5];
                                            }
                                            if (colors.Length > 6)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[6]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child1.Color7 = colors[6] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color7 = colors[6];
                                                }
                                                // emblishment.Child1.Color7 = colors[6];
                                            }
                                            if (colors.Length > 7)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[7]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child1.Color8 = colors[7] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color8 = colors[7];
                                                }
                                                //  emblishment.Child1.Color8 = colors[7];
                                            }
                                            if (colors.Length > 8)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[8]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child1.Color9 = colors[8] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color9 = colors[8];
                                                }
                                                //emblishment.Child1.Color9 = colors[8];
                                            }
                                            if (colors.Length > 9)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[9]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child1.Color10 = colors[9] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color10 = colors[9];
                                                }
                                                //  emblishment.Child1.Color10 = colors[9];
                                            }
                                            if (colors.Length > 10)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[10]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child1.Color11 = colors[10] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color11 = colors[10];
                                                }
                                                //    emblishment.Child1.Color11 = colors[10];
                                            }
                                            if (colors.Length > 11)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i].PrintMethod).Where(p => p.ColorCode == colors[11]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child1.Color12 = colors[11] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child1.Color12 = colors[11];
                                                }
                                                //  emblishment.Child1.Color12 = colors[11];
                                            }
                                        }

                                        if (design.OrderEmblishmentDetails.Count > i + 1)
                                        {
                                            var colors = design.OrderEmblishmentDetails[i + 1].Color.Split(';');
                                            emblishment.Child2 = new ChildDataSource();
                                            // emblishment.Child2.Printing = design.OrderEmblishmentDetails[i + 1].PrintMethod;
                                            //emblishment.Child2.Position = design.OrderEmblishmentDetails[i + 1].EmblishmentType;
                                            //emblishment.Child2.Size = design.OrderEmblishmentDetails[i + 1].LogoWidth + "x" + design.OrderEmblishmentDetails[i + 1].LogoHeight;
                                            //emblishment.Child2.ImagePath = design.OrderEmblishmentDetails[i + 1].LogoPath;

                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.PlayerNameType)
                                            {
                                                emblishment.Child2.Size = string.Empty;
                                                emblishment.Child2.ImagePath = HttpContext.Current.Server.MapPath("~/Files/Images/PlayerName.png");
                                                var print = new DYORepository().getPrintingMethods("").Where(p => p.Key == design.OrderEmblishmentDetails[i].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {

                                                    emblishment.Child2.Printing = print.Value;

                                                }
                                                else
                                                {
                                                    emblishment.Child2.Printing = design.OrderEmblishmentDetails[i].PrintMethod;
                                                }
                                            }
                                            else if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.PlayerNumberType)
                                            {
                                                emblishment.Child2.Size = string.Empty;
                                                emblishment.Child2.ImagePath = HttpContext.Current.Server.MapPath("~/Files/Images/PlayerNumber.png");
                                                var print = new DYORepository().getPrintingMethods("").Where(p => p.Key == design.OrderEmblishmentDetails[i].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {

                                                    emblishment.Child2.Printing = print.Value;

                                                }
                                                else
                                                {
                                                    emblishment.Child2.Printing = design.OrderEmblishmentDetails[i].PrintMethod;
                                                }
                                            }
                                            else
                                            {

                                                if (!string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 1].LogoWidth) && !string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 1].LogoHeight))
                                                {
                                                    emblishment.Child2.Size = design.OrderEmblishmentDetails[i + 1].LogoWidth + "(w)" + " x " + design.OrderEmblishmentDetails[i + 1].LogoHeight + "(h)";
                                                }
                                                else if (string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 1].LogoWidth) && string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 1].LogoHeight))
                                                {
                                                    emblishment.Child2.Size = string.Empty;
                                                }
                                                else
                                                {
                                                    if (!string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 1].LogoWidth) && string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 1].LogoHeight))
                                                    {
                                                        emblishment.Child2.Size = design.OrderEmblishmentDetails[i + 1].LogoWidth + "(w) " + "[AR]";
                                                    }
                                                    if (string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 1].LogoWidth) && !string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 1].LogoHeight))
                                                    {
                                                        emblishment.Child2.Size = design.OrderEmblishmentDetails[i + 1].LogoHeight + "(h) " + "[AR]";
                                                    }
                                                }

                                                //    emblishment.Child2.Size = design.OrderEmblishmentDetails[i + 1].LogoWidth + " x " + design.OrderEmblishmentDetails[i + 1].LogoHeight;
                                                emblishment.Child2.ImagePath = HttpContext.Current.Server.MapPath(design.OrderEmblishmentDetails[i + 1].LogoPath);
                                                var print = new DYORepository().getPrintingMethods("PrintMethods").Where(p => p.Key == design.OrderEmblishmentDetails[i + 1].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {

                                                    emblishment.Child2.Printing = print.Value;

                                                }
                                                else
                                                {
                                                    emblishment.Child2.Printing = design.OrderEmblishmentDetails[i + 1].PrintMethod;
                                                }
                                            }

                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.PlayerNameType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.PlayerNameDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.PlayerNumberType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.PlayerNumberDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.InsertTextType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.InsertTextDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.BackNeckType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.BackNeckDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.BackThighLeftType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.BackThighLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.BackThighRightType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.BackThighRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.ChestCenterTopType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.ChestCenterTopDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.FrontMainType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.FrontMainDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.LeftChestType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.LeftChestDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.LeftShoulder)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.LeftShoulderDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.LowerArmLeftType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.LowerArmLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.LowerArmRightType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.LowerArmRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.LowerBackType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.LowerBackDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.LowerSleeveLeftType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.LowerSleeveLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.LowerSleeveRightType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.LowerSleeveRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.MidBackType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.MidBackDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.RightChestType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.RightChestDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.RightShoulder)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.RightShoulderDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.SockCalfLeftType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.SockCalfLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.SockCalfRightType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.SockCalfRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.SockFrontLeftType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.SockFrontLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.SockFrontRightType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.SockFrontRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.ThighLeftType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.ThighLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.ThighRightType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.ThighRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.UploadLogoType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.UplaodLogoDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.UpperArmLeftType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.UpperArmLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == EmblishmentOptionEnum.UpperArmRightType)
                                            {
                                                emblishment.Child2.Position = EmblishmentOptionEnum.UpperArmRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 1].EmblishmentType == "DefaultLogo")
                                            {
                                                emblishment.Child2.Position = "Default Logo";
                                            }

                                            emblishment.Child2.Font = design.OrderEmblishmentDetails[i + 1].Font;
                                            if (colors.Length > 0)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[0]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color1 = colors[0] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color1 = colors[0];
                                                }

                                                // emblishment.Child2.Color1 = colors[0];
                                            }
                                            if (colors.Length > 1)
                                            {

                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[1]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color2 = colors[1] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color2 = colors[1];
                                                }

                                                // emblishment.Child2.Color2 = colors[1];
                                            }
                                            if (colors.Length > 2)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[2]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color3 = colors[2] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color3 = colors[2];
                                                }
                                                //  emblishment.Child2.Color3 = colors[2];
                                            }
                                            if (colors.Length > 3)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[3]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color4 = colors[3] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color4 = colors[3];
                                                }
                                                // emblishment.Child2.Color4 = colors[3];
                                            }
                                            if (colors.Length > 4)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[4]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color5 = colors[4] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color5 = colors[4];
                                                }
                                                //    emblishment.Child2.Color5 = colors[4];
                                            }
                                            if (colors.Length > 5)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[5]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color6 = colors[5] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color6 = colors[5];
                                                }
                                                //  emblishment.Child2.Color6 = colors[5];
                                            }
                                            if (colors.Length > 6)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[6]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color7 = colors[6] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color7 = colors[6];
                                                }
                                                // emblishment.Child2.Color7 = colors[6];
                                            }
                                            if (colors.Length > 7)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[7]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color8 = colors[7] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color8 = colors[7];
                                                }
                                                //  emblishment.Child2.Color8 = colors[7];
                                            }
                                            if (colors.Length > 8)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[8]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color9 = colors[8] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color9 = colors[8];
                                                }
                                                //  emblishment.Child2.Color9 = colors[8];
                                            }
                                            if (colors.Length > 9)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[9]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color10 = colors[9] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color10 = colors[9];
                                                }
                                                //  emblishment.Child1.Color10 = colors[9];
                                            }
                                            if (colors.Length > 10)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[10]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color11 = colors[10] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color11 = colors[10];
                                                }
                                                // emblishment.Child2.Color11 = colors[10];
                                            }
                                            if (colors.Length > 11)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 1].PrintMethod).Where(p => p.ColorCode == colors[11]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child2.Color12 = colors[11] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child2.Color12 = colors[11];
                                                }
                                                // emblishment.Child2.Color12 = colors[11];
                                            }

                                        }
                                        if (design.OrderEmblishmentDetails.Count > i + 2)
                                        {
                                            var colors = design.OrderEmblishmentDetails[i + 2].Color.Split(';');

                                            emblishment.Child3 = new ChildDataSource();
                                            emblishment.Child3 = new ChildDataSource();
                                            //  emblishment.Child3.Printing = design.OrderEmblishmentDetails[i + 2].PrintMethod;
                                            //emblishment.Child3.Position = design.OrderEmblishmentDetails[i + 2].EmblishmentType;
                                            //emblishment.Child3.Size = design.OrderEmblishmentDetails[i + 2].LogoWidth + "x" + design.OrderEmblishmentDetails[i + 2].LogoHeight;
                                            //emblishment.Child3.ImagePath = design.OrderEmblishmentDetails[i + 2].LogoPath;
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.PlayerNameType)
                                            {
                                                emblishment.Child3.Size = string.Empty;
                                                emblishment.Child3.ImagePath = HttpContext.Current.Server.MapPath("~/Files/Images/PlayerName.png");
                                                var print = new DYORepository().getPrintingMethods("").Where(p => p.Key == design.OrderEmblishmentDetails[i + 2].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {

                                                    emblishment.Child3.Printing = print.Value;

                                                }
                                                else
                                                {
                                                    emblishment.Child3.Printing = design.OrderEmblishmentDetails[i + 2].PrintMethod;
                                                }
                                            }
                                            else if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.PlayerNumberType)
                                            {
                                                emblishment.Child3.Size = string.Empty;
                                                emblishment.Child3.ImagePath = HttpContext.Current.Server.MapPath("~/Files/Images/PlayerNumber.png");
                                                var print = new DYORepository().getPrintingMethods("").Where(p => p.Key == design.OrderEmblishmentDetails[i + 2].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {

                                                    emblishment.Child3.Printing = print.Value;

                                                }
                                                else
                                                {
                                                    emblishment.Child3.Printing = design.OrderEmblishmentDetails[i + 2].PrintMethod;
                                                }

                                            }
                                            else
                                            {

                                                if (!string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 2].LogoWidth) && !string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 2].LogoHeight))
                                                {
                                                    emblishment.Child3.Size = design.OrderEmblishmentDetails[i + 2].LogoWidth + "(w)" + " x " + design.OrderEmblishmentDetails[i + 2].LogoHeight + "(h)";
                                                }
                                                else if (string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 2].LogoWidth) && string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 2].LogoHeight))
                                                {
                                                    emblishment.Child3.Size = string.Empty;
                                                }
                                                else
                                                {
                                                    if (!string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 2].LogoWidth) && string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 2].LogoHeight))
                                                    {
                                                        emblishment.Child3.Size = design.OrderEmblishmentDetails[i + 2].LogoWidth + "(w) " + "[AR]";
                                                    }
                                                    if (string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 2].LogoWidth) && !string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 2].LogoHeight))
                                                    {
                                                        emblishment.Child3.Size = design.OrderEmblishmentDetails[i + 2].LogoHeight + "(h) " + "[AR]";
                                                    }
                                                }

                                                //  emblishment.Child3.Size = design.OrderEmblishmentDetails[i + 2].LogoWidth + " x " + design.OrderEmblishmentDetails[i + 2].LogoHeight;
                                                emblishment.Child3.ImagePath = HttpContext.Current.Server.MapPath(design.OrderEmblishmentDetails[i + 2].LogoPath);
                                                var print = new DYORepository().getPrintingMethods("PrintMethods").Where(p => p.Key == design.OrderEmblishmentDetails[i + 2].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {

                                                    emblishment.Child3.Printing = print.Value;

                                                }
                                                else
                                                {
                                                    emblishment.Child3.Printing = design.OrderEmblishmentDetails[i + 2].PrintMethod;
                                                }
                                            }

                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.PlayerNameType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.PlayerNameDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.PlayerNumberType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.PlayerNumberDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.InsertTextType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.InsertTextDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.BackNeckType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.BackNeckDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.BackThighLeftType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.BackThighLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.BackThighRightType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.BackThighRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.ChestCenterTopType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.ChestCenterTopDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.FrontMainType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.FrontMainDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.LeftChestType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.LeftChestDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.LeftShoulder)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.LeftShoulderDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.LowerArmLeftType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.LowerArmLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.LowerArmRightType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.LowerArmRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.LowerBackType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.LowerBackDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.LowerSleeveLeftType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.LowerSleeveLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.LowerSleeveRightType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.LowerSleeveRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.MidBackType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.MidBackDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.RightChestType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.RightChestDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.RightShoulder)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.RightShoulderDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.SockCalfLeftType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.SockCalfLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.SockCalfRightType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.SockCalfRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.SockFrontLeftType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.SockFrontLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.SockFrontRightType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.SockFrontRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.ThighLeftType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.ThighLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.ThighRightType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.ThighRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.UploadLogoType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.UplaodLogoDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.UpperArmLeftType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.UpperArmLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 2].EmblishmentType == EmblishmentOptionEnum.UpperArmRightType)
                                            {
                                                emblishment.Child3.Position = EmblishmentOptionEnum.UpperArmRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == "DefaultLogo")
                                            {
                                                emblishment.Child3.Position = "Default Logo";
                                            }

                                            emblishment.Child3.Font = design.OrderEmblishmentDetails[i + 2].Font;
                                            if (colors.Length > 0)
                                            {

                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[0]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color1 = colors[0] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color1 = colors[0];
                                                }
                                                // emblishment.Child3.Color1 = colors[0];
                                            }
                                            if (colors.Length > 1)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[1]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color2 = colors[1] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color2 = colors[1];
                                                }
                                                // emblishment.Child3.Color2 = colors[1];
                                            }
                                            if (colors.Length > 2)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[2]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color3 = colors[2] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color3 = colors[2];
                                                }
                                                // emblishment.Child3.Color3 = colors[2];
                                            }
                                            if (colors.Length > 3)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[3]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color4 = colors[3] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color4 = colors[3];
                                                }
                                                //  emblishment.Child3.Color4 = colors[3];
                                            }
                                            if (colors.Length > 4)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[4]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color5 = colors[4] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color5 = colors[4];
                                                }
                                                // emblishment.Child3.Color5 = colors[4];
                                            }
                                            if (colors.Length > 5)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[5]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color6 = colors[5] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color6 = colors[5];
                                                }
                                                //  emblishment.Child3.Color6 = colors[5];
                                            }
                                            if (colors.Length > 6)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[6]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color7 = colors[6] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color7 = colors[6];
                                                }
                                                //    emblishment.Child3.Color7 = colors[6];
                                            }
                                            if (colors.Length > 7)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[7]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color8 = colors[7] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color8 = colors[7];
                                                }

                                                emblishment.Child3.Color8 = colors[7];
                                            }
                                            if (colors.Length > 8)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[8]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color9 = colors[8] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color9 = colors[8];
                                                }

                                                //  emblishment.Child3.Color9 = colors[8];
                                            }
                                            if (colors.Length > 9)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[9]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color10 = colors[9] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color10 = colors[9];
                                                }
                                                //     emblishment.Child3.Color10 = colors[9];
                                            }
                                            if (colors.Length > 10)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[10]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color11 = colors[10] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color11 = colors[10];
                                                }
                                                //   emblishment.Child3.Color11 = colors[10];
                                            }
                                            if (colors.Length > 11)
                                            {

                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 2].PrintMethod).Where(p => p.ColorCode == colors[11]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child3.Color12 = colors[11] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child3.Color12 = colors[11];
                                                }
                                                // emblishment.Child3.Color12 = colors[11];
                                            }

                                        }
                                        if (design.OrderEmblishmentDetails.Count > i + 3)
                                        {
                                            emblishment.Child4 = new ChildDataSource();
                                            var colors = design.OrderEmblishmentDetails[i + 3].Color.Split(';');
                                            //emblishment.Child4.Printing = design.OrderEmblishmentDetails[i + 3].PrintMethod;
                                            //emblishment.Child4.Position = design.OrderEmblishmentDetails[i + 3].EmblishmentType;
                                            //emblishment.Child4.Size = design.OrderEmblishmentDetails[i + 3].LogoWidth + "x" + design.OrderEmblishmentDetails[i + 3].LogoHeight;
                                            //emblishment.Child4.ImagePath = design.OrderEmblishmentDetails[i + 3].LogoPath;

                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.PlayerNameType)
                                            {
                                                emblishment.Child4.Size = string.Empty;
                                                emblishment.Child4.ImagePath = HttpContext.Current.Server.MapPath("~/Files/Images/PlayerName.png");
                                                var print = new DYORepository().getPrintingMethods("").Where(p => p.Key == design.OrderEmblishmentDetails[i + 3].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {
                                                    emblishment.Child4.Printing = print.Value;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Printing = design.OrderEmblishmentDetails[i + 3].PrintMethod;
                                                }
                                            }
                                            else if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.PlayerNumberType)
                                            {
                                                emblishment.Child4.Size = string.Empty;
                                                emblishment.Child4.ImagePath = HttpContext.Current.Server.MapPath("~/Files/Images/PlayerNumber.png");
                                                var print = new DYORepository().getPrintingMethods("").Where(p => p.Key == design.OrderEmblishmentDetails[i + 3].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {
                                                    emblishment.Child4.Printing = print.Value;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Printing = design.OrderEmblishmentDetails[i + 3].PrintMethod;
                                                }
                                            }
                                            else
                                            {

                                                if (!string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 3].LogoWidth) && !string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 3].LogoHeight))
                                                {
                                                    emblishment.Child4.Size = design.OrderEmblishmentDetails[i + 3].LogoWidth + "(w)" + " x " + design.OrderEmblishmentDetails[i + 3].LogoHeight + "(h)";
                                                }
                                                else if (string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 3].LogoWidth) && string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 3].LogoHeight))
                                                {
                                                    emblishment.Child4.Size = string.Empty;
                                                }
                                                else
                                                {
                                                    if (!string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 3].LogoWidth) && string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 3].LogoHeight))
                                                    {
                                                        emblishment.Child4.Size = design.OrderEmblishmentDetails[i + 3].LogoWidth + "(w) " + "[AR]";
                                                    }
                                                    if (string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 3].LogoWidth) && !string.IsNullOrEmpty(design.OrderEmblishmentDetails[i + 3].LogoHeight))
                                                    {
                                                        emblishment.Child4.Size = design.OrderEmblishmentDetails[i + 3].LogoHeight + "(h) " + "[AR]";
                                                    }
                                                }

                                                //  emblishment.Child4.Size = design.OrderEmblishmentDetails[i + 3].LogoWidth + " x " + design.OrderEmblishmentDetails[i + 3].LogoHeight;
                                                emblishment.Child4.ImagePath = HttpContext.Current.Server.MapPath(design.OrderEmblishmentDetails[i + 3].LogoPath);
                                                var print = new DYORepository().getPrintingMethods("PrintMethods").Where(p => p.Key == design.OrderEmblishmentDetails[i + 3].PrintMethod).FirstOrDefault();
                                                if (print != null)
                                                {
                                                    emblishment.Child4.Printing = print.Value;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Printing = design.OrderEmblishmentDetails[i + 3].PrintMethod;
                                                }
                                            }

                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.PlayerNameType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.PlayerNameDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.PlayerNumberType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.PlayerNumberDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.InsertTextType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.InsertTextDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.BackNeckType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.BackNeckDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.BackThighLeftType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.BackThighLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.BackThighRightType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.BackThighRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.ChestCenterTopType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.ChestCenterTopDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.FrontMainType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.FrontMainDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.LeftChestType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.LeftChestDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.LeftShoulder)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.LeftShoulderDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.LowerArmLeftType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.LowerArmLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.LowerArmRightType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.LowerArmRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.LowerBackType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.LowerBackDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.LowerSleeveLeftType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.LowerSleeveLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.LowerSleeveRightType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.LowerSleeveRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.MidBackType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.MidBackDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.RightChestType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.RightChestDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.RightShoulder)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.RightShoulderDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.SockCalfLeftType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.SockCalfLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.SockCalfRightType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.SockCalfRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.SockFrontLeftType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.SockFrontLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.SockFrontRightType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.SockFrontRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.ThighLeftType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.ThighLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.ThighRightType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.ThighRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.UploadLogoType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.UplaodLogoDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.UpperArmLeftType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.UpperArmLeftDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i + 3].EmblishmentType == EmblishmentOptionEnum.UpperArmRightType)
                                            {
                                                emblishment.Child4.Position = EmblishmentOptionEnum.UpperArmRightDisplay;
                                            }
                                            if (design.OrderEmblishmentDetails[i].EmblishmentType == "DefaultLogo")
                                            {
                                                emblishment.Child4.Position = "Default Logo";
                                            }

                                            emblishment.Child4.Font = design.OrderEmblishmentDetails[i + 3].Font;
                                            if (colors.Length > 0)
                                            {

                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[0]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color1 = colors[0] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color1 = colors[0];
                                                }
                                                // emblishment.Child4.Color1 = colors[0];
                                            }
                                            if (colors.Length > 1)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[1]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color2 = colors[1] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color2 = colors[1];
                                                }
                                                // emblishment.Child4.Color2 = colors[1];
                                            }
                                            if (colors.Length > 2)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[2]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color3 = colors[2] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color3 = colors[2];
                                                }
                                                // emblishment.Child4.Color3 = colors[2];
                                            }
                                            if (colors.Length > 3)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[3]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color4 = colors[3] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color4 = colors[3];
                                                }
                                                //  emblishment.Child4.Color4 = colors[3];
                                            }
                                            if (colors.Length > 4)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[4]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color5 = colors[4] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color5 = colors[4];
                                                }
                                                // emblishment.Child3.Color5 = colors[4];
                                            }
                                            if (colors.Length > 5)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[5]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color6 = colors[5] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color6 = colors[5];
                                                }
                                                //  emblishment.Child4.Color6 = colors[5];
                                            }
                                            if (colors.Length > 6)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[6]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color7 = colors[6] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color7 = colors[6];
                                                }
                                                //    emblishment.Child3.Color7 = colors[6];
                                            }
                                            if (colors.Length > 7)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[7]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color8 = colors[7] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color8 = colors[7];
                                                }

                                                //    emblishment.Child4.Color8 = colors[7];
                                            }
                                            if (colors.Length > 8)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[8]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color9 = colors[8] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color9 = colors[8];
                                                }

                                                //  emblishment.Child3.Color9 = colors[8];
                                            }
                                            if (colors.Length > 9)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[9]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color10 = colors[9] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color10 = colors[9];
                                                }
                                                //     emblishment.Child3.Color10 = colors[9];
                                            }
                                            if (colors.Length > 10)
                                            {
                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[10]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color11 = colors[10] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color11 = colors[10];
                                                }
                                                //   emblishment.Child3.Color11 = colors[10];
                                            }
                                            if (colors.Length > 11)
                                            {

                                                var colorWithCode = new DYORepository().getThermoColor(design.OrderEmblishmentDetails[i + 3].PrintMethod).Where(p => p.ColorCode == colors[11]).FirstOrDefault(); // colors[0]
                                                if (colorWithCode != null)
                                                {
                                                    emblishment.Child4.Color12 = colors[11] + "~" + colorWithCode.Color;
                                                }
                                                else
                                                {
                                                    emblishment.Child4.Color12 = colors[11];
                                                }
                                                // emblishment.Child3.Color12 = colors[11];
                                            }
                                        }
                                        if (emblishment.Child1 == null && emblishment.Child2 == null && emblishment.Child3 == null && emblishment.Child4 == null)
                                        {

                                        }
                                        else
                                        {
                                            obj.Parent.Add(emblishment);
                                        }
                                    }
                                }
                                #endregion

                                obj.OrderDesigns = new List<OrderDesignDetailReportModel>();

                                designDetail = new OrderDesignDetailReportModel();
                                if (design.ProductCode == "DSS1234" || design.ProductCode == "DSS1234")
                                {
                                    designDetail.NameHeader = "Name";
                                }
                                else
                                {
                                    designDetail.NameHeader = "Player Name";
                                }
                                designDetail.GarmentType = design.ProductName;                                
                                designDetail.StyleCode = design.ProductCode;
                                designDetail.Color1 = new ReportColorModel();
                                designDetail.OrderDesignDetails = new List<OrderDesignDetailReportModelDetail>();
                                foreach (var data in design.OrderDesignDetail)
                                {
                                    OrderDesignDetailReportModelDetail obj23 = new OrderDesignDetailReportModelDetail();

                                    obj23.PlayerName = data.PlayerName;
                                    obj23.PlayerNumber = data.PlayerNumber;
                                    obj23.Quantity = data.Quantity;
                                    obj23.size = data.size;
                                    designDetail.OrderDesignDetails.Add(obj23);
                                }
                                obj.OrderDesigns.Add(designDetail);
                                dataSource.Add(obj);
                            }
                            #endregion
                        }
                    }
                }
            }
            return dataSource;
        }

        public List<MainDataSourceHeader> GetJobSheetShiipingImage(int orderId)
        {
            List<MainDataSourceHeader> list = new List<MainDataSourceHeader>();

            MainDataSourceHeader HeaderInformation = new MainDataSourceHeader();

            var reportHeader = (from r in dbContext.Orders
                                join uc in dbContext.WMSUsers on r.CustomerId equals uc.Id
                                join uadd in dbContext.WMSUserAddresses on r.CustomerId equals uadd.UserId
                                join uaadc in dbContext.Countries on uadd.CountryId equals uaadc.CountryId
                                join u in dbContext.WMSUserAdditionals on r.CustomerId equals u.UserId
                                join us in dbContext.WMSUsers on r.CreatedBy equals us.Id
                                join oa in dbContext.OrderAddresses on r.OrderAddressId equals oa.Id
                                join oadC in dbContext.Countries on oa.CountryId equals oadC.CountryId
                                join repcode in dbContext.WMSUserAdditionals on r.CreatedBy equals repcode.UserId

                                where r.Id == orderId
                                select new
                                {
                                    OrderNumber = r.OrderNumber,
                                    CustomerRef = u.AccountNumber,
                                    ValidatedExFactoryDate = r.RequestedExFactoryDate,
                                    OrderDate = r.CreatedOnUtc,
                                    SalesRepEmail = us.Email,
                                    SalesRepCode = repcode.UserCode,
                                    PONumber = r.PONumber,
                                    DeliveryCompany = oa.CompanyName,
                                    DeliveryContact = oa.ContactFirstName + " " + oa.ContactLastName,
                                    DeliveryPhone = "(+" + oadC.CountryPhoneCode + ")" + oa.PhoneNumber,
                                    DeliveryEmail = oa.Email,
                                    DeliveryAddress1 = oa.Address,
                                    DeliveryAddress2 = oa.Address2,
                                    DeliveryCity = oa.City,
                                    DeliveryState = oa.State,
                                    DeliveryPostCode = oa.PostCode,
                                    DeliverySuberb = oa.Suburb,
                                    DeliveryCounty = oadC.CountryName,

                                    CustomerCompany = uc.CompanyName,
                                    CustomerContact = uc.ContactFirstName + " " + uc.ContactLastName,
                                    CustomerPhone = "(+" + uaadc.CountryPhoneCode + ") " + uc.TelephoneNo,
                                    CustomerEmail = uc.Email,
                                    CustomerAddress1 = uadd.Address,
                                    CustomerAddress2 = uadd.Address2,
                                    CustomerCity = uadd.City,
                                    CustomerState = uadd.State,
                                    CustomerPostCode = uadd.PostCode,
                                    CustomerSuberb = uadd.Suburb,
                                    CustomerCounty = uaadc.CountryName,

                                }).FirstOrDefault();

            if (reportHeader != null)
            {
                string Address = "";

                #region "Header Address"
                if (!string.IsNullOrEmpty(reportHeader.CustomerCompany))
                {
                    Address = Address + reportHeader.CustomerCompany + Environment.NewLine;
                }
                else
                {
                    Address = Address + reportHeader.CustomerContact + Environment.NewLine;
                }
                Address += "Address: ";
                if (!string.IsNullOrEmpty(reportHeader.CustomerAddress1))
                {
                    Address = Address + reportHeader.CustomerAddress1 + Environment.NewLine;
                }
                if (!string.IsNullOrEmpty(reportHeader.CustomerAddress2))
                {
                    Address = Address + reportHeader.CustomerAddress2 + Environment.NewLine;
                }
                if (!string.IsNullOrEmpty(reportHeader.CustomerCity))
                {
                    Address = Address + reportHeader.CustomerCity;
                }
                if (!string.IsNullOrEmpty(reportHeader.CustomerPostCode))
                {
                    Address = Address + " - " + reportHeader.CustomerPostCode + Environment.NewLine;
                }
                if (!string.IsNullOrEmpty(reportHeader.CustomerState))
                {
                    Address = Address + reportHeader.CustomerState + Environment.NewLine; ;
                }
                if (reportHeader.CustomerCounty != null && !string.IsNullOrEmpty(reportHeader.CustomerCounty))
                {
                    Address = Address + reportHeader.CustomerCounty + Environment.NewLine;
                }
                Address += "Phone: ";
                if (reportHeader.CustomerPhone != null)
                {
                    Address += reportHeader.CustomerPhone + Environment.NewLine;
                }
                Address += "Email: ";

                if (!string.IsNullOrEmpty(reportHeader.CustomerEmail))
                {
                    Address += reportHeader.CustomerEmail + Environment.NewLine;
                }

                #endregion

                HeaderInformation.CustomerAddress = Address;
                HeaderInformation.DeliveryCustomerName = reportHeader.DeliveryCompany;
                HeaderInformation.DeliveryContactName = reportHeader.DeliveryContact;
                HeaderInformation.DeliveryPhoneNumber = reportHeader.DeliveryPhone;
                HeaderInformation.DeliveryEmailAddress = reportHeader.DeliveryEmail;
                HeaderInformation.DeliveryAddressStreet = reportHeader.DeliveryAddress1;
                HeaderInformation.DeliveryAddressSuburb = reportHeader.DeliveryAddress2;
                HeaderInformation.DeliveryAddressState = reportHeader.DeliveryState;
                HeaderInformation.DeliveryPostCode = reportHeader.DeliveryPostCode;
                HeaderInformation.DeliveryCountry = reportHeader.DeliveryCounty;
                HeaderInformation.DeliveryPort = string.Empty;
                HeaderInformation.OrderDate = reportHeader.OrderDate.ToString("dd-MMM-yyyy");
                HeaderInformation.PONumber = reportHeader.PONumber;

                HeaderInformation.SalesRepCode = reportHeader.SalesRepCode; //string.Empty;
                HeaderInformation.SalesOrderNumber = reportHeader.OrderNumber;
                HeaderInformation.SalesRepEmailAddress = reportHeader.SalesRepEmail;
                HeaderInformation.SalesCustomerReference = reportHeader.CustomerRef;
                HeaderInformation.FactoryDate = reportHeader.ValidatedExFactoryDate.HasValue ? reportHeader.ValidatedExFactoryDate.Value.ToString("dd-MMM-yyyy") : "";
                HeaderInformation.Logo = "~/Files/Images/dynasty.png";
                HeaderInformation.Logo = HttpContext.Current.Server.MapPath("~/Files/Images/dynasty.png"); ;
                HeaderInformation.Customer = "DYNASTY STOCK ORDER DETAILS - CHINA";
                HeaderInformation.SalesDeliveryTimelines = "DELIVERY TIMELINES - APPROXIMATE - 2.5 weeks";
                list.Add(HeaderInformation);
            }

            return list;
        }

        public string GetReportFileName(ReportTrackandTraceModel ReportDetail)
        {
            var UserType = (from Usr in dbContext.WMSUserRoles
                            join rl in dbContext.WMSRoles on Usr.RoleId equals rl.Id
                            where Usr.UserId == ReportDetail.UserId
                            select new
                            {
                                RoleId = Usr.RoleId
                            }).FirstOrDefault();

            string fileName = string.Empty;
            if (UserType.RoleId == (int)WMSUserRoleEnum.Customer)
            {
                ReportDetail.CustomerId = ReportDetail.UserId;
                var customer = dbContext.WMSUsers.Find(ReportDetail.CustomerId);
                if (customer != null)
                {
                    fileName = customer.CompanyName + "_Orders";
                }
                else
                {
                    fileName = "All_Orders";
                }
            }
            else
            {
                if (ReportDetail.CustomerId > 0)
                {
                    var customer = dbContext.WMSUsers.Find(ReportDetail.CustomerId);
                    if (customer != null)
                    {
                        fileName = customer.CompanyName + "_Orders";
                    }
                    else
                    {
                        fileName = "All_Orders";
                    }
                }
                else
                {
                    fileName = "All_Orders";
                }
            }

            return fileName;
        }

        public List<PickupRequestReportModel> GetPickRequestObj(int orderId)
        {
            List<PickupRequestReportModel> dataSource = new List<PickupRequestReportModel>();
            PickupRequestReportModel obj = new PickupRequestReportModel();
            var Order = dbContext.Orders.Find(orderId);
            obj.OrderNumber = Order.OrderNumber;
            obj.OrderDate = Order.CreatedOnUtc;
            obj.Barcode = Order.OrderNumber;
            var dbOrder = dbContext.Orders.Find(orderId);

            #region "Shipping Address"

            var OrderAddress = (from r in dbContext.OrderAddresses
                                join c in dbContext.Countries on r.CountryId equals c.CountryId
                                where r.Id == dbOrder.OrderAddressId
                                select new DYOOrderAddress
                                {
                                    Address = r.Address,
                                    Address2 = r.Address2,
                                    Area = r.Suburb,
                                    State = r.State,
                                    City = r.City,
                                    FirstName = r.ContactFirstName,
                                    LastName = r.ContactLastName,
                                    CompanyName = r.CompanyName,
                                    Email = r.Email,
                                    Phone = "(+" + c.CountryPhoneCode + ") " + r.PhoneNumber,
                                    PostCode = r.PostCode,
                                    Country = new Model.User.WMSCountry
                                    {
                                        Code = c.CountryCode,
                                        Code2 = c.CountryCode2,
                                        CountryPhoneCode = c.CountryPhoneCode,
                                        Name = c.CountryName
                                    }
                                }).FirstOrDefault();

            if (OrderAddress != null)
            {
                string Address = string.Empty;

                if (!string.IsNullOrEmpty(Order.PONumber))
                {
                    Address += "PO Number: ";
                    Address = Address + Order.PONumber + Environment.NewLine;
                }

                Address += "Order Number: ";
                if (!string.IsNullOrEmpty(obj.OrderNumber))
                {
                    Address = Address + obj.OrderNumber + Environment.NewLine;
                }
                Address += "Order Date: ";

                Address = Address + Order.CreatedOnUtc.ToString("dd-MMM-yyyy") + Environment.NewLine;

                Address += "Company: ";
                if (!string.IsNullOrEmpty(OrderAddress.CompanyName))
                {
                    Address = Address + OrderAddress.CompanyName + Environment.NewLine;
                }
                else
                {
                    Address = Address + OrderAddress.FirstName + " " + OrderAddress.LastName + Environment.NewLine;
                }
                Address += "Address: ";
                if (!string.IsNullOrEmpty(OrderAddress.Address))
                {
                    Address = Address + OrderAddress.Address + Environment.NewLine;
                }
                if (!string.IsNullOrEmpty(OrderAddress.Address2))
                {
                    Address = Address + OrderAddress.Address2 + Environment.NewLine;
                }
                if (!string.IsNullOrEmpty(OrderAddress.City))
                {
                    Address = Address + OrderAddress.City;
                }
                if (!string.IsNullOrEmpty(OrderAddress.PostCode))
                {
                    Address = Address + " - " + OrderAddress.PostCode + Environment.NewLine;
                }
                if (!string.IsNullOrEmpty(OrderAddress.State))
                {
                    Address = Address + OrderAddress.State + Environment.NewLine; ;
                }
                if (OrderAddress.Country != null && !string.IsNullOrEmpty(OrderAddress.Country.Name))
                {
                    Address = Address + OrderAddress.Country.Name + Environment.NewLine;
                }
                Address += "Phone: ";
                if (OrderAddress.Country != null && !string.IsNullOrEmpty(OrderAddress.Country.CountryPhoneCode) && !string.IsNullOrEmpty(OrderAddress.Phone))
                {
                    Address += OrderAddress.Phone + Environment.NewLine;
                }
                Address += "Email: ";

                if (!string.IsNullOrEmpty(OrderAddress.Email))
                {
                    Address += OrderAddress.Email + Environment.NewLine;
                }

                obj.ShippingAddress = Address;
            }

            #endregion

            #region "Customer Address"

            var customreAddress = (from r in dbContext.WMSUsers
                                   join ua in dbContext.WMSUserAddresses on r.Id equals ua.UserId
                                   join c in dbContext.Countries on ua.CountryId equals c.CountryId
                                   where r.Id == dbOrder.CustomerId
                                   select new DYOOrderAddress
                                   {
                                       Address = ua.Address,
                                       Address2 = ua.Address2,
                                       Area = ua.Suburb,
                                       State = ua.State,
                                       City = ua.City,
                                       FirstName = r.ContactFirstName,
                                       LastName = r.ContactLastName,
                                       CompanyName = r.CompanyName,
                                       Email = r.Email,
                                       Phone = "(+" + c.CountryPhoneCode + ") " + r.TelephoneNo,
                                       PostCode = ua.PostCode,
                                       Country = new Model.User.WMSCountry
                                       {
                                           Code = c.CountryCode,
                                           Code2 = c.CountryCode2,
                                           CountryPhoneCode = c.CountryPhoneCode,
                                           Name = c.CountryName
                                       }
                                   }).FirstOrDefault();

            var customreDetail = dbContext.WMSUserAdditionals.Where(p => p.UserId == dbOrder.CustomerId).FirstOrDefault();
            if (customreDetail != null)
            {
                string Address = "Company: ";
                if (!string.IsNullOrEmpty(customreAddress.CompanyName))
                {
                    Address = Address + customreAddress.CompanyName + Environment.NewLine;
                }
                else
                {
                    Address = Address + customreAddress.FirstName + " " + customreAddress.LastName + Environment.NewLine;
                }
                Address += "Address: ";
                if (!string.IsNullOrEmpty(customreAddress.Address))
                {
                    Address = Address + customreAddress.Address + Environment.NewLine;
                }
                if (!string.IsNullOrEmpty(customreAddress.Address2))
                {
                    Address = Address + customreAddress.Address2 + Environment.NewLine;
                }
                if (!string.IsNullOrEmpty(customreAddress.City))
                {
                    Address = Address + customreAddress.City;
                }
                if (!string.IsNullOrEmpty(customreAddress.PostCode))
                {
                    Address = Address + " - " + customreAddress.PostCode + Environment.NewLine;
                }
                if (!string.IsNullOrEmpty(customreAddress.State))
                {
                    Address = Address + customreAddress.State + Environment.NewLine; ;
                }
                if (customreAddress.Country != null && !string.IsNullOrEmpty(customreAddress.Country.Name))
                {
                    Address = Address + customreAddress.Country.Name + Environment.NewLine;
                }
                Address += "Phone: ";
                if (customreAddress.Country != null && !string.IsNullOrEmpty(customreAddress.Country.CountryPhoneCode) && !string.IsNullOrEmpty(customreAddress.Phone))
                {
                    Address += customreAddress.Phone + Environment.NewLine;
                }
                Address += "Email: ";

                if (!string.IsNullOrEmpty(customreAddress.Email))
                {
                    Address += customreAddress.Email + Environment.NewLine;
                }
                obj.InvoiceAddress = Address;
            }

            #endregion

            obj.Stocks = new List<PickupRequestReportModelDetail>();
            var designs = dbContext.OrderDesigns.Where(p => p.OrderId == orderId).ToList();

            PickupRequestReportModelDetail pick;
            foreach (var item in designs)
            {
                List<DYOOrdePickup> pickDatas = new List<DYOOrdePickup>();
                var orderDesigns = dbContext.OrderDesignDetails.Where(p => p.OrderDesignId == item.Id).ToList();
                int count = 0;
                foreach (var detail in orderDesigns)
                {
                    var SKU = dbContext.ProductSKUs.Find(detail.ProductSKUId);
                    var product = dbContext.ProductMasters.Find(SKU.ProductId);
                    var skus = dbContext.OrderPickupDetails.Where(p => p.OrderDesignDetailId == detail.Id).ToList();
                    var color = dbContext.Colors.Find(SKU.ColorId);

                    foreach (var pk in skus)
                    {
                        //.// DYOOrdePickup model = new DYOOrdePickup();
                        pick = new PickupRequestReportModelDetail();
                        var pickData = (from w in dbContext.SectionDetails
                                        join a in dbContext.ShelfRowDetails on w.ShelfRowID equals a.ID
                                        join l in dbContext.LineMasters on a.LineID equals l.ID
                                        join b in dbContext.ShelfMasters on a.ShelfID equals b.ID
                                        join c in dbContext.RowMasters on a.RowID equals c.ID
                                        join q in dbContext.SectionMasters on w.SectionID equals q.ID
                                        where w.ID == pk.SectionDetailID
                                        select new
                                        {
                                            ID = w.ID,
                                            Line = l.Name,
                                            Rack = b.Name,
                                            Row = c.Name,
                                            Location = q.Name,
                                        }).FirstOrDefault();
                        #region "Commented Code"
                        //model.Line = pickData.Line;
                        //model.PickLocation = pickData.Location;
                        //model.Rack = pickData.Rack;
                        //model.Row = pickData.Row;
                        //model.QuantityToPick = pk.RequiredQty.Value;
                        #endregion
                        pick.GrossWeight = SKU.Weight * (pk.RequiredQty.HasValue ? pk.RequiredQty.Value : 0);
                        pick.NetWeight = SKU.Weight * (pk.RequiredQty.HasValue ? pk.RequiredQty.Value : 0);
                        pick.QuantityToPick = pk.RequiredQty.HasValue ? pk.RequiredQty.Value : 0;
                        pick.PickedQuantity = pk.PickedQty.HasValue ? pk.PickedQty.Value : 0;
                        pick.Description = product.ProductName + " (" + product.ProductCode + " )";
                        pick.SKU = SKU.SKU;
                        if (color != null)
                        {
                            pick.ProductColor = color.color1;
                        }
                        pick.DesignNumber = item.DesignNumber;
                        pick.PickLocation = pickData.Location;
                        pick.Row = pickData.Row;
                        pick.Rack = pickData.Rack;
                        pick.Line = pickData.Line;
                        pick.SNo = ++count;
                        obj.Stocks.Add(pick);
                    }

                    #region "Commented Code"
                    //var data = (from w in dbContext.SectionDetails
                    //            join a in dbContext.ShelfRowDetails on w.ShelfRowID equals a.ID
                    //            join l in dbContext.LineMasters on a.LineID equals l.ID
                    //            join b in dbContext.ShelfMasters on a.ShelfID equals b.ID
                    //            join c in dbContext.RowMasters on a.RowID equals c.ID
                    //            join q in dbContext.SectionMasters on w.SectionID equals q.ID
                    //            where w.ID == section.SectionDetailID
                    //            select new
                    //            {
                    //                Line = l.Name,
                    //                Rack = b.Name,
                    //                Row = c.Name,
                    //                Location = q.Name,
                    //            }).FirstOrDefault();

                    //if (data != null)
                    //{
                    //    pick.GrossWeight = SKU.Weight * detail.Quantity;
                    //    pick.NetWeight = SKU.Weight * detail.Quantity;
                    //    pick.QuantityToPick = detail.Quantity;
                    //    pick.Description = product.ProductName + " (" + product.ProductCode + " )";
                    //    pick.SKU = SKU.SKU;
                    //    pick.PickLocation = data.Location;
                    //    pick.Row = data.Row;
                    //    pick.Rack = data.Rack;
                    //    pick.Line = data.Line;
                    //    pick.SNo = ++count;
                    //}
                    //else
                    //{
                    //    pick.GrossWeight = 0;
                    //    pick.NetWeight = 0;
                    //    pick.QuantityToPick = 0;
                    //    pick.Description = "";
                    //    pick.SKU = "";
                    //    pick.PickLocation = "";
                    //    pick.Row = "";
                    //    pick.Rack = "";
                    //    pick.Line = "";
                    //    pick.SNo = ++count;
                    //}
                    //obj.Stocks.Add(pick);
                    #endregion
                }
            }

            dataSource.Add(obj);
            return dataSource;
        }

        public List<PickupRequestReportModel> GetDispatchProcessObj(int orderId)
        {
            List<PickupRequestReportModel> dataSource = new List<PickupRequestReportModel>();
            PickupRequestReportModel obj = new PickupRequestReportModel();
            var Order = dbContext.Orders.Find(orderId);
            obj.OrderNumber = Order.OrderNumber;
            obj.OrderDate = Order.CreatedOnUtc;
            obj.Barcode = Order.OrderNumber;

            obj.Stocks = new List<PickupRequestReportModelDetail>();
            var designs = dbContext.OrderDesigns.Where(p => p.OrderId == orderId).ToList();

            PickupRequestReportModelDetail pick;
            foreach (var item in designs)
            {
                List<DYOOrdePickup> pickDatas = new List<DYOOrdePickup>();
                var orderDesigns = dbContext.OrderDesignDetails.Where(p => p.OrderDesignId == item.Id).ToList();
                int count = 0;
                foreach (var detail in orderDesigns)
                {
                    var SKU = dbContext.ProductSKUs.Find(detail.ProductSKUId);

                    var product = dbContext.ProductMasters.Find(SKU.ProductId);

                    var skus = dbContext.OrderPickupDetails.Where(p => p.OrderDesignDetailId == detail.Id).ToList();


                    foreach (var pk in skus)
                    {
                        //.// DYOOrdePickup model = new DYOOrdePickup();
                        pick = new PickupRequestReportModelDetail();
                        var pickData = (from w in dbContext.SectionDetails
                                        join a in dbContext.ShelfRowDetails on w.ShelfRowID equals a.ID
                                        join l in dbContext.LineMasters on a.LineID equals l.ID
                                        join b in dbContext.ShelfMasters on a.ShelfID equals b.ID
                                        join c in dbContext.RowMasters on a.RowID equals c.ID
                                        join q in dbContext.SectionMasters on w.SectionID equals q.ID
                                        where w.ID == pk.SectionDetailID
                                        select new
                                        {
                                            ID = w.ID,
                                            Line = l.Name,
                                            Rack = b.Name,
                                            Row = c.Name,
                                            Location = q.Name,
                                        }).FirstOrDefault();

                        pick.GrossWeight = SKU.Weight * detail.Quantity;
                        pick.NetWeight = SKU.Weight * detail.Quantity;
                        pick.QuantityToPick = detail.Quantity;
                        pick.PickedQuantity = detail.PickedQuantity.HasValue ? detail.PickedQuantity.Value : 0;
                        pick.Description = product.ProductName + " (" + product.ProductCode + " )";
                        pick.SKU = SKU.SKU;
                        pick.PickLocation = pickData.Location;
                        pick.Row = pickData.Row;
                        pick.Rack = pickData.Rack;
                        pick.Line = pickData.Line;
                        pick.SNo = ++count;
                        obj.Stocks.Add(pick);
                    }
                }
            }

            dataSource.Add(obj);
            return dataSource;
        }

        public void SaveOrderPickUpDetail(int orderId, OrderDesignDetail detail, List<DYOOrdePickup> pickData)
        {
            if (pickData.Count > 0)
            {
                foreach (var item in pickData)
                {
                    OrderPickupDetail pickUp = new OrderPickupDetail();
                    pickUp.OrderID = orderId;
                    pickUp.OrderDesignDetailId = detail.Id;
                    pickUp.ProductSKUID = detail.ProductSKUId;
                    pickUp.RequiredQty = detail.Quantity;
                    pickUp.SectionDetailID = pickUp.SectionDetailID;


                    dbContext.OrderPickupDetails.Add(pickUp);
                    dbContext.SaveChanges();
                }
            }

            throw new NotImplementedException();
        }

        public List<WarehouseLabels> WarehouseDispatchLabel(int orderId, int userId)
        {
            List<WarehouseLabels> _labels = new List<WarehouseLabels>();
            List<WarehouseDispatch> _warehouse = new List<WarehouseDispatch>();

            WarehouseDispatch warehouse;

            var dispatch = (from od in dbContext.Orders
                            join odc in dbContext.OrderCartons on od.Id equals odc.OrderID
                            join odcd in dbContext.OrderCartonDetails on odc.ID equals odcd.CartonID
                            join psku in dbContext.ProductSKUs on odcd.ProductSKUID equals psku.Id
                            join ua in dbContext.WMSUserAdditionals on od.CustomerId equals ua.UserId
                            join whu in dbContext.WMSUsers on od.WarehouseUserId equals whu.Id
                            join tz in dbContext.Timezones on whu.TimezoneId equals tz.TimezoneId into leftJoin
                            from temp in leftJoin.DefaultIfEmpty()
                            where od.Id == orderId
                            select new
                            {
                                PickNumber = od.OrderNumber,
                                PONumber = od.PONumber,
                                CartonId = odcd.CartonID,
                                Pices = odcd.Quantity.HasValue ? odcd.Quantity.Value : 0,
                                GrossWeight = psku.Weight,
                                NetWeight = psku.Weight,
                                CustmerId = ua.AccountNumber,
                                TimeZoneName = temp == null ? "" : temp.OffsetShort,
                                Unit = psku.WeightUnit
                            }).ToList();

            var group = dispatch.GroupBy(p => new { p.PickNumber, p.PONumber, p.CartonId, p.CustmerId, p.TimeZoneName })
                                .Select(x => new
                                {
                                    x.Key.CustmerId,
                                    x.Key.TimeZoneName,
                                    x.Key.PickNumber,
                                    x.Key.PONumber,
                                    detail = x.Select(h => new
                                    {
                                        Pices = h.Pices,
                                        Unit = h.Unit,
                                        GrossWeight = h.GrossWeight * h.Pices,
                                        NetWeight = h.NetWeight * h.Pices
                                    }).ToList()
                                }).ToList();

            int srno = 1;
            foreach (var ware in group)
            {
                warehouse = new WarehouseDispatch();
                warehouse.PickNumber = ware.PickNumber;
                warehouse.CustomerId = ware.CustmerId;
                warehouse.Date = DateTime.Now;
                warehouse.TimeZone = ware.TimeZoneName;
                warehouse.PONumber = ware.PONumber;

                foreach (var dd in ware.detail)
                {
                    warehouse.Pices += dd.Pices;
                    warehouse.WeightUnit = dd.Unit;
                    warehouse.GrossWeight += dd.GrossWeight;
                    warehouse.NetWeight += dd.NetWeight;
                }
                warehouse.Carton = srno + "/" + group.Count().ToString();
                _warehouse.Add(warehouse);
                srno++;
            }

            WarehouseLabels lables = new WarehouseLabels();
            lables.warehouse = _warehouse;
            _labels.Add(lables);
            return _labels;
        }

        #region Vikshit Code
        public List<AllSkuReportModel> GetAllSKU(ReportTrackandTraceModel ReportDetail)
        {
            var ProductSKUDetail = (from SKU in dbContext.ProductSKUs
                                    join pMstr in dbContext.ProductMasters on SKU.ProductId equals pMstr.Id
                                    join Clr in dbContext.Colors on SKU.ColorId equals Clr.ID
                                    join Sz in dbContext.Sizes on SKU.SizeId equals Sz.ID
                                    select new AllSkuReportModel
                                    {
                                        ProductId = pMstr.Id,
                                        ProductName = pMstr.ProductName,
                                        Colour = Clr.color1,
                                        Size = Sz.size1,
                                        SizeId = Sz.ID,
                                        StyleCode = pMstr.ProductCode,
                                        NetWeight = SKU.Weight,
                                        Quantity = SKU.ActualQuantity.Value,
                                        WeightUnit = SKU.WeightUnit
                                    }).OrderBy(p => new { p.ProductId, p.Colour, p.SizeId }).ToList();
            if (ProductSKUDetail.Count > 0)
            {
                foreach (var item in ProductSKUDetail)
                {
                    if (item.WeightUnit == WMSOrdersUnit.Unit2)
                    {
                        item.NetWeight = Math.Round(item.NetWeight * item.Quantity / 1000, 2);
                    }
                }
                return ProductSKUDetail;
            }
            else
            {
                return null;
            }
        }

        public List<DynaStySportAllStockReportModel> GetStockOrder(ReportTrackandTraceModel ReportDetail)
        {
            List<DynaStySportAllStockReportModel> ProductSKUDetail = new List<DynaStySportAllStockReportModel>();
            var CheckCustomerIsExist = dbContext.ProductCatagories.Where(a => a.CustomerId == ReportDetail.CustomerId).FirstOrDefault();
            if (CheckCustomerIsExist == null)
            {
                if (ReportDetail.ReportStatus == "Available")
                {
                    //if (ReportDetail.FromDate != null && ReportDetail.ToDate != null)
                    //{
                    //    ProductSKUDetail = (from UsrStc in dbContext.UserStocks
                    //                        join SKU in dbContext.ProductSKUs on UsrStc.ProductSKUId equals SKU.Id
                    //                        join pMstr in dbContext.ProductMasters on SKU.ProductId equals pMstr.Id
                    //                        join Clr in dbContext.Colors on SKU.ColorId equals Clr.ID
                    //                        join Sz in dbContext.Sizes on SKU.SizeId equals Sz.ID
                    //                        where SKU.ActualQuantity > SKU.WarningLevel
                    //                        && ReportDetail.FromDate <= SKU.CreatedOnUtc && ReportDetail.ToDate >= SKU.CreatedOnUtc
                    //                        && UsrStc.UserId == ReportDetail.CustomerId
                    //                        select new DynaStySportAllStockReportModel
                    //                        {
                    //                            ProductName = pMstr.ProductName,
                    //                            Colour = Clr.color1,
                    //                            Size = Sz.size1,
                    //                            StyleCode = pMstr.ProductCode,
                    //                            NetWeight = SKU.Weight,
                    //                            Quantity = SKU.ActualQuantity.Value
                    //                        }).ToList();
                    //}
                    // else
                    // {

                    ProductSKUDetail = (from UsrStc in dbContext.UserStocks
                                        join SKU in dbContext.ProductSKUs on UsrStc.ProductSKUId equals SKU.Id
                                        join pMstr in dbContext.ProductMasters on SKU.ProductId equals pMstr.Id
                                        join Clr in dbContext.Colors on SKU.ColorId equals Clr.ID
                                        join Sz in dbContext.Sizes on SKU.SizeId equals Sz.ID
                                        where SKU.ActualQuantity > SKU.WarningLevel
                                        && UsrStc.UserId == ReportDetail.CustomerId
                                        select new DynaStySportAllStockReportModel
                                        {
                                            ProductName = pMstr.ProductName,
                                            Colour = Clr.color1,
                                            Size = Sz.size1,
                                            StyleCode = pMstr.ProductCode,
                                            //NetWeight = SKU.WeightUnit == WMSOrdersUnit.Unit2 ? Math.Round(SKU.Weight * SKU.ActualQuantity.Value / 1000, 2) : SKU.Weight,
                                            NetWeight = SKU.ActualQuantity.Value > 0 ? (SKU.WeightUnit == WMSOrdersUnit.Unit2 ? Math.Round(SKU.Weight * SKU.ActualQuantity.Value / 1000, 2) : SKU.Weight) : 0,
                                            Quantity = SKU.ActualQuantity.Value,
                                        }).ToList();
                    // }
                }
                else if (ReportDetail.ReportStatus == "RunningOutOfStock")
                {
                    //if (ReportDetail.FromDate != null && ReportDetail.ToDate != null)
                    //{
                    //    ProductSKUDetail = (from UsrStc in dbContext.UserStocks
                    //                        join SKU in dbContext.ProductSKUs on UsrStc.ProductSKUId equals SKU.Id
                    //                        join pMstr in dbContext.ProductMasters on SKU.ProductId equals pMstr.Id
                    //                        join Clr in dbContext.Colors on SKU.ColorId equals Clr.ID
                    //                        join Sz in dbContext.Sizes on SKU.SizeId equals Sz.ID
                    //                        where SKU.ActualQuantity == SKU.WarningLevel
                    //                        && ReportDetail.FromDate <= SKU.CreatedOnUtc && ReportDetail.ToDate >= SKU.CreatedOnUtc
                    //                        && UsrStc.UserId == ReportDetail.CustomerId
                    //                        select new DynaStySportAllStockReportModel
                    //                        {
                    //                            ProductName = pMstr.ProductName,
                    //                            Colour = Clr.color1,
                    //                            Size = Sz.size1,
                    //                            StyleCode = pMstr.ProductCode,
                    //                            NetWeight = SKU.Weight,
                    //                            Quantity = SKU.ActualQuantity.Value
                    //                        }).ToList();
                    //}
                    //  else
                    //  {
                    ProductSKUDetail = (from UsrStc in dbContext.UserStocks
                                        join SKU in dbContext.ProductSKUs on UsrStc.ProductSKUId equals SKU.Id
                                        join pMstr in dbContext.ProductMasters on SKU.ProductId equals pMstr.Id
                                        join Clr in dbContext.Colors on SKU.ColorId equals Clr.ID
                                        join Sz in dbContext.Sizes on SKU.SizeId equals Sz.ID
                                        where SKU.ActualQuantity == SKU.WarningLevel
                                        && UsrStc.UserId == ReportDetail.CustomerId
                                        select new DynaStySportAllStockReportModel
                                        {
                                            ProductName = pMstr.ProductName,
                                            Colour = Clr.color1,
                                            Size = Sz.size1,
                                            StyleCode = pMstr.ProductCode,
                                            //NetWeight = SKU.WeightUnit == WMSOrdersUnit.Unit2 ? Math.Round(SKU.Weight * SKU.ActualQuantity.Value / 1000, 2) : SKU.Weight,
                                            NetWeight = SKU.ActualQuantity.Value > 0 ? (SKU.WeightUnit == WMSOrdersUnit.Unit2 ? Math.Round(SKU.Weight * SKU.ActualQuantity.Value / 1000, 2) : SKU.Weight) : 0,
                                            Quantity = SKU.ActualQuantity.Value
                                        }).ToList();
                    //   }
                }
                else if (ReportDetail.ReportStatus == "OutOfStock")
                {
                    //if (ReportDetail.FromDate != null && ReportDetail.ToDate != null)
                    //{
                    //    ProductSKUDetail = (from UsrStc in dbContext.UserStocks
                    //                        join SKU in dbContext.ProductSKUs on UsrStc.ProductSKUId equals SKU.Id
                    //                        join pMstr in dbContext.ProductMasters on SKU.ProductId equals pMstr.Id
                    //                        join Clr in dbContext.Colors on SKU.ColorId equals Clr.ID
                    //                        join Sz in dbContext.Sizes on SKU.SizeId equals Sz.ID
                    //                        where SKU.ActualQuantity < SKU.WarningLevel
                    //                        && ReportDetail.FromDate <= SKU.CreatedOnUtc && ReportDetail.ToDate >= SKU.CreatedOnUtc
                    //                        && UsrStc.UserId == ReportDetail.CustomerId
                    //                        select new DynaStySportAllStockReportModel
                    //                        {
                    //                            ProductName = pMstr.ProductName,
                    //                            Colour = Clr.color1,
                    //                            Size = Sz.size1,
                    //                            StyleCode = pMstr.ProductCode,
                    //                            NetWeight = SKU.Weight,
                    //                            Quantity = SKU.ActualQuantity.Value
                    //                        }).ToList();
                    //}
                    //else
                    //  {
                    ProductSKUDetail = (from UsrStc in dbContext.UserStocks
                                        join SKU in dbContext.ProductSKUs on UsrStc.ProductSKUId equals SKU.Id
                                        join pMstr in dbContext.ProductMasters on SKU.ProductId equals pMstr.Id
                                        join Clr in dbContext.Colors on SKU.ColorId equals Clr.ID
                                        join Sz in dbContext.Sizes on SKU.SizeId equals Sz.ID
                                        where SKU.ActualQuantity < SKU.WarningLevel
                                        && UsrStc.UserId == ReportDetail.CustomerId
                                        select new DynaStySportAllStockReportModel
                                        {
                                            ProductName = pMstr.ProductName,
                                            Colour = Clr.color1,
                                            Size = Sz.size1,
                                            StyleCode = pMstr.ProductCode,
                                            //NetWeight = SKU.WeightUnit == WMSOrdersUnit.Unit2 ? Math.Round(SKU.Weight * SKU.ActualQuantity.Value / 1000, 2) : SKU.Weight,
                                            NetWeight = SKU.ActualQuantity.Value > 0 ? (SKU.WeightUnit == WMSOrdersUnit.Unit2 ? Math.Round(SKU.Weight * SKU.ActualQuantity.Value / 1000, 2) : SKU.Weight) : 0,
                                            Quantity = SKU.ActualQuantity.Value
                                        }).ToList();
                    // }
                }
            }
            else
            {
                if (ReportDetail.ReportStatus == "Available")
                {
                    ProductSKUDetail = (from SKU in dbContext.ProductSKUs
                                        join pMstr in dbContext.ProductMasters on SKU.ProductId equals pMstr.Id
                                        join Clr in dbContext.Colors on SKU.ColorId equals Clr.ID
                                        join Sz in dbContext.Sizes on SKU.SizeId equals Sz.ID
                                        where SKU.ActualQuantity > SKU.WarningLevel
                                        select new DynaStySportAllStockReportModel
                                        {
                                            ProductName = pMstr.ProductName,
                                            Colour = Clr.color1,
                                            Size = Sz.size1,
                                            StyleCode = pMstr.ProductCode,
                                            //NetWeight = SKU.WeightUnit == WMSOrdersUnit.Unit2 ? Math.Round(SKU.Weight * SKU.ActualQuantity.Value / 1000, 2) : SKU.Weight,
                                            NetWeight = SKU.ActualQuantity.Value > 0 ? (SKU.WeightUnit == WMSOrdersUnit.Unit2 ? Math.Round(SKU.Weight * SKU.ActualQuantity.Value / 1000, 2) : SKU.Weight) : 0,
                                            Quantity = SKU.ActualQuantity.Value
                                        }).ToList();
                }
                else if (ReportDetail.ReportStatus == "RunningOutOfStock")
                {
                    ProductSKUDetail = (from SKU in dbContext.ProductSKUs
                                        join pMstr in dbContext.ProductMasters on SKU.ProductId equals pMstr.Id
                                        join Clr in dbContext.Colors on SKU.ColorId equals Clr.ID
                                        join Sz in dbContext.Sizes on SKU.SizeId equals Sz.ID
                                        where SKU.ActualQuantity == SKU.WarningLevel
                                        select new DynaStySportAllStockReportModel
                                        {
                                            ProductName = pMstr.ProductName,
                                            Colour = Clr.color1,
                                            Size = Sz.size1,
                                            StyleCode = pMstr.ProductCode,
                                            //NetWeight = SKU.WeightUnit == WMSOrdersUnit.Unit2 ? Math.Round(SKU.Weight * SKU.ActualQuantity.Value / 1000, 2) : SKU.Weight,
                                            NetWeight = SKU.ActualQuantity.Value > 0 ? (SKU.WeightUnit == WMSOrdersUnit.Unit2 ? Math.Round(SKU.Weight * SKU.ActualQuantity.Value / 1000, 2) : SKU.Weight) : 0,
                                            Quantity = SKU.ActualQuantity.Value
                                        }).ToList();
                }
                else if (ReportDetail.ReportStatus == "OutOfStock")
                {
                    ProductSKUDetail = (from SKU in dbContext.ProductSKUs
                                        join pMstr in dbContext.ProductMasters on SKU.ProductId equals pMstr.Id
                                        join Clr in dbContext.Colors on SKU.ColorId equals Clr.ID
                                        join Sz in dbContext.Sizes on SKU.SizeId equals Sz.ID
                                        where SKU.ActualQuantity < SKU.WarningLevel
                                        select new DynaStySportAllStockReportModel
                                        {
                                            ProductName = pMstr.ProductName,
                                            Colour = Clr.color1,
                                            Size = Sz.size1,
                                            StyleCode = pMstr.ProductCode,
                                            //NetWeight = SKU.WeightUnit == WMSOrdersUnit.Unit2 ? Math.Round(SKU.Weight * SKU.ActualQuantity.Value / 1000, 2) : SKU.Weight,
                                            NetWeight = SKU.ActualQuantity.Value > 0 ? (SKU.WeightUnit == WMSOrdersUnit.Unit2 ? Math.Round(SKU.Weight * SKU.ActualQuantity.Value / 1000, 2) : SKU.Weight) : 0,
                                            Quantity = SKU.ActualQuantity.Value
                                        }).ToList();
                }
                else
                {
                    ProductSKUDetail = (from SKU in dbContext.ProductSKUs
                                        join pMstr in dbContext.ProductMasters on SKU.ProductId equals pMstr.Id
                                        join Clr in dbContext.Colors on SKU.ColorId equals Clr.ID
                                        join Sz in dbContext.Sizes on SKU.SizeId equals Sz.ID
                                        select new DynaStySportAllStockReportModel
                                        {
                                            ProductName = pMstr.ProductName,
                                            Colour = Clr.color1,
                                            Size = Sz.size1,
                                            StyleCode = pMstr.ProductCode,
                                            //NetWeight = SKU.WeightUnit == WMSOrdersUnit.Unit2 ? Math.Round(SKU.Weight * SKU.ActualQuantity.Value / 1000, 2) : SKU.Weight,
                                            NetWeight = SKU.ActualQuantity.Value > 0 ? (SKU.WeightUnit == WMSOrdersUnit.Unit2 ? Math.Round(SKU.Weight * SKU.ActualQuantity.Value / 1000, 2) : SKU.Weight) : 0,
                                            Quantity = SKU.ActualQuantity.Value
                                        }).ToList();
                }
            }


            if (ProductSKUDetail.Count > 0)
            {
                return ProductSKUDetail;
            }
            else
            {
                return null;
            }
        }

        public List<CustomerReportModel> GetCustomer(ReportTrackandTraceModel ReportDetail)
        {
            var UserType = (from Usr in dbContext.WMSUserRoles
                            join rl in dbContext.WMSRoles on Usr.RoleId equals rl.Id
                            where Usr.UserId == ReportDetail.UserId
                            select new
                            {
                                RoleId = Usr.RoleId
                            }).FirstOrDefault();

            var CustomerDetail = (from Usr in dbContext.WMSUsers
                                  join UsrRl in dbContext.WMSUserRoles on Usr.Id equals UsrRl.UserId
                                  join UsrAdd in dbContext.WMSUserAdditionals on Usr.Id equals UsrAdd.UserId
                                  join UsrAd in dbContext.WMSUserAddresses on UsrAdd.UserId equals UsrAd.UserId
                                  join Merch in dbContext.WMSUsers on UsrAdd.MerchandiseUserId equals Merch.Id
                                  join Cntry in dbContext.Countries on UsrAd.CountryId equals Cntry.CountryId
                                  where Usr.IsActive == true && UsrRl.RoleId == (int)WMSUserRoleEnum.Customer
                                  select new CustomerReportModel
                                  {
                                      SalesCoordinator = UsrAdd.SalesCoOrdinatorId.HasValue ? UsrAdd.SalesCoOrdinatorId.Value : 0,
                                      MerchandiserId = UsrAdd.MerchandiseUserId.HasValue ? UsrAdd.MerchandiseUserId.Value : 0,
                                      WarehoiuseUser = UsrAdd.WarehouseUserId.HasValue ? UsrAdd.WarehouseUserId.Value : 0,

                                      AccountNumber = UsrAdd.AccountNumber,
                                      Company = Usr.CompanyName,
                                      ContactPerson = Usr.ContactFirstName + " " + Usr.ContactLastName,
                                      Country = Cntry.CountryName,
                                      Email = Usr.Email,
                                      Merchandiser = Merch.ContactFirstName + " " + Merch.ContactLastName
                                  }).ToList();


            // Show only relevant customers to users

            if (UserType.RoleId == (int)WMSUserRoleEnum.SalesCoOrdinator)
            {
                CustomerDetail = CustomerDetail.Where(p => p.SalesCoordinator == ReportDetail.UserId).ToList();
            }
            if (UserType.RoleId == (int)WMSUserRoleEnum.Merchandise)
            {
                CustomerDetail = CustomerDetail.Where(p => p.MerchandiserId == ReportDetail.UserId).ToList();
            }
            if (UserType.RoleId == (int)WMSUserRoleEnum.Warehouse)
            {
                CustomerDetail = CustomerDetail.Where(p => p.WarehoiuseUser == ReportDetail.UserId).ToList();
            }

            if (CustomerDetail.Count > 0)
            {
                return CustomerDetail;
            }
            else
            {
                return null;
            }
        }

        public Tuple<List<DynaStySportOrdersReportModel>, List<OrdersReportModel>> GetOrders(ReportTrackandTraceModel ReportDetail)
        {
            List<DynaStySportOrdersReportModel> OrderDetail = new List<DynaStySportOrdersReportModel>();
            List<OrdersReportModel> OrderDetail1 = new List<OrdersReportModel>();
            var UserType = (from Usr in dbContext.WMSUserRoles
                            join rl in dbContext.WMSRoles on Usr.RoleId equals rl.Id
                            where Usr.UserId == ReportDetail.UserId
                            select new
                            {
                                RoleId = Usr.RoleId
                            }).FirstOrDefault();

            if (UserType.RoleId == (int)WMSUserRoleEnum.Customer)
            {
                if (ReportDetail.FromDate != null && ReportDetail.ToDate != null)
                {
                    var colection = (from Odr in dbContext.Orders
                                     join UsrDsn in dbContext.OrderDesigns on Odr.Id equals UsrDsn.OrderId
                                     join OrdDes in dbContext.OrderDesignDetails on UsrDsn.Id equals OrdDes.OrderDesignId
                                     join OdrSt in dbContext.OrderStatus on Odr.OrderStatusId equals OdrSt.Id
                                     join Usr in dbContext.WMSUsers on Odr.CustomerId equals Usr.Id
                                     where Odr.CustomerId == ReportDetail.UserId
                                     && ReportDetail.FromDate <= Odr.CreatedOnUtc && ReportDetail.ToDate >= Odr.CreatedOnUtc && Odr.OrderStatusId != 1
                                     select new
                                     {
                                         SalesCoordinator = Odr.SalesCordinator.HasValue ? Odr.SalesCordinator.Value : 0,
                                         Merchandiser = Odr.Mechandiser.HasValue ? Odr.Mechandiser.Value : 0,
                                         WarehouseUser = Odr.WarehouseUserId.HasValue ? Odr.WarehouseUserId.Value : 0,
                                         CreatedOn = Odr.CreatedOnUtc,
                                         OrderName = Odr.OrderName,
                                         DesingCollection = UsrDsn.DesignName,
                                         OrderNumber = Odr.OrderNumber,
                                         OrderStatus = OdrSt.StatusDisplay,
                                         OrderType = Odr.OrderType,
                                         OrderId = Odr.Id,
                                         OrderDesignId = UsrDsn.Id,
                                         Quantity = OrdDes.Quantity
                                     }).ToList();
                    OrderDetail = colection.GroupBy(p => p.OrderId)
                                                .Select(group => new DynaStySportOrdersReportModel
                                                {
                                                    CreatedOn = group.FirstOrDefault().CreatedOn,
                                                    DesingCollection = group.FirstOrDefault().OrderName,
                                                    Merchandiser = group.FirstOrDefault().Merchandiser,
                                                    OrderNumber = group.FirstOrDefault().OrderNumber,
                                                    OrderStatus = group.FirstOrDefault().OrderStatus,
                                                    OrderType = group.FirstOrDefault().OrderType,
                                                    SalesCoordinator = group.FirstOrDefault().SalesCoordinator,
                                                    WarehouseUser = group.FirstOrDefault().WarehouseUser,
                                                    DesignName = group.FirstOrDefault().DesingCollection,
                                                    Quantity = group.Where(o => o.OrderId == group.Key).Select(ss => new { ss.Quantity }).Sum(l => l.Quantity)

                                                }).ToList();

                    foreach (var item in OrderDetail)
                    {
                        if (string.IsNullOrEmpty(item.DesingCollection))
                        {
                            item.DesingCollection = item.DesignName;
                        }
                    }
                }
                else
                {
                    var collection = (from Odr in dbContext.Orders
                                      join UsrDsn in dbContext.OrderDesigns on Odr.Id equals UsrDsn.OrderId
                                      join Odrdes in dbContext.OrderDesignDetails on UsrDsn.Id equals Odrdes.OrderDesignId
                                      join OdrSt in dbContext.OrderStatus on Odr.OrderStatusId equals OdrSt.Id
                                      join Usr in dbContext.WMSUsers on Odr.CustomerId equals Usr.Id
                                      where Odr.CustomerId == ReportDetail.UserId && Odr.OrderStatusId != 1
                                      select new
                                      {
                                          SalesCoordinator = Odr.SalesCordinator.HasValue ? Odr.SalesCordinator.Value : 0,
                                          Merchandiser = Odr.Mechandiser.HasValue ? Odr.Mechandiser.Value : 0,
                                          WarehouseUser = Odr.WarehouseUserId.HasValue ? Odr.WarehouseUserId.Value : 0,
                                          CreatedOn = Odr.CreatedOnUtc,
                                          OrderName = Odr.OrderName,
                                          DesingCollection = UsrDsn.DesignName,
                                          OrderNumber = Odr.OrderNumber,
                                          OrderStatus = OdrSt.StatusDisplay,
                                          OrderType = Odr.OrderType,
                                          OrderId = Odr.Id,
                                          OrderDesignId = UsrDsn.Id,
                                          Quantity = Odrdes.Quantity
                                      }).ToList();

                    OrderDetail = collection.GroupBy(p => p.OrderId)
                                                .Select(group => new DynaStySportOrdersReportModel
                                                {
                                                    CreatedOn = group.FirstOrDefault().CreatedOn,
                                                    DesingCollection = group.FirstOrDefault().OrderName,
                                                    Merchandiser = group.FirstOrDefault().Merchandiser,
                                                    OrderNumber = group.FirstOrDefault().OrderNumber,
                                                    OrderStatus = group.FirstOrDefault().OrderStatus,
                                                    OrderType = group.FirstOrDefault().OrderType,
                                                    SalesCoordinator = group.FirstOrDefault().SalesCoordinator,
                                                    WarehouseUser = group.FirstOrDefault().WarehouseUser,
                                                    DesignName = group.FirstOrDefault().DesingCollection,
                                                    Quantity = group.Where(o => o.OrderId == group.Key).Select(ss => new { ss.Quantity }).Sum(l => l.Quantity)

                                                }).ToList();
                    foreach (var item in OrderDetail)
                    {
                        if (string.IsNullOrEmpty(item.DesingCollection))
                        {
                            item.DesingCollection = item.DesignName;
                        }
                    }
                }
            }
            else
            {
                if (ReportDetail.CustomerId > 0)
                {
                    if (ReportDetail.FromDate != null && ReportDetail.ToDate != null)
                    {
                        var collection = (from Odr in dbContext.Orders
                                          join UsrDsn in dbContext.OrderDesigns on Odr.Id equals UsrDsn.OrderId
                                          join OrdDes in dbContext.OrderDesignDetails on UsrDsn.Id equals OrdDes.OrderDesignId
                                          join OdrSt in dbContext.OrderStatus on Odr.OrderStatusId equals OdrSt.Id
                                          join Usr in dbContext.WMSUsers on Odr.CustomerId equals Usr.Id
                                          where Odr.CustomerId == ReportDetail.CustomerId && Odr.OrderStatusId != 1
                                          && ReportDetail.FromDate <= Odr.CreatedOnUtc && ReportDetail.ToDate >= Odr.CreatedOnUtc
                                          select new
                                          {
                                              SalesCoordinator = Odr.SalesCordinator.HasValue ? Odr.SalesCordinator.Value : 0,
                                              Merchandiser = Odr.Mechandiser.HasValue ? Odr.Mechandiser.Value : 0,
                                              WarehouseUser = Odr.WarehouseUserId.HasValue ? Odr.WarehouseUserId.Value : 0,
                                              CreatedOn = Odr.CreatedOnUtc,
                                              DesingCollection = UsrDsn.DesignName,
                                              OrderNumber = Odr.OrderNumber,
                                              OrderStatus = OdrSt.StatusDisplay,
                                              OrderType = Odr.OrderType,
                                              OrderId = Odr.Id,
                                              OrderName = Odr.OrderName,
                                              OrderDesignId = UsrDsn.Id,
                                              Quantity = OrdDes.Quantity
                                          }).ToList();
                        OrderDetail = collection.GroupBy(p => p.OrderId)
                                               .Select(group => new DynaStySportOrdersReportModel
                                               {
                                                   CreatedOn = group.FirstOrDefault().CreatedOn,
                                                   DesingCollection = group.FirstOrDefault().OrderName,
                                                   Merchandiser = group.FirstOrDefault().Merchandiser,
                                                   OrderNumber = group.FirstOrDefault().OrderNumber,
                                                   OrderStatus = group.FirstOrDefault().OrderStatus,
                                                   OrderType = group.FirstOrDefault().OrderType,
                                                   SalesCoordinator = group.FirstOrDefault().SalesCoordinator,
                                                   WarehouseUser = group.FirstOrDefault().WarehouseUser,
                                                   DesignName = group.FirstOrDefault().DesingCollection,
                                                   Quantity = group.Where(o => o.OrderId == group.Key).Select(ss => new { ss.Quantity }).Sum(l => l.Quantity)
                                               }).ToList();


                        foreach (var item in OrderDetail)
                        {
                            if (string.IsNullOrEmpty(item.DesingCollection))
                            {
                                item.DesingCollection = item.DesignName;
                            }
                        }
                    }
                    else
                    {
                        var collection = (from Odr in dbContext.Orders
                                          join UsrDsn in dbContext.OrderDesigns on Odr.Id equals UsrDsn.OrderId
                                          join OrdDes in dbContext.OrderDesignDetails on UsrDsn.Id equals OrdDes.OrderDesignId
                                          join OdrSt in dbContext.OrderStatus on Odr.OrderStatusId equals OdrSt.Id
                                          join Usr in dbContext.WMSUsers on Odr.CustomerId equals Usr.Id
                                          where Odr.CustomerId == ReportDetail.CustomerId && Odr.OrderStatusId != 1
                                          select new
                                          {
                                              SalesCoordinator = Odr.SalesCordinator.HasValue ? Odr.SalesCordinator.Value : 0,
                                              Merchandiser = Odr.Mechandiser.HasValue ? Odr.Mechandiser.Value : 0,
                                              WarehouseUser = Odr.WarehouseUserId.HasValue ? Odr.WarehouseUserId.Value : 0,
                                              CreatedOn = Odr.CreatedOnUtc,
                                              OrderName = Odr.OrderName,
                                              DesingCollection = UsrDsn.DesignName,
                                              OrderNumber = Odr.OrderNumber,
                                              OrderStatus = OdrSt.StatusDisplay,
                                              OrderType = Odr.OrderType,
                                              OrderId = Odr.Id,
                                              OrderDesignId = UsrDsn.Id,
                                              Quantity = OrdDes.Quantity
                                          }).ToList();
                        OrderDetail = collection.GroupBy(p => p.OrderId)
                                               .Select(group => new DynaStySportOrdersReportModel
                                               {
                                                   CreatedOn = group.FirstOrDefault().CreatedOn,
                                                   DesingCollection = group.FirstOrDefault().OrderName,
                                                   Merchandiser = group.FirstOrDefault().Merchandiser,
                                                   OrderNumber = group.FirstOrDefault().OrderNumber,
                                                   OrderStatus = group.FirstOrDefault().OrderStatus,
                                                   OrderType = group.FirstOrDefault().OrderType,
                                                   SalesCoordinator = group.FirstOrDefault().SalesCoordinator,
                                                   WarehouseUser = group.FirstOrDefault().WarehouseUser,
                                                   DesignName = group.FirstOrDefault().DesingCollection,
                                                   Quantity = group.Where(o => o.OrderId == group.Key).Select(ss => new { ss.Quantity }).Sum(l => l.Quantity)


                                               }).ToList();
                        foreach (var item in OrderDetail)
                        {
                            if (string.IsNullOrEmpty(item.DesingCollection))
                            {
                                item.DesingCollection = item.DesignName;
                            }
                        }
                    }

                    if (UserType.RoleId == (int)WMSUserRoleEnum.SalesCoOrdinator)
                    {
                        OrderDetail = OrderDetail.Where(p => p.SalesCoordinator == ReportDetail.UserId).ToList();
                    }
                    if (UserType.RoleId == (int)WMSUserRoleEnum.Merchandise)
                    {
                        OrderDetail = OrderDetail.Where(p => p.Merchandiser == ReportDetail.UserId).ToList();
                    }
                    if (UserType.RoleId == (int)WMSUserRoleEnum.Warehouse)
                    {
                        OrderDetail = OrderDetail.Where(p => p.WarehouseUser == ReportDetail.UserId).ToList();
                    }


                }
                else
                {
                    if (ReportDetail.FromDate != null && ReportDetail.ToDate != null)
                    {
                        var collection = (from Odr in dbContext.Orders
                                          join UsrDsn in dbContext.OrderDesigns on Odr.Id equals UsrDsn.OrderId
                                          join OrdDes in dbContext.OrderDesignDetails on UsrDsn.Id equals OrdDes.OrderDesignId
                                          join OdrSt in dbContext.OrderStatus on Odr.OrderStatusId equals OdrSt.Id
                                          join Usr in dbContext.WMSUsers on Odr.CustomerId equals Usr.Id
                                          join UsrAdd in dbContext.WMSUserAdditionals on Usr.Id equals UsrAdd.UserId
                                          join UsrAd in dbContext.WMSUserAddresses on UsrAdd.UserId equals UsrAd.UserId
                                          join Cntry in dbContext.Countries on UsrAd.CountryId equals Cntry.CountryId
                                          where ReportDetail.FromDate <= Odr.CreatedOnUtc && ReportDetail.ToDate >= Odr.CreatedOnUtc && Odr.OrderStatusId != 1
                                          select new
                                          {
                                              SalesCoordinator = Odr.SalesCordinator.HasValue ? Odr.SalesCordinator.Value : 0,
                                              Merchandiser = Odr.Mechandiser.HasValue ? Odr.Mechandiser.Value : 0,
                                              WarehouseUser = Odr.WarehouseUserId.HasValue ? Odr.WarehouseUserId.Value : 0,
                                              AccountNumber = UsrAdd.AccountNumber,
                                              Company = Usr.CompanyName,
                                              Country = Cntry.CountryName,
                                              CreatedOn = Odr.CreatedOnUtc,
                                              DesingCollection = UsrDsn.DesignName,
                                              OrderNumber = Odr.OrderNumber,
                                              OrderStatus = OdrSt.StatusDisplay,
                                              OrderType = Odr.OrderType,
                                              OrderId = Odr.Id,
                                              OrderName = Odr.OrderName,
                                              OrderDesignId = UsrDsn.Id,
                                              Quantity = OrdDes.Quantity
                                          }).ToList();

                        OrderDetail1 = collection.GroupBy(p => p.OrderId)
                                               .Select(group => new OrdersReportModel
                                               {
                                                   SalesCoordinator = group.FirstOrDefault().SalesCoordinator, // Odr.SalesCordinator.HasValue ? Odr.SalesCordinator.Value : 0,
                                                   Merchandiser = group.FirstOrDefault().Merchandiser, // Odr.Mechandiser.HasValue ? Odr.Mechandiser.Value : 0,
                                                   WarehouseUser = group.FirstOrDefault().WarehouseUser, //  Odr.WarehouseUserId.HasValue ? Odr.WarehouseUserId.Value : 0,
                                                   AccountNumber = group.FirstOrDefault().AccountNumber, //  UsrAdd.AccountNumber,
                                                   Company = group.FirstOrDefault().Company, // Usr.CompanyName,
                                                   Country = group.FirstOrDefault().Country, // Cntry.CountryName,
                                                   CreatedOn = group.FirstOrDefault().CreatedOn, // Odr.CreatedOnUtc,
                                                   DesingCollection = group.FirstOrDefault().OrderName, // UsrDsn.DesignName,
                                                   OrderNumber = group.FirstOrDefault().OrderNumber, //  Odr.OrderNumber,
                                                   OrderStatus = group.FirstOrDefault().OrderStatus, //  OdrSt.StatusDisplay,
                                                   OrderType = group.FirstOrDefault().OrderType, // Odr.OrderType,
                                                   DesignName = group.FirstOrDefault().DesingCollection,
                                                   Quantity = group.Where(o => o.OrderId == group.Key).Select(ss => new { ss.Quantity }).Sum(l => l.Quantity)
                                               }).ToList();

                        foreach (var item in OrderDetail1)
                        {
                            if (string.IsNullOrEmpty(item.DesingCollection))
                            {
                                item.DesingCollection = item.DesignName;
                            }
                        }
                    }
                    else
                    {
                        var collection = (from Odr in dbContext.Orders
                                          join UsrDsn in dbContext.OrderDesigns on Odr.Id equals UsrDsn.OrderId
                                          join OrdDes in dbContext.OrderDesignDetails on UsrDsn.Id equals OrdDes.OrderDesignId
                                          join OdrSt in dbContext.OrderStatus on Odr.OrderStatusId equals OdrSt.Id
                                          join Usr in dbContext.WMSUsers on Odr.CustomerId equals Usr.Id
                                          join UsrAdd in dbContext.WMSUserAdditionals on Usr.Id equals UsrAdd.UserId
                                          join UsrAd in dbContext.WMSUserAddresses on UsrAdd.UserId equals UsrAd.UserId
                                          join Cntry in dbContext.Countries on UsrAd.CountryId equals Cntry.CountryId
                                          where Odr.OrderStatusId != 1
                                          select new
                                          {
                                              SalesCoordinator = Odr.SalesCordinator.HasValue ? Odr.SalesCordinator.Value : 0,
                                              Merchandiser = Odr.Mechandiser.HasValue ? Odr.Mechandiser.Value : 0,
                                              WarehouseUser = Odr.WarehouseUserId.HasValue ? Odr.WarehouseUserId.Value : 0,
                                              AccountNumber = UsrAdd.AccountNumber,
                                              Company = Usr.CompanyName,
                                              Country = Cntry.CountryName,
                                              CreatedOn = Odr.CreatedOnUtc,
                                              DesingCollection = UsrDsn.DesignName,
                                              OrderNumber = Odr.OrderNumber,
                                              OrderStatus = OdrSt.StatusDisplay,
                                              OrderType = Odr.OrderType,
                                              OrderId = Odr.Id,
                                              OrderName = Odr.OrderName,
                                              OrderDesignId = UsrDsn.Id,
                                              Quantity = OrdDes.Quantity
                                          }).ToList();


                        OrderDetail1 = collection.GroupBy(p => p.OrderId)
                                                 .Select(group => new OrdersReportModel
                                                 {
                                                     SalesCoordinator = group.FirstOrDefault().SalesCoordinator, // Odr.SalesCordinator.HasValue ? Odr.SalesCordinator.Value : 0,
                                                     Merchandiser = group.FirstOrDefault().Merchandiser, // Odr.Mechandiser.HasValue ? Odr.Mechandiser.Value : 0,
                                                     WarehouseUser = group.FirstOrDefault().WarehouseUser, //  Odr.WarehouseUserId.HasValue ? Odr.WarehouseUserId.Value : 0,
                                                     AccountNumber = group.FirstOrDefault().AccountNumber, //  UsrAdd.AccountNumber,
                                                     Company = group.FirstOrDefault().Company, // Usr.CompanyName,
                                                     Country = group.FirstOrDefault().Country, // Cntry.CountryName,
                                                     CreatedOn = group.FirstOrDefault().CreatedOn, // Odr.CreatedOnUtc,
                                                     DesingCollection = group.FirstOrDefault().OrderName, // UsrDsn.DesignName,
                                                     OrderNumber = group.FirstOrDefault().OrderNumber, //  Odr.OrderNumber,
                                                     OrderStatus = group.FirstOrDefault().OrderStatus, //  OdrSt.StatusDisplay,
                                                     OrderType = group.FirstOrDefault().OrderType, // Odr.OrderType,
                                                     DesignName = group.FirstOrDefault().DesingCollection,
                                                     Quantity = group.Where(o => o.OrderId == group.Key).Select(ss => new { ss.Quantity }).Sum(l => l.Quantity)
                                                 }).ToList();

                        foreach (var item in OrderDetail1)
                        {
                            if (string.IsNullOrEmpty(item.DesingCollection))
                            {
                                item.DesingCollection = item.DesignName;
                            }
                        }
                    }
                    if (UserType.RoleId == (int)WMSUserRoleEnum.SalesCoOrdinator)
                    {
                        OrderDetail1 = OrderDetail1.Where(p => p.SalesCoordinator == ReportDetail.UserId).ToList();
                    }
                    if (UserType.RoleId == (int)WMSUserRoleEnum.Merchandise)
                    {
                        OrderDetail1 = OrderDetail1.Where(p => p.Merchandiser == ReportDetail.UserId).ToList();
                    }
                    if (UserType.RoleId == (int)WMSUserRoleEnum.Warehouse)
                    {
                        OrderDetail1 = OrderDetail1.Where(p => p.WarehouseUser == ReportDetail.UserId).ToList();
                    }
                }
            }
            return Tuple.Create(OrderDetail, OrderDetail1);
        }

        public Tuple<List<CustomerModel>, decimal, int> GetStockReport(ReportTrackandTraceModel ReportDetail)
        {
            List<CustomerModel> lstcustomerModels = new List<CustomerModel>();
            CustomerModel objCustomerModel;
            var _lstCustomers = dbContext.ProductCatagories.Select(a => a.CustomerId).Distinct().ToList();
            int TotalSize = 12;
            decimal NetQuantity = 0;

            foreach (var objCust in _lstCustomers)
            {
                objCustomerModel = new CustomerModel();
                objCustomerModel.HeaderName = new HeaderModel();

                string CustomerName = new CustomerRepository().GetUser(objCust).CompanyName;
                string StyleNo = (from Pc in dbContext.ProductCatagories
                                  join PM in dbContext.ProductMasters on Pc.Id equals PM.ProductCatagoryId
                                  where Pc.CustomerId == objCust
                                  select PM).FirstOrDefault().ProductCode;

                string Range = dbContext.ProductCatagories.Where(a => a.CustomerId == objCust).FirstOrDefault().CatagoryDisplay;

                var hlist = GetHeader(objCust, TotalSize);

                objCustomerModel.HeaderName = hlist.Item1;
                objCustomerModel.valuesCollection = new List<ValuesModel>();
                objCustomerModel.valuesCollection = GetQuantityforSizeValues(objCust, hlist.Item2, hlist.Item3, CustomerName, StyleNo, Range, TotalSize);

                NetQuantity = NetQuantity + objCustomerModel.valuesCollection.Sum(a => a.Total); //GetSum(objCustomerModel.valuesCollection);

                lstcustomerModels.Add(objCustomerModel);
            }
            return Tuple.Create(lstcustomerModels, NetQuantity, TotalSize);
        }

        private decimal GetSum(List<ValuesModel> valuesCollection)
        {
            decimal Total = 0;
            foreach (var qy in valuesCollection)
            {
                Total += qy.Total;
            }
            return Total;
        }

        public List<ValuesModel> GetQuantityforSizeValues(int CustomerId, List<HeaderSize> lstSize, List<CustomerColor> lstCustomerColor, string CustomerName, string StyleNo, string Range, int TotalSize)
        {
            List<ValuesModel> _lstValues = new List<ValuesModel>();
            ValuesModel hm;
            ValuesModel hm1;

            var lstorderDesign = dbContext.spGetOrderList().ToList();

            #region Get Row Values

            foreach (var clr in lstCustomerColor)
            {
                #region Get Caption Values

                hm = new ValuesModel();
                hm.Customer = CustomerName;
                hm.Range = Range;
                hm.StyleNo = StyleNo;
                hm.Color = clr.ColorName;
                hm.DataType = "Current Available";
                hm.HeadersValue = new List<decimal>();

                hm1 = new ValuesModel();
                hm1.Customer = CustomerName;
                hm1.Range = Range;
                hm1.StyleNo = StyleNo;
                hm1.Color = clr.ColorName;
                hm1.DataType = "In Production";
                hm1.HeadersValue = new List<decimal>();

                #endregion

                var CustDetail = dbContext.spGetStockHeader(CustomerId, null, null).ToList();

                #region "Get Second Row Values"

                foreach (var sz in lstSize)
                {
                    int qty1 = 0;
                    int qty = 0;
                    int ProductSkuID = 0;
                    var objProductSku = CustDetail.Where(p => p.ColorId == clr.ColorId && p.SizeId == sz.SizeId).FirstOrDefault();
                    if (objProductSku != null)
                        ProductSkuID = objProductSku.ProductSKUId;
                    if (ProductSkuID > 0)
                    {
                        var obj1 = lstorderDesign.Where(p => p.ProductSKUId == ProductSkuID).FirstOrDefault();
                        if (obj1 != null)
                            qty1 = obj1.Quantity.Value;
                    }

                    hm1.HeadersValue.Add(qty1);

                    var obj = CustDetail.Where(p => p.ColorId == clr.ColorId && p.SizeId == sz.SizeId).FirstOrDefault();
                    if (obj != null)
                        qty = obj.ActualQuantity.Value;

                    if (qty1 > 0 && qty > qty1)
                        hm.HeadersValue.Add(qty - qty1);
                    else
                        hm.HeadersValue.Add(qty);
                }

                #endregion

                #region Get Total 11 Header Values

                for (int i = 1; i <= TotalSize - hm.HeadersValue.Count; i++)
                {
                    hm.HeadersValue.Add(0);
                }

                for (int i = 1; i <= TotalSize - hm1.HeadersValue.Count; i++)
                {
                    hm1.HeadersValue.Add(0);
                }

                #endregion
                #region Add into list

                hm.Total = GetTotal(hm.HeadersValue);
                _lstValues.Add(hm);
                hm1.Total = GetTotal(hm1.HeadersValue);
                _lstValues.Add(hm1);

                #endregion
            }

            #endregion

            return _lstValues;
        }

        private decimal GetTotal(List<decimal> lst)
        {
            decimal sum = 0;
            foreach (decimal d in lst)
            {
                sum += d;
            }
            return sum;
        }

        private Tuple<HeaderModel, List<HeaderSize>, List<CustomerColor>> GetHeader(int CustomerId, int TotalSize)
        {
            HeaderModel hm = new HeaderModel();

            #region "HeadersName"

            hm.Customer = "Customer";
            hm.Color = "Color";
            hm.DataType = "DataType";
            hm.Range = "Range";
            hm.StyleNo = "Style No";
            hm.Total = "Total";

            #endregion

            hm.Headers = new List<string>();

            #region "HeadersValues"

            var CustDetail = dbContext.spGetStockHeader(CustomerId, null, null).ToList();
            var _lstSize = CustDetail.Select(a => new { a.SizeId, a.SizeName }).Distinct().OrderBy(c => c.SizeId).ToList();
            var _lstColor = CustDetail.Select(a => new { a.ColorId, a.Color }).Distinct().OrderBy(c => c.ColorId).ToList();

            List<HeaderSize> _lstHeaderSize = new List<HeaderSize>();
            List<CustomerColor> _lstCustomerColor = new List<CustomerColor>();

            foreach (var objsize in _lstSize)
            {
                _lstHeaderSize.Add(new HeaderSize { SizeId = objsize.SizeId, SizeHeader = objsize.SizeName });
            }

            foreach (var objsize in _lstColor)
            {
                _lstCustomerColor.Add(new CustomerColor { ColorId = objsize.ColorId, ColorName = objsize.Color });
            }

            foreach (var objsize in _lstHeaderSize)
            {
                hm.Headers.Add(objsize.SizeHeader);
            }

            #region Get Total 11 Header Values

            for (int i = 1; i <= TotalSize - hm.Headers.Count; i++)
            {
                hm.Headers.Add("");
            }

            #endregion

            #endregion

            return Tuple.Create(hm, _lstHeaderSize, _lstCustomerColor);
        }

        public string GenerateExcel(List<CustomerModel> _lstCust, decimal NetQuantity, int TotalSize, string path)
        {
            //Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            //if (xlApp == null)
            //{
            //    return null;
            //}

            //Excel.Workbook xlWorkBook;
            //Excel.Worksheet xlWorkSheet;
            //object misValue = System.Reflection.Missing.Value;

            //xlWorkBook = xlApp.Workbooks.Add(misValue);
            //xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            //int rownum = 1;
            //int col = 1;

            //xlWorkSheet.Range[xlWorkSheet.Cells[rownum, 1], xlWorkSheet.Cells[rownum, 3]].Merge();
            //xlWorkSheet.Cells[rownum, col].Font.Bold = true;
            //xlWorkSheet.Cells[rownum, col].Font.Size = 24;
            //xlWorkSheet.Cells[rownum, col] = "Stock" + System.DateTime.Today.Date.ToString("yyyy-MM-dd");

            //rownum++;
            //rownum++;

            //foreach (var objCust in _lstCust)
            //{
            //    col = 1;

            //    xlWorkSheet.Cells[rownum, col].Font.Bold = true;
            //    xlWorkSheet.Cells[rownum, col].ColumnWidth = 17;
            //    xlWorkSheet.Cells[rownum, col++] = objCust.HeaderName.Customer;

            //    xlWorkSheet.Cells[rownum, col].Font.Bold = true;
            //    xlWorkSheet.Cells[rownum, col].ColumnWidth = 30;
            //    xlWorkSheet.Cells[rownum, col++] = objCust.HeaderName.Range;

            //    xlWorkSheet.Cells[rownum, col].Font.Bold = true;
            //    xlWorkSheet.Cells[rownum, col].ColumnWidth = 12;
            //    xlWorkSheet.Cells[rownum, col++] = objCust.HeaderName.StyleNo;

            //    xlWorkSheet.Cells[rownum, col].Font.Bold = true;
            //    xlWorkSheet.Cells[rownum, col].ColumnWidth = 12;
            //    xlWorkSheet.Cells[rownum, col++] = objCust.HeaderName.Color;

            //    foreach (var val in objCust.HeaderName.Headers)
            //    {
            //        xlWorkSheet.Cells[rownum, col].Font.Bold = true;
            //        xlWorkSheet.Cells[rownum, col].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Yellow);
            //        xlWorkSheet.Cells[rownum, col++] = val;
            //    }

            //    xlWorkSheet.Cells[rownum, col].Font.Bold = true;
            //    xlWorkSheet.Cells[rownum, col++] = objCust.HeaderName.Total;
            //    xlWorkSheet.Cells[rownum, col].Font.Bold = true;
            //    xlWorkSheet.Cells[rownum, col].ColumnWidth = 17;
            //    xlWorkSheet.Cells[rownum, col++] = objCust.HeaderName.DataType;

            //    rownum++;

            //    foreach (var rec in objCust.valuesCollection)
            //    {
            //        col = 1;

            //        xlWorkSheet.Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

            //        xlWorkSheet.Cells[rownum, col++] = rec.Customer;
            //        xlWorkSheet.Cells[rownum, col++] = rec.Range;
            //        xlWorkSheet.Cells[rownum, col++] = rec.StyleNo;
            //        xlWorkSheet.Cells[rownum, col++] = rec.Color;

            //        foreach (var val in rec.HeadersValue)
            //        {
            //            xlWorkSheet.Cells[rownum, col].ColumnWidth = 7;
            //            xlWorkSheet.Cells[rownum, col++] = val;
            //        }

            //        xlWorkSheet.Cells[rownum, col++] = rec.Total;
            //        xlWorkSheet.Cells[rownum, col++] = rec.DataType;

            //        rownum++;
            //    }

            //    if (_lstCust.LastOrDefault() != objCust)
            //        xlWorkSheet.Range[xlWorkSheet.Cells[rownum, 1], xlWorkSheet.Cells[rownum, 20]].Merge();

            //    rownum++;
            //}

            //xlWorkSheet.Cells[rownum + 1, 4 + TotalSize].Font.Bold = true;
            //xlWorkSheet.Cells[rownum + 1, 4 + TotalSize] = "Total";
            //xlWorkSheet.Cells[rownum + 1, 5 + TotalSize].Font.Bold = true;
            //xlWorkSheet.Cells[rownum + 1, 5 + TotalSize] = NetQuantity;

            ////string FilePath = @"D:\StockReport.xls";
            ////string FilePath = @"D:\WMSAPIProject\WMS.Api\Files\Reports\AllSKU Report.xls";
            string FilePath = path;

            //if (File.Exists(FilePath))
            //{
            //    File.Delete(FilePath);
            //}

            //xlWorkBook.SaveAs(FilePath, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            //xlWorkBook.Close(true, misValue, misValue);
            //xlApp.Quit();

            //Marshal.ReleaseComObject(xlWorkSheet);
            //Marshal.ReleaseComObject(xlWorkBook);
            //Marshal.ReleaseComObject(xlApp);

            return FilePath;
        }

        #endregion

        #region Rohit Code
        public List<DailyDispatchReportModel> GetDailyDispatchReport(ReportTrackandTraceModel ReportDetail)
        {
            List<DailyDispatchReportModel> _lstdetails = new List<DailyDispatchReportModel>();
            List<DailyDispatchReportModel> _lstdetails2 = new List<DailyDispatchReportModel>();


            _lstdetails = (from Ord in dbContext.Orders
                           join Ordg in dbContext.OrderDesigns on Ord.Id equals Ordg.OrderId
                           join Odt in dbContext.OrderDesignDetails on Ordg.Id equals Odt.OrderDesignId
                           join Dlm in dbContext.DelivieryMethods on Ord.DelivieryMethodId equals Dlm.Id
                           join Psku in dbContext.ProductSKUs on Odt.ProductSKUId equals Psku.Id
                           join Ost in dbContext.OrderStatus on Ord.OrderStatusId equals Ost.Id
                           where Ord.CustomerId == ReportDetail.UserId && Ord.OrderStatusId == (byte)WMSOrderStatusEnum.Shipped && EntityFunctions.TruncateTime(Ord.DispatchedOnUtc) == EntityFunctions.TruncateTime(ReportDetail.FromDate)
                           select new DailyDispatchReportModel
                           {
                               OrderNumber = Ord.OrderNumber,
                               OrderCreatedOn = Ord.CreatedOnUtc,
                               DeliveryType = Ord.OrderType,
                               ShippingMethod = Dlm.DelivieryNameDisplay,
                               ActualQuantity = Psku.ActualQuantity,
                               Unit = Psku.WeightUnit,
                               Weight = Psku.Weight,
                               Quantity = Odt.Quantity,
                           }).ToList();

            var group = _lstdetails.GroupBy(p => new { p.OrderNumber })
                                .Select(x => new
                                {
                                    x.Key.OrderNumber,
                                    x.FirstOrDefault().OrderCreatedOn,
                                    x.FirstOrDefault().ShippingMethod,
                                    x.FirstOrDefault().DeliveryType,
                                    detail = x.Select(h => new
                                    {
                                        Pieces = h.Quantity,
                                        GrossWeight = h.Quantity > 0 ? (h.Unit == WMSOrdersUnit.Unit2 ? Math.Round(h.Weight * h.Quantity / 1000, 2) : Math.Round(h.Weight * h.Quantity, 2)) : 0,
                                        NetWeight = h.Quantity > 0 ? (h.Unit == WMSOrdersUnit.Unit2 ? Math.Round(h.Weight * h.Quantity / 1000, 2) : Math.Round(h.Weight * h.Quantity, 2)) : 0,
                                        //GrossWeight = (h.Weight * h.ActualQuantity),
                                        //NetWeight = (h.Weight * h.ActualQuantity)
                                    }).ToList()
                                }).ToList();

            int srno = 1;
            foreach (var ware in group)
            {
                DailyDispatchReportModel Report = new DailyDispatchReportModel();
                Report.OrderNumber = ware.OrderNumber;
                Report.OrderCreatedOn = ware.OrderCreatedOn;
                Report.ShippingMethod = ware.ShippingMethod;
                if (ware.DeliveryType == WMSOrderDeliveryTypeEnum.StandardDelivery)
                {
                    Report.DeliveryType = WMSOrderDeliveryTypeEnum.StandardDeliveryDisplay;
                }
                else if (ware.DeliveryType == WMSOrderDeliveryTypeEnum.UrgentDelivery)
                {
                    Report.DeliveryType = WMSOrderDeliveryTypeEnum.UrgentDeliveryDisplay;
                }
                else if (ware.DeliveryType == WMSOrderDeliveryTypeEnum.ExpressDelivery)
                {
                    Report.DeliveryType = WMSOrderDeliveryTypeEnum.ExpressDeliveryDisplay;
                }
                else
                {
                    Report.DeliveryType = ware.DeliveryType;
                }
                foreach (var dd in ware.detail)
                {
                    Report.Pieces += dd.Pieces;
                    Report.TotalGrossWeight += Convert.ToDecimal(dd.GrossWeight);
                    Report.TotalNetWeight += Convert.ToDecimal(dd.NetWeight);

                }
                _lstdetails2.Add(Report);
                srno++;
            }

            if (_lstdetails2.Count > 0)
            {
                return _lstdetails2;
            }
            else
            {
                return null;
            }
        }

        public List<TotalDailyDispatchReportModel> GetTotalDailyDispatchReport(ReportTrackandTraceModel ReportDetail)
        {
            List<TotalDailyDispatchReportModel> _lstdetails = new List<TotalDailyDispatchReportModel>();
            List<TotalDailyDispatchReportModel> _lstdetailss = new List<TotalDailyDispatchReportModel>();

            var UserType = (from Usr in dbContext.WMSUserRoles
                            join rl in dbContext.WMSRoles on Usr.RoleId equals rl.Id
                            where Usr.UserId == ReportDetail.UserId
                            select new
                            {
                                RoleId = Usr.RoleId
                            }).FirstOrDefault();

            if (ReportDetail.CustomerId != 0)
            {
                _lstdetails = (from Ord in dbContext.Orders
                               join Ordg in dbContext.OrderDesigns on Ord.Id equals Ordg.OrderId
                               join Odt in dbContext.OrderDesignDetails on Ordg.Id equals Odt.OrderDesignId
                               join Dlm in dbContext.DelivieryMethods on Ord.DelivieryMethodId equals Dlm.Id
                               join Psku in dbContext.ProductSKUs on Odt.ProductSKUId equals Psku.Id
                               join Ost in dbContext.OrderStatus on Ord.OrderStatusId equals Ost.Id
                               join WMSUse in dbContext.WMSUsers on Ord.CustomerId equals WMSUse.Id
                               join WMSAdd in dbContext.WMSUserAdditionals on WMSUse.Id equals WMSAdd.UserId
                               where Ord.CustomerId == ReportDetail.CustomerId && Ord.OrderStatusId == (byte)WMSOrderStatusEnum.Shipped && EntityFunctions.TruncateTime(Ord.DispatchedOnUtc) == EntityFunctions.TruncateTime(ReportDetail.FromDate)
                               select new TotalDailyDispatchReportModel
                               {
                                   Salescordinator = Ord.SalesCordinator.HasValue ? Ord.SalesCordinator.Value : 0,
                                   Merchandiser = Ord.Mechandiser.HasValue ? Ord.Mechandiser.Value : 0,
                                   WarehouseUser = Ord.WarehouseUserId.HasValue ? Ord.WarehouseUserId.Value : 0,
                                   CustomerName = WMSUse.CompanyName,
                                   AccountNumber = WMSAdd.AccountNumber,
                                   OrderNumber = Ord.OrderNumber,
                                   OrderCreatedOn = Ord.CreatedOnUtc,
                                   DeliveryType = Ord.OrderType,
                                   ShippingMethod = Dlm.DelivieryNameDisplay,
                                   ActualQuantity = Psku.ActualQuantity,
                                   Unit = Psku.WeightUnit,
                                   Weight = Psku.Weight,
                                   //Weight = Psku.WeightUnit == WMSOrdersUnit.Unit2 ? Psku.Weight / 1000 : Psku.Weight,
                                   //Weight = Psku.ActualQuantity.Value > 0 ? (Psku.WeightUnit == WMSOrdersUnit.Unit2 ? Math.Round(Psku.Weight * Psku.ActualQuantity.Value / 1000, 2) : Psku.Weight) : 0,
                                   //ProductSKUId = Odt.ProductSKUId,
                                   Quantity = Odt.Quantity,
                               }).ToList();


                if (UserType.RoleId == (int)WMSUserRoleEnum.SalesCoOrdinator)
                {
                    _lstdetails = _lstdetails.Where(p => p.Salescordinator == ReportDetail.UserId).ToList();
                }
                if (UserType.RoleId == (int)WMSUserRoleEnum.Merchandise)
                {
                    _lstdetails = _lstdetails.Where(p => p.Merchandiser == ReportDetail.UserId).ToList();
                }
                if (UserType.RoleId == (int)WMSUserRoleEnum.Warehouse)
                {
                    _lstdetails = _lstdetails.Where(p => p.WarehouseUser == ReportDetail.UserId).ToList();
                }

                var group = _lstdetails.GroupBy(p => new { p.OrderNumber })
                                    .Select(x => new
                                    {
                                        x.Key.OrderNumber,
                                        x.FirstOrDefault().OrderCreatedOn,
                                        x.FirstOrDefault().ShippingMethod,
                                        x.FirstOrDefault().DeliveryType,
                                        x.FirstOrDefault().CustomerName,
                                        x.FirstOrDefault().AccountNumber,
                                        //x.Key.Unit,
                                        detail = x.Select(h => new
                                        {
                                            Pieces = h.Quantity,
                                            GrossWeight = h.Quantity > 0 ? (h.Unit == WMSOrdersUnit.Unit2 ? Math.Round(h.Weight * h.Quantity / 1000, 2) : Math.Round(h.Weight * h.Quantity, 2)) : 0,
                                            NetWeight = h.Quantity > 0 ? (h.Unit == WMSOrdersUnit.Unit2 ? Math.Round(h.Weight * h.Quantity / 1000, 2) : Math.Round(h.Weight * h.Quantity, 2)) : 0,
                                            //GrossWeight = (h.Weight * h.ActualQuantity),
                                            //NetWeight = (h.Weight * h.ActualQuantity),
                                            //GrossWeight = h.Weight,
                                            //NetWeight = h.Weight,
                                        }).ToList()
                                    }).ToList();

                int srno = 1;
                foreach (var ware in group)
                {
                    TotalDailyDispatchReportModel Report = new TotalDailyDispatchReportModel();
                    Report.OrderNumber = ware.OrderNumber;
                    Report.OrderCreatedOn = ware.OrderCreatedOn;
                    Report.ShippingMethod = ware.ShippingMethod;
                    Report.AccountNumber = ware.AccountNumber;
                    Report.CustomerName = ware.CustomerName;

                    //Report.DeliveryType = ware.DeliveryType;
                    if (ware.DeliveryType == WMSOrderDeliveryTypeEnum.StandardDelivery)
                    {
                        Report.DeliveryType = WMSOrderDeliveryTypeEnum.StandardDeliveryDisplay;
                    }
                    else if (ware.DeliveryType == WMSOrderDeliveryTypeEnum.UrgentDelivery)
                    {
                        Report.DeliveryType = WMSOrderDeliveryTypeEnum.UrgentDeliveryDisplay;
                    }
                    else if (ware.DeliveryType == WMSOrderDeliveryTypeEnum.ExpressDelivery)
                    {
                        Report.DeliveryType = WMSOrderDeliveryTypeEnum.ExpressDeliveryDisplay;
                    }
                    else
                    {
                        Report.DeliveryType = ware.DeliveryType;
                    }
                    foreach (var dd in ware.detail)
                    {

                        Report.Pieces += dd.Pieces;
                        Report.TotalGrossWeight += Convert.ToDecimal(dd.GrossWeight);
                        Report.TotalNetWeight += Convert.ToDecimal(dd.NetWeight);

                    }
                    _lstdetailss.Add(Report);
                    srno++;
                }
            }
            else
            {
                _lstdetails = (from Ord in dbContext.Orders
                               join Ordg in dbContext.OrderDesigns on Ord.Id equals Ordg.OrderId
                               join Odt in dbContext.OrderDesignDetails on Ordg.Id equals Odt.OrderDesignId
                               join Dlm in dbContext.DelivieryMethods on Ord.DelivieryMethodId equals Dlm.Id
                               join Psku in dbContext.ProductSKUs on Odt.ProductSKUId equals Psku.Id
                               join Ost in dbContext.OrderStatus on Ord.OrderStatusId equals Ost.Id
                               join WMSUse in dbContext.WMSUsers on Ord.CustomerId equals WMSUse.Id
                               join WMSAdd in dbContext.WMSUserAdditionals on WMSUse.Id equals WMSAdd.UserId
                               where Ord.OrderStatusId == (byte)WMSOrderStatusEnum.Shipped && EntityFunctions.TruncateTime(Ord.DispatchedOnUtc) == EntityFunctions.TruncateTime(ReportDetail.FromDate)
                               select new TotalDailyDispatchReportModel
                               {
                                   Salescordinator = Ord.SalesCordinator.HasValue ? Ord.SalesCordinator.Value : 0,
                                   Merchandiser = Ord.Mechandiser.HasValue ? Ord.Mechandiser.Value : 0,
                                   WarehouseUser = Ord.WarehouseUserId.HasValue ? Ord.WarehouseUserId.Value : 0,
                                   CustomerName = WMSUse.CompanyName,
                                   AccountNumber = WMSAdd.AccountNumber,
                                   OrderNumber = Ord.OrderNumber,
                                   OrderCreatedOn = Ord.CreatedOnUtc,
                                   DeliveryType = Ord.OrderType,
                                   ShippingMethod = Dlm.DelivieryNameDisplay,
                                   Unit = Psku.WeightUnit,
                                   Weight = Psku.Weight,
                                   //Weight = Psku.ActualQuantity.Value > 0 ? (Psku.WeightUnit == WMSOrdersUnit.Unit2 ? Math.Round(Psku.Weight * Psku.ActualQuantity.Value / 1000, 2) : Psku.Weight) : 0,
                                   ActualQuantity = Psku.ActualQuantity,
                                   Quantity = Odt.Quantity,
                               }).ToList();


                if (UserType.RoleId == (int)WMSUserRoleEnum.SalesCoOrdinator)
                {
                    _lstdetails = _lstdetails.Where(p => p.Salescordinator == ReportDetail.UserId).ToList();
                }
                if (UserType.RoleId == (int)WMSUserRoleEnum.Merchandise)
                {
                    _lstdetails = _lstdetails.Where(p => p.Merchandiser == ReportDetail.UserId).ToList();
                }
                if (UserType.RoleId == (int)WMSUserRoleEnum.Warehouse)
                {
                    _lstdetails = _lstdetails.Where(p => p.WarehouseUser == ReportDetail.UserId).ToList();
                }


                var group = _lstdetails.GroupBy(p => new { p.OrderNumber })
                                    .Select(x => new
                                    {
                                        x.FirstOrDefault().AccountNumber,
                                        x.FirstOrDefault().CustomerName,
                                        x.FirstOrDefault().OrderNumber,
                                        x.FirstOrDefault().OrderCreatedOn,
                                        x.FirstOrDefault().ShippingMethod,
                                        x.FirstOrDefault().DeliveryType,
                                        detail = x.Select(h => new
                                        {
                                            Pieces = h.Quantity,
                                            GrossWeight = h.Quantity > 0 ? (h.Unit == WMSOrdersUnit.Unit2 ? Math.Round(h.Weight * h.Quantity / 1000, 2) : Math.Round(h.Weight * h.Quantity, 2)) : 0,
                                            NetWeight = h.Quantity > 0 ? (h.Unit == WMSOrdersUnit.Unit2 ? Math.Round(h.Weight * h.Quantity / 1000, 2) : Math.Round(h.Weight * h.Quantity, 2)) : 0,
                                            //GrossWeight = (h.Weight * h.ActualQuantity),
                                            //NetWeight = (h.Weight * h.ActualQuantity)
                                            //GrossWeight = h.Weight,
                                            //NetWeight = h.Weight
                                        }).ToList()
                                    }).ToList();

                int srno = 1;
                foreach (var ware in group)
                {
                    TotalDailyDispatchReportModel Report = new TotalDailyDispatchReportModel();
                    Report.OrderNumber = ware.OrderNumber;
                    Report.OrderCreatedOn = ware.OrderCreatedOn;
                    Report.ShippingMethod = ware.ShippingMethod;
                    Report.CustomerName = ware.CustomerName;
                    Report.AccountNumber = ware.AccountNumber;
                    //Report.DeliveryType = ware.DeliveryType;
                    if (ware.DeliveryType == WMSOrderDeliveryTypeEnum.StandardDelivery)
                    {
                        Report.DeliveryType = WMSOrderDeliveryTypeEnum.StandardDeliveryDisplay;
                    }
                    else if (ware.DeliveryType == WMSOrderDeliveryTypeEnum.UrgentDelivery)
                    {
                        Report.DeliveryType = WMSOrderDeliveryTypeEnum.UrgentDeliveryDisplay;
                    }
                    else if (ware.DeliveryType == WMSOrderDeliveryTypeEnum.ExpressDelivery)
                    {
                        Report.DeliveryType = WMSOrderDeliveryTypeEnum.ExpressDeliveryDisplay;
                    }
                    else
                    {
                        Report.DeliveryType = ware.DeliveryType;
                    }
                    foreach (var dd in ware.detail)
                    {
                        Report.Pieces += dd.Pieces;
                        Report.TotalGrossWeight += Convert.ToDecimal(dd.GrossWeight);
                        Report.TotalNetWeight += Convert.ToDecimal(dd.NetWeight);

                    }
                    _lstdetailss.Add(Report);
                    srno++;
                }
            }



            if (_lstdetailss.Count > 0)
            {
                return _lstdetailss;
            }
            else
            {
                return null;
            }
        }

        public List<OrderPendingDispatchReportModel> GetOrdersPendingDispatchReport(ReportTrackandTraceModel ReportDetail)
        {
            List<OrderPendingDispatchReportModel> _lstdetails = new List<OrderPendingDispatchReportModel>();
            List<OrderPendingDispatchReportModel> _lstdetailss = new List<OrderPendingDispatchReportModel>();
            var UserType = (from Usr in dbContext.WMSUserRoles
                            join rl in dbContext.WMSRoles on Usr.RoleId equals rl.Id
                            where Usr.UserId == ReportDetail.UserId
                            select new
                            {
                                RoleId = Usr.RoleId
                            }).FirstOrDefault();



            if (UserType.RoleId == (int)WMSUserRoleEnum.Customer)
            {
                ReportDetail.CustomerId = ReportDetail.UserId;
                if (ReportDetail.CustomerId != 0)
                {
                    _lstdetails = (from Ord in dbContext.Orders
                                   join Ordg in dbContext.OrderDesigns on Ord.Id equals Ordg.OrderId
                                   join Odt in dbContext.OrderDesignDetails on Ordg.Id equals Odt.OrderDesignId
                                   join Dlm in dbContext.DelivieryMethods on Ord.DelivieryMethodId equals Dlm.Id
                                   join Psku in dbContext.ProductSKUs on Odt.ProductSKUId equals Psku.Id
                                   join Ost in dbContext.OrderStatus on Ord.OrderStatusId equals Ost.Id
                                   join WMSUse in dbContext.WMSUsers on Ord.CustomerId equals WMSUse.Id
                                   join WMSAdd in dbContext.WMSUserAdditionals on WMSUse.Id equals WMSAdd.UserId
                                   where Ord.CustomerId == ReportDetail.UserId && Ord.OrderStatusId == (byte)WMSOrderStatusEnum.ReadyForDispatch
                                   select new OrderPendingDispatchReportModel
                                   {
                                       SalesCordinator = Ord.SalesCordinator.HasValue ? Ord.SalesCordinator.Value : 0,
                                       Merchandiser = Ord.Mechandiser.HasValue ? Ord.Mechandiser.Value : 0,
                                       WarehouseUser = Ord.WarehouseUserId.HasValue ? Ord.WarehouseUserId.Value : 0,
                                       CustomerName = WMSUse.CompanyName,
                                       AccountNumber = WMSAdd.AccountNumber,
                                       OrderNumber = Ord.OrderNumber,
                                       OrderCreatedOn = Ord.CreatedOnUtc,
                                       DeliveryType = Ord.OrderType,
                                       ShippingMethod = Dlm.DelivieryNameDisplay,
                                       ActualQuantity = Psku.ActualQuantity,
                                       Unit = Psku.WeightUnit,
                                       Weight = Psku.Weight,
                                       //Weight = Psku.WeightUnit == WMSOrdersUnit.Unit2 ? Psku.Weight / 1000 : Psku.Weight,
                                       //Weight = Psku.ActualQuantity.Value > 0 ? (Psku.WeightUnit == WMSOrdersUnit.Unit2 ? Math.Round(Psku.Weight * Psku.ActualQuantity.Value / 1000, 2) : Psku.Weight) : 0,
                                       Quantity = Odt.Quantity,
                                   }).ToList();

                    if (UserType.RoleId == (int)WMSUserRoleEnum.SalesCoOrdinator)
                    {
                        _lstdetails = _lstdetails.Where(p => p.SalesCordinator == ReportDetail.UserId).ToList();
                    }
                    if (UserType.RoleId == (int)WMSUserRoleEnum.Merchandise)
                    {
                        _lstdetails = _lstdetails.Where(p => p.Merchandiser == ReportDetail.UserId).ToList();
                    }

                    if (UserType.RoleId == (int)WMSUserRoleEnum.Warehouse)
                    {
                        _lstdetails = _lstdetails.Where(p => p.WarehouseUser == ReportDetail.UserId).ToList();
                    }

                    var group = _lstdetails.GroupBy(p => new { p.OrderNumber })
                                        .Select(x => new
                                        {
                                            x.Key.OrderNumber,
                                            x.FirstOrDefault().OrderCreatedOn,
                                            x.FirstOrDefault().ShippingMethod,
                                            x.FirstOrDefault().DeliveryType,
                                            x.FirstOrDefault().CustomerName,
                                            x.FirstOrDefault().AccountNumber,
                                            detail = x.Select(h => new
                                            {
                                                Pieces = h.Quantity,
                                                GrossWeight = h.Quantity > 0 ? (h.Unit == WMSOrdersUnit.Unit2 ? Math.Round(h.Weight * h.Quantity / 1000, 2) : Math.Round(h.Weight * h.Quantity, 2)) : 0,
                                                NetWeight = h.Quantity > 0 ? (h.Unit == WMSOrdersUnit.Unit2 ? Math.Round(h.Weight * h.Quantity / 1000, 2) : Math.Round(h.Weight * h.Quantity, 2)) : 0,
                                                //GrossWeight = h.Quantity > 0 ? (h.Unit == WMSOrdersUnit.Unit2 ? Math.Round(h.Weight * h.Quantity / 1000, 2) : h.Weight) : 0,
                                                //NetWeight = h.Quantity > 0 ? (h.Unit == WMSOrdersUnit.Unit2 ? Math.Round(h.Weight * h.Quantity / 1000, 2) : h.Weight) : 0,
                                                //GrossWeight = (h.Weight * h.ActualQuantity),
                                                //NetWeight = (h.Weight * h.ActualQuantity),
                                            }).ToList()
                                        }).ToList();

                    int srno = 1;
                    foreach (var ware in group)
                    {
                        OrderPendingDispatchReportModel Report = new OrderPendingDispatchReportModel();
                        Report.OrderNumber = ware.OrderNumber;
                        Report.OrderCreatedOn = ware.OrderCreatedOn;
                        Report.ShippingMethod = ware.ShippingMethod;
                        Report.AccountNumber = ware.AccountNumber;
                        Report.CustomerName = ware.CustomerName;
                        if (ware.DeliveryType == WMSOrderDeliveryTypeEnum.StandardDelivery)
                        {
                            Report.DeliveryType = WMSOrderDeliveryTypeEnum.StandardDeliveryDisplay;
                        }
                        else if (ware.DeliveryType == WMSOrderDeliveryTypeEnum.UrgentDelivery)
                        {
                            Report.DeliveryType = WMSOrderDeliveryTypeEnum.UrgentDeliveryDisplay;
                        }
                        else if (ware.DeliveryType == WMSOrderDeliveryTypeEnum.ExpressDelivery)
                        {
                            Report.DeliveryType = WMSOrderDeliveryTypeEnum.ExpressDeliveryDisplay;
                        }
                        else
                        {
                            Report.DeliveryType = ware.DeliveryType;
                        }
                        foreach (var dd in ware.detail)
                        {
                            Report.Pieces += dd.Pieces;
                            Report.TotalGrossWeight += Convert.ToDecimal(dd.GrossWeight);
                            Report.TotalNetWeight += Convert.ToDecimal(dd.NetWeight);

                        }
                        _lstdetailss.Add(Report);
                        srno++;
                    }
                }
                else
                {
                    _lstdetails = (from Ord in dbContext.Orders
                                   join Ordg in dbContext.OrderDesigns on Ord.Id equals Ordg.OrderId
                                   join Odt in dbContext.OrderDesignDetails on Ordg.Id equals Odt.OrderDesignId
                                   join Dlm in dbContext.DelivieryMethods on Ord.DelivieryMethodId equals Dlm.Id
                                   join Psku in dbContext.ProductSKUs on Odt.ProductSKUId equals Psku.Id
                                   join Ost in dbContext.OrderStatus on Ord.OrderStatusId equals Ost.Id
                                   join WMSUse in dbContext.WMSUsers on Ord.CustomerId equals WMSUse.Id
                                   join WMSAdd in dbContext.WMSUserAdditionals on WMSUse.Id equals WMSAdd.UserId
                                   where Ord.OrderStatusId == (byte)WMSOrderStatusEnum.ReadyForDispatch && Ord.CustomerId == ReportDetail.UserId
                                   select new OrderPendingDispatchReportModel
                                   {
                                       CustomerName = WMSUse.CompanyName,
                                       AccountNumber = WMSAdd.AccountNumber,
                                       OrderNumber = Ord.OrderNumber,
                                       OrderCreatedOn = Ord.CreatedOnUtc,
                                       DeliveryType = Ord.OrderType,
                                       ShippingMethod = Dlm.DelivieryNameDisplay,
                                       ActualQuantity = Psku.ActualQuantity,
                                       Unit = Psku.WeightUnit,
                                       Weight = Psku.Weight,
                                       //Weight = Psku.WeightUnit == WMSOrdersUnit.Unit2 ? Psku.Weight / 1000 : Psku.Weight,
                                       // Weight = Psku.ActualQuantity.Value > 0 ? (Psku.WeightUnit == WMSOrdersUnit.Unit2 ? Math.Round(Psku.Weight * Psku.ActualQuantity.Value / 1000, 2) : Psku.Weight) : 0,
                                       Quantity = Odt.Quantity,
                                   }).ToList();

                    var group = _lstdetails.GroupBy(p => new { p.OrderNumber })
                                        .Select(x => new
                                        {
                                            x.Key.OrderNumber,
                                            x.FirstOrDefault().OrderCreatedOn,
                                            x.FirstOrDefault().ShippingMethod,
                                            x.FirstOrDefault().DeliveryType,
                                            x.FirstOrDefault().CustomerName,
                                            x.FirstOrDefault().AccountNumber,
                                            detail = x.Select(h => new
                                            {
                                                Pieces = h.Quantity,
                                                GrossWeight = h.Quantity > 0 ? (h.Unit == WMSOrdersUnit.Unit2 ? Math.Round(h.Weight * h.Quantity / 1000, 2) : Math.Round(h.Weight * h.Quantity, 2)) : 0,
                                                NetWeight = h.Quantity > 0 ? (h.Unit == WMSOrdersUnit.Unit2 ? Math.Round(h.Weight * h.Quantity / 1000, 2) : Math.Round(h.Weight * h.Quantity, 2)) : 0,
                                                //GrossWeight = h.Quantity > 0 ? (h.Unit == WMSOrdersUnit.Unit2 ? Math.Round(h.Weight * h.Quantity / 1000, 2) : h.Weight) : 0,
                                                //NetWeight = h.Quantity > 0 ? (h.Unit == WMSOrdersUnit.Unit2 ? Math.Round(h.Weight * h.Quantity / 1000, 2) : h.Weight) : 0,
                                                //GrossWeight = (h.Weight * h.ActualQuantity),
                                                //NetWeight = (h.Weight * h.ActualQuantity),
                                            }).ToList()
                                        }).ToList();

                    int srno = 1;
                    foreach (var ware in group)
                    {
                        OrderPendingDispatchReportModel Report = new OrderPendingDispatchReportModel();
                        Report.OrderNumber = ware.OrderNumber;
                        Report.OrderCreatedOn = ware.OrderCreatedOn;
                        Report.ShippingMethod = ware.ShippingMethod;
                        Report.AccountNumber = ware.AccountNumber;
                        Report.CustomerName = ware.CustomerName;
                        if (ware.DeliveryType == WMSOrderDeliveryTypeEnum.StandardDelivery)
                        {
                            Report.DeliveryType = WMSOrderDeliveryTypeEnum.StandardDeliveryDisplay;
                        }
                        else if (ware.DeliveryType == WMSOrderDeliveryTypeEnum.UrgentDelivery)
                        {
                            Report.DeliveryType = WMSOrderDeliveryTypeEnum.UrgentDeliveryDisplay;
                        }
                        else if (ware.DeliveryType == WMSOrderDeliveryTypeEnum.ExpressDelivery)
                        {
                            Report.DeliveryType = WMSOrderDeliveryTypeEnum.ExpressDeliveryDisplay;
                        }
                        else
                        {
                            Report.DeliveryType = ware.DeliveryType;
                        }
                        foreach (var dd in ware.detail)
                        {
                            Report.Pieces += dd.Pieces;
                            Report.TotalGrossWeight += Convert.ToDecimal(dd.GrossWeight);
                            Report.TotalNetWeight += Convert.ToDecimal(dd.NetWeight);

                        }
                        _lstdetailss.Add(Report);
                        srno++;
                    }
                }
            }
            else
            {
                if (ReportDetail.CustomerId != 0)
                {
                    _lstdetails = (from Ord in dbContext.Orders
                                   join Ordg in dbContext.OrderDesigns on Ord.Id equals Ordg.OrderId
                                   join Odt in dbContext.OrderDesignDetails on Ordg.Id equals Odt.OrderDesignId
                                   join Dlm in dbContext.DelivieryMethods on Ord.DelivieryMethodId equals Dlm.Id
                                   join Psku in dbContext.ProductSKUs on Odt.ProductSKUId equals Psku.Id
                                   join Ost in dbContext.OrderStatus on Ord.OrderStatusId equals Ost.Id
                                   join WMSUse in dbContext.WMSUsers on Ord.CustomerId equals WMSUse.Id
                                   join WMSAdd in dbContext.WMSUserAdditionals on WMSUse.Id equals WMSAdd.UserId
                                   where Ord.CustomerId == ReportDetail.CustomerId && Ord.OrderStatusId == (byte)WMSOrderStatusEnum.ReadyForDispatch
                                   select new OrderPendingDispatchReportModel
                                   {
                                       SalesCordinator = Ord.SalesCordinator.HasValue ? Ord.SalesCordinator.Value : 0,
                                       Merchandiser = Ord.Mechandiser.HasValue ? Ord.Mechandiser.Value : 0,
                                       WarehouseUser = Ord.WarehouseUserId.HasValue ? Ord.WarehouseUserId.Value : 0,
                                       CustomerName = WMSUse.CompanyName,
                                       AccountNumber = WMSAdd.AccountNumber,
                                       OrderNumber = Ord.OrderNumber,
                                       OrderCreatedOn = Ord.CreatedOnUtc,
                                       DeliveryType = Ord.OrderType,
                                       ShippingMethod = Dlm.DelivieryNameDisplay,
                                       ActualQuantity = Psku.ActualQuantity,
                                       Unit = Psku.WeightUnit,
                                       Weight = Psku.Weight,
                                       //Weight = Psku.WeightUnit == WMSOrdersUnit.Unit2 ? Psku.Weight / 1000 : Psku.Weight,
                                       //Weight = Psku.ActualQuantity.Value > 0 ? (Psku.WeightUnit == WMSOrdersUnit.Unit2 ? Math.Round(Psku.Weight * Psku.ActualQuantity.Value / 1000, 2) : Psku.Weight) : 0,
                                       Quantity = Odt.Quantity,
                                   }).ToList();

                    if (UserType.RoleId == (int)WMSUserRoleEnum.SalesCoOrdinator)
                    {
                        _lstdetails = _lstdetails.Where(p => p.SalesCordinator == ReportDetail.UserId).ToList();
                    }
                    if (UserType.RoleId == (int)WMSUserRoleEnum.Merchandise)
                    {
                        _lstdetails = _lstdetails.Where(p => p.Merchandiser == ReportDetail.UserId).ToList();
                    }
                    if (UserType.RoleId == (int)WMSUserRoleEnum.Warehouse)
                    {
                        _lstdetails = _lstdetails.Where(p => p.WarehouseUser == ReportDetail.UserId).ToList();
                    }

                    var group = _lstdetails.GroupBy(p => new { p.OrderNumber })
                                        .Select(x => new
                                        {
                                            x.Key.OrderNumber,
                                            x.FirstOrDefault().OrderCreatedOn,
                                            x.FirstOrDefault().ShippingMethod,
                                            x.FirstOrDefault().DeliveryType,
                                            x.FirstOrDefault().CustomerName,
                                            x.FirstOrDefault().AccountNumber,
                                            detail = x.Select(h => new
                                            {
                                                Pieces = h.Quantity,
                                                GrossWeight = h.Quantity > 0 ? (h.Unit == WMSOrdersUnit.Unit2 ? Math.Round(h.Weight * h.Quantity / 1000, 2) : Math.Round(h.Weight * h.Quantity, 2)) : 0,
                                                NetWeight = h.Quantity > 0 ? (h.Unit == WMSOrdersUnit.Unit2 ? Math.Round(h.Weight * h.Quantity / 1000, 2) : Math.Round(h.Weight * h.Quantity, 2)) : 0,
                                                //GrossWeight = h.Quantity > 0 ? (h.Unit == WMSOrdersUnit.Unit2 ? Math.Round(h.Weight * h.Quantity / 1000, 2) : h.Weight) : 0,
                                                //NetWeight = h.Quantity > 0 ? (h.Unit == WMSOrdersUnit.Unit2 ? Math.Round(h.Weight * h.Quantity / 1000, 2) : h.Weight) : 0,
                                                //GrossWeight = (h.Weight * h.ActualQuantity),
                                                //NetWeight = (h.Weight * h.ActualQuantity),
                                            }).ToList()
                                        }).ToList();

                    int srno = 1;
                    foreach (var ware in group)
                    {
                        OrderPendingDispatchReportModel Report = new OrderPendingDispatchReportModel();
                        Report.OrderNumber = ware.OrderNumber;
                        Report.OrderCreatedOn = ware.OrderCreatedOn;
                        Report.ShippingMethod = ware.ShippingMethod;
                        Report.AccountNumber = ware.AccountNumber;
                        Report.CustomerName = ware.CustomerName;
                        if (ware.DeliveryType == WMSOrderDeliveryTypeEnum.StandardDelivery)
                        {
                            Report.DeliveryType = WMSOrderDeliveryTypeEnum.StandardDeliveryDisplay;
                        }
                        else if (ware.DeliveryType == WMSOrderDeliveryTypeEnum.UrgentDelivery)
                        {
                            Report.DeliveryType = WMSOrderDeliveryTypeEnum.UrgentDeliveryDisplay;
                        }
                        else if (ware.DeliveryType == WMSOrderDeliveryTypeEnum.ExpressDelivery)
                        {
                            Report.DeliveryType = WMSOrderDeliveryTypeEnum.ExpressDeliveryDisplay;
                        }
                        else
                        {
                            Report.DeliveryType = ware.DeliveryType;
                        }
                        foreach (var dd in ware.detail)
                        {
                            Report.Pieces += dd.Pieces;
                            Report.TotalGrossWeight += Convert.ToDecimal(dd.GrossWeight);
                            Report.TotalNetWeight += Convert.ToDecimal(dd.NetWeight);

                        }
                        _lstdetailss.Add(Report);
                        srno++;
                    }
                }
                else
                {
                    _lstdetails = (from Ord in dbContext.Orders
                                   join Ordg in dbContext.OrderDesigns on Ord.Id equals Ordg.OrderId
                                   join Odt in dbContext.OrderDesignDetails on Ordg.Id equals Odt.OrderDesignId
                                   join Dlm in dbContext.DelivieryMethods on Ord.DelivieryMethodId equals Dlm.Id
                                   join Psku in dbContext.ProductSKUs on Odt.ProductSKUId equals Psku.Id
                                   join Ost in dbContext.OrderStatus on Ord.OrderStatusId equals Ost.Id
                                   join WMSUse in dbContext.WMSUsers on Ord.CustomerId equals WMSUse.Id
                                   join WMSAdd in dbContext.WMSUserAdditionals on WMSUse.Id equals WMSAdd.UserId
                                   where Ord.OrderStatusId == (byte)WMSOrderStatusEnum.ReadyForDispatch
                                   select new OrderPendingDispatchReportModel
                                   {
                                       CustomerName = WMSUse.CompanyName,
                                       AccountNumber = WMSAdd.AccountNumber,
                                       OrderNumber = Ord.OrderNumber,
                                       OrderCreatedOn = Ord.CreatedOnUtc,
                                       DeliveryType = Ord.OrderType,
                                       ShippingMethod = Dlm.DelivieryNameDisplay,
                                       ActualQuantity = Psku.ActualQuantity,
                                       Unit = Psku.WeightUnit,
                                       Weight = Psku.Weight,
                                       //Weight = Psku.WeightUnit == WMSOrdersUnit.Unit2 ? Psku.Weight / 1000 : Psku.Weight,
                                       //Weight = Psku.ActualQuantity.Value > 0 ? (Psku.WeightUnit == WMSOrdersUnit.Unit2 ? Math.Round(Psku.Weight * Psku.ActualQuantity.Value / 1000, 2) : Psku.Weight) : 0,
                                       Quantity = Odt.Quantity,
                                   }).ToList();

                    var group = _lstdetails.GroupBy(p => new { p.OrderNumber })
                                        .Select(x => new
                                        {
                                            x.Key.OrderNumber,
                                            x.FirstOrDefault().OrderCreatedOn,
                                            x.FirstOrDefault().ShippingMethod,
                                            x.FirstOrDefault().DeliveryType,
                                            x.FirstOrDefault().CustomerName,
                                            x.FirstOrDefault().AccountNumber,
                                            detail = x.Select(h => new
                                            {
                                                Pieces = h.Quantity,
                                                GrossWeight = h.Quantity > 0 ? (h.Unit == WMSOrdersUnit.Unit2 ? Math.Round(h.Weight * h.Quantity / 1000, 2) : Math.Round(h.Weight * h.Quantity, 2)) : 0,
                                                NetWeight = h.Quantity > 0 ? (h.Unit == WMSOrdersUnit.Unit2 ? Math.Round(h.Weight * h.Quantity / 1000, 2) : Math.Round(h.Weight * h.Quantity, 2)) : 0,
                                                //GrossWeight = h.Quantity > 0 ? (h.Unit == WMSOrdersUnit.Unit2 ? Math.Round(h.Weight * h.Quantity / 1000, 2) : h.Weight) : 0,
                                                //NetWeight = h.Quantity > 0 ? (h.Unit == WMSOrdersUnit.Unit2 ? Math.Round(h.Weight * h.Quantity / 1000, 2) : h.Weight) : 0,
                                                //GrossWeight = (h.Weight * h.ActualQuantity),
                                                //NetWeight = (h.Weight * h.ActualQuantity),
                                            }).ToList()
                                        }).ToList();

                    int srno = 1;
                    foreach (var ware in group)
                    {
                        OrderPendingDispatchReportModel Report = new OrderPendingDispatchReportModel();
                        Report.OrderNumber = ware.OrderNumber;
                        Report.OrderCreatedOn = ware.OrderCreatedOn;
                        Report.ShippingMethod = ware.ShippingMethod;
                        Report.AccountNumber = ware.AccountNumber;
                        Report.CustomerName = ware.CustomerName;
                        if (ware.DeliveryType == WMSOrderDeliveryTypeEnum.StandardDelivery)
                        {
                            Report.DeliveryType = WMSOrderDeliveryTypeEnum.StandardDeliveryDisplay;
                        }
                        else if (ware.DeliveryType == WMSOrderDeliveryTypeEnum.UrgentDelivery)
                        {
                            Report.DeliveryType = WMSOrderDeliveryTypeEnum.UrgentDeliveryDisplay;
                        }
                        else if (ware.DeliveryType == WMSOrderDeliveryTypeEnum.ExpressDelivery)
                        {
                            Report.DeliveryType = WMSOrderDeliveryTypeEnum.ExpressDeliveryDisplay;
                        }
                        else
                        {
                            Report.DeliveryType = ware.DeliveryType;
                        }
                        foreach (var dd in ware.detail)
                        {
                            Report.Pieces += dd.Pieces;
                            Report.TotalGrossWeight += Convert.ToDecimal(dd.GrossWeight);
                            Report.TotalNetWeight += Convert.ToDecimal(dd.NetWeight);

                        }
                        _lstdetailss.Add(Report);
                        srno++;
                    }
                }
            }


            if (_lstdetailss.Count > 0)
            {
                return _lstdetailss;
            }
            else
            {
                return null;
            }
        }

        public List<PendingPickRequestDispatchReportModel> GetPendingPickRequestDispatchReport(ReportTrackandTraceModel ReportDetail)
        {
            List<PendingPickRequestDispatchReportModel> _lstdetails = new List<PendingPickRequestDispatchReportModel>();
            List<PendingPickRequestDispatchReportModel> _lstdetails2 = new List<PendingPickRequestDispatchReportModel>();
            var UserType = (from Usr in dbContext.WMSUserRoles
                            join rl in dbContext.WMSRoles on Usr.RoleId equals rl.Id
                            where Usr.UserId == ReportDetail.UserId
                            select new
                            {
                                RoleId = Usr.RoleId
                            }).FirstOrDefault();

            if (ReportDetail.CustomerId != 0)
            {
                _lstdetails = (from Ord in dbContext.Orders
                               join Ordg in dbContext.OrderDesigns on Ord.Id equals Ordg.OrderId
                               join Odt in dbContext.OrderDesignDetails on Ordg.Id equals Odt.OrderDesignId
                               join Ost in dbContext.OrderStatus on Ord.OrderStatusId equals Ost.Id
                               join Dlm in dbContext.DelivieryMethods on Ord.DelivieryMethodId equals Dlm.Id
                               join WMSUse in dbContext.WMSUsers on Ord.CustomerId equals WMSUse.Id
                               join WMSAdd in dbContext.WMSUserAdditionals on WMSUse.Id equals WMSAdd.UserId
                               where Ord.CustomerId == ReportDetail.CustomerId && Ord.OrderStatusId == (byte)WMSOrderStatusEnum.SampleCreation && Ord.PickupRequestedOnUtc <= DateTime.Now
                               select new PendingPickRequestDispatchReportModel
                               {
                                   SalesCordinator = Ord.SalesCordinator.HasValue ? Ord.SalesCordinator.Value : 0,
                                   Merchandiser = Ord.Mechandiser.HasValue ? Ord.Mechandiser.Value : 0,
                                   WarehouseUser = Ord.WarehouseUserId.HasValue ? Ord.WarehouseUserId.Value : 0,
                                   OrderNumber = Ord.OrderNumber,
                                   OrderCreatedOn = Ord.CreatedOnUtc,
                                   CustomerName = WMSUse.CompanyName,
                                   AccountNumber = WMSAdd.AccountNumber,
                                   PickupRequestedOn = Ord.PickupRequestedOnUtc,
                                   Quantity = Odt.Quantity,
                               }).ToList();

                if (UserType.RoleId == (int)WMSUserRoleEnum.SalesCoOrdinator)
                {
                    _lstdetails = _lstdetails.Where(p => p.SalesCordinator == ReportDetail.UserId).ToList();
                }
                if (UserType.RoleId == (int)WMSUserRoleEnum.Merchandise)
                {
                    _lstdetails = _lstdetails.Where(p => p.Merchandiser == ReportDetail.UserId).ToList();
                }
                if (UserType.RoleId == (int)WMSUserRoleEnum.Warehouse)
                {
                    _lstdetails = _lstdetails.Where(p => p.WarehouseUser == ReportDetail.UserId).ToList();
                }

                var group = _lstdetails.GroupBy(p => new { p.OrderNumber })
                                    .Select(x => new
                                    {
                                        x.Key.OrderNumber,
                                        x.FirstOrDefault().OrderCreatedOn,
                                        x.FirstOrDefault().CustomerName,
                                        x.FirstOrDefault().AccountNumber,
                                        x.FirstOrDefault().PickupRequestedOn,

                                        detail = x.Select(h => new
                                        {
                                            Pieces = h.Quantity,

                                        }).ToList()
                                    }).ToList();

                int srno = 1;
                foreach (var ware in group)
                {
                    PendingPickRequestDispatchReportModel Report = new PendingPickRequestDispatchReportModel();
                    Report.OrderNumber = ware.OrderNumber;
                    Report.OrderCreatedOn = ware.OrderCreatedOn;
                    Report.CustomerName = ware.CustomerName;
                    Report.AccountNumber = ware.AccountNumber;
                    Report.PickupRequestedOn = ware.PickupRequestedOn;


                    foreach (var dd in ware.detail)
                    {
                        Report.Pieces += dd.Pieces;


                    }
                    _lstdetails2.Add(Report);
                    srno++;
                }

            }
            else
            {
                _lstdetails = (from Ord in dbContext.Orders
                               join Ordg in dbContext.OrderDesigns on Ord.Id equals Ordg.OrderId
                               join Odt in dbContext.OrderDesignDetails on Ordg.Id equals Odt.OrderDesignId
                               join Ost in dbContext.OrderStatus on Ord.OrderStatusId equals Ost.Id
                               join Dlm in dbContext.DelivieryMethods on Ord.DelivieryMethodId equals Dlm.Id
                               join WMSUse in dbContext.WMSUsers on Ord.CustomerId equals WMSUse.Id
                               join WMSAdd in dbContext.WMSUserAdditionals on WMSUse.Id equals WMSAdd.UserId
                               where Ord.OrderStatusId == (byte)WMSOrderStatusEnum.SampleCreation && Ord.PickupRequestedOnUtc <= DateTime.Now
                               select new PendingPickRequestDispatchReportModel
                               {
                                   OrderNumber = Ord.OrderNumber,
                                   OrderCreatedOn = Ord.CreatedOnUtc,
                                   CustomerName = WMSUse.CompanyName,
                                   AccountNumber = WMSAdd.AccountNumber,
                                   PickupRequestedOn = Ord.PickupRequestedOnUtc,
                                   Quantity = Odt.Quantity,
                               }).ToList();

                var group = _lstdetails.GroupBy(p => new { p.OrderNumber })
                                    .Select(x => new
                                    {
                                        x.Key.OrderNumber,
                                        x.FirstOrDefault().OrderCreatedOn,
                                        x.FirstOrDefault().CustomerName,
                                        x.FirstOrDefault().AccountNumber,
                                        x.FirstOrDefault().PickupRequestedOn,

                                        detail = x.Select(h => new
                                        {
                                            Pieces = h.Quantity,

                                        }).ToList()
                                    }).ToList();

                int srno = 1;
                foreach (var ware in group)
                {
                    PendingPickRequestDispatchReportModel Report = new PendingPickRequestDispatchReportModel();
                    Report.OrderNumber = ware.OrderNumber;
                    Report.OrderCreatedOn = ware.OrderCreatedOn;
                    Report.CustomerName = ware.CustomerName;
                    Report.AccountNumber = ware.AccountNumber;
                    Report.PickupRequestedOn = ware.PickupRequestedOn;


                    foreach (var dd in ware.detail)
                    {
                        Report.Pieces += dd.Pieces;


                    }
                    _lstdetails2.Add(Report);
                    srno++;
                }

            }

            if (_lstdetails2.Count > 0)
            {
                return _lstdetails2;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}