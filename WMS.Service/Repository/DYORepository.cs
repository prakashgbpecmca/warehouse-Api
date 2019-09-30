using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Model.ApiResponse;
using WMS.Model.Common;
using WMS.Model.DYO;
using WMS.Service.DataAccess;
using WMS.Services.Utility;
using System.IO;
using System.Drawing;
using WMS.Model.User;
using System.Web;
using System.Data.Entity.Validation;
using WMS.Model.Reports;
using System.Net.Http;
using Frayte.Services;

namespace WMS.Service.Repository
{
    public class DYORepository
    {
        WMSEntities dbContext = new WMSEntities();

        public int TotalOrderDesigns(int OrderID)
        {
            var dbOrderDesigns = dbContext.OrderDesigns.Where(p => p.OrderId == OrderID).ToList();
            return dbOrderDesigns.Count;
        }

        public bool DeleteOrderDesign(int orderDesignId, ref int MyOrderID)
        {
            try
            {
                var dbOrderDesign = dbContext.OrderDesigns.Find(orderDesignId);
                if (dbOrderDesign != null)
                {
                    int OrderID = dbOrderDesign.OrderId.Value;
                    MyOrderID = OrderID;

                    #region "Remove the Order Design Details"

                    var dbOrderEmblishments = dbContext.OrderEmblishments.Where(p => p.OrderDesignId == dbOrderDesign.Id).ToList();
                    if (dbOrderEmblishments.Count > 0)
                    {
                        foreach (var item in dbOrderEmblishments)
                        {
                            var dbOrderEmblishmentDetails = dbContext.OrderEmblishmentDetails.Where(p => p.OrderEmblishmentId == item.Id).ToList();

                            if (dbOrderEmblishmentDetails.Count > 0)
                            {
                                dbContext.OrderEmblishmentDetails.RemoveRange(dbOrderEmblishmentDetails);
                                dbContext.SaveChanges();
                            }
                        }
                    }

                    dbContext.OrderEmblishments.RemoveRange(dbOrderEmblishments);
                    dbContext.SaveChanges();

                    var dbOrderDesignDetail = dbContext.OrderDesignDetails.Where(p => p.OrderDesignId == orderDesignId).ToList();
                    if (dbOrderDesignDetail != null)
                    {
                        #region "Update the actual quantity of the Product SKU"

                        foreach (var objODDetail in dbOrderDesignDetail)
                        {
                            int Quantity = objODDetail.Quantity;
                            int ProductSkuId = objODDetail.ProductSKUId;

                            var dbProductSKU = dbContext.ProductSKUs.Where(p => p.Id == ProductSkuId).FirstOrDefault();
                            if (dbProductSKU != null)
                            {
                                dbProductSKU.ActualQuantity = dbProductSKU.ActualQuantity + Quantity;                                
                                dbProductSKU.UpdatedOnUtc = DateTime.UtcNow;
                                //dbProductSKU.UpdatedBy
                                dbContext.Entry(dbProductSKU).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                        }

                        #endregion

                        dbContext.OrderDesignDetails.RemoveRange(dbOrderDesignDetail);
                        dbContext.SaveChanges();
                    }

                    var dbDesignStyle = dbContext.OrderDesignStyles.Where(p => p.OrderDesignId == orderDesignId).ToList();
                    if (dbDesignStyle.Count > 0)
                    {
                        dbContext.OrderDesignStyles.RemoveRange(dbDesignStyle);
                        dbContext.SaveChanges();
                    }

                    dbContext.OrderDesigns.Remove(dbOrderDesign);
                    dbContext.SaveChanges();

                    #endregion

                    var dbOrderDesigns = dbContext.OrderDesigns.Where(p => p.OrderId == OrderID).ToList();
                    if (dbOrderDesigns == null || dbOrderDesigns.Count == 0)
                    {
                        #region "Remove the order details"

                        var dbOrder = dbContext.Orders.Find(OrderID);
                        var dbOrderAddress = dbContext.OrderAddresses.Find(dbOrder.OrderAddressId);
                        if (dbOrderAddress != null)
                        {
                            dbContext.OrderAddresses.Remove(dbOrderAddress);
                            dbContext.SaveChanges();
                        }

                        var dbOrderStrikeOffs = dbContext.OrderStrikeOffLogos.Where(p => p.OrderID == OrderID).ToList();
                        if (dbOrderStrikeOffs.Count > 0)
                        {
                            dbContext.OrderStrikeOffLogos.RemoveRange(dbOrderStrikeOffs);
                            dbContext.SaveChanges();
                        }

                        if (dbOrder != null)
                        {
                            dbContext.Orders.Remove(dbOrder);
                            dbContext.SaveChanges();
                        }

                        #endregion
                    }

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsSendToEmailToMerchandiser(int OrderID)
        {
            bool IsExist = false;

            var dbOrderDesigns = dbContext.OrderDesigns.Where(p => p.OrderId == OrderID).ToList();

            if (dbOrderDesigns.Count > 0)
            {
                #region "Get Order Design Details"

                var dbOrderDesignDetails = (from od in dbContext.OrderDesigns
                                            join os in dbContext.OrderStatus
                                            on od.OrderDesignStatusId equals os.Id
                                            where od.OrderId == OrderID
                                            select new
                                            {
                                                OrderDesignID = od.Id,
                                                CustomerID = od.CustomerId,
                                                DesignName = od.DesignName,
                                                DesignNumber = od.DesignNumber,
                                                StatusName = os.StatusName,
                                                DesignStatusID = od.OrderDesignStatusId,
                                                Type = os.Type
                                            }
                                           ).ToList();

                #endregion

                if (dbOrderDesignDetails.Where(p => p.DesignStatusID == (int)WMSOrderDesignStatusEnum.DesignProcessed).Count() == dbOrderDesignDetails.Count)
                {
                    IsExist = true;
                    //assign merchandiser/warehouse
                    var dborder = dbContext.Orders.Where(x => x.Id == OrderID).FirstOrDefault();

                    var dbWMSUser = (from a in dbContext.WMSUsers
                                     join b in dbContext.WMSUserAdditionals on a.Id equals b.UserId
                                     where a.Id == dborder.CustomerId
                                     select new { b.MerchandiseUserId, b.WarehouseUserId, b.SalesCoOrdinatorId }).FirstOrDefault();

                    if (dbWMSUser != null)
                    {
                        dborder.Mechandiser = dbWMSUser.MerchandiseUserId;
                        dborder.DispatchedOnUtc = DateTime.UtcNow;
                        dborder.OrderStatusId = (int)WMSOrderStatusEnum.Processed;                        // to set the order status Processed
                        dbContext.SaveChanges();

                        // add communication log
                        OrderStrickOffModel mode = new OrderStrickOffModel();
                        var SaleCordinatorName = dbContext.WMSUsers.Where(x => x.Id == dbWMSUser.SalesCoOrdinatorId).FirstOrDefault().ContactFirstName;

                        mode.ispublic = true;
                        mode.reason = "Sales coordinator " + SaleCordinatorName + " has accepted the order and order status changed from New Order to Processed";
                        mode.userid = dbWMSUser.SalesCoOrdinatorId.Value;
                        mode.orderid = OrderID;
                        AddCommLog(mode);
                    }
                }
            }

            return IsExist;
        }

        public ReportResult CollectionInfo(int orderId)
        {

            var dbOrder = dbContext.Orders.Find(orderId);
            if (dbOrder != null && !string.IsNullOrEmpty(dbOrder.JobSheetFile))
            {
                string filePath = HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/");
                if (File.Exists(filePath + "\\" + dbOrder.JobSheetFile))
                {
                    File.Delete(filePath + "\\" + dbOrder.JobSheetFile);
                }

                ReportResult result = (from r in dbContext.Orders
                                       join u in dbContext.WMSUsers on r.CustomerId equals u.Id
                                       where r.Id == orderId
                                       select new ReportResult
                                       {
                                           CustomerCompanyName = u.CompanyName,
                                           CustomerName = u.ContactFirstName + " " + u.ContactLastName,
                                           JobSheetNumber = dbOrder.OrderNumber
                                       }
                           ).FirstOrDefault();
                if (string.IsNullOrEmpty(result.JobSheetNumber))
                {
                    string number = new Random().Next(1000000, 99999999).ToString();
                    var result1 = (from r in dbContext.Orders
                                   join u in dbContext.WMSUsers on r.CustomerId equals u.Id
                                   where r.Id == orderId
                                   select new ReportResult
                                   {
                                       CustomerCompanyName = u.CompanyName,
                                       CustomerName = u.ContactFirstName + " " + u.ContactLastName,
                                       JobSheetNumber = number
                                   }
                                  ).FirstOrDefault();

                    return result1;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                string number = new Random().Next(1000000, 99999999).ToString();
                var result = (from r in dbContext.Orders
                              join u in dbContext.WMSUsers on r.CustomerId equals u.Id
                              where r.Id == orderId
                              select new ReportResult
                              {
                                  CustomerCompanyName = u.CompanyName,
                                  CustomerName = u.ContactFirstName + " " + u.ContactLastName,
                                  JobSheetNumber = number
                              }
                              ).FirstOrDefault();

                return result;
            }
        }

        public DeliveryDateModel GetDeliveryDate(int userId, int orderId)
        {
            var userRole = dbContext.WMSUserRoles.Where(p => p.UserId == userId).FirstOrDefault();
            DeliveryDateModel obj = new DeliveryDateModel();
            var dbOrder = dbContext.Orders.Find(orderId);

            if (dbOrder != null && userRole != null)
            {
                if (userRole.RoleId == (int)WMSUserRoleEnum.SalesCoOrdinator)
                {
                    obj.DeliveryDate = dbOrder.RequestedExFactoryDate.HasValue ? dbOrder.RequestedExFactoryDate.Value : DateTime.UtcNow;
                }
                if (userRole.RoleId == (int)WMSUserRoleEnum.Merchandise)
                {
                    obj.DeliveryDate = dbOrder.ConfirmedExFactoryDate.HasValue ? dbOrder.ConfirmedExFactoryDate.Value : DateTime.UtcNow;
                }
                if (userRole.RoleId == (int)WMSUserRoleEnum.SalesRepresentative || userRole.RoleId == (int)WMSUserRoleEnum.Customer)
                {
                    obj.DeliveryDate = dbOrder.RequestedDeliveryDate.HasValue ? dbOrder.RequestedDeliveryDate.Value : DateTime.UtcNow;
                    obj.DeliveryDate = dbOrder.RequestedDeliveryDate.HasValue ? dbOrder.RequestedDeliveryDate.Value : DateTime.UtcNow;
                }
                return obj;
            }
            return null;
        }

        public object UpdateDeliveryDate(DeliveryDateModel obj)
        {
            bool status = false;
            var userRole = dbContext.WMSUserRoles.Where(p => p.UserId == obj.UserId).FirstOrDefault();
            var dbOrder = dbContext.Orders.Find(obj.OrderId);
            if (dbOrder != null && userRole != null)
            {
                if (userRole.RoleId == (int)WMSUserRoleEnum.SalesCoOrdinator)
                {
                    dbOrder.RequestedExFactoryDate = UtilityRepository.UpdatedDateTime(obj.DeliveryDate);
                    dbContext.SaveChanges();
                    status = true;
                    return new { Status = status, Date = dbOrder.RequestedExFactoryDate.Value.ToString("dd-MMM-yyyy") };
                }
                if (userRole.RoleId == (int)WMSUserRoleEnum.Merchandise)
                {
                    dbOrder.ConfirmedExFactoryDate = UtilityRepository.UpdatedDateTime(obj.DeliveryDate);
                    dbContext.SaveChanges();
                    status = true;
                    return new { Status = status, Date = dbOrder.ConfirmedExFactoryDate.Value.ToString("dd-MMM-yyyy") };
                }

            }

            return null;
        }

        public string OrderNumber(int orderId)
        {

            var dbOrder = dbContext.Orders.Find(orderId);
            if (dbOrder != null)
            {
                return dbOrder.OrderNumber;
            }
            else
            {
                return "";
            }

        }

        public bool SaveCollection(CollectionSaveModel collection)
        {
            if (collection != null)
            {
                if (collection.OrderId > 0)
                {
                    var dbOrder = dbContext.Orders.Find(collection.OrderId);
                    if (dbOrder != null)
                    {
                        dbOrder.OrderName = collection.CollectionName;
                        dbOrder.OrderStatusId = (byte)WMSOrderStatusEnum.Draft;
                        dbContext.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        public bool DeleteCollection(int collectionId)
        {
            try
            {
                var orderDesigns = dbContext.OrderDesigns.Where(p => p.OrderId == collectionId).ToList();
                foreach (var item in orderDesigns)
                {
                    var orderEmblishments = dbContext.OrderEmblishments.Where(p => p.OrderDesignId == item.Id).ToList();
                    dbContext.OrderEmblishments.RemoveRange(orderEmblishments);
                    dbContext.SaveChanges();
                }
                dbContext.OrderDesigns.RemoveRange(orderDesigns);
                dbContext.SaveChanges();

                var order = dbContext.Orders.Find(collectionId);
                dbContext.Orders.Remove(order);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }



            throw new NotImplementedException();
        }

        public CollectionDYOOrder CollectionDetail(int collectionId)
        {
            try
            {
                var collection = (from r in dbContext.Orders
                                  join od in dbContext.OrderDesigns on r.Id equals od.OrderId
                                  join p in dbContext.ProductMasters on od.ProductId equals p.Id
                                  where r.Id == collectionId
                                  select new
                                  {
                                      OrderId = r.Id,
                                      OrderNumber = r.OrderNumber,
                                      OrderName = r.OrderName,
                                      JobSheetName = r.JobSheetFile,
                                      JobSheetPath = AppSettings.ProductPath + "Files/Orders/" + od.OrderId + "/" + r.JobSheetFile,
                                      OrderDesignId = od.Id,
                                      OrderDesignName = od.DesignName,
                                      ProductId = p.Id,
                                      ProductName = p.ProductName,
                                      ProductCode = p.ProductCode,
                                      StyleCode = od.ProductColor,
                                      ProductImageUrl = AppSettings.ProductPath + "Files/Orders/" + od.OrderId + "/" + od.Id + "/" + od.ProductDesignImage
                                  }
                       ).ToList();



                CollectionDYOOrder detail = collection.GroupBy(p => p.OrderId)
                                                             .Select(group => new CollectionDYOOrder
                                                             {
                                                                 OrderId = group.FirstOrDefault().OrderId,
                                                                 OrderName = group.FirstOrDefault().OrderName,
                                                                 OrderDescripton = group.FirstOrDefault().OrderName,
                                                                 OrderNumber = group.FirstOrDefault().OrderNumber,
                                                                 JobSheetName = group.FirstOrDefault().JobSheetName,
                                                                 JobSheetPath = group.FirstOrDefault().JobSheetPath,
                                                                 OrderDesigns = group.GroupBy(subGroup => new { subGroup.OrderId, subGroup.OrderDesignId })
                                                                                        .Select(p => new CollectionOrderDesign
                                                                                        {
                                                                                            OrderDesignId = p.FirstOrDefault().OrderDesignId,
                                                                                            OrderDesignName = p.FirstOrDefault().OrderDesignName,
                                                                                            ProductCode = (p.FirstOrDefault().ProductCode == "DSS1234" || p.FirstOrDefault().ProductCode == "DSS1235") ? p.FirstOrDefault().StyleCode : p.FirstOrDefault().ProductCode,
                                                                                            ProductImageURL = p.FirstOrDefault().ProductImageUrl,
                                                                                            ProductName = p.FirstOrDefault().ProductName
                                                                                        }).ToList()

                                                             }).FirstOrDefault();

                return detail;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public AcceptRejectDesignResultModel AcceptRejectDesign(AcceptRejectDesign obj)
        {
            AcceptRejectDesignResultModel result = new AcceptRejectDesignResultModel();

            var orderDesign = dbContext.OrderDesigns.Find(obj.OrderDesignId);

            var dbOrderDesigns = dbContext.OrderDesigns.Where(p => p.OrderId == obj.OrderId).ToList();

            var userRole = dbContext.WMSUserRoles.Where(p => p.UserId == obj.UserId).FirstOrDefault();
            if (orderDesign != null && userRole != null)
            {
                if (userRole.RoleId == (int)WMSUserRoleEnum.SalesCoOrdinator)
                {
                    if (obj.ActionType == "Accept")
                    {
                        orderDesign.UpdatedOn = DateTime.UtcNow;
                        orderDesign.UpdatedBy = orderDesign.UpdatedBy;

                        orderDesign.OrderDesignStatusId = (int)WMSOrderDesignStatusEnum.DesignProcessed;
                        result.DesignStatusId = (int)WMSOrderDesignStatusEnum.DesignProcessed;
                        // add communication log 
                        var SaleCordinatorName = dbContext.WMSUsers.Where(x => x.Id == obj.UserId).FirstOrDefault().ContactFirstName;

                        CommunicationLog comObj = new CommunicationLog();
                        comObj.OrderID = obj.OrderId;
                        comObj.OrderDesignId = obj.OrderDesignId;
                        comObj.Reason = "Sales coordinator " + SaleCordinatorName + " has accepted the design" + orderDesign.DesignName;
                        comObj.UserID = obj.UserId;
                        comObj.IsPublic = obj.IsPublic;
                        comObj.Type = "Accept";
                        comObj.Date = DateTime.UtcNow;
                        dbContext.CommunicationLogs.Add(comObj);
                        dbContext.SaveChanges();
                        result.Status = true;
                        if (dbOrderDesigns.Count > 1)
                        {
                            bool s = new EmailRepository().EMailE4_4(obj.OrderId, obj.OrderDesignId);
                        }

                    }
                    else
                    {
                        orderDesign.UpdatedOn = DateTime.UtcNow;
                        orderDesign.UpdatedBy = orderDesign.UpdatedBy;
                        orderDesign.OrderDesignStatusId = (int)WMSOrderDesignStatusEnum.DesignRejected;
                        result.DesignStatusId = (int)WMSOrderDesignStatusEnum.DesignRejected;

                        // add communication log 
                        var SaleCordinatorName = dbContext.WMSUsers.Where(x => x.Id == obj.UserId).FirstOrDefault().ContactFirstName;

                        CommunicationLog comObj = new CommunicationLog();
                        comObj.OrderID = obj.OrderId;
                        comObj.OrderDesignId = obj.OrderDesignId;
                        comObj.Reason = obj.Reason;
                        comObj.Type = "Reject";
                        comObj.UserID = obj.UserId;
                        comObj.IsPublic = obj.IsPublic;
                        comObj.Date = DateTime.UtcNow;
                        dbContext.CommunicationLogs.Add(comObj);
                        dbContext.SaveChanges();
                        result.Status = true;

                        bool s = new EmailRepository().EMailE4_2(obj.OrderId, obj.OrderDesignId);
                    }

                    // if all the designs are accepted then no need to accpet manually
                    //var collection = dbContext.OrderDesigns.Where(p => p.OrderId == obj.OrderId).Where(p => p.OrderDesignStatusId == (int)WMSOrderDesignStatusEnum.DesignProcessed).ToList();
                    //if (collection.Count == 0)
                    //{
                    //    var order = dbContext.Orders.Find(obj.OrderId);
                    //    if (order != null)
                    //    {
                    //        order.OrderStatusId = (int)WMSOrderStatusEnum.Processed;
                    //    }
                    //}

                    // Send email to created by 

                }
                else if (userRole.RoleId == (int)WMSUserRoleEnum.Merchandise)
                {
                    if (obj.ActionType == "Accept")
                    {
                        orderDesign.UpdatedOn = DateTime.UtcNow;
                        orderDesign.UpdatedBy = orderDesign.UpdatedBy;
                        orderDesign.OrderDesignStatusId = (int)WMSOrderDesignStatusEnum.DesignCreation;
                        result.DesignStatusId = (int)WMSOrderDesignStatusEnum.DesignCreation;
                        var Merchandiser = dbContext.WMSUsers.Where(x => x.Id == obj.UserId).FirstOrDefault().ContactFirstName;

                        CommunicationLog comObj = new CommunicationLog();
                        comObj.OrderID = obj.OrderId;
                        comObj.Reason = "Merchandiser " + Merchandiser + " has accepted the design" + orderDesign.DesignName;
                        comObj.UserID = obj.UserId;
                        comObj.OrderDesignId = obj.OrderDesignId;
                        comObj.Type = "Accept";
                        comObj.IsPublic = obj.IsPublic;
                        comObj.Date = DateTime.UtcNow;
                        dbContext.CommunicationLogs.Add(comObj);
                        dbContext.SaveChanges();
                        result.Status = true;

                        if (dbOrderDesigns.Count > 1)
                        {
                            bool s = new EmailRepository().EMailE4_5(obj.OrderId, obj.OrderDesignId);
                        }

                    }
                    else
                    {
                        orderDesign.UpdatedOn = DateTime.UtcNow;
                        orderDesign.UpdatedBy = orderDesign.UpdatedBy;
                        orderDesign.OrderDesignStatusId = 14;
                        result.DesignStatusId = (int)WMSOrderDesignStatusEnum.DesignSampleRekjected;
                        var Merchandiser = dbContext.WMSUsers.Where(x => x.Id == obj.UserId).FirstOrDefault().ContactFirstName;

                        CommunicationLog comObj = new CommunicationLog();
                        comObj.OrderID = obj.OrderId;
                        comObj.Reason = obj.Reason;
                        comObj.UserID = obj.UserId;
                        comObj.OrderDesignId = obj.OrderDesignId;
                        comObj.Type = "Reject";
                        comObj.IsPublic = obj.IsPublic;
                        comObj.Date = DateTime.UtcNow;
                        dbContext.CommunicationLogs.Add(comObj);
                        dbContext.SaveChanges();
                        result.Status = true;
                        bool s = new EmailRepository().EMailE4_3(obj.OrderId, obj.OrderDesignId);
                    }

                    // add communication log 
                    // add communication log 

                    // if all the designs are accepted then no need to accpet manually
                    //var collection = dbContext.OrderDesigns.Where(p => p.OrderId == obj.OrderId).Where(p => p.OrderDesignStatusId == (int)WMSOrderDesignStatusEnum.DesignCreation).ToList();
                    //if (collection.Count == 0)
                    //{
                    //    var order = dbContext.Orders.Find(obj.OrderId);
                    //    if (order != null)
                    //    {
                    //        order.OrderStatusId = (int)WMSOrderStatusEnum.SampleCreation;
                    //    }
                    //}
                    // Send email to created by and sales coordinator

                }
            }
            return result;
        }

        public int GetLogoStatus(int orderId, int userId)
        {
            var samples = dbContext.OrderStrikeOffLogos.Where(p => p.OrderID == orderId).ToList();
            bool isAccepted = false;
            bool isRejected = false;
            foreach (var item in samples)
            {
                if (item.Status.HasValue)
                {
                    if (item.Status.Value == 1)
                    {
                        isAccepted = true;
                        break;
                    }
                    if (item.Status.Value == 2)
                    {
                        isRejected = true;
                        break;
                        break;
                    }
                }
            }
            if (isAccepted)
            {
                return 1;
            }
            else if (isRejected)
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }

        public List<WMSFile> OrderCartons(int orderId)
        {
            List<WMSFile> list = new List<WMSFile>();
            WMSFile obj;
            var cartons = dbContext.OrderCartons.Where(p => p.OrderID == orderId).ToList();
            if (cartons.Count > 0)
            {
                foreach (var item in cartons)
                {
                    string file = HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/Cartons/") + item.CartonDisplayName + ".pdf";

                    if (File.Exists(file))
                    {
                        obj = new WMSFile();
                        obj.CartonName = item.CartonDisplayName;
                        obj.FileName = item.CartonDisplayName + ".pdf";
                        obj.FilePath = AppSettings.ProductPath + "Files/Orders/" + orderId + "/Cartons/" + item.CartonDisplayName + ".pdf";
                        list.Add(obj);
                    }
                }
            }
            return list;
        }

        public OrderDesignModel GetAcceptOrderDYO(OrderDesignModel obj)
        {
            var userRole = dbContext.WMSUserRoles.Where(P => P.UserId == obj.userid).FirstOrDefault();
            obj.roleid = userRole.RoleId;



            return obj;
        }

        public WMSFile getDesignNamePath(int orderDesignId)
        {
            WMSFile file = new WMSFile();
            var orderDesign = dbContext.OrderDesigns.Find(orderDesignId);
            if (orderDesign != null)
            {
                file.FileName = orderDesign.ProductDesignImage;
                file.FilePath = AppSettings.ProductPath + "Files/Orders/" + orderDesign.OrderId + "/" + orderDesign.Id + "/" + orderDesign.ProductDesignImage;
            }
            return file;
        }

        public bool SaveJobsheetFile(int orderId, string fileName)
        {
            var dbOrder = dbContext.Orders.Find(orderId);
            if (dbOrder != null)
            {
                dbOrder.JobSheetFile = fileName;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }
        public bool SaveJobsheetShippingFile(int orderId, string fileName)
        {
            var dbOrder = dbContext.Orders.Find(orderId);
            if (dbOrder != null)
            {
                dbOrder.JobSheetShippingDetailFile = fileName;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }
        public List<CollectionDYOOrder> ViewCollection(MyOrderTrackAndTrace trackObj)
        {

            var loggedInUserDetail = (from r in dbContext.WMSUsers
                                      join ur in dbContext.WMSUserRoles on r.Id equals ur.UserId
                                      where r.Id == trackObj.UserId
                                      select new
                                      { RoleId = ur.RoleId }
                                         ).FirstOrDefault();

            int customerId = 0;

            if (loggedInUserDetail.RoleId == (int)WMSUserRoleEnum.SalesRepresentative)
            {
                var userAd = dbContext.WMSUserAdditionals.Where(p => p.UserId == trackObj.UserId).FirstOrDefault();
                if (userAd != null)
                {
                    customerId = userAd.CustomerId.HasValue ? userAd.CustomerId.Value : trackObj.UserId;
                }
            }
            else
            {
                customerId = trackObj.UserId;
            }

            var collection = (from r in dbContext.Orders
                              join od in dbContext.OrderDesigns on r.Id equals od.OrderId
                              join p in dbContext.ProductMasters on od.ProductId equals p.Id
                              join c in dbContext.Colors on od.ProductColor equals c.code into leftJoinCOlor
                              from tc in leftJoinCOlor.DefaultIfEmpty()
                              join u in dbContext.WMSUsers on r.CreatedBy equals u.Id
                              where r.OrderStatusId == (byte)WMSOrderStatusEnum.Draft &&
                             (r.OrderName != "" && r.OrderName != null)
                              select new
                              {
                                  OrderId = r.Id,
                                  CustomerId = r.CustomerId,
                                  OrderNumber = r.OrderNumber,
                                  OrderName = r.OrderName,
                                  CreatedOn = r.CreatedOnUtc,
                                  CreatedBy = u.ContactFirstName + " " + u.ContactLastName,
                                  ProductColor = tc == null ? "" : tc.color1,
                                  ProductColorCode = tc == null ? "" : tc.code,
                                  OrderDesignId = od.Id,
                                  OrderDesignName = od.DesignName,
                                  ProductId = p.Id,
                                  ProductName = p.ProductName,
                                  ProductCode = tc == null ? od.ProductColor : p.ProductCode,
                                  ProductImageUrl = AppSettings.ProductPath + "Files/Orders/" + od.OrderId + "/" + od.Id + "/" + od.ProductDesignImage
                              }
                  ).Where(p => p.CustomerId == customerId).OrderByDescending(p => p.OrderId).ToList();

            List<CollectionDYOOrder> list = collection
                                                    .GroupBy(p => p.OrderId)
                                                    .Select(group => new CollectionDYOOrder
                                                    {
                                                        TotalRows = collection.Select(p => p.OrderId).Distinct().Count(),
                                                        OrderId = group.FirstOrDefault().OrderId,
                                                        OrderName = group.FirstOrDefault().OrderName,
                                                        OrderDescripton = group.FirstOrDefault().OrderName,
                                                        OrderNumber = group.FirstOrDefault().OrderNumber,
                                                        CreatedBy = group.FirstOrDefault().CreatedBy,
                                                        CreatedOn = group.FirstOrDefault().CreatedOn,
                                                        OrderDesigns = group.GroupBy(subGroup => new { subGroup.OrderId, subGroup.OrderDesignId })
                                                                                .Select(p => new CollectionOrderDesign
                                                                                {
                                                                                    OrderDesignId = p.FirstOrDefault().OrderDesignId,
                                                                                    OrderDesignName = p.FirstOrDefault().OrderDesignName,
                                                                                    ProductColor = p.FirstOrDefault().ProductColor,
                                                                                    ProductColorCode = p.FirstOrDefault().ProductColorCode,
                                                                                    ProductCode = p.FirstOrDefault().ProductCode,
                                                                                    ProductImageURL = p.FirstOrDefault().ProductImageUrl,
                                                                                    ProductName = p.FirstOrDefault().ProductName
                                                                                }).ToList()

                                                    }).Skip(trackObj.CurrentPage - 1).Take(trackObj.TakeRows).ToList();
            return list;

            //    var collection = (from r in dbContext.Orders
            //                      join od in dbContext.OrderDesigns on r.Id equals od.OrderId
            //                      join p in dbContext.ProductMasters on od.ProductId equals p.Id
            //                      join u in dbContext.WMSUsers on r.CreatedBy equals u.Id
            //                      where r.OrderStatusId == (byte)WMSOrderStatusEnum.Draft &&
            //                     (r.OrderName != "" && r.OrderName != null)
            //                      select new
            //                      {
            //                          OrderId = r.Id,
            //                          OrderNumber = r.OrderNumber,
            //                          OrderName = r.OrderName,
            //                          CreatedOn = r.CreatedOnUtc,
            //                          CreatedBy = u.ContactFirstName + " " + u.ContactLastName,

            //                          OrderDesignId = od.Id,
            //                          OrderDesignName = od.DesignName,
            //                          ProductId = p.Id,
            //                          ProductName = p.ProductName,
            //                          ProductCode = p.ProductCode,
            //                          ProductImageUrl = AppSettings.ProductPath + "Files/Orders/" + od.OrderId + "/" + od.Id + "/" + od.ProductDesignImage
            //                      }
            //          ).ToList();

            //    List<CollectionDYOOrder> list = collection
            //                                            .GroupBy(p => p.OrderId)
            //                                            .Select(group => new CollectionDYOOrder
            //                                            {
            //                                                TotalRows = collection.Select(p => p.OrderId).Distinct().Count(),
            //                                                OrderId = group.FirstOrDefault().OrderId,
            //                                                OrderName = group.FirstOrDefault().OrderName,
            //                                                OrderDescripton = group.FirstOrDefault().OrderName,
            //                                                OrderNumber = group.FirstOrDefault().OrderNumber,
            //                                                CreatedBy = group.FirstOrDefault().CreatedBy,
            //                                                CreatedOn = group.FirstOrDefault().CreatedOn,

            //                                                OrderDesigns = group.GroupBy(subGroup => new { subGroup.OrderId, subGroup.OrderDesignId })
            //                                                                        .Select(p => new CollectionOrderDesign
            //                                                                        {
            //                                                                            OrderDesignId = p.FirstOrDefault().OrderDesignId,
            //                                                                            OrderDesignName = p.FirstOrDefault().OrderDesignName,
            //                                                                            ProductCode = p.FirstOrDefault().ProductCode,
            //                                                                            ProductImageURL = p.FirstOrDefault().ProductImageUrl,
            //                                                                            ProductName = p.FirstOrDefault().ProductName
            //                                                                        }).ToList()

            //                                            }).Skip(trackObj.CurrentPage - 1).Take(trackObj.TakeRows).OrderByDescending(p => p.CreatedOn).ToList();
            //    return list;
            //}
        }

        public void AssignWarehouseUserToOrder(int orderid)
        {
            var warehouseUser = (from r in dbContext.Orders
                                 join c in dbContext.WMSUsers on r.CustomerId equals c.Id
                                 join ca in dbContext.WMSUserAdditionals on c.Id equals ca.WarehouseUserId
                                 join wu in dbContext.WMSUsers on ca.UserId equals wu.Id
                                 where r.Id == orderid
                                 select wu
               ).FirstOrDefault();

            if (warehouseUser != null)
            {
                var dbOrder = dbContext.Orders.Find(orderid);
                if (dbOrder != null)
                {
                    dbOrder.WarehouseUserId = warehouseUser.Id;
                    dbContext.SaveChanges();
                }
            }
        }

        public void SaveUpdatedPickupInformation(int orderid, string fileName)
        {
            var dbOrder = dbContext.Orders.Find(orderid);
            if (dbOrder != null)
            {
                dbOrder.UpdatedPickupNoteFile = fileName;
                dbContext.SaveChanges();
            }
        }
        public void SavePickupInformation(int orderid, string fileName)
        {
            var dbOrder = dbContext.Orders.Find(orderid);
            if (dbOrder != null)
            {
                dbOrder.PickupRequestedOnUtc = DateTime.UtcNow;
                dbOrder.PickupNoteFile = fileName;
                dbContext.SaveChanges();
            }
        }

        public void SaveOrderPickupDetail(int orderId)
        {
            var designs = dbContext.OrderDesigns.Where(p => p.OrderId == orderId).ToList();

            PickupRequestReportModelDetail pick;
            foreach (var item in designs)
            {
                var orderDesigns = dbContext.OrderDesignDetails.Where(p => p.OrderDesignId == item.Id).ToList();

                int count = 0;

                //List<SKUQuantity> list1 = new List<Model.DYO.SKUQuantity>();

                //foreach (var dt in orderDesigns)
                //{
                //    SKUQuantity gh = new SKUQuantity();
                //    var found = list1.Where(p => p.ProductSKUId == dt.ProductSKUId).FirstOrDefault();
                //    if (found == null)
                //    {
                //        gh.ProductSKUId = dt.ProductSKUId;
                //        gh.Quantity = dt.Quantity;
                //        list1.Add(gh);
                //    } else
                //    {
                //        found.Quantity += dt.Quantity; 
                //    }
                //}

                foreach (var detail in orderDesigns)
                {
                    List<DYOOrdePickup> list = new List<DYOOrdePickup>();

                    DYOOrdePickup model;
                    var sections = dbContext.WarehouseAllocationDetails.Where(p => p.ProductSKUID == detail.ProductSKUId).Where(p => p.Quantity > 0).OrderBy(p => p.Quantity).ToList();

                    int totalQuantityToPick = detail.Quantity;
                    for (int i = 0; i < sections.Count && totalQuantityToPick != 0; i++)
                    {
                        if (sections[i].Quantity < totalQuantityToPick)
                        {
                            model = new DYOOrdePickup();
                            model.RequiredQty = sections[i].Quantity.Value;
                            model.ID = sections[i].SectionDetailID.Value;

                            list.Add(model);
                            totalQuantityToPick -= sections[i].Quantity.Value;

                        }
                        else if (totalQuantityToPick == sections[i].Quantity)  //12==12
                        {

                            model = new DYOOrdePickup();
                            model.RequiredQty = sections[i].Quantity.Value;
                            model.ID = sections[i].SectionDetailID.Value;

                            list.Add(model);
                            totalQuantityToPick -= sections[i].Quantity.Value;
                        }
                        else
                        {
                            model = new DYOOrdePickup();
                            model.RequiredQty = totalQuantityToPick;
                            model.ID = sections[i].SectionDetailID.Value;

                            list.Add(model);
                            totalQuantityToPick = 0;
                        }
                    }
                    SaveOrderPickUpDetail(orderId, detail, list);
                }
            }
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
                    pickUp.RequiredQty = item.RequiredQty;
                    pickUp.SectionDetailID = item.ID;
                    dbContext.OrderPickupDetails.Add(pickUp);
                    dbContext.SaveChanges();
                }
            }
        }
        #region Design stock 
        public List<DYODesignCatagory> GetAvailableStock(int userId, int orderId)
        {
            int stockRageId = 0;
            var userRole = (from r in dbContext.WMSUsers
                            join ur in dbContext.WMSUserRoles on r.Id equals ur.UserId
                            where r.Id == userId
                            select new
                            {
                                RoleId = ur.RoleId
                            }).FirstOrDefault();


            if (userRole.RoleId == (int)WMSUserRoleEnum.SalesRepresentative)
            {
                var usre = dbContext.WMSUserAdditionals.Where(p => p.UserId == userId).FirstOrDefault();
                if (usre != null)
                {
                    stockRageId = usre.CustomerId.HasValue ? usre.CustomerId.Value : userId;
                }
                else
                {
                    stockRageId = userId;
                }
            }
            else if (userRole.RoleId == (int)WMSUserRoleEnum.Customer)
            {
                stockRageId = userId;
            }
            else
            {
                stockRageId = userId;
            }

            var orderdetail = (from r in dbContext.Orders
                               join d in dbContext.OrderDesigns on r.Id equals d.OrderId
                               join pm in dbContext.ProductMasters on d.ProductId equals pm.Id
                               where r.Id == orderId
                               select new
                               {
                                   ProductId = d.ProductId,
                                   ProductCatagoryId = pm.Id
                               }).ToList();

            bool isSocks = false;
            bool isStock = false;

            if (orderdetail.Where(p => p.ProductId == 17 || p.ProductId == 18).ToList().Count() > 0)
            {
                isSocks = true;
            }
            if (orderdetail.Where(p => p.ProductId != 17 && p.ProductId != 18).ToList().Count() > 0)
            {
                isStock = true;
            }
            List<DYODesignCatagory> list;
            var stockRange = dbContext.ProductCatagories.Where(p => p.CustomerId == stockRageId).FirstOrDefault();

            if (stockRange != null || userRole.RoleId == (int)WMSUserRoleEnum.Admin)
            {
                var orders = (from r in dbContext.Orders
                              join od in dbContext.OrderDesigns on r.Id equals od.OrderId
                              where r.CustomerId == stockRageId && (od.OrderDesignStatusId == (int)WMSOrderDesignStatusEnum.DesignRejected || od.OrderDesignStatusId == (int)WMSOrderDesignStatusEnum.DesignSampleRekjected)
                              select r
                     ).ToList();
                int count = 0;
                if (userRole.RoleId == (int)WMSUserRoleEnum.Admin)
                {
                    count = 0;
                }
                else
                {
                    count = orders.Count;
                }

                var collection = (from r in dbContext.ProductCatagories
                                  join p in dbContext.ProductMasters on r.Id equals p.ProductCatagoryId
                                  join psku in dbContext.ProductSKUs on p.Id equals psku.ProductId
                                  join s in dbContext.Sizes on psku.SizeId equals s.ID
                                  join c in dbContext.Colors on psku.ColorId equals c.ID into colorTemp
                                  from leftColor in colorTemp.DefaultIfEmpty()
                                  select new
                                  {
                                      ProductCaratogyId = r.Id,
                                      ProductCatatgory = r.CatagoryName,
                                      ProductCatagoryDisplay = r.CatagoryDisplay,
                                      ProductId = p.Id,
                                      ProductCode = p.ProductCode,
                                      ProductName = p.ProductName,
                                      ProductDescription = p.ProductDescription,
                                      ProductSKUId = psku.Id,
                                      SizeId = s.ID,
                                      ColorId = leftColor == null ? 0 : leftColor.ID,
                                      ColorName = leftColor == null ? "" : leftColor.color1,
                                      ColorCode = leftColor == null ? "" : leftColor.code,
                                      SizeName = s.size1,
                                      SizeOrderNumber = s.OrderNumber,
                                      SizeDisplayName = s.description,
                                      Quantity = psku.ActualQuantity,
                                      ProductThumbnailURL = AppSettings.ProductPath + "/Files/" + "/Products/" + p.Id + "/" + p.LogoThumbnail
                                  }).OrderBy(zc => new { zc.ColorName, zc.SizeId }).ToList();

                list = collection.GroupBy(p => p.ProductCaratogyId)
                                                              .Select(p => new DYODesignCatagory
                                                              {
                                                                  CatagoryId = p.Key,
                                                                  Catagory = p.FirstOrDefault().ProductCatatgory,
                                                                  CatagoryDispaly = p.FirstOrDefault().ProductCatagoryDisplay,
                                                                  PendingOrders = count,
                                                                  Products = p.GroupBy(s => new { s.ProductCaratogyId, s.ProductId })
                                                                  .Select(pu => new DYODesignStock
                                                                  {
                                                                      ProductSizes = pu.GroupBy(cv => new { cv.SizeId, cv.ColorId }).Select(zc => new DYODesignSizeStock
                                                                      {
                                                                          Quantity = zc.Where(fg => fg.SizeId == zc.FirstOrDefault().SizeId && fg.ColorId == zc.FirstOrDefault().ColorId).Sum(px => px.Quantity).Value,
                                                                          SizeDisplayName = zc.FirstOrDefault().SizeDisplayName,
                                                                          SizeId = zc.FirstOrDefault().SizeId,
                                                                          SizeName = zc.FirstOrDefault().SizeName,
                                                                          Color = zc.FirstOrDefault().ColorName,
                                                                          ColorCode = zc.FirstOrDefault().ColorCode,
                                                                          ColorId = zc.FirstOrDefault().ColorId,
                                                                          OrderNumber = zc.FirstOrDefault().SizeOrderNumber.HasValue ? zc.FirstOrDefault().SizeOrderNumber.Value : 100
                                                                      }).ToList(),
                                                                      ProductThumbnailURL = pu.FirstOrDefault().ProductThumbnailURL,
                                                                      ProductCode = pu.FirstOrDefault().ProductCode == "DSS1235" ? "LSSO003" : pu.FirstOrDefault().ProductCode,
                                                                      ProductId = pu.FirstOrDefault().ProductId,
                                                                      ProductName = pu.FirstOrDefault().ProductName,
                                                                      Quantity = p.Where(pz => pz.ProductId == pu.FirstOrDefault().ProductId).Sum(px => px.Quantity).Value
                                                                  }).OrderBy(pm => pm.ProductId).ToList()
                                                              }).OrderBy(p => p.CatagoryId).ToList();

                foreach (var item in list)
                {
                    if (isStock == true)
                    {
                        if (item.CatagoryId == 2)
                        {
                            item.IsDisable = true;
                        }
                    }
                    if (isSocks == true)
                    {
                        if (item.CatagoryId == 1)
                        {
                            item.IsDisable = true;
                        }
                    }
                }
            }
            else
            {
                var orders = (from r in dbContext.Orders
                              join od in dbContext.OrderDesigns on r.Id equals od.OrderId
                              where r.CustomerId == stockRageId && od.OrderDesignStatusId == (int)WMSOrderDesignStatusEnum.DesignRejected
                              select r
                         ).ToList();

                var collection = (from us in dbContext.UserStocks
                                  join psku in dbContext.ProductSKUs on us.ProductSKUId equals psku.Id
                                  join p in dbContext.ProductMasters on psku.ProductId equals p.Id
                                  join r in dbContext.ProductCatagories on p.ProductCatagoryId equals r.Id
                                  join s in dbContext.Sizes on psku.SizeId equals s.ID
                                  join c in dbContext.Colors on psku.ColorId equals c.ID into colorTemp
                                  from leftColor in colorTemp.DefaultIfEmpty()
                                  where us.UserId == stockRageId
                                  select new
                                  {
                                      ProductCaratogyId = r.Id,
                                      ProductCatatgory = r.CatagoryName,
                                      ProductCatagoryDisplay = r.CatagoryDisplay,
                                      ProductId = p.Id,
                                      ProductCode = p.ProductCode,
                                      ProductName = p.ProductName,
                                      ProductDescription = p.ProductDescription,
                                      ProductSKUId = psku.Id,
                                      SizeId = s.ID,
                                      ColorId = leftColor == null ? 0 : leftColor.ID,
                                      ColorName = leftColor == null ? "" : leftColor.color1,
                                      ColorCode = leftColor == null ? "" : leftColor.code,
                                      SizeName = s.size1,
                                      SizeOrderNumber = s.OrderNumber,
                                      SizeDisplayName = s.description,
                                      Quantity = psku.ActualQuantity,
                                      ProductThumbnailURL = AppSettings.ProductPath + "/Files/" + "/Products/" + p.Id + "/" + p.LogoThumbnail
                                  }).OrderBy(zc => new { zc.ColorName, zc.SizeId }).ToList();

                list = collection.GroupBy(p => p.ProductCaratogyId)
                                                              .Select(p => new DYODesignCatagory
                                                              {
                                                                  CatagoryId = p.Key,
                                                                  Catagory = p.FirstOrDefault().ProductCatatgory,
                                                                  CatagoryDispaly = p.FirstOrDefault().ProductCatagoryDisplay,
                                                                  PendingOrders = orders.Count,
                                                                  Products = p.GroupBy(s => new { s.ProductCaratogyId, s.ProductId })
                                                                  .Select(pu => new DYODesignStock
                                                                  {
                                                                      ProductSizes = pu.GroupBy(cv => new { cv.SizeId, cv.ColorId }).Select(zc => new DYODesignSizeStock
                                                                      {
                                                                          Quantity = zc.Where(fg => fg.SizeId == zc.FirstOrDefault().SizeId && fg.ColorId == zc.FirstOrDefault().ColorId).Sum(px => px.Quantity).Value,
                                                                          SizeDisplayName = zc.FirstOrDefault().SizeDisplayName,
                                                                          SizeId = zc.FirstOrDefault().SizeId,
                                                                          SizeName = zc.FirstOrDefault().SizeName,
                                                                          Color = zc.FirstOrDefault().ColorName,
                                                                          ColorCode = zc.FirstOrDefault().ColorCode,
                                                                          ColorId = zc.FirstOrDefault().ColorId,
                                                                          OrderNumber = zc.FirstOrDefault().SizeOrderNumber.HasValue ? zc.FirstOrDefault().SizeOrderNumber.Value : 100
                                                                      }).ToList(),
                                                                      ProductThumbnailURL = pu.FirstOrDefault().ProductThumbnailURL,
                                                                      ProductCode = pu.FirstOrDefault().ProductCode == "DSS1235" ? "LSSO003" : pu.FirstOrDefault().ProductCode,
                                                                      ProductId = pu.FirstOrDefault().ProductId,
                                                                      ProductName = pu.FirstOrDefault().ProductName,
                                                                      Quantity = p.Where(pz => pz.ProductId == pu.FirstOrDefault().ProductId).Sum(px => px.Quantity).Value
                                                                  }).OrderBy(pm => pm.ProductId).ToList()
                                                              }).OrderBy(p => p.CatagoryId).ToList();

                foreach (var item in list)
                {
                    if (isStock == true)
                    {
                        if (item.CatagoryId == 2)
                        {
                            item.IsDisable = true;
                        }
                    }
                    if (isSocks == true)
                    {
                        if (item.CatagoryId == 1)
                        {
                            item.IsDisable = true;
                        }
                    }
                }
            }





            return list;
        }
        public int sum(int a)
        {
            return 0;
        }
        public int sum(float a)
        {
            return 0;
        }
        public bool AddTOCollection(int orderId)
        {
            var order = dbContext.Orders.Find(orderId);
            if (order != null)
            {
                order.OrderStatusId = (byte)WMSOrderStatusEnum.Draft;
                dbContext.SaveChanges();
                return true;
            }

            return false;
        }

        public DYOOrderDetail OrderDetail(int orderId)
        {
            DYOOrderDetail orderDetail = new DYOOrderDetail();
            orderDetail.OrderId = orderId;
            getOrderDetail(orderDetail);

            throw new NotImplementedException();
        }

        public OrderTrackAndTraceInitials OrderTRackAndTraceInitials(int userId)
        {
            OrderTrackAndTraceInitials initials = new OrderTrackAndTraceInitials();
            try
            {
                initials.Customers = (from r in dbContext.WMSUsers
                                      join ur in dbContext.WMSUserRoles on r.Id equals ur.UserId
                                      where ur.RoleId == (byte)WMSUserRoleEnum.Customer
                                      where r.IsActive == true
                                      select new WMSActiveCustomer
                                      {
                                          Company = (r.CompanyName == "" || r.CompanyName == null) ? r.ContactFirstName + " " + r.ContactLastName : r.CompanyName,
                                          Email = r.Email,
                                          Name = r.ContactFirstName + " " + r.ContactLastName,
                                          UserId = r.Id
                                      }
                           ).OrderBy(z => z.Company).ToList();

                initials.Status = (from r in dbContext.OrderStatus
                                   where r.Type != "OrderDesign"
                                   select new WMSOrderStatus
                                   {
                                       StatusId = r.Id,
                                       StatusName = r.StatusName,
                                       StatusDisplay = r.StatusDisplay,
                                       Image = r.Image
                                   }
                                   ).Where(p => p.StatusId != (byte)WMSOrderStatusEnum.Draft).ToList();
            }
            catch (Exception ex)
            {

            }

            return initials;
        }

        private void getOrderDetail(DYOOrderDetail orderDetail)
        {
            var dbOrderDetail = dbContext.Orders.Find(orderDetail.OrderId);
            if (dbOrderDetail != null)
            {
                var userDetail = (from r in dbContext.WMSUsers
                                  join ua in dbContext.WMSUserAddresses on r.Id equals ua.UserId
                                  join c in dbContext.Countries on ua.CountryId equals c.CountryId
                                  join tz in dbContext.Timezones on c.TimeZoneId equals tz.TimezoneId
                                  where r.Id == dbOrderDetail.CreatedBy
                                  select new
                                  {
                                      Name = r.ContactFirstName + " " + r.ContactLastName,
                                      TimeZone = new TimeZoneModal
                                      {
                                          Name = tz.Name,
                                          Offset = tz.Offset,
                                          OffsetShort = tz.OffsetShort,
                                          TimezoneId = tz.TimezoneId
                                      }

                                  }).FirstOrDefault();
                if (userDetail != null)
                {
                    var TimeZoneInformation = TimeZoneInfo.FindSystemTimeZoneById(userDetail.TimeZone.Name);
                    //if (result.CollectionTime.HasValue)
                    //{
                    //    dbDetail.ReferenceDetail.CollectionDate = DateTime.UtcNow;
                    //    dbDetail.ReferenceDetail.CollectionTime = UtilityRepository.Get24HourFormatedTime(UtilityRepository.UtcDateToOtherTimezone(Convert.ToDateTime(result.CollectionDate), result.CollectionTime.Value, TimeZoneInformation).Item2);
                    //}
                }

            }
        }

        #endregion

        #region Design Initials
        public DYOProductDesign GetDesignDetail(int designId)
        {
            DYOProductDesign design = new DYOProductDesign();

            var dbDesign = dbContext.OrderDesigns.Find(designId);
            if (dbDesign != null)
            {
                design.OrderDesignId = dbDesign.Id;
                design.OrderDesignName = dbDesign.DesignName;
                design.OrderDesignStatusId = dbDesign.OrderDesignStatusId.HasValue ? dbDesign.OrderDesignStatusId.Value : 0;
                design.ProductColor = dbDesign.ProductColor;
                design.IsDefaultLogo = dbDesign.IsDeafultLogo;
                design.OrderId = dbDesign.OrderId.HasValue ? dbDesign.OrderId.Value : 0;

                var product = dbContext.ProductMasters.Find(dbDesign.ProductId);
                if (product.ProductCode == "DSS1234" || product.ProductCode == "DSS1235")
                {
                    design.ProductType = "SockRange";

                }
                else
                {
                    design.ProductType = "StockRange";
                }
                design.ProductTwoDImageBase64 = getImageNase64(dbDesign.OrderId.Value, designId, dbDesign.ProductDesignImage);
                design.CreatedBy = dbDesign.CreatedBy;
                design.CustomerId = dbDesign.CustomerId;
                design.ProductTwoDImageName = dbDesign.ProductDesignImage;
                design.ProductId = dbDesign.ProductId;
                design.ProductSections = new List<ProductSection>();
                var dbStyles = dbContext.OrderDesignStyles.Where(p => p.OrderDesignId == designId).ToList();
                foreach (var item in dbStyles)
                {
                    ProductSection sec = new ProductSection();
                    sec.ProductSectionId = item.Id;
                    sec.SectionColor = item.Color;
                    sec.SectionNumber = item.Section;
                    design.ProductSections.Add(sec);
                }

                var dbEmblishments = dbContext.OrderEmblishments.Where(p => p.OrderDesignId == designId).ToList();
                design.Embelishments = new List<DYOProductDesignEmblishment>();
                if (dbEmblishments.Count > 0)
                {
                    foreach (var item in dbEmblishments)
                    {
                        DYOProductDesignEmblishment emb = new DYOProductDesignEmblishment();
                        if (item.EmblishmentType != EmblishmentOptionEnum.PlayerNameType && item.EmblishmentType != EmblishmentOptionEnum.PlayerNumberType && item.EmblishmentType != EmblishmentOptionEnum.InsertTextType)
                        {
                            emb.LogoImageBase64 = getImageNase64(dbDesign.OrderId.Value, designId, item.LogoImage);
                        }

                        emb.OrderEmblishmentId = item.Id;
                        emb.OrderDesignId = item.OrderDesignId;
                        emb.EmblishmentType = item.EmblishmentType;
                        emb.FontColor = item.Color;
                        emb.FontFamily = item.FontFamily;
                        emb.IsActive = true;
                        emb.LogoDimension = new LogoDimension();
                        emb.LogoDimension.DimensionUnit = item.DimensionUnit;
                        emb.LogoDimension.Height = item.LogoHeight.HasValue ? item.LogoHeight.Value : 0M;
                        emb.LogoDimension.Width = item.LogoWidth.HasValue ? item.LogoWidth.Value : 0M;
                        emb.LogoDimension.IsAspectRatio = item.IsAspectRatio.HasValue ? item.IsAspectRatio.Value : false;
                        emb.PrintMethod = item.PrintMethod;
                        emb.LogoColor = item.Color;
                        emb.LogoName = item.LogoImage;
                        emb.CustomColors = new List<DYOOrderEmblishmentDetail>();
                        var dbEmblishmentDetails = dbContext.OrderEmblishmentDetails.Where(p => p.OrderEmblishmentId == item.Id).ToList();
                        foreach (var embd in dbEmblishmentDetails)
                        {
                            DYOOrderEmblishmentDetail detail = new DYOOrderEmblishmentDetail();

                            detail.Color = embd.Color;
                            detail.OrderEmblishmentDetailId = embd.Id;
                            detail.OrderEmblishmentId = embd.OrderEmblishmentId;
                            detail.Type = embd.Type;
                            emb.CustomColors.Add(detail);
                        }
                        design.Embelishments.Add(emb);
                    }
                }
                else
                {
                    design.IsRawGarment = true;
                }
            }
            return design;
        }



        public ProductDesignInitials DesignInitials(int productId, int customerId)
        {
            ProductDesignInitials initial = new ProductDesignInitials();

            initial.ProductId = productId;
            initial.CustomerId = customerId;

            var customer = dbContext.WMSUsers.Find(customerId);
            if (customer != null)
            {
                initial.CustomerName = !string.IsNullOrEmpty(customer.CompanyName) ? customer.CompanyName : customer.ContactFirstName + " " + customer.ContactLastName;
            }

            var product = dbContext.ProductMasters.Find(productId);
            if (product != null)
            {
                //  initial.Emblishment = new DYODesignProductEmblishment();

                if (product.ProductCode == "DSSTSHM001" ||
                    product.ProductCode == "DSSTSHJ001" ||
                    product.ProductCode == "DSSTSHM002" ||
                    product.ProductCode == "DSSTSHL002" ||
                    product.ProductCode == "DSSTPTM001"
                    )
                {
                    initial.IsDefaultLogo = true;
                }
                else
                {
                    initial.IsDefaultLogo = false;
                }

                initial.EmblishmentOptions = new List<EmblishmentOption>();

                var productStyles = dbContext.ProductStyles.Where(p => p.ProductId == productId).ToList();

                if (productStyles.Count > 0)
                {
                    initial.ProductType = "SockRange";
                }
                else
                {
                    initial.ProductType = "StckRange";
                }
                //  initial.Emblishment.IsUpperArmRight = product.IsUpperArmRight;
                if (product.IsUpperArmRight)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.UpperArmRightType, EmblishmentOptionEnum.UpperArmRightDisplay));
                }


                //  initial.Emblishment.IsLowerArmRight = product.IsLowerArmRight;
                if (product.IsLowerArmRight)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.LowerArmRightType, EmblishmentOptionEnum.LowerArmRightDisplay));
                }

                //    initial.Emblishment.IsUpperArmLeft = product.IsUpperArmLeft;
                if (product.IsUpperArmLeft)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.UpperArmLeftType, EmblishmentOptionEnum.UpperArmLeftDisplay));
                }


                // initial.Emblishment.IsLowerArmLeft = product.IsLowerArmLeft;
                if (product.IsLowerArmLeft)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.LowerArmLeftType, EmblishmentOptionEnum.LowerArmLeftDisplay));
                }
                if (product.IsLeftShoulder)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.LeftShoulder, EmblishmentOptionEnum.LeftShoulderDisplay));
                }
                if (product.IsRightShoulder)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.RightShoulder, EmblishmentOptionEnum.RightShoulderDisplay));
                }
                // initial.Emblishment.IsChestCenterTop = product.IsChestCenterTop;
                if (product.IsChestCenterTop)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.ChestCenterTopType, EmblishmentOptionEnum.ChestCenterTopDisplay));
                }

                //  initial.Emblishment.IsRightChest = product.IsRightChest;
                if (product.IsRightChest)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.RightChestType, EmblishmentOptionEnum.RightChestDisplay));
                }

                // initial.Emblishment.IsLeftChest = product.IsLeftChest;
                if (product.IsLeftChest)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.LeftChestType, EmblishmentOptionEnum.LeftChestDisplay));
                }

                // initial.Emblishment.IsFrontMain = product.IsFrontMain;
                if (product.IsFrontMain)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.FrontMainType, EmblishmentOptionEnum.FrontMainDisplay));
                }

                //  initial.Emblishment.IsBackNeck = product.IsBackNeck;
                if (product.IsBackNeck)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.BackNeckType, EmblishmentOptionEnum.BackNeckDisplay));
                }

                // initial.Emblishment.IsPlayerName = product.IsPlayerName;
                if (product.IsPlayerName)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.PlayerNameType, EmblishmentOptionEnum.PlayerNameDisplay));
                }

                //  initial.Emblishment.IsPlayerNumber = product.IsPlayerNumber;
                if (product.IsPlayerNumber)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.PlayerNumberType, EmblishmentOptionEnum.PlayerNumberDisplay));
                }

                //initial.Emblishment.IsMidBack = product.IsMidBack;
                if (product.IsMidBack)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.MidBackType, EmblishmentOptionEnum.MidBackDisplay));
                }

                //  initial.Emblishment.IsLowerBack = product.IsLowerBack;
                if (product.IsLowerBack)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.LowerBackType, EmblishmentOptionEnum.LowerBackDisplay));
                }

                //  initial.Emblishment.IsLowerSleeveRight = product.IsLowerSleeveRight;
                if (product.IsLowerSleeveRight)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.LowerSleeveRightType, EmblishmentOptionEnum.LowerSleeveRightDisplay));
                }

                // initial.Emblishment.IsLowerSleeveLeft = product.IsLowerSleeveLeft;
                if (product.IsLowerSleeveLeft)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.LowerSleeveLeftType, EmblishmentOptionEnum.LowerSleeveLeftDisplay));
                }

                //  initial.Emblishment.IsThighRight = product.IsThighRight;
                if (product.IsThighRight)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.ThighRightType, EmblishmentOptionEnum.ThighRightDisplay));
                }

                // initial.Emblishment.IsThighLeft = product.IsThighLeft;
                if (product.IsThighLeft)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.ThighLeftType, EmblishmentOptionEnum.ThighLeftDisplay));
                }

                //   initial.Emblishment.IsBackThighLeft = product.IsBackThighLeft;
                if (product.IsBackThighLeft)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.BackThighLeftType, EmblishmentOptionEnum.BackThighLeftDisplay));
                }

                // initial.Emblishment.IsBackThighRight = product.IsBackThighRight;
                if (product.IsBackThighRight)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.BackThighRightType, EmblishmentOptionEnum.BackThighRightDisplay));
                }

                //initial.Emblishment.IsSockFrontRight = product.IsSockFrontRight;
                if (product.IsSockFrontRight)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.SockFrontRightType, EmblishmentOptionEnum.SockFrontRightDisplay));
                }

                //initial.Emblishment.IsSockFrontLeft = product.IsSockFrontLeft;
                if (product.IsSockFrontLeft)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.SockFrontLeftType, EmblishmentOptionEnum.SockFrontLeftDisplay));
                }

                // initial.Emblishment.IsSockCalfLeft = product.IsSockCalfLeft;
                if (product.IsSockCalfLeft)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.SockCalfLeftType, EmblishmentOptionEnum.SockCalfLeftDisplay));
                }

                //   initial.Emblishment.IsSockCalfRight = product.IsSockCalfRight;
                if (product.IsSockCalfRight)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.SockCalfRightType, EmblishmentOptionEnum.SockCalfRightDisplay));
                }
                if (product.IsUploadLogo)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.UploadLogoType, EmblishmentOptionEnum.UplaodLogoDisplay));
                }
                if (product.IsText)
                {
                    initial.EmblishmentOptions.Add(emblishmentOption(product.ProductCode, EmblishmentOptionEnum.InsertTextType, EmblishmentOptionEnum.InsertTextDisplay));
                }

                initial.ProductName = product.ProductName;
                initial.ProductCode = product.ProductCode;
                initial.Product3DModelName = product.ThreeDModelObjFile;
                initial.Product3DModelURL = AppSettings.ProductPath + "Files/Products/" + product.Id + "/" + product.ThreeDModelObjFile;
                initial.NumberOfSection = product.NumberOfSection;


                if (productStyles.Count > 0)
                {
                    initial.ProductType = "SockRange";
                    initial.ProductStyles = new List<DYODesignProductStyle>();
                    DYODesignProductStyle style;
                    foreach (var item in productStyles)
                    {
                        style = new DYODesignProductStyle();
                        style.ProductStyleId = item.Id;
                        style.ProductStyleCode = item.StyleCode;
                        style.ProductStyleThumbnail = item.StyleThumbnail;
                        style.ProductStyleThumbnailURL = AppSettings.ProductPath + "/Files/Products/" + productId + "/Styles/" + item.StyleThumbnail;
                        style.ProductStyle3DModelURL = AppSettings.ProductPath + "/Files/Products/" + productId + "/Styles/" + item.ThreeDStyleObj;
                        style.ProductStyle3DModelName = item.ThreeDStyleObj;
                        style.NumberOfSection = item.SectionNumber;
                        style.IsUploadLogo = item.IsUploadLogo;
                        style.IsText = item.IsText;

                        initial.ProductStyles.Add(style);
                    }

                    initial.Sections = new List<SockRangeSection>();
                    SockRangeSection sec;
                    for (int i = 0; i < initial.NumberOfSection; i++)
                    {
                        sec = new SockRangeSection();
                        if (i == 0)
                        {
                            sec.IsActive = true;
                        }
                        else
                        {
                            sec.IsActive = false;
                        }
                        sec.SectionNumber = (i + 1).ToString();
                        sec.SectionColors = getColors("");

                        initial.Sections.Add(sec);
                    }
                }
                if (initial.Sections == null || initial.Sections.Count == 0)
                {
                    initial.ProductType = "StockRange";
                    initial.ProductColors = (from r in dbContext.ProductSKUs
                                             join c in dbContext.Colors on r.ColorId equals c.ID
                                             where r.ProductId == productId
                                             select new WMSColor
                                             {
                                                 Color = c.color1,
                                                 ColorCode = c.code,
                                                 IsActive = false
                                             }
                              ).Distinct().ToList();
                    if (initial.ProductColors.Count == 0)
                    {
                        initial.ProductColors = getColors("");
                    }
                }
            }
            return initial;
        }

        private int getDimensionLimit(int dimension)
        {
            if (dimension > 0)
            {
                int data = dimension - (dimension * 70) / 100;
                return data;
            }
            return 0;
        }
        private List<EmblishCustomColor> colorTypes()
        {
            List<EmblishCustomColor> list = new List<EmblishCustomColor>();
            EmblishCustomColor item = new EmblishCustomColor();
            item.ColorType = "";
            item.CustomColor = "";

            item.ColorTypes = new List<EmblishMethod>() {
                                     new EmblishMethod () { Key="RGB", Value ="RGB (R,G,B)" },
                                     new EmblishMethod() { Key = "CMYK", Value = "CMYK (C%,M%,Y%,K%)" },
                                     new EmblishMethod() { Key ="Pentose", Value = "Pantone" },
                                     new EmblishMethod() { Key = "Other", Value = "Others" }
            };
            list.Add(item);
            return list;
        }

        private EmblishmentOption emblishmentOption(string styleCode, string type, string display)
        {
            EmblishmentOption option = new EmblishmentOption();

            option.EmplishmentType = type;
            option.EmplishmentTypeDisplay = display;
            option.HeightRange = getDimesionRange();
            option.WidthRange = getDimesionRange();

            option.ColorTypes = colorTypes();

            #region RHS
            if (type == EmblishmentOptionEnum.UpperArmRightType)
            {
                //  Men T-shits
                if (styleCode == "DSSTTEM001" || styleCode == "DSSTPOM001")
                {
                    option.LogoMaxWidth = 70;
                    option.LogoMaxHeight = 60;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }
                // Women T-shits
                if (styleCode == "DSSTPOL001" || styleCode == "DSSTTEL001")
                {
                    option.LogoMaxWidth = 60;
                    option.LogoMaxHeight = 40;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }
                // Junior T-shits
                if (styleCode == "DSSTTEJ001")
                {
                    option.LogoMaxWidth = 50;
                    option.LogoMaxHeight = 40;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }

                // Men shorts
                if (styleCode == "DSSTSHM001" || styleCode == "DSSTSHM002")
                {

                }

                // Women shorts
                if (styleCode == "DSSTSHL002")
                {

                }

                // Junior shorts
                if (styleCode == "DSSTSHJ001")
                {

                }

                // Track pants
                if (styleCode == "DSSTPTM001")
                {

                }

                // Jackets
                if (styleCode == "DSSTJKM004")
                {

                }

                // Hoodies
                if (styleCode == "DSSTHOM001" || styleCode == "DSSTHOM001")
                {


                }
            }
            #endregion

            #region LHS
            else if (type == EmblishmentOptionEnum.UpperArmLeftType)
            {
                //  Men T-shits 
                if (styleCode == "DSSTTEM001" || styleCode == "DSSTPOM001")
                {
                    option.LogoMaxWidth = 70;
                    option.LogoMaxHeight = 60;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }
                // Women T-shits
                if (styleCode == "DSSTPOL001" || styleCode == "DSSTTEL001")
                {
                    option.LogoMaxWidth = 60;
                    option.LogoMaxHeight = 40;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }
                // Junior T-shits
                if (styleCode == "DSSTTEJ001")
                {
                    option.LogoMaxWidth = 50;
                    option.LogoMaxHeight = 40;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }

                // Men shorts
                if (styleCode == "DSSTSHM001" || styleCode == "DSSTSHM002")
                {

                }

                // Women shorts
                if (styleCode == "DSSTSHL002")
                {

                }

                // Junior shorts
                if (styleCode == "DSSTSHJ001")
                {

                }

                // Track pants
                if (styleCode == "DSSTPTM001")
                {

                }

                // Jackets
                if (styleCode == "DSSTJKM004")
                {

                }

                // Hoodies
                if (styleCode == "DSSTHOM001" || styleCode == "DSSTHOM001")
                {


                }
            }
            #endregion
            #region RHC 
            else if (type == EmblishmentOptionEnum.RightChestType)
            {
                //  Men T-shits 
                if (styleCode == "DSSTTEM001" || styleCode == "DSSTPOM001")
                {
                    option.LogoMaxWidth = 100;
                    option.LogoMaxHeight = 100;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }
                // Women T-shits
                if (styleCode == "DSSTPOL001" || styleCode == "DSSTTEL001")
                {
                    option.LogoMaxWidth = 90;
                    option.LogoMaxHeight = 90;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }
                // Junior T-shits
                if (styleCode == "DSSTTEJ001")
                {
                    option.LogoMaxWidth = 70;
                    option.LogoMaxHeight = 70;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }

                // Men shorts
                if (styleCode == "DSSTSHM001" || styleCode == "DSSTSHM002")
                {

                }

                // Women shorts
                if (styleCode == "DSSTSHL002")
                {

                }

                // Junior shorts
                if (styleCode == "DSSTSHJ001")
                {

                }

                // Track pants
                if (styleCode == "DSSTPTM001")
                {

                }

                // Jackets
                if (styleCode == "DSSTJKM004")
                {
                    option.LogoMaxWidth = 90;
                    option.LogoMaxHeight = 80;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }

                // Hoodies men
                if (styleCode == "DSSTHOM001")
                {

                    option.LogoMaxWidth = 100;
                    option.LogoMaxHeight = 100;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }
                // Hoodies Jnr
                if (styleCode == "DSSTHOJ001")
                {
                    option.LogoMaxWidth = 70;
                    option.LogoMaxHeight = 60;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);

                }

            }
            #endregion
            #region LHC
            else if (type == EmblishmentOptionEnum.LeftChestType)
            {
                //  Men T-shits 
                if (styleCode == "DSSTTEM001" || styleCode == "DSSTPOM001")
                {
                    option.LogoMaxWidth = 100;
                    option.LogoMaxHeight = 100;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }
                // Women T-shits
                if (styleCode == "DSSTPOL001" || styleCode == "DSSTTEL001")
                {
                    option.LogoMaxWidth = 90;
                    option.LogoMaxHeight = 90;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }

                // Junior T-shits
                if (styleCode == "DSSTTEJ001")
                {
                    option.LogoMaxWidth = 70;
                    option.LogoMaxHeight = 70;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }

                // Men shorts
                if (styleCode == "DSSTSHM001" || styleCode == "DSSTSHM002")
                {

                }

                // Women shorts
                if (styleCode == "DSSTSHL002")
                {

                }

                // Junior shorts
                if (styleCode == "DSSTSHJ001")
                {

                }

                // Track pants
                if (styleCode == "DSSTPTM001")
                {

                }

                // Jackets
                if (styleCode == "DSSTJKM004")
                {
                    option.LogoMaxWidth = 90;
                    option.LogoMaxHeight = 80;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }

                // Hoodies men
                if (styleCode == "DSSTHOM001")
                {

                    option.LogoMaxWidth = 100;
                    option.LogoMaxHeight = 100;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }
                // Hoodies Jnr
                if (styleCode == "DSSTHOJ001")
                {
                    option.LogoMaxWidth = 70;
                    option.LogoMaxHeight = 60;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);

                }
            }
            #endregion
            #region TB
            else if (type == EmblishmentOptionEnum.BackNeckType)
            {
                //  Men T-shits 
                if (styleCode == "DSSTTEM001" || styleCode == "DSSTPOM001")
                {
                    option.LogoMaxWidth = 180;
                    option.LogoMaxHeight = 70;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }
                // Women T-shits
                if (styleCode == "DSSTPOL001" || styleCode == "DSSTTEL001")
                {
                    option.LogoMaxWidth = 160;
                    option.LogoMaxHeight = 60;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }
                // Junior T-shits
                if (styleCode == "DSSTTEJ001")
                {
                    option.LogoMaxWidth = 100;
                    option.LogoMaxHeight = 50;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }

                // Men shorts
                if (styleCode == "DSSTSHM001" || styleCode == "DSSTSHM002")
                {

                }

                // Women shorts
                if (styleCode == "DSSTSHL002")
                {

                }

                // Junior shorts
                if (styleCode == "DSSTSHJ001")
                {

                }

                // Track pants
                if (styleCode == "DSSTPTM001")
                {

                }

                // Jackets
                if (styleCode == "DSSTJKM004")
                {
                    option.LogoMaxWidth = 180;
                    option.LogoMaxHeight = 60;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }

                // Hoodies
                if (styleCode == "DSSTHOM001" || styleCode == "DSSTHOM001")
                {


                }
            }
            #endregion
            #region FLT
            else if (type == EmblishmentOptionEnum.ThighLeftType)
            {
                //  Men T-shits 
                if (styleCode == "DSSTTEM001" || styleCode == "DSSTPOM001")
                {

                }
                // Women T-shits
                if (styleCode == "DSSTPOL001" || styleCode == "DSSTTEL001")
                {

                }
                // Junior T-shits
                if (styleCode == "DSSTTEJ001")
                {

                }

                // Men shorts zym
                if (styleCode == "DSSTSHM002")
                {
                    option.LogoMaxWidth = 100;
                    option.LogoMaxHeight = 70;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }
                // Men shorts ruby
                if (styleCode == "DSSTSHM001")
                {
                    option.LogoMaxWidth = 120;
                    option.LogoMaxHeight = 110;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }

                // Women shorts
                if (styleCode == "DSSTSHL002")
                {
                    option.LogoMaxWidth = 90;
                    option.LogoMaxHeight = 50;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }

                // Junior shorts
                if (styleCode == "DSSTSHJ001")
                {
                    option.LogoMaxWidth = 80;
                    option.LogoMaxHeight = 40;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }

                // Track pants
                if (styleCode == "DSSTPTM001")
                {
                    option.LogoMaxWidth = 90;
                    option.LogoMaxHeight = 50;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }

                // Jackets
                if (styleCode == "DSSTJKM004")
                {

                }

                // Hoodies
                if (styleCode == "DSSTHOM001" || styleCode == "DSSTHOM001")
                {


                }

            }
            #endregion
            #region  BRT
            else if (type == EmblishmentOptionEnum.BackThighRightType)
            {
                //  Men T-shits 
                if (styleCode == "DSSTTEM001" || styleCode == "DSSTPOM001")
                {

                }
                // Women T-shits
                if (styleCode == "DSSTPOL001" || styleCode == "DSSTTEL001")
                {

                }
                // Junior T-shits
                if (styleCode == "DSSTTEJ001")
                {

                }

                // Men shorts zym 
                if (styleCode == "DSSTSHM002")
                {
                    option.LogoMaxWidth = 100;
                    option.LogoMaxHeight = 70;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }

                // Men shorts rugby
                if (styleCode == "DSSTSHM001")
                {
                    option.LogoMaxWidth = 150;
                    option.LogoMaxHeight = 80;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);

                }

                // Women shorts
                if (styleCode == "DSSTSHL002")
                {
                    option.LogoMaxWidth = 90;
                    option.LogoMaxHeight = 50;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }

                // Junior shorts
                if (styleCode == "DSSTSHJ001")
                {
                    option.LogoMaxWidth = 80;
                    option.LogoMaxHeight = 40;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }

                // Track pants
                if (styleCode == "DSSTPTM001")
                {

                }

                // Jackets
                if (styleCode == "DSSTJKM004")
                {

                }

                // Hoodies
                if (styleCode == "DSSTHOM001" || styleCode == "DSSTHOM001")
                {


                }
            }
            #endregion
            #region BLT
            else if (type == EmblishmentOptionEnum.BackThighLeftType)
            {
                //  Men T-shits 
                if (styleCode == "DSSTTEM001" || styleCode == "DSSTPOM001")
                {

                }
                // Women T-shits
                if (styleCode == "DSSTPOL001" || styleCode == "DSSTTEL001")
                {

                }
                // Junior T-shits
                if (styleCode == "DSSTTEJ001")
                {

                }

                // Men shorts Zym 
                if (styleCode == "DSSTSHM002")
                {
                    option.LogoMaxWidth = 100;
                    option.LogoMaxHeight = 70;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }

                // Men shorts ruby 
                if (styleCode == "DSSTSHM001")
                {
                    option.LogoMaxWidth = 150;
                    option.LogoMaxHeight = 80;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }

                // Women shorts
                if (styleCode == "DSSTSHL002")
                {
                    option.LogoMaxWidth = 90;
                    option.LogoMaxHeight = 50;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }

                // Junior shorts
                if (styleCode == "DSSTSHJ001")
                {
                    option.LogoMaxWidth = 80;
                    option.LogoMaxHeight = 40;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }

                // Track pants
                if (styleCode == "DSSTPTM001")
                {

                }

                // Jackets
                if (styleCode == "DSSTJKM004")
                {

                }

                // Hoodies
                if (styleCode == "DSSTHOM001" || styleCode == "DSSTHOM001")
                {


                }

            }
            #endregion
            else if (type == EmblishmentOptionEnum.ChestCenterTopType)
            {
                //  Men T-shits 
                if (styleCode == "DSSTTEM001" || styleCode == "DSSTPOM001")
                {

                }
                // Women T-shits
                if (styleCode == "DSSTPOL001" || styleCode == "DSSTTEL001")
                {

                }
                // Junior T-shits
                if (styleCode == "DSSTTEJ001")
                {

                }

                // Men shorts
                if (styleCode == "DSSTSHM001" || styleCode == "DSSTSHM002")
                {

                }

                // Women shorts
                if (styleCode == "DSSTSHL002")
                {

                }

                // Junior shorts
                if (styleCode == "DSSTSHJ001")
                {

                }

                // Track pants
                if (styleCode == "DSSTPTM001")
                {
                }
                // Jackets
                if (styleCode == "DSSTJKM004")
                {
                }
                // Hoodies
                if (styleCode == "DSSTHOM001" || styleCode == "DSSTHOJ001")
                {
                    if (styleCode == "DSSTHOM001")
                    {
                        option.LogoMaxWidth = 250;
                        option.LogoMaxHeight = 160;
                        option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                        option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                    }
                    if (styleCode == "DSSTHOJ001")
                    {
                        option.LogoMaxWidth = 200;
                        option.LogoMaxHeight = 140;
                        option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                        option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                    }
                }
            }
            #region LHSD
            else if (type == EmblishmentOptionEnum.LeftShoulder)
            {
                //  Men T-shits 
                if (styleCode == "DSSTTEM001" || styleCode == "DSSTPOM001")
                {

                }
                // Women T-shits
                if (styleCode == "DSSTPOL001" || styleCode == "DSSTTEL001")
                {

                }
                // Junior T-shits
                if (styleCode == "DSSTTEJ001")
                {

                }

                // Men shorts
                if (styleCode == "DSSTSHM001" || styleCode == "DSSTSHM002")
                {

                }

                // Women shorts
                if (styleCode == "DSSTSHL002")
                {

                }

                // Junior shorts
                if (styleCode == "DSSTSHJ001")
                {

                }

                // Track pants
                if (styleCode == "DSSTPTM001")
                {

                }

                // Jackets
                if (styleCode == "DSSTJKM004")
                {
                    option.LogoMaxWidth = 70;
                    option.LogoMaxHeight = 30;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }
                // Hoodies
                if (styleCode == "DSSTHOM001" || styleCode == "DSSTHOM001")
                {

                }

            }
            #endregion
            #region RHSD
            else if (type == EmblishmentOptionEnum.RightShoulder)
            {
                //  Men T-shits 
                if (styleCode == "DSSTTEM001" || styleCode == "DSSTPOM001")
                {

                }
                // Women T-shits
                if (styleCode == "DSSTPOL001" || styleCode == "DSSTTEL001")
                {

                }
                // Junior T-shits
                if (styleCode == "DSSTTEJ001")
                {

                }

                // Men shorts
                if (styleCode == "DSSTSHM001" || styleCode == "DSSTSHM002")
                {

                }

                // Women shorts
                if (styleCode == "DSSTSHL002")
                {

                }

                // Junior shorts
                if (styleCode == "DSSTSHJ001")
                {

                }

                // Track pants
                if (styleCode == "DSSTPTM001")
                {

                }

                // Jackets
                if (styleCode == "DSSTJKM004")
                {
                    option.LogoMaxWidth = 70;
                    option.LogoMaxHeight = 30;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }

                // Hoodies
                if (styleCode == "DSSTHOM001" || styleCode == "DSSTHOM001")
                {


                }
            }
            #endregion
            #region Lower Back
            else if (type == EmblishmentOptionEnum.LowerBackType)
            {
                //  Men T-shits 
                if (styleCode == "DSSTTEM001" || styleCode == "DSSTPOM001")
                {

                }
                // Women T-shits
                if (styleCode == "DSSTPOL001" || styleCode == "DSSTTEL001")
                {

                }
                // Junior T-shits
                if (styleCode == "DSSTTEJ001")
                {

                }

                // Men shorts
                if (styleCode == "DSSTSHM001" || styleCode == "DSSTSHM002")
                {

                }

                // Women shorts
                if (styleCode == "DSSTSHL002")
                {

                }

                // Junior shorts
                if (styleCode == "DSSTSHJ001")
                {

                }

                // Track pants
                if (styleCode == "DSSTPTM001")
                {

                }

                // Jackets
                if (styleCode == "DSSTJKM004")
                {
                    option.LogoMaxWidth = 180;
                    option.LogoMaxHeight = 60;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }

                // Hoodies
                if (styleCode == "DSSTHOM001" || styleCode == "DSSTHOM001")
                {


                }
            }
            #endregion
            else
            {
                //  Men T-shits 
                if (styleCode == "DSSTTEM001" || styleCode == "DSSTPOM001")
                {

                }
                // Women T-shits
                if (styleCode == "DSSTPOL001" || styleCode == "DSSTTEL001")
                {

                }
                // Junior T-shits
                if (styleCode == "DSSTTEJ001")
                {

                }

                // Men shorts
                if (styleCode == "DSSTSHM001" || styleCode == "DSSTSHM002")
                {

                }

                // Women shorts
                if (styleCode == "DSSTSHL002")
                {

                }

                // Junior shorts
                if (styleCode == "DSSTSHJ001")
                {

                }

                // Track pants
                if (styleCode == "DSSTPTM001")
                {

                }

                // Jackets
                if (styleCode == "DSSTJKM004")
                {

                }

                // Hoodies
                if (styleCode == "DSSTHOM001" || styleCode == "DSSTHOM001")
                {


                }
                if (styleCode == "DSS1234" || styleCode == "DSS1235")
                {
                    option.LogoMaxWidth = 10;
                    option.LogoMaxHeight = 10;
                    option.LogoMinHeight = getDimensionLimit(option.LogoMaxHeight);
                    option.LogoMinWidth = getDimensionLimit(option.LogoMaxWidth);
                }
            }
            option.DimensionUnit = "mm";
            if (type == EmblishmentOptionEnum.PlayerNameType || type == EmblishmentOptionEnum.PlayerNumberType || type == EmblishmentOptionEnum.InsertTextType)
            {
                //  Men T-shits 
                if (styleCode == "DSSTTEM001" || styleCode == "DSSTPOM001")
                {

                }
                // Women T-shits
                if (styleCode == "DSSTPOL001" || styleCode == "DSSTTEL001")
                {

                }
                // Junior T-shits
                if (styleCode == "DSSTTEJ001")
                {

                }

                // Men shorts
                if (styleCode == "DSSTSHM001" || styleCode == "DSSTSHM002")
                {

                }

                // Women shorts
                if (styleCode == "DSSTSHL002")
                {

                }

                // Junior shorts
                if (styleCode == "DSSTSHJ001")
                {

                }

                // Track pants
                if (styleCode == "DSSTPTM001")
                {

                }

                // Jackets
                if (styleCode == "DSSTJKM004")
                {

                }
                // Hoodies
                if (styleCode == "DSSTHOM001" || styleCode == "DSSTHOM001")
                {


                }
                if (styleCode == "DSS1234" || styleCode == "DSS1235")
                {
                    option.FontColors = getColors("");
                    option.PrintingMethods = getPrintingMethods(type, styleCode, "PrintMethods");
                }
                else
                {
                    option.FontColors = getColors(type);
                    option.PrintingMethods = getPrintingMethods("");
                }

                // For Name and Number Add font family
                option.FontFamilies = getPrintingMethods("FontFamily");

                option.PrintMethod = option.PrintingMethods[0].Key;
            }
            else
            {
                if (styleCode == "DSS1234" || styleCode == "DSS1235")
                {
                    option.LogoColors = getColors("");
                }
                else
                {
                    option.LogoColors = getColors(type);
                }
                option.PrintingMethods = getPrintingMethods(type, styleCode, "PrintMethods");

            }

            return option;

        }

        public List<WMSColor> getThermoColor(string printMethod)
        {
            if (printMethod == "Thermo")
            {
                return new List<WMSColor> {new WMSColor { ColorCode = "#231F20", Color = "Black", IsActive = false },
                        new WMSColor { ColorCode = "#FEFEFE", Color = "White", IsActive = false },
                        new WMSColor { ColorCode = "#FFBF44", Color = "Gold", IsActive = false },
                        new WMSColor { ColorCode = "#FCA33F", Color = "Golden Yellow", IsActive = false },
                        new WMSColor { ColorCode = "#F88038", Color = "Orange", IsActive = false },
                        new WMSColor { ColorCode = "#D3A853", Color = "Metal Gold", IsActive = false },
                        new WMSColor { ColorCode = "#7EB7CA", Color = "Cronulla", IsActive = false },
                        new WMSColor { ColorCode = "#7EB7CA", Color = "Sky", IsActive = false },
                        new WMSColor { ColorCode = "#003968", Color = "Royal", IsActive = false },
                        new WMSColor { ColorCode = "#001E30", Color = "Navy", IsActive = false },
                        new WMSColor { ColorCode = "#00854C", Color = "Emerald", IsActive = false },
                        new WMSColor { ColorCode = "#004B25", Color = "Forest", IsActive = false },
                        new WMSColor { ColorCode = "#FBBEB8", Color = "Pink", IsActive = false },
                        new WMSColor { ColorCode = "#D11024", Color = "Red", IsActive = false },
                        new WMSColor { ColorCode = "#99131D", Color = "Maroon", IsActive = false },
                        new WMSColor { ColorCode = "#682F8B", Color = "Purple", IsActive = false },
                        new WMSColor { ColorCode = "#6D3317", Color = "Brown", IsActive = false },
                        new WMSColor { ColorCode = "#CCC9B5", Color = "Grey CDP17", IsActive = false }, // 24
                        new WMSColor { ColorCode = "#C6D0B4", Color = "Silver", IsActive = false },
                        new WMSColor { ColorCode = "#F55731", Color = "Neon Orange", IsActive = false },
                        new WMSColor { ColorCode = "#FFE44D", Color = "Neon Yellow", IsActive = false },
                        new WMSColor { ColorCode = "#88C557", Color = "Neon Green", IsActive = false },
                        new WMSColor { ColorCode = "#F10862", Color = "Neon Pink", IsActive = false }
            };
            }
            else if (printMethod == "Knitted")
            {
                return new List<WMSColor>() {
                        new WMSColor { ColorCode= "#FFFFFF", Color="LS WHITE",IsActive = false },
                        new WMSColor { ColorCode= "#000000", Color="LS BLACK",IsActive = false } ,
                        new WMSColor { ColorCode= "#445A65", Color="LS GUNMETAL",IsActive = false },
                        new WMSColor { ColorCode= "#A7A8A9", Color="LS LIGHT GRAY",IsActive = false },
                        new WMSColor { ColorCode= "#53565A", Color="LS DARK GRAY",IsActive = false },
                        new WMSColor { ColorCode= "#172F4E", Color="LS NAVY",IsActive = false } ,
                        new WMSColor { ColorCode= "#87BFE0", Color="LS LIGHT BLUE",IsActive = false },
                        new WMSColor { ColorCode= "#76AAD2", Color="LS MALIBU",IsActive = false },
                        new WMSColor { ColorCode=  "#2983BB", Color="LS ROYAL",IsActive = false },
                        new WMSColor { ColorCode=  "#1E4773", Color="LS INDIGO",IsActive = false } ,
                        new WMSColor { ColorCode=  "#2A387B", Color="LS OCEAN",IsActive = false } ,
                        new WMSColor { ColorCode=  "#132845", Color="LS INK",IsActive = false } ,
                        new WMSColor { ColorCode=  "#42AE97", Color="LS JADE",IsActive = false } ,
                        new WMSColor { ColorCode=  "#5FA65E", Color="LS GREEN",IsActive = false } ,
                        new WMSColor { ColorCode=  "#44AE6B", Color="LS EMERALD",IsActive = false } ,
                        new WMSColor { ColorCode=  "#194E36 ", Color="LS NEW FOREST",IsActive = false } ,
                        new WMSColor { ColorCode=  "#701A33", Color="LS MAROON",IsActive = false } ,
                        new WMSColor { ColorCode=  "#DD333B", Color="LS BRIGHT RED",IsActive = false } ,
                        new WMSColor { ColorCode=  "#C7334A", Color="LS RED",IsActive = false } ,
                        new WMSColor { ColorCode=  "#B9303A", Color="LS SCARLET",IsActive = false } ,
                        new WMSColor { ColorCode=  "#54357B", Color="LS GRAPE",IsActive = false } ,
                        new WMSColor { ColorCode=  "#4C3266", Color="LS PURPLE",IsActive = false },
                        new WMSColor { ColorCode=  "#C73266", Color="LS PINK",IsActive = false },
                        new WMSColor { ColorCode=  "#D979A7", Color="LS LIGHT PINK",IsActive = false },
                        new WMSColor { ColorCode=  "#DA5C3E", Color="LS ORANGE",IsActive = false },
                        new WMSColor { ColorCode=  "#E9814E", Color="LS LIGHT ORANGE",IsActive = false },
                        new WMSColor { ColorCode=  "#F0BC5A", Color="LS GOLD",IsActive = false },
                        new WMSColor { ColorCode=  "#F9C364", Color="LS YELLOW",IsActive = false },
                        new WMSColor { ColorCode=  "#CAA369", Color="LS METAL GOLD",IsActive = false },
                        new WMSColor { ColorCode=  "#58452C", Color="LS BROWN",IsActive = false },
                        new WMSColor { ColorCode=  "#06A0D3", Color="LS BRIGHT BLUE",IsActive = false },
                        new WMSColor { ColorCode=  "#D64E91", Color="LS BRIGHT PINK",IsActive = false },
                        new WMSColor { ColorCode=  "#FFEC6B", Color="LS BRIGHT YELLOW",IsActive = false },
                        new WMSColor { ColorCode=  "#CFD868", Color="LS BRIGHT GREEN",IsActive = false }
                    };
            }
            else
            {
                return getColors(printMethod);
            }
        }
        public List<WMSColor> getColors(string type)
        {
            if (type == EmblishmentOptionEnum.PlayerNameType || type == EmblishmentOptionEnum.PlayerNumberType || type == EmblishmentOptionEnum.InsertTextType)
            {
                return new List<WMSColor>() {
                        new WMSColor { ColorCode=  "#231F20", Color="01",IsActive = false },
                        new WMSColor { ColorCode=  "#FEFEFE", Color="02",IsActive = false },
                        new WMSColor { ColorCode=  "#FFBF44", Color="03",IsActive = false },
                        new WMSColor { ColorCode=  "#FCA33F", Color="04",IsActive = false },
                        new WMSColor { ColorCode=  "#F88038", Color="05",IsActive = false },
                        new WMSColor { ColorCode=  "#D3A853", Color="06",IsActive = false },
                        new WMSColor { ColorCode=  "#7EB7CA", Color="07",IsActive = false },
                        new WMSColor { ColorCode=  "#7EB7CA", Color="08",IsActive = false },
                        new WMSColor { ColorCode=  "#003968", Color="09",IsActive = false },
                        new WMSColor { ColorCode=  "#001E30", Color="10",IsActive = false },
                        new WMSColor { ColorCode=  "#00854C", Color="11",IsActive = false },
                        new WMSColor { ColorCode=  "#004B25", Color="12",IsActive = false },
                        new WMSColor { ColorCode=  "#FBBEB8", Color="13",IsActive = false },
                        new WMSColor { ColorCode=  "#D11024", Color="14",IsActive = false },
                        new WMSColor { ColorCode=  "#99131D", Color="15",IsActive = false },
                        new WMSColor { ColorCode=  "#682F8B", Color="16",IsActive = false },
                        new WMSColor { ColorCode=  "#6D3317", Color="17",IsActive = false },
                        new WMSColor { ColorCode=  "#CCC9B5", Color="18",IsActive = false },
                        new WMSColor { ColorCode=  "#C6D0B4", Color="19",IsActive = false },
                        new WMSColor { ColorCode=  "#F55731", Color="20",IsActive = false },
                        new WMSColor { ColorCode=  "#FFE44D", Color="21",IsActive = false },
                        new WMSColor { ColorCode=  "#88C557", Color="22",IsActive = false },
                        new WMSColor { ColorCode=  "#F10862", Color="23",IsActive = false }
                    };
            }

            if (!string.IsNullOrEmpty(type))
            {
                // Logo color  Loin star & Heat Transfer Viny color

                return new List<WMSColor>() {

                        new WMSColor { ColorCode= "#FFFFFF", Color="LS WHITE",IsActive = false },
                        new WMSColor { ColorCode= "#F6FCFA", Color="LS ICE",IsActive = false } ,
                        new WMSColor { ColorCode= "#231F20", Color="LS BLACK ",IsActive = false },
                        new WMSColor { ColorCode= "#CEDCDB", Color="LS SILVER",IsActive = false },
                        new WMSColor { ColorCode= "#738692", Color="LS GRAY",IsActive = false },
                        new WMSColor { ColorCode= "#3F5A67", Color="LS GUNMETAL",IsActive = false } ,
                        new WMSColor { ColorCode= "#A9A8AB", Color="LS LIGHT GRAY",IsActive = false },
                        new WMSColor { ColorCode= "#56565A", Color="LS DARK GRAY",IsActive = false },
                        new WMSColor { ColorCode=  "#112F51", Color="LS NAVY",IsActive = false },
                        new WMSColor { ColorCode=  "#77C0E4", Color="LS LIGHT BLUE",IsActive = false } ,
                        new WMSColor { ColorCode=  "#68ABD7", Color="LS MALLBU",IsActive = false } ,
                        new WMSColor { ColorCode=  "#1584C0", Color="LS ROYAL",IsActive = false } ,
                        new WMSColor { ColorCode=  "#0C4777", Color="LS INDIGO",IsActive = false } ,
                        new WMSColor { ColorCode=  "#122389", Color="LS OCEAN",IsActive = false } ,
                        new WMSColor { ColorCode=  "#0E2848", Color="LS INK",IsActive = false } ,
                        new WMSColor { ColorCode=  "#B1DFD1", Color="LS MINT",IsActive = false } ,
                        new WMSColor { ColorCode=  "#09B295", Color="LS JADE",IsActive = false } ,
                        new WMSColor { ColorCode=  "#3AA956", Color="LS GREEN",IsActive = false } ,
                        new WMSColor { ColorCode=  "#13B264", Color="LS EMERALD",IsActive = false } ,
                        new WMSColor { ColorCode=  "#065134", Color="LS NEW FOREST",IsActive = false } ,
                        new WMSColor { ColorCode=  "#981947", Color="LS LIGHT MAROOM",IsActive = false } ,
                        new WMSColor { ColorCode=  "#7A0E32", Color="LS MAROON",IsActive = false },
                        new WMSColor { ColorCode=  "#580E28", Color="LS DARK MAROON",IsActive = false },
                        new WMSColor { ColorCode=  "#F1002D", Color="LS BRIGHT RED",IsActive = false },
                        new WMSColor { ColorCode=  "#D91A47", Color="LS RED",IsActive = false },
                        new WMSColor { ColorCode=  "#CA1D36", Color="LS SCARLET",IsActive = false },
                        new WMSColor { ColorCode=  "#A71C35", Color="LS DARK RED",IsActive = false },
                        new WMSColor { ColorCode=  "#5D4394", Color="LS LIGHT PURPLE",IsActive = false },
                        new WMSColor { ColorCode=  "#5C2D7D", Color="LS GRAPE",IsActive = false },
                        new WMSColor { ColorCode=  "#542D6A", Color="LS PURPLE",IsActive = false },
                        new WMSColor { ColorCode=  "#CD2F8E", Color="LS FUSCHA",IsActive = false },
                        new WMSColor { ColorCode=  "#DA1164", Color="LS PINK",IsActive = false },
                        new WMSColor { ColorCode=  "#F16FB7", Color="LS LIGHT PINK",IsActive = false },
                        new WMSColor { ColorCode=  "#EC4F1D", Color="LS ORANGE",IsActive = false },
                        new WMSColor { ColorCode=  "#F97C41", Color="LS LIGHT ORANGE",IsActive = false },
                        new WMSColor { ColorCode=  "#F8BD3C", Color="LS GOLD",IsActive = false },
                        new WMSColor { ColorCode=  "#FFC44E", Color="LS YELLOW",IsActive = false },
                        new WMSColor { ColorCode=  "#CFA35E", Color="LS METAL GOLD",IsActive = false },
                        new WMSColor { ColorCode=  "#5D4425", Color="LS BROWN",IsActive = false },
                        new WMSColor { ColorCode=  "#00A3DA", Color="LS BRIGHT BLUE",IsActive = false },
                        new WMSColor { ColorCode=  "#EA3993", Color="LS BRIGHT PINK",IsActive = false },
                        new WMSColor { ColorCode=  "#FFEE4F", Color="LS BRIGHT YELLOW",IsActive = false },
                        new WMSColor { ColorCode=  "#CDDB53", Color="LS BRIGHT GREEN",IsActive = false },
                        new WMSColor { ColorCode=  "#FFFEF0", Color="LS CREAM",IsActive = false },
                        new WMSColor { ColorCode=  "#F2D6BF", Color="LS SAND",IsActive = false },
                        new WMSColor { ColorCode=  "#231F20", Color="01",IsActive = false },
                        new WMSColor { ColorCode=  "#FEFEFE", Color="02",IsActive = false },
                        new WMSColor { ColorCode=  "#FFBF44", Color="03",IsActive = false },
                        new WMSColor { ColorCode=  "#FCA33F", Color="04",IsActive = false },
                        new WMSColor { ColorCode=  "#F88038", Color="05",IsActive = false },
                        new WMSColor { ColorCode=  "#D3A853", Color="06",IsActive = false },
                        new WMSColor { ColorCode=  "#7EB7CA", Color="07",IsActive = false },
                        new WMSColor { ColorCode=  "#7EB7CA", Color="08",IsActive = false },
                        new WMSColor { ColorCode=  "#003968", Color="09",IsActive = false },
                        new WMSColor { ColorCode=  "#001E30", Color="10",IsActive = false },
                        new WMSColor { ColorCode=  "#00854C", Color="11",IsActive = false },
                        new WMSColor { ColorCode=  "#004B25", Color="12",IsActive = false },
                        new WMSColor { ColorCode=  "#FBBEB8", Color="13",IsActive = false },
                        new WMSColor { ColorCode=  "#D11024", Color="14",IsActive = false },
                        new WMSColor { ColorCode=  "#99131D", Color="15",IsActive = false },
                        new WMSColor { ColorCode=  "#682F8B", Color="16",IsActive = false },
                        new WMSColor { ColorCode=  "#6D3317", Color="17",IsActive = false },
                        new WMSColor { ColorCode=  "#CCC9B5", Color="18",IsActive = false },
                        new WMSColor { ColorCode=  "#C6D0B4", Color="19",IsActive = false },
                        new WMSColor { ColorCode=  "#F55731", Color="20",IsActive = false },
                        new WMSColor { ColorCode=  "#FFE44D", Color="21",IsActive = false },
                        new WMSColor { ColorCode=  "#88C557", Color="22",IsActive = false },
                        new WMSColor { ColorCode=  "#F10862", Color="23",IsActive = false }
                    };

            }
            else
            {
                // Sock range color  

                return new List<WMSColor>() {
                        new WMSColor { ColorCode= "#FFFFFF", Color="LS WHITE",IsActive = false },
                        new WMSColor { ColorCode= "#000000", Color="LS BLACK",IsActive = false } ,
                        new WMSColor { ColorCode= "#445A65", Color="LS GUNMETAL",IsActive = false },
                        new WMSColor { ColorCode= "#A7A8A9", Color="LS LIGHT GRAY",IsActive = false },
                        new WMSColor { ColorCode= "#53565A", Color="LS DARK GRAY",IsActive = false },
                        new WMSColor { ColorCode= "#172F4E", Color="LS NAVY",IsActive = false } ,
                        new WMSColor { ColorCode= "#87BFE0", Color="LS LIGHT BLUE",IsActive = false },
                        new WMSColor { ColorCode= "#76AAD2", Color="LS MALIBU",IsActive = false },
                        new WMSColor { ColorCode=  "#2983BB", Color="LS ROYAL",IsActive = false },
                        new WMSColor { ColorCode=  "#1E4773", Color="LS INDIGO",IsActive = false } ,
                        new WMSColor { ColorCode=  "#2A387B", Color="LS OCEAN",IsActive = false } ,
                        new WMSColor { ColorCode=  "#132845", Color="LS INK",IsActive = false } ,
                        new WMSColor { ColorCode=  "#42AE97", Color="LS JADE",IsActive = false } ,
                        new WMSColor { ColorCode=  "#5FA65E", Color="LS GREEN",IsActive = false } ,
                        new WMSColor { ColorCode=  "#44AE6B", Color="LS EMERALD",IsActive = false } ,
                        new WMSColor { ColorCode=  "#194E36 ", Color="LS NEW FOREST",IsActive = false } ,
                        new WMSColor { ColorCode=  "#701A33", Color="LS MAROON",IsActive = false } ,
                        new WMSColor { ColorCode=  "#DD333B", Color="LS BRIGHT RED",IsActive = false } ,
                        new WMSColor { ColorCode=  "#C7334A", Color="LS RED",IsActive = false } ,
                        new WMSColor { ColorCode=  "#B9303A", Color="LS SCARLET",IsActive = false } ,
                        new WMSColor { ColorCode=  "#54357B", Color="LS GRAPE",IsActive = false } ,
                        new WMSColor { ColorCode=  "#4C3266", Color="LS PURPLE",IsActive = false },
                        new WMSColor { ColorCode=  "#C73266", Color="LS PINK",IsActive = false },
                        new WMSColor { ColorCode=  "#D979A7", Color="LS LIGHT PINK",IsActive = false },
                        new WMSColor { ColorCode=  "#DA5C3E", Color="LS ORANGE",IsActive = false },
                        new WMSColor { ColorCode=  "#E9814E", Color="LS LIGHT ORANGE",IsActive = false },
                        new WMSColor { ColorCode=  "#F0BC5A", Color="LS GOLD",IsActive = false },
                        new WMSColor { ColorCode=  "#F9C364", Color="LS YELLOW",IsActive = false },
                        new WMSColor { ColorCode=  "#CAA369", Color="LS METAL GOLD",IsActive = false },
                        new WMSColor { ColorCode=  "#58452C", Color="LS BROWN",IsActive = false },
                        new WMSColor { ColorCode=  "#06A0D3", Color="LS BRIGHT BLUE",IsActive = false },
                        new WMSColor { ColorCode=  "#D64E91", Color="LS BRIGHT PINK",IsActive = false },
                        new WMSColor { ColorCode=  "#FFEC6B", Color="LS BRIGHT YELLOW",IsActive = false },
                        new WMSColor { ColorCode=  "#CFD868", Color="LS BRIGHT GREEN",IsActive = false }
                    };
            }
        }
        public List<EmblishMethod> getPrintingMethods(string type)
        {
            if (type == "PrintMethods")
            {
                List<EmblishMethod> list = new List<EmblishMethod>()
                                            {
                                                new EmblishMethod
                                                {
                                                     Key = "ScreenPrint",
                                                     Value = "Screen Print"
                                                },
                                                new EmblishMethod
                                                {
                                                    Key = "Embroidery",
                                                    Value = "Embroidery"
                                                },
                                                new EmblishMethod
                                                {
                                                     Key = "Tramfix",
                                                     Value = "Transfix"
                                                },
                                                //new EmblishMethod
                                                //{
                                                //    Key = "Thermo",
                                                //    Value = "Thermo"
                                                //},
                                                new EmblishMethod
                                                {
                                                     Key = "GelPrint",
                                                     Value = "Gel Print"
                                                }

                                            };

                return list.OrderBy(p => p.Key).ToList();
            }
            else if (type == "FontFamily")
            {
                List<EmblishMethod> list = new List<EmblishMethod>()
                                            {
                                                new EmblishMethod
                                                {
                                                     Key = "Times New Roman",
                                                     Value = "Times New Roman"
                                                },
                                                new EmblishMethod
                                                {
                                                    Key = "Georgia",
                                                    Value = "Georgia"
                                                },
                                                new EmblishMethod
                                                {
                                                     Key = "Sans-Serif",
                                                     Value = "Sans-Serif"
                                                },
                                                new EmblishMethod
                                                {
                                                    Key = "Serif",
                                                    Value = "Serif"
                                                },
                                                new EmblishMethod
                                                {
                                                     Key = "Helvetica",
                                                     Value = "Helvetica"
                                                },
                                                new EmblishMethod
                                                {
                                                    Key = "Arial",
                                                    Value = "Arial"
                                                },
                                                new EmblishMethod
                                                {
                                                    Key = "Verdana",
                                                    Value = "Verdana"
                                                },
                                                new EmblishMethod
                                                {
                                                     Key = "Monospace",
                                                     Value = "Monospace"
                                                },
                                                new EmblishMethod
                                                {
                                                    Key = "Cursive",
                                                    Value = "Cursive"
                                                },
                                                new EmblishMethod
                                                {
                                                    Key = "Fantasy",
                                                    Value = "Fantasy"
                                                }
                                            };

                return list.OrderBy(p => p.Key).ToList();
            }
            else
            {
                List<EmblishMethod> list = new List<EmblishMethod>()
                                            {
                                                new EmblishMethod
                                                {
                                                    Key = "Thermo",
                                                    Value = "Thermo"
                                                }
                                            };

                return list.OrderBy(p => p.Key).ToList();
            }

        }
        public List<EmblishMethod> getPrintingMethods(string embType, string styleCode, string type)
        {
            if (styleCode == "DSS1234" || styleCode == "DSS1235")
            {
                List<EmblishMethod> list = new List<EmblishMethod>()
                                            {
                                                new EmblishMethod
                                                {
                                                     Key = "Knitted",
                                                     Value = "Knitted"
                                                }
                                            };

                return list;
            }


            if (type == "PrintMethods")
            {
                List<EmblishMethod> list = new List<EmblishMethod>()
                                            {
                                                new EmblishMethod
                                                {
                                                     Key = "ScreenPrint",
                                                     Value = "Screen Print"
                                                },
                                                new EmblishMethod
                                                {
                                                    Key = "Embroidery",
                                                    Value = "Embroidery"
                                                },
                                                new EmblishMethod
                                                {
                                                     Key = "Tramfix",
                                                     Value = "Transfix"
                                                },
                                                //new EmblishMethod
                                                //{
                                                //    Key = "Thermo",
                                                //    Value = "Thermo"
                                                //},
                                                new EmblishMethod
                                                {
                                                     Key = "GelPrint",
                                                     Value = "Gel Print"
                                                }
                                            };

                // for t-shirts 

                if (styleCode == "DSSTTEM001" || styleCode == "DSSTTEJ001" ||
                    styleCode == "DSSTTEL001" || styleCode == "DSSTPOM001" ||
                    styleCode == "DSSTPOL001" || styleCode == "DSSTPOM002")
                {
                    if (embType == EmblishmentOptionEnum.UpperArmRightType || embType == EmblishmentOptionEnum.UpperArmLeftType)
                    {
                        return list.Where(p => p.Key != "ScreenPrint" && p.Key != "GelPrint" && p.Key != "Tramfix").ToList();
                    }
                }

                // Jackets
                if (styleCode == "DSSTJKM004")
                {
                    if (embType == EmblishmentOptionEnum.LeftChestType ||
                        embType == EmblishmentOptionEnum.RightChestType ||
                        embType == EmblishmentOptionEnum.RightShoulder ||
                        embType == EmblishmentOptionEnum.LeftShoulder ||
                        embType == EmblishmentOptionEnum.BackNeckType ||
                        embType == EmblishmentOptionEnum.LowerBackType
                        )
                    {
                        return list.Where(p => p.Key != "GelPrint" && p.Key != "Tramfix" && p.Key != "GelPrint").ToList();
                    }
                }

                // hoodie
                if (styleCode == "DSSTHOM001" || styleCode == "DSSTHOJ001")
                {
                    if (embType == EmblishmentOptionEnum.ChestCenterTopType)
                    {
                        return list.Where(p => p.Key != "Embroidery").ToList();
                    }
                }
                // track pant 
                if (styleCode == "DSSTPTM001")
                {
                    if (embType == EmblishmentOptionEnum.ThighLeftType)
                    {
                        return list.Where(p => p.Key != "ScreenPrint" && p.Key != "GelPrint" && p.Key != "Tramfix").ToList();
                    }
                }

                // do not suggest screen print for shorts and jackets
                if (styleCode == "DSSTSHM001" || styleCode == "DSSTSHJ001" || styleCode == "DSSTSHM002" || styleCode == "DSSTSHL002" || styleCode == "DSSTJKM004")
                {
                    return list.Where(p => p.Key != "ScreenPrint").ToList();
                }
                if ((styleCode == "DSSTTEM001" || styleCode == "DSSTTEJ001" || styleCode == "DSSTTEL001" || styleCode == "DSSTPOM001" || styleCode == "DSSTPOL001" || styleCode == "DSSTPOM002") && (embType == EmblishmentOptionEnum.BackNeckType || embType == EmblishmentOptionEnum.UpperArmRightType || embType == EmblishmentOptionEnum.UpperArmLeftType))
                {
                    return list.Where(p => p.Key != "ScreenPrint").ToList();
                }

                return list.OrderBy(p => p.Key).ToList();
            }
            else
            {
                return null;
            }

        }
        private List<int> getDimesionRange()
        {
            return new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        }

        #endregion
        public void SaveDispatchProcessInformation(int orderid, string fileName)
        {
            var dbOrder = dbContext.Orders.Find(orderid);
            if (dbOrder != null)
            {
                dbOrder.DispatchNoteFile = fileName;
                dbContext.SaveChanges();
            }
        }

        #region  Design Product
        public DYOOrderCollection DesignProduct(DYOOrderCollection collection)
        {
            //Step 1: save order collection

            orderCollection(collection);

            int count = 0;
            foreach (var productDesign in collection.OrderDesigns)
            {
                productDesign.OrderId = collection.OrderId;

                bool isMailSendToSalesCordinator = false;
                bool isMailToMerchandiser = false;
                // Step 1.1: Order Design
                count++;
                orderDesign(productDesign, count);

                // Step 1.2: Order emblishment
                if (!productDesign.IsRawGarment)
                {
                    orderEmblishmentDetail(productDesign);
                }
            }
            return collection;
        }

        private void orderCollection(DYOOrderCollection collection)
        {
            try
            {
                Order order;
                int customerId = 0;
                var userDetail = (from r in dbContext.WMSUsers
                                  join ur in dbContext.WMSUserRoles on r.Id equals ur.UserId
                                  where r.Id == collection.CreatedBy
                                  select new
                                  {
                                      RoleId = ur.RoleId
                                  }).FirstOrDefault();

                if (userDetail.RoleId == (int)WMSUserRoleEnum.Customer)
                {
                    customerId = collection.CreatedBy;
                }
                else if (userDetail.RoleId == (int)WMSUserRoleEnum.SalesRepresentative)
                {
                    var df = (from r in dbContext.WMSUserAdditionals
                              where r.UserId == collection.CreatedBy
                              select new
                              {
                                  UserId = r.CustomerId
                              }
                                   ).FirstOrDefault();
                    if (df != null)
                    {
                        customerId = df.UserId.HasValue ? df.UserId.Value : collection.CustomerId;
                    }
                }
                else
                {
                    customerId = collection.CustomerId;
                }
                if (collection.OrderId == 0)
                {
                    order = new Order();
                    order.OrderName = collection.OrderName;
                    order.OrderNumber = "";
                    order.CreatedBy = collection.CreatedBy;
                    order.CustomerId = customerId;
                    order.CreatedOnUtc = DateTime.UtcNow;
                    order.OrderStatusId = 0;
                    dbContext.Orders.Add(order);
                    dbContext.SaveChanges();
                    collection.OrderId = order.Id;

                }
                else
                {
                    order = dbContext.Orders.Find(collection.OrderId);
                    order.CreatedBy = collection.CreatedBy;
                    order.CustomerId = customerId;
                    order.CreatedOnUtc = DateTime.UtcNow;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }

        }
        private void orderDesign(DYOProductDesign orderDesign, int count)
        {
            OrderDesign dbOrderDesign;
            bool isMailSendToSalesCordinator = false;
            bool isMailSendToMerchandiser = false;
            try
            {
                var order = dbContext.Orders.Find(orderDesign.OrderId);
                if (orderDesign.OrderDesignId == 0)
                {

                    dbOrderDesign = new OrderDesign();
                    dbOrderDesign.ProductId = orderDesign.ProductId;
                    dbOrderDesign.OrderDesignStatusId = (int)WMSOrderDesignStatusEnum.NewDesign;
                    dbOrderDesign.OrderId = orderDesign.OrderId;
                    dbOrderDesign.CustomerId = order.CustomerId;
                    dbOrderDesign.DesignNumber = "";
                    dbOrderDesign.DesignName = orderDesign.OrderDesignName;
                    dbOrderDesign.IsDeafultLogo = orderDesign.IsDefaultLogo;
                    if (orderDesign.ProductType == StockTypeEnum.SockRange)
                    {
                        dbOrderDesign.ProductColor = orderDesign.ProductColor;
                    }
                    else
                    {
                        dbOrderDesign.ProductColor = orderDesign.ProductColor;
                    }
                    dbOrderDesign.ProductDesignImage = "";
                    dbOrderDesign.JobSeetImage = "";
                    dbOrderDesign.CreatedOnUtc = DateTime.UtcNow;
                    dbOrderDesign.CreatedBy = orderDesign.CreatedBy;
                    dbContext.OrderDesigns.Add(dbOrderDesign);
                    dbContext.SaveChanges();
                    orderDesign.OrderDesignId = dbOrderDesign.Id;

                    if (orderDesign.ProductType == StockTypeEnum.SockRange)
                    {
                        saveOrderDesignStyle(orderDesign);
                    }

                    // TODO conver base64 to image 
                    SaveImageToByte(orderDesign.OrderId, orderDesign.OrderDesignId, orderDesign.ProductTwoDImageBase64);
                    isMailSendToSalesCordinator = false;
                    isMailSendToMerchandiser = false;
                }
                else
                {
                    dbOrderDesign = dbContext.OrderDesigns.Find(orderDesign.OrderDesignId);

                    if (dbOrderDesign.OrderDesignStatusId.HasValue && (dbOrderDesign.OrderDesignStatusId.Value == (int)WMSOrderDesignStatusEnum.DesignRejected || dbOrderDesign.OrderDesignStatusId.Value == (int)WMSOrderDesignStatusEnum.DesignSampleRekjected))
                    {

                        if (dbOrderDesign.OrderDesignStatusId.Value == (int)WMSOrderDesignStatusEnum.DesignRejected)
                        {
                            isMailSendToSalesCordinator = true;
                            dbOrderDesign.OrderDesignStatusId = (int)WMSOrderDesignStatusEnum.NewDesign;
                        }
                        if (dbOrderDesign.OrderDesignStatusId.Value == (int)WMSOrderDesignStatusEnum.DesignSampleRekjected)
                        {
                            isMailSendToMerchandiser = true;
                            dbOrderDesign.OrderDesignStatusId = (int)WMSOrderDesignStatusEnum.DesignProcessed;
                        }
                    }
                    dbOrderDesign.OrderId = orderDesign.OrderId;
                    dbOrderDesign.ProductId = orderDesign.ProductId;
                    dbOrderDesign.CustomerId = order.CustomerId;
                    dbOrderDesign.IsDeafultLogo = orderDesign.IsDefaultLogo;
                    dbOrderDesign.ProductColor = orderDesign.ProductColor;
                    dbOrderDesign.DesignName = orderDesign.OrderDesignName;
                    dbOrderDesign.CreatedOnUtc = DateTime.UtcNow;
                    dbOrderDesign.CreatedBy = orderDesign.CreatedBy;
                    dbContext.SaveChanges();
                    orderDesign.OrderDesignId = dbOrderDesign.Id;
                    if (orderDesign.ProductType == StockTypeEnum.SockRange)
                    {
                        saveOrderDesignStyle(orderDesign);
                    }
                    // TODO conver base64 to image
                    SaveImageToByte(orderDesign.OrderId, orderDesign.OrderDesignId, orderDesign.ProductTwoDImageBase64);

                }

                //  send emails that design rejected design is updated 
                if (isMailSendToMerchandiser)
                {
                    orderDesign.IsMailSendToMerchandiser = true;
                    var SaleCordinatorName = dbContext.WMSUsers.Where(x => x.Id == orderDesign.CustomerId).FirstOrDefault().ContactFirstName;

                    CommunicationLog comObj = new CommunicationLog();
                    comObj.OrderID = orderDesign.OrderId;
                    comObj.OrderDesignId = orderDesign.OrderDesignId;
                    comObj.Reason = "Sales representative " + SaleCordinatorName + " has modified the design" + orderDesign.OrderDesignName;
                    comObj.UserID = orderDesign.CustomerId;
                    comObj.IsPublic = true;
                    comObj.Type = "Modify";
                    comObj.Date = DateTime.UtcNow;
                    dbContext.CommunicationLogs.Add(comObj);
                    dbContext.SaveChanges();

                }
                if (isMailSendToSalesCordinator)
                {
                    orderDesign.IsMailSendToSalesCordinator = true;

                    var SaleCordinatorName = dbContext.WMSUsers.Where(x => x.Id == orderDesign.CustomerId).FirstOrDefault().ContactFirstName;

                    CommunicationLog comObj = new CommunicationLog();
                    comObj.OrderID = orderDesign.OrderId;
                    comObj.OrderDesignId = orderDesign.OrderDesignId;
                    comObj.Reason = "Sales representative " + SaleCordinatorName + " has modified the design" + orderDesign.OrderDesignName;
                    comObj.UserID = orderDesign.CustomerId;
                    comObj.IsPublic = true;
                    comObj.Type = "Modify";
                    comObj.Date = DateTime.UtcNow;
                    dbContext.CommunicationLogs.Add(comObj);
                    dbContext.SaveChanges();


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void saveOrderDesignStyle(DYOProductDesign orderDesign)
        {
            if (orderDesign.ProductSections.Count > 0)
            {
                OrderDesignStyle style;
                foreach (var item in orderDesign.ProductSections)
                {
                    if (!string.IsNullOrEmpty(item.SectionColor))
                    {

                        if (item.ProductSectionId == 0)
                        {
                            style = new OrderDesignStyle();
                            style.OrderDesignId = orderDesign.OrderDesignId;
                            style.Color = item.SectionColor;
                            style.Section = item.SectionNumber;
                            dbContext.OrderDesignStyles.Add(style);
                            dbContext.SaveChanges();
                            item.ProductSectionId = style.Id;
                        }
                        else
                        {
                            style = dbContext.OrderDesignStyles.Find(item.ProductSectionId);
                            if (style != null)
                            {
                                style.OrderDesignId = orderDesign.OrderDesignId;
                                style.Color = item.SectionColor;
                                style.Section = item.SectionNumber;
                                dbContext.SaveChanges();
                                item.ProductSectionId = style.Id;
                            }
                        }
                    }
                    else
                    {
                        style = dbContext.OrderDesignStyles.Find(item.ProductSectionId);
                        if (style != null)
                        {
                            dbContext.OrderDesignStyles.Remove(style);
                            dbContext.SaveChanges();
                        }
                    }

                }
            }
        }

        private void orderEmblishmentDetail(DYOProductDesign orderDesign)
        {
            try
            {
                if (orderDesign.Embelishments != null && orderDesign.Embelishments.Count > 0)
                {
                    OrderEmblishment dbOrderEmblishment;
                    foreach (var item in orderDesign.Embelishments)
                    {

                        if (item.OrderEmblishmentId == 0)
                        {
                            if (item.IsActive)
                            {
                                dbOrderEmblishment = new OrderEmblishment();
                                dbOrderEmblishment.OrderDesignId = orderDesign.OrderDesignId;
                                dbOrderEmblishment.EmblishmentType = item.EmblishmentType;
                                dbOrderEmblishment.PrintMethod = item.PrintMethod;

                                // To convert base64 to image
                                if (item.EmblishmentType != EmblishmentOptionEnum.InsertTextType && item.EmblishmentType != EmblishmentOptionEnum.PlayerNameType && item.EmblishmentType != EmblishmentOptionEnum.PlayerNumberType)
                                {
                                    dbOrderEmblishment.LogoImage = SaveEmblishmentImageToByte(orderDesign.OrderId, orderDesign.OrderDesignId, item.LogoImageBase64, item.EmblishmentType);
                                }
                                else
                                {
                                    dbOrderEmblishment.LogoImage = string.Empty;
                                }

                                if (item.LogoDimension != null)
                                {
                                    dbOrderEmblishment.LogoWidth = item.LogoDimension.Width;
                                    dbOrderEmblishment.LogoHeight = item.LogoDimension.Height;
                                    dbOrderEmblishment.IsAspectRatio = item.LogoDimension.IsAspectRatio;
                                    dbOrderEmblishment.DimensionUnit = string.IsNullOrEmpty(item.LogoDimension.DimensionUnit) ? "mm" : item.LogoDimension.DimensionUnit;
                                }
                                dbOrderEmblishment.FontFamily = item.FontFamily;

                                if (!string.IsNullOrEmpty(item.LogoColor))
                                {
                                    dbOrderEmblishment.Color = item.LogoColor;
                                }
                                else
                                {
                                    dbOrderEmblishment.Color = "#272425";
                                }
                                if (!string.IsNullOrEmpty(item.FontColor))
                                {
                                    dbOrderEmblishment.Color = item.FontColor;
                                }
                                dbContext.OrderEmblishments.Add(dbOrderEmblishment);
                                dbContext.SaveChanges();
                                item.OrderEmblishmentId = dbOrderEmblishment.Id;
                                saveOrderEmblishmentDetail(item);
                            }

                        }
                        else
                        {
                            dbOrderEmblishment = dbContext.OrderEmblishments.Find(item.OrderEmblishmentId);
                            if (item.IsActive)
                            {
                                dbOrderEmblishment.OrderDesignId = orderDesign.OrderDesignId;
                                dbOrderEmblishment.EmblishmentType = item.EmblishmentType;
                                dbOrderEmblishment.PrintMethod = item.PrintMethod;

                                // To convert base64 to image
                                if (item.EmblishmentType != EmblishmentOptionEnum.InsertTextType && item.EmblishmentType != EmblishmentOptionEnum.PlayerNameType && item.EmblishmentType != EmblishmentOptionEnum.PlayerNumberType)
                                {
                                    dbOrderEmblishment.LogoImage = SaveEmblishmentImageToByte(orderDesign.OrderId, orderDesign.OrderDesignId, item.LogoImageBase64, item.EmblishmentType);
                                }
                                else
                                {
                                    dbOrderEmblishment.LogoImage = string.Empty;
                                }
                                if (item.LogoDimension != null)
                                {
                                    dbOrderEmblishment.LogoWidth = item.LogoDimension.Width;
                                    dbOrderEmblishment.LogoHeight = item.LogoDimension.Height;
                                    dbOrderEmblishment.IsAspectRatio = item.LogoDimension.IsAspectRatio;
                                    dbOrderEmblishment.DimensionUnit = string.IsNullOrEmpty(item.LogoDimension.DimensionUnit) ? "mm" : item.LogoDimension.DimensionUnit;
                                }
                                dbOrderEmblishment.FontFamily = item.FontFamily;

                                if (!string.IsNullOrEmpty(item.LogoColor))
                                {
                                    dbOrderEmblishment.Color = item.LogoColor;
                                }
                                if (!string.IsNullOrEmpty(item.FontColor))
                                {
                                    dbOrderEmblishment.Color = item.FontColor;
                                }
                                saveOrderEmblishmentDetail(item);
                            }
                            else
                            {
                                // remove 
                                var detals = dbContext.OrderEmblishmentDetails.Where(p => p.OrderEmblishmentId == item.OrderEmblishmentId).ToList();

                                if (detals.Count > 0)
                                {
                                    dbContext.OrderEmblishmentDetails.RemoveRange(detals);
                                }

                                dbContext.OrderEmblishments.Remove(dbOrderEmblishment);
                                //  saveOrderEmblishmentDetail(item);
                            }
                            dbContext.SaveChanges();

                        }

                    }
                }
                // OrderEmblishment dbOrderEmblishment = new OrderEmblishment();
                //foreach (var item in orderDesign.Embelishments)
                //{
                //    if (item.IsActive)
                //    {
                //        if (item.OrderEmblishmentId == 0)
                //        {
                //            //dbOrderEmblishment = new OrderEmblishment();

                //            dbOrderEmblishment.OrderDesignId = orderDesign.OrderDesignId;
                //            dbOrderEmblishment.EmblishmentType = item.EmblishmentType;
                //            dbOrderEmblishment.PrintMethod = item.PrintMethod;

                //            // To convert base64 to image
                //            if (item.EmblishmentType != EmblishmentOptionEnum.PlayerNameType && item.EmblishmentType != EmblishmentOptionEnum.PlayerNumberType)
                //            {
                //                dbOrderEmblishment.LogoImage = SaveEmblishmentImageToByte(orderDesign.OrderId, orderDesign.OrderDesignId, item.LogoImageBase64, item.EmblishmentType);
                //            }
                //            else
                //            {
                //                dbOrderEmblishment.LogoImage = string.Empty;
                //            }

                //            if (item.LogoDimension != null)
                //            {
                //                dbOrderEmblishment.LogoWidth = item.LogoDimension.Width;
                //                dbOrderEmblishment.LogoHeight = item.LogoDimension.Height;
                //                dbOrderEmblishment.IsAspectRatio = item.LogoDimension.IsAspectRatio;
                //                dbOrderEmblishment.DimensionUnit = string.IsNullOrEmpty(item.LogoDimension.DimensionUnit) ? "mm" : item.LogoDimension.DimensionUnit;
                //            }
                //            dbOrderEmblishment.FontFamily = item.FontFamily;

                //            if (!string.IsNullOrEmpty(item.LogoColor))
                //            {
                //                dbOrderEmblishment.Color = item.LogoColor;
                //            }
                //            if (!string.IsNullOrEmpty(item.FontColor))
                //            {
                //                dbOrderEmblishment.Color = item.LogoColor;
                //            }

                //            dbContext.OrderEmblishments.Add(dbOrderEmblishment);
                //            dbContext.SaveChanges();
                //            item.OrderEmblishmentId = dbOrderEmblishment.Id;
                //        }
                //        else
                //        {
                //            dbOrderEmblishment = dbContext.OrderEmblishments.Find(item.OrderEmblishmentId);

                //            dbOrderEmblishment.OrderDesignId = orderDesign.OrderDesignId;
                //            dbOrderEmblishment.EmblishmentType = item.EmblishmentType;
                //            dbOrderEmblishment.PrintMethod = item.PrintMethod;

                //            // To convert base64 to image
                //            if (item.EmblishmentType != EmblishmentOptionEnum.PlayerNameType && item.EmblishmentType != EmblishmentOptionEnum.PlayerNumberType)
                //            {
                //                dbOrderEmblishment.LogoImage = SaveEmblishmentImageToByte(orderDesign.OrderId, orderDesign.OrderDesignId, item.LogoImageBase64, item.EmblishmentType);
                //            }
                //            else
                //            {
                //                dbOrderEmblishment.LogoImage = string.Empty;
                //            }
                //            if (item.LogoDimension != null)
                //            {
                //                dbOrderEmblishment.LogoWidth = item.LogoDimension.Width;
                //                dbOrderEmblishment.LogoHeight = item.LogoDimension.Height;
                //                dbOrderEmblishment.IsAspectRatio = item.LogoDimension.IsAspectRatio;
                //                dbOrderEmblishment.DimensionUnit = string.IsNullOrEmpty(item.LogoDimension.DimensionUnit) ? "mm" : item.LogoDimension.DimensionUnit;
                //            }
                //            dbOrderEmblishment.FontFamily = item.FontFamily;

                //            if (!string.IsNullOrEmpty(item.LogoColor))
                //            {
                //                dbOrderEmblishment.Color = item.LogoColor;
                //            }
                //            if (!string.IsNullOrEmpty(item.FontColor))
                //            {
                //                dbOrderEmblishment.Color = item.LogoColor;
                //            }
                //            dbContext.SaveChanges();
                //        }
            }
            // }
            //  }

            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void saveOrderEmblishmentDetail(DYOProductDesignEmblishment emb)
        {
            if (emb.CustomColors != null && emb.CustomColors.Count > 0)
            {
                foreach (var item in emb.CustomColors)
                {
                    OrderEmblishmentDetail dbdetail;
                    if (item.OrderEmblishmentDetailId == 0)
                    {
                        dbdetail = new OrderEmblishmentDetail();
                        dbdetail.OrderEmblishmentId = emb.OrderEmblishmentId;
                        dbdetail.Type = item.Type;
                        dbdetail.Color = item.Color;

                        dbContext.OrderEmblishmentDetails.Add(dbdetail);
                        dbContext.SaveChanges();
                        item.OrderEmblishmentDetailId = dbdetail.Id;
                        dbdetail.OrderEmblishmentId = dbdetail.OrderEmblishmentId;
                    }
                    else
                    {
                        dbdetail = dbContext.OrderEmblishmentDetails.Find(item.OrderEmblishmentDetailId);
                        if (dbdetail != null)
                        {
                            dbdetail.OrderEmblishmentId = emb.OrderEmblishmentId;
                            dbdetail.Type = item.Type;
                            dbdetail.Color = item.Color;

                            dbContext.SaveChanges();
                            item.OrderEmblishmentDetailId = dbdetail.Id;
                            dbdetail.OrderEmblishmentId = dbdetail.OrderEmblishmentId;
                        }
                    }
                }
            }
        }
        public string SaveEmblishmentImageToByte(int id, int designId, string imageAWBDocumentName, string embType)
        {
            try
            {
                string base64 = imageAWBDocumentName.Split(',')[1];
                byte[] image = Convert.FromBase64String(base64);
                string name = string.Empty;
                string extension = string.Empty;
                System.Drawing.Image labelimage;

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(image))
                {
                    labelimage = System.Drawing.Image.FromStream(ms);

                    string labelName = string.Empty;
                    if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/Files/Orders/" + id + "/" + designId + "/")))
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Files/Orders/" + id + "/" + designId + "/"));
                    }
                    if (System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/Files/Orders/" + id + "/" + designId + "/")))
                    {
                        string ImgName = "" + ".png";
                        extension = Path.GetExtension(ImgName);
                        string FileName = Path.GetFileName(ImgName);
                        string[] ff = FileName.ToString().Split('.');
                        name = ff[0].ToString();
                        if (string.IsNullOrEmpty(name))
                        {
                            name = "logo_" + embType;
                        }
                        else
                        {
                            name = name + embType;
                        }
                        if (File.Exists(HttpContext.Current.Server.MapPath("~/Files/Orders/" + id + "/" + designId + "/") + name + extension))
                        {
                            File.Delete(HttpContext.Current.Server.MapPath("~/Files/Orders/" + id + "/" + designId + "/") + name + extension);
                        }

                        labelimage.Save(HttpContext.Current.Server.MapPath("~/Files/Orders/" + id + "/" + designId + "/") + name + extension);


                    }

                }
                return name + extension;
            }
            catch (Exception ex)
            {
                return "";
            }



        }
        public bool SaveImageToByte(int id, string imageAWBDocumentName)
        {

            //Image img = Image.FromFile(@"D:\ProjectFrayte\Frayte\Frayte.WebApi\UploadFiles\ProfilePhoto\5_544810894.png");
            // EF.Status = true;

            if (imageAWBDocumentName != null)
            {
                try
                {
                    //byte[] image = imgToByteArray(Result.AWBDocumentName);
                    // byte[] image = imageAWBDocumentName;
                    byte[] image = Convert.FromBase64String(imageAWBDocumentName);

                    System.Drawing.Image labelimage;
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream(image))
                    {
                        labelimage = System.Drawing.Image.FromStream(ms);
                        string labelName = string.Empty;
                        //   EF.FilePath = AppSettings.ProductPath + "AwbImage/" + Result.AWBBarcode + ".jpg";
                        //   EF.FileName = Result.AWBBarcode + ".jpg";
                        if (System.IO.Directory.Exists(AppSettings.ProductPath))
                        {
                            string ImgName = "" + ".jpg";
                            System.Drawing.Bitmap bmpUploadedImage = new System.Drawing.Bitmap(labelimage);
                            System.Drawing.Image thumbnailImage = bmpUploadedImage.GetThumbnailImage(700, 1200, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
                            string extension = Path.GetExtension(ImgName);
                            string FileName = Path.GetFileName(ImgName);
                            string[] ff = FileName.ToString().Split('.');
                            string name = ff[0].ToString();
                            thumbnailImage.Save(AppSettings.ProductPath + "/Orders/" + id + "/" + name + extension);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // EF.Status = false;
                }

            }
            else
            {
                // EF.Status = false;
            }
            return true;
        }

        private int getJobseetImageRev(string image)
        {
            if (!string.IsNullOrEmpty(image))
            {
                var aar = image.Split('_');

                if (aar.Length > 0)
                {
                    var arr1 = aar[1].Split('.');
                    return CommonConversion.ConvertToInt(arr1[0]) + 1;
                }
            }
            return 0;
        }

        public bool SaveImageToByte(int id, int designId, string imageAWBDocumentName)
        {

            try
            {

                var productMaster = (from r in dbContext.OrderDesigns
                                     join p in dbContext.ProductMasters on r.ProductId equals p.Id
                                     where r.Id == designId
                                     select new
                                     {
                                         ProductId = p.Id,
                                         ProductCode = p.ProductCode
                                     }
                                ).FirstOrDefault();

                //string path = HttpContext.Current.Server.MapPath("~/Files/Images/");
                //byte[] image = File.ReadAllBytes(path + "Men polo t-shirt.png");
                string base64 = imageAWBDocumentName.Split(',')[1];
                byte[] image = Convert.FromBase64String(base64);
                System.Drawing.Image labelimage;
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(image))
                {
                    labelimage = System.Drawing.Image.FromStream(ms);
                    string labelName = string.Empty;
                    if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/Files/Orders/" + id + "/" + designId + "/")))
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Files/Orders/" + id + "/" + designId + "/"));
                    }
                    if (System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/Files/Orders/" + id + "/" + designId + "/")))
                    {
                        string ImgName = "" + ".png";
                        System.Drawing.Bitmap bmpUploadedImage = new System.Drawing.Bitmap(labelimage);

                        int width = 0;
                        int height = 0;
                        if (productMaster.ProductCode == "DSS1234" || productMaster.ProductCode == "DSS1235")
                        {
                            width = 1170;
                            height = 496;
                        }
                        else
                        {
                            width = 1170;
                            height = 496;
                        }

                        System.Drawing.Image thumbnailImage = bmpUploadedImage.GetThumbnailImage(width, height, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);

                        var orderDesign = dbContext.OrderDesigns.Find(designId);

                        int rev = 0;
                        if (orderDesign != null)
                        {
                            rev = getJobseetImageRev(orderDesign.ProductDesignImage);
                        }


                        string extension = Path.GetExtension(ImgName);
                        string FileName = Path.GetFileName(ImgName);
                        string[] ff = FileName.ToString().Split('.');
                        string name = ff[0].ToString();
                        if (string.IsNullOrEmpty(name))
                        {
                            var product = (from r in dbContext.OrderDesigns
                                           join p in dbContext.ProductMasters on r.ProductId equals p.Id
                                           where r.Id == designId
                                           select new
                                           {
                                               ProductName = p.ProductName
                                           }).FirstOrDefault();
                            if (product != null)
                            {
                                name = product.ProductName;
                            }

                        }
                        name += "_" + (rev + 1);
                        if (File.Exists(HttpContext.Current.Server.MapPath("~/Files/Orders/" + id + "/" + designId + "/") + name + extension))
                        {
                            File.Delete(HttpContext.Current.Server.MapPath("~/Files/Orders/" + id + "/" + designId + "/") + name + extension);

                        }
                        thumbnailImage.Save(HttpContext.Current.Server.MapPath("~/Files/Orders/" + id + "/" + designId + "/") + name + extension);


                        if (orderDesign != null)
                        {
                            orderDesign.JobSeetImage = name + extension;
                            orderDesign.ProductDesignImage = name + extension;
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // EF.Status = false;
            }

            return true;
        }

        private string getImageNase64(int orderId, int designId, string imageName)
        {
            string filePath = HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/" + designId + "/" + imageName);
            if (File.Exists(filePath))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] imageBytes = new byte[file.Length];
                        file.Read(imageBytes, 0, (int)file.Length);
                        ms.Write(imageBytes, 0, (int)file.Length);
                        string base64String = Convert.ToBase64String(imageBytes);
                        return "data:image/png;base64," + base64String;
                    }
                }
            }
            else
            {
                return "";
            }
        }
        internal bool ThumbnailCallback()
        {
            return true;
        }
        #endregion

        #region  Order

        public DYOPrderCollectionInitials OrderDetailInitials(int orderDesignId)
        {
            DYOPrderCollectionInitials initials = new DYOPrderCollectionInitials();
            var design = dbContext.OrderDesigns.Find(orderDesignId);
            var product = (from r in dbContext.ProductMasters
                           join pc in dbContext.ProductCatagories on r.ProductCatagoryId equals pc.Id
                           where r.Id == design.ProductId
                           select new
                           {
                               StockType = pc.CatagoryName
                           }
                          ).FirstOrDefault();

            if (product.StockType == StockTypeEnum.SockRange)
            {
                initials.ProductSizes = (from r in dbContext.OrderDesigns
                                         join p in dbContext.ProductMasters on r.ProductId equals p.Id
                                         join psku in dbContext.ProductSKUs on p.Id equals psku.ProductId
                                         join s in dbContext.Sizes on psku.SizeId equals s.ID
                                         where r.Id == orderDesignId && s.isActive == true && psku.Style == r.ProductColor
                                         select new DYOProductSpec
                                         {
                                             Id = s.ID,
                                             Name = s.size1,
                                             OrderNumber = s.OrderNumber.HasValue ? s.OrderNumber.Value : 100,
                                         }).Distinct().OrderBy(p => p.OrderNumber).ToList();

                initials.ProductColors = (from r in dbContext.Colors
                                          where r.isActive == true
                                          select new DYOProductSpec
                                          {
                                              Id = r.ID,
                                              Name = r.color1
                                          }).ToList();
                initials.ProductStyles = new List<string>();
            }
            else
            {
                var color = dbContext.Colors.Where(p => p.code == design.ProductColor).FirstOrDefault();
                initials.ProductSizes = (from r in dbContext.OrderDesigns
                                         join p in dbContext.ProductMasters on r.ProductId equals p.Id
                                         join psku in dbContext.ProductSKUs on p.Id equals psku.ProductId
                                         join s in dbContext.Sizes on psku.SizeId equals s.ID
                                         where r.Id == orderDesignId && s.isActive == true && psku.ActualQuantity > 0 && psku.ColorId == color.ID

                                         select new DYOProductSpec
                                         {
                                             OrderNumber = s.OrderNumber.HasValue ? s.OrderNumber.Value : 100,
                                             Id = s.ID,
                                             Name = s.size1
                                         }).Distinct().OrderBy(p => p.OrderNumber).ToList();

                initials.ProductColors = (from r in dbContext.Colors
                                          where r.isActive == true
                                          select new DYOProductSpec
                                          {
                                              Id = r.ID,
                                              Name = r.color1
                                          }).ToList();
                initials.ProductStyles = new List<string>();
            }
            return initials;
        }
        public int SKUQuantity(int orderDesignId, int sizeId)
        {
            int quantity = 0;
            var design = dbContext.OrderDesigns.Find(orderDesignId);
            if (design != null)
            {
                var color = dbContext.Colors.Where(p => p.code == design.ProductColor).FirstOrDefault();
                var product = dbContext.ProductMasters.Find(design.ProductId);
                if (color != null && product != null)
                {
                    var pSkU = dbContext.ProductSKUs.Where(p => p.ProductId == product.Id && p.ColorId == color.ID && p.SizeId == sizeId).FirstOrDefault();
                    if (pSkU != null)
                    {
                        return pSkU.ActualQuantity.HasValue ? pSkU.ActualQuantity.Value : 0;
                    }
                }
            }
            return quantity;
        }
        public DYOPrderCollectionInitials OrderCollectionInitials(int orderId)
        {
            DYOPrderCollectionInitials initials = new DYOPrderCollectionInitials();
            initials.DiliveryMethods = (from r in dbContext.DelivieryMethods
                                        where r.IsActive == true
                                        select new WMSDeliveryMethod
                                        {
                                            Code = r.DeliveryCode,
                                            DelivieryMethodId = r.Id,
                                            DisplayName = r.DelivieryNameDisplay,
                                            Name = r.DelivieryName
                                        }
                                         ).ToList();

            initials.ProductSizes = (from r in dbContext.Sizes
                                     where r.isActive == true
                                     orderby r.OrderNumber
                                     select new DYOProductSpec
                                     {
                                         Id = r.ID,
                                         Name = r.size1
                                     }).ToList();

            initials.ProductColors = (from r in dbContext.Colors
                                      where r.isActive == true
                                      select new DYOProductSpec
                                      {
                                          Id = r.ID,
                                          Name = r.color1
                                      }).ToList();
            initials.ProductStyles = new List<string>();
            return initials;
        }

        public DYOOrder OrderInitials(int orderId)
        {
            DYOOrder order = new DYOOrder();
            var dbOrder = dbContext.Orders.Find(orderId);
            if (dbOrder != null)
            {
                order.OrderId = orderId;
                order.OrderName = dbOrder.OrderName;
                order.OrderNumber = dbOrder.OrderNumber;
                order.OrderNote = string.Empty;
                order.OrderType = "StandardDelivery";
                order.CreatedBy = dbOrder.CreatedBy;
                order.OrderAddress = new DYOOrderAddress();
                order.OrderAddress.Id = dbOrder.OrderAddressId.HasValue ? dbOrder.OrderAddressId.Value : 0;
                order.OrderAddress = (from r in dbContext.WMSUserAddresses
                                      join u in dbContext.WMSUsers on r.UserId equals u.Id
                                      join c in dbContext.Countries on r.CountryId equals c.CountryId
                                      where r.UserId == dbOrder.CustomerId
                                      select new DYOOrderAddress
                                      {
                                          Address = r.Address,
                                          Address2 = r.Address2,
                                          Area = r.Suburb,
                                          City = r.City,
                                          CompanyName = u.CompanyName,
                                          FirstName = u.ContactFirstName,
                                          LastName = u.ContactLastName,
                                          Email = u.Email,
                                          Phone = u.PhoneNumber,
                                          PostCode = r.PostCode,
                                          State = r.State,
                                          Country = new WMSCountry
                                          {
                                              Code = c.CountryCode,
                                              Code2 = c.CountryCode2,
                                              CountryId = c.CountryId,
                                              CountryPhoneCode = c.CountryPhoneCode,
                                              Name = c.CountryName,
                                              TimezoneId = c.TimeZoneId.HasValue ? c.TimeZoneId.Value : 0
                                          }

                                      }).FirstOrDefault();

                var designs = dbContext.OrderDesigns.Where(p => p.OrderId == orderId).ToList();
                if (designs != null && designs.Count > 0)
                {
                    order.OrderDesigns = new List<DYOOrderDesign>();
                    DYOOrderDesign design;
                    foreach (var item in designs)
                    {
                        design = (from o in dbContext.OrderDesigns
                                  join p in dbContext.ProductMasters on o.ProductId equals p.Id
                                  where o.Id == item.Id
                                  select new DYOOrderDesign
                                  {
                                      OrderDesignId = o.Id,
                                      ProductId = o.ProductId,
                                      ProductType = "",
                                      ProductImage = o.ProductDesignImage,
                                      ProductImageURL = AppSettings.ProductPath + "/Files/Orders/" + o.OrderId + "/" + o.Id + "/" + o.ProductDesignImage,
                                      ProductCode = p.ProductCode,
                                      ProductName = p.ProductName,
                                      ProductDescription = p.ProductDescription,
                                      DesignName = o.DesignName
                                  }).FirstOrDefault();

                        string stockType = string.Empty;
                        var product = (from r in dbContext.ProductMasters
                                       join c in dbContext.ProductCatagories on r.ProductCatagoryId equals c.Id
                                       where r.Id == design.ProductId
                                       select new
                                       {
                                           StockType = c.CatagoryName
                                       }
                                       ).FirstOrDefault();
                        if (product != null)
                        {
                            if (product.StockType == "SockRange")
                            {
                                stockType = StockTypeEnum.SockRange;
                            }
                            else
                            {
                                stockType = StockTypeEnum.StockRange;
                            }
                        }
                        else
                        {
                            stockType = StockTypeEnum.StockRange;
                        }

                        design.ProductType = stockType;

                        if (stockType == StockTypeEnum.SockRange)
                        {
                            var productStyle = dbContext.ProductStyles.Where(p => p.ProductId == design.ProductId).FirstOrDefault();

                            bool playerName = false;
                            bool playerNumber = false;
                            bool insertText = false;
                            var orderEmblishments = (dbContext.OrderEmblishments.Where(p => p.OrderDesignId == design.OrderDesignId)).ToList();
                            if (orderEmblishments.Where(p => p.EmblishmentType == EmblishmentOptionEnum.PlayerNameType).ToList().Count > 0)
                            {
                                playerName = true;
                            }
                            if (orderEmblishments.Where(p => p.EmblishmentType == EmblishmentOptionEnum.PlayerNumberType).ToList().Count > 0)
                            {
                                playerNumber = true;
                            }
                            if (orderEmblishments.Where(p => p.EmblishmentType == EmblishmentOptionEnum.InsertTextType).ToList().Count > 0)
                            {
                                insertText = true;
                            }
                            design.IsPlayerName = playerName;
                            design.IsPlayerNumber = playerNumber;
                            design.IsInserText = insertText;

                            design.OrderDesignDetails = new List<DYOOrderDesignDetail>() { new DYOOrderDesignDetail
                                                            {

                                                                StyleCode = productStyle == null ? "" : productStyle.StyleCode,
                                                                Color = "",
                                                                ColorId = 0,
                                                                OrderDesignDetailId = 0,
                                                                OrderDesignId = item.Id,
                                                                IsPlayerName = playerName,
                                                                IsPlayerNumber = playerNumber,
                                                                 IsInserText = insertText,
                                                                PlayerName = null,
                                                                PlayerNumber= null,
                                                                InserText =  null,
                                                                ProductId = item.ProductId,
                                                                Quantity = 0,
                                                                SizeId = 0
                                                            } };

                            design.ProductSizes = OrderDetailInitials(design.OrderDesignId).ProductSizes;
                            order.OrderDesigns.Add(design);
                        }
                        else
                        {
                            var color = dbContext.Colors.Where(p => p.code == item.ProductColor).FirstOrDefault();

                            bool playerName = false;
                            bool playerNumber = false;
                            bool insertText = false;

                            var orderEmblishments = (dbContext.OrderEmblishments.Where(p => p.OrderDesignId == design.OrderDesignId)).ToList();
                            if (orderEmblishments.Where(p => p.EmblishmentType == EmblishmentOptionEnum.PlayerNameType).ToList().Count > 0)
                            {
                                playerName = true;
                            }
                            if (orderEmblishments.Where(p => p.EmblishmentType == EmblishmentOptionEnum.PlayerNumberType).ToList().Count > 0)
                            {
                                playerNumber = true;
                            }
                            if (orderEmblishments.Where(p => p.EmblishmentType == EmblishmentOptionEnum.InsertTextType).ToList().Count > 0)
                            {
                                insertText = true;
                            }
                            design.IsPlayerName = playerName;
                            design.IsPlayerNumber = playerNumber;
                            design.IsInserText = insertText;

                            design.OrderDesignDetails = new List<DYOOrderDesignDetail>() { new DYOOrderDesignDetail
                                                            {
                                                                Color = color.code,
                                                                ColorId = color == null ? (byte)0: color.ID,
                                                                OrderDesignDetailId = 0,
                                                                OrderDesignId = item.Id,
                                                                PlayerName = null,
                                                                PlayerNumber= null,
                                                                InserText =  null,
                                                                ProductId = item.ProductId,
                                                                IsInserText = insertText,
                                                                IsPlayerName= playerName,
                                                                IsPlayerNumber = playerNumber,
                                                                Quantity = 0,
                                                                SizeId = 0
                                                            } };

                            design.ProductSizes = OrderDetailInitials(design.OrderDesignId).ProductSizes;
                            order.OrderDesigns.Add(design);
                        }
                    }
                }
            }
            return order;
        }
        public bool IsJobSheetCreate(int orderId)
        {
            bool status = false;
            var orderDesigns = dbContext.OrderDesigns.Where(p => p.OrderId == orderId).ToList();
            foreach (var item in orderDesigns)
            {
                var embs = dbContext.OrderEmblishments.Where(p => p.OrderDesignId == item.Id).ToList();
                if (embs.Count > 0)
                {
                    status = true;
                    break;
                }
            }
            return status;
        }
        public ApiResponse PlaceOrder(DYOOrder order)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                // save order  
                saveOrder(order);

                // order design 
                saveOrderDesign(order);

                // order desigbn detail   
                saveOrderDesignDetail(order);

                // saveOrderAddress
                saveOrderAddress(order);

                // update address 
                updateOrderAddress(order);

                // Assign sales coordintaor to order 
                assignSalesCordinotor(order);

                response.Status = true;
            }
            catch (Exception ex)
            {
                response.Status = false;
            }
            return response;
        }

        private void assignSalesCordinotor(DYOOrder order)
        {

            var salesCordinator = (from o in dbContext.Orders
                                   join r in dbContext.WMSUsers on o.CustomerId equals r.Id
                                   join ua in dbContext.WMSUserAdditionals on r.Id equals ua.UserId
                                   join sc in dbContext.WMSUsers on ua.SalesCoOrdinatorId equals sc.Id
                                   where o.Id == order.OrderId
                                   select new { UserId = sc.Id }).FirstOrDefault();
            var dbOrder = dbContext.Orders.Find(order.OrderId);
            if (dbOrder != null && salesCordinator != null)
            {
                dbOrder.SalesCordinator = salesCordinator.UserId;
                dbContext.SaveChanges();
            }
        }

        private void updateOrderAddress(DYOOrder order)
        {
            if (order != null)
            {
                var dbOrder = dbContext.Orders.Find(order.OrderId);
                if (dbOrder != null)
                {
                    dbOrder.OrderAddressId = order.OrderAddress.Id;
                    dbContext.SaveChanges();
                }
            }
        }

        private void saveOrderAddress(DYOOrder order)
        {
            if (order.OrderAddress != null)
            {
                OrderAddress dbAddress;
                if (order.OrderAddress.Id == 0)
                {
                    dbAddress = new OrderAddress();
                    if (order.OrderAddress.Country != null)
                    {
                        dbAddress.CountryId = order.OrderAddress.Country.CountryId;
                    }
                    dbAddress.CompanyName = order.OrderAddress.CompanyName;
                    dbAddress.ContactFirstName = order.OrderAddress.FirstName;
                    dbAddress.ContactLastName = order.OrderAddress.LastName;
                    dbAddress.City = order.OrderAddress.City;
                    dbAddress.State = order.OrderAddress.State;
                    dbAddress.PostCode = order.OrderAddress.PostCode;
                    dbAddress.Address = order.OrderAddress.Address;
                    dbAddress.Address2 = order.OrderAddress.Address2;
                    dbAddress.Email = order.OrderAddress.Email;
                    dbAddress.PhoneNumber = order.OrderAddress.Phone;
                    dbAddress.Suburb = order.OrderAddress.Area;
                    dbContext.OrderAddresses.Add(dbAddress);
                    dbContext.SaveChanges();
                    order.OrderAddress.Id = dbAddress.Id;
                }
                else
                {
                    dbAddress = dbContext.OrderAddresses.Find(order.OrderAddress.Id);
                    if (order.OrderAddress.Country != null)
                    {
                        dbAddress.CountryId = order.OrderAddress.Country.CountryId;
                    }
                    dbAddress.CompanyName = order.OrderAddress.CompanyName;
                    dbAddress.ContactFirstName = order.OrderAddress.FirstName;
                    dbAddress.ContactLastName = order.OrderAddress.LastName;
                    dbAddress.City = order.OrderAddress.City;
                    dbAddress.State = order.OrderAddress.State;
                    dbAddress.PostCode = order.OrderAddress.PostCode;
                    dbAddress.Address = order.OrderAddress.Address;
                    dbAddress.Address2 = order.OrderAddress.Address2;
                    dbAddress.Email = order.OrderAddress.Email;
                    dbAddress.PhoneNumber = order.OrderAddress.Phone;
                    dbAddress.Suburb = order.OrderAddress.Area;
                    dbContext.SaveChanges();
                    order.OrderAddress.Id = dbAddress.Id;
                }
            }
        }
        private void saveOrderDesignDetail(DYOOrder order)
        {
            if (order.OrderDesigns != null && order.OrderDesigns.Count > 0)
            {
                foreach (var item in order.OrderDesigns)
                {
                    saveOrderDesignDetail(item);
                }
            }
        }
        private void saveOrderDesign(DYOOrder order)
        {
            if (order.OrderDesigns != null && order.OrderDesigns.Count > 0)
            {
                int count = 0;
                foreach (var item in order.OrderDesigns)
                {
                    var dbOrderDesign = dbContext.OrderDesigns.Find(item.OrderDesignId);
                    if (dbOrderDesign != null)
                    {
                        count++;
                        dbOrderDesign.OrderId = order.OrderId;
                        dbOrderDesign.OrderDesignStatusId = (int)WMSOrderDesignStatusEnum.NewDesign;
                        dbOrderDesign.DesignNumber = item.DesignNumber;

                        dbOrderDesign.UpdatedOn = DateTime.UtcNow;
                        dbOrderDesign.UpdatedBy = order.UpdatedBy;

                        dbContext.SaveChanges();
                        item.OrderDesignId = dbOrderDesign.Id;
                    }
                }
            }
        }


        private List<DYOOrderDesignDetail> formatOrderDesignDetailObj(List<DYOOrderDesignDetail> OrderDesignDetails)
        {

            return OrderDesignDetails.GroupBy(p => p.SizeId)
                 .Select(d => new DYOOrderDesignDetail
                 {
                     SizeId = d.Key,
                     OrderDesignDetailId = d.FirstOrDefault().OrderDesignDetailId,
                     Quantity = (short)d.Sum(p => p.Quantity),
                     InserText = d.FirstOrDefault().InserText,
                     IsInserText = d.FirstOrDefault().IsInserText,
                     IsPlayerName = d.FirstOrDefault().IsPlayerName,
                     PlayerName = d.FirstOrDefault().PlayerName,
                     IsPlayerNumber = d.FirstOrDefault().IsPlayerNumber,
                     PlayerNumber = d.FirstOrDefault().PlayerNumber,
                     OrderDesignId = d.FirstOrDefault().OrderDesignId,
                     Color = d.FirstOrDefault().Color,
                     ColorId = d.FirstOrDefault().ColorId,
                     ActualQuantity = d.FirstOrDefault().ActualQuantity,
                     ProductId = d.FirstOrDefault().ProductId,
                     StyleCode = d.FirstOrDefault().StyleCode

                 }).ToList();


        }
        private void saveOrderDesignDetail(DYOOrderDesign orderDesign)
        {
            if (orderDesign.OrderDesignDetails != null && orderDesign.OrderDesignDetails.Count > 0)
            {
                OrderDesignDetail dbOrderDesignDetail;
                var data = orderDesign.OrderDesignDetails.Where(p => p.IsInserText == false && p.IsPlayerName == false && p.IsPlayerNumber == false).ToList();
                if (data.Count > 0)
                {
                    // orderDesign.OrderDesignDetails = formatOrderDesignDetailObj(orderDesign.OrderDesignDetails);
                }
                foreach (var item in orderDesign.OrderDesignDetails)
                {
                    if (item.OrderDesignDetailId == 0)
                    {
                        dbOrderDesignDetail = new OrderDesignDetail();
                        dbOrderDesignDetail.OrderDesignId = orderDesign.OrderDesignId;
                        dbOrderDesignDetail.PlayerName = item.PlayerName;
                        if (orderDesign.ProductType == StockTypeEnum.SockRange)
                        {
                            if (item.IsInserText)
                            {
                                dbOrderDesignDetail.PlayerName = item.InserText;
                            }
                        }
                        dbOrderDesignDetail.PlayerNumber = item.PlayerNumber;
                        dbOrderDesignDetail.Quantity = item.Quantity;
                        dbOrderDesignDetail.ProductSKUId = getProductSKU(item.ProductId, item.Color, item.SizeId, orderDesign.ProductType, orderDesign.OrderDesignId);
                        dbContext.OrderDesignDetails.Add(dbOrderDesignDetail);
                        dbContext.SaveChanges();
                        item.OrderDesignDetailId = dbOrderDesignDetail.Id;

                        // update stock 
                        if (orderDesign.ProductType != StockTypeEnum.SockRange)
                        {
                            updateStock(dbOrderDesignDetail.ProductSKUId, item.Quantity);
                        }

                    }
                    else
                    {
                        dbOrderDesignDetail = dbContext.OrderDesignDetails.Find(item.OrderDesignDetailId);
                        dbOrderDesignDetail.OrderDesignId = orderDesign.OrderDesignId;
                        dbOrderDesignDetail.PlayerName = item.PlayerName;
                        dbOrderDesignDetail.PlayerNumber = item.PlayerNumber;
                        dbOrderDesignDetail.Quantity = item.Quantity;
                        dbOrderDesignDetail.ProductSKUId = getProductSKU(item.ProductId, item.Color, item.SizeId, orderDesign.ProductType, orderDesign.OrderDesignId);
                        dbContext.SaveChanges();

                        // update stock 
                        if (orderDesign.ProductType != StockTypeEnum.SockRange)
                        {
                            updateStock(dbOrderDesignDetail.ProductSKUId, item.Quantity);
                        }
                    }
                }

            }
        }

        private void updateStock(int productSKUId, short quantity)
        {
            var productSKU = dbContext.ProductSKUs.Find(productSKUId);
            if (productSKU != null)
            {
                if (productSKU.ActualQuantity - quantity >= 0)
                {
                    productSKU.ActualQuantity = productSKU.ActualQuantity - quantity;
                }
                dbContext.SaveChanges();
            }
        }

        private int getProductSKU(int productId, string color, byte sizeId, string stockType, int orderDesignId)
        {
            try
            {
                if (stockType == StockTypeEnum.SockRange)
                {
                    var dbOrderDeisgn = dbContext.OrderDesigns.Find(orderDesignId);

                    var productSKU = dbContext.ProductSKUs.Where(p => p.ProductId == productId && p.SizeId == sizeId && p.Style == dbOrderDeisgn.ProductColor).FirstOrDefault();
                    if (productSKU != null)
                    {
                        return productSKU.Id;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    var color1 = dbContext.Colors.Where(p => p.code == color).FirstOrDefault();
                    if (color1 != null)
                    {
                        var productSKU = dbContext.ProductSKUs.Where(p => p.ProductId == productId && p.SizeId == sizeId && p.ColorId == color1.ID).FirstOrDefault();
                        if (productSKU != null)
                        {
                            return productSKU.Id;
                        }
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        private int getProductSKU(int productId, int colorId, byte sizeId)
        {
            try
            {
                var productSKU = dbContext.ProductSKUs.Where(p => p.ProductId == productId && p.SizeId == sizeId && p.ColorId == colorId).FirstOrDefault();
                if (productSKU != null)
                {
                    return productSKU.Id;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        private void saveOrder(DYOOrder order)
        {
            var userRole = dbContext.WMSUserRoles.Where(p => p.UserId == order.CreatedBy).FirstOrDefault();
            int modifyDays = 2;
            int customerId = 0;
            if (userRole != null)
            {
                if (userRole.RoleId == (int)WMSUserRoleEnum.SalesRepresentative)
                {
                    var customerModifyDays = (from r in dbContext.WMSUserAdditionals
                                              join u in dbContext.WMSUserAdditionals on r.CustomerId equals u.UserId
                                              where r.UserId == order.CustomerId
                                              select u.ModifyDays).FirstOrDefault();

                    if (customerModifyDays.HasValue)
                    {
                        modifyDays = 2;
                    }
                    else
                    {
                        modifyDays = 2;
                    }
                    var customer = dbContext.WMSUserAdditionals.Where(p => p.UserId == order.CreatedBy).FirstOrDefault();
                    if (customer != null)
                    {
                        customerId = customer.CustomerId.HasValue ? customer.CustomerId.Value : 0;
                    }

                }
                if (userRole.RoleId == (int)WMSUserRoleEnum.Customer)
                {
                    var customerModifyDays = (from r in dbContext.WMSUserAdditionals
                                              where r.UserId == order.CustomerId
                                              select r.ModifyDays).FirstOrDefault();

                    if (customerModifyDays.HasValue)
                    {
                        modifyDays = 2;
                    }
                    else
                    {
                        modifyDays = 2;
                    }
                }

            }

            Order dbOrder;
            if (order.OrderId == 0)
            {
                dbOrder = new Order();
                dbOrder.OrderName = order.OrderName;
                dbOrder.OrderNumber = ISexistAccountNo(customerId);
                dbOrder.PONumber = order.PONumber;
                dbOrder.OrderStatusId = (int)WMSOrderStatusEnum.NewOrder;
                dbOrder.DelivieryMethodId = order.DelivieryMethodId;
                dbOrder.CreatedOnUtc = DateTime.UtcNow;
                dbOrder.CreatedBy = order.CreatedBy;
                dbOrder.ModifyDays = modifyDays;
                dbContext.Orders.Add(dbOrder);
                dbContext.SaveChanges();
                order.OrderId = dbOrder.Id;
            }
            else
            {
                dbOrder = dbContext.Orders.Find(order.OrderId);
                dbOrder.OrderNumber = ISexistAccountNo(dbOrder.CustomerId);
                dbOrder.PONumber = string.IsNullOrEmpty(order.PONumber) ? dbOrder.PONumber : order.PONumber;
                dbOrder.OrderName = order.OrderName;
                dbOrder.OrderType = order.OrderType;
                dbOrder.OrderNote = order.OrderNote;
                dbOrder.CreatedOnUtc = DateTime.UtcNow;
                dbOrder.CreatedBy = order.CreatedBy;
                dbOrder.OrderName = order.OrderName;
                dbOrder.OrderStatusId = (int)WMSOrderStatusEnum.NewOrder;
                dbOrder.DelivieryMethodId = order.DelivieryMethodId;
                dbOrder.ModifyDays = modifyDays;
                if (order.DeliveryDate.HasValue)
                {
                    dbOrder.RequestedDeliveryDate = UtilityRepository.UpdatedDateTime(order.DeliveryDate.Value);
                }
                else
                {
                    dbOrder.RequestedDeliveryDate = order.DeliveryDate;
                }
                dbContext.SaveChanges();
                order.OrderNumber = dbOrder.OrderNumber;
            }
        }
        public string ISexistAccountNo(int customerId)
        {
            string customerAccount = string.Empty;
            int id = 0;
            var userRole = (from r in dbContext.WMSUserRoles
                            where r.UserId == customerId
                            select r
                            ).FirstOrDefault();
            if (userRole != null)
            {
                if (userRole.RoleId == (int)WMSUserRoleEnum.SalesRepresentative)
                {
                    var id1 = dbContext.WMSUserAdditionals.Where(p => p.UserId == customerId).Select(p => p.CustomerId).FirstOrDefault();
                    if (id1.HasValue)
                    {
                        id = id1.Value;
                    }
                }
                if (userRole.RoleId == (int)WMSUserRoleEnum.Customer)
                {
                    id = customerId;
                }
            }

            var userDetail = dbContext.WMSUserAdditionals.Where(p => p.UserId == id).FirstOrDefault();
            if (userDetail != null)
            {
                customerAccount = userDetail.AccountNumber.Substring(0, 3);
            }
            Random rnd = new Random();
            string acc = customerAccount + "-" + rnd.Next(100000000, 999999999).ToString();
            var data = dbContext.Orders.Where(x => x.OrderNumber == acc).FirstOrDefault();
            if (data != null)
            {
                return ISexistAccountNo(customerId);
            }
            else { return acc; }
        }
        #endregion

        #region My Orders
        public WebAPIResponse AddCommunicationLog(DYOOrderComunicationLog comm)
        {
            try
            {
                if (comm.OrderId != 0)
                {
                    CommunicationLog comObj = new CommunicationLog();
                    comObj.OrderID = comm.OrderId;
                    comObj.Reason = comm.Reason;
                    comObj.UserID = comm.UserId;
                    comObj.IsPublic = comm.Ispublic;
                    comObj.Date = DateTime.UtcNow;
                    dbContext.CommunicationLogs.Add(comObj);
                    dbContext.SaveChanges();
                    return new WebAPIResponse { Message = "Success", Result = comm.OrderId };
                }
                else
                {
                    return new WebAPIResponse { Message = "Error" };
                }
            }
            catch (Exception ex)
            {
                return new WebAPIResponse { Message = ex.Message };
            }
        }
        public bool AcceptOrder(int orderId, string type)
        {
            var dbOrder = dbContext.Orders.Find(orderId);
            if (dbOrder != null)
            {
                if (type == "Accept")
                {
                    var user = (from o in dbContext.Orders
                                join u in dbContext.WMSUsers on o.CustomerId equals u.Id
                                join ua in dbContext.WMSUserAdditionals on u.Id equals ua.UserId
                                join ma in dbContext.WMSUsers on ua.MerchandiseUserId equals ma.Id
                                where o.Id == orderId
                                select new
                                {
                                    UserId = ma.Id
                                }).FirstOrDefault();

                    if (user != null)
                    {
                        dbOrder.Mechandiser = user.UserId;
                        dbOrder.OrderStatusId = (byte)WMSOrderStatusEnum.Processed;
                        dbContext.SaveChanges();
                        return true;
                    }
                }
                else
                {
                    if (dbOrder != null)
                    {
                        dbOrder.OrderStatusId = (byte)WMSOrderStatusEnum.Rejected;
                        dbContext.SaveChanges();
                    }
                    return true;
                }

            }
            return false;
        }
        public List<DYOOrderGrid> TrackAndTraceOrders(MyOrderTrackAndTrace track)
        {
            try
            {
                var loggedInUserDetail = (from r in dbContext.WMSUsers
                                          join ur in dbContext.WMSUserRoles on r.Id equals ur.UserId
                                          where r.Id == track.UserId
                                          select new
                                          { RoleId = ur.RoleId }
                                          ).FirstOrDefault();

                var collection = (from r in dbContext.Orders
                                  join d in dbContext.DelivieryMethods on r.DelivieryMethodId equals d.Id
                                  join s in dbContext.OrderStatus on r.OrderStatusId equals s.Id
                                  join od in dbContext.OrderDesigns on r.Id equals od.OrderId
                                  join pm in dbContext.ProductMasters on od.ProductId equals pm.Id
                                  join odd in dbContext.OrderDesignDetails on od.Id equals odd.OrderDesignId
                                  join psku in dbContext.ProductSKUs on odd.ProductSKUId equals psku.Id
                                  join sd in dbContext.Sizes on psku.SizeId equals sd.ID
                                  join u in dbContext.WMSUsers on r.CustomerId equals u.Id
                                  join sc in dbContext.WMSUsers on r.SalesCordinator equals sc.Id
                                  join cd in dbContext.Colors on psku.ColorId equals cd.ID into leftjoinColor
                                  from colorTemp in leftjoinColor.DefaultIfEmpty()
                                      // join cl in dbContext.CommunicationLogs on r.Id equals cl.OrderID into leftjoincomm
                                      //from commTemp in leftjoincomm.DefaultIfEmpty()
                                  join ocart in dbContext.OrderCartons on r.Id equals ocart.OrderID into leftJoinOCart
                                  from carTemp in leftJoinOCart.DefaultIfEmpty()

                                  where (track.OrderStatusId == 0 || r.OrderStatusId == track.OrderStatusId) &&
                                    r.OrderStatusId != (byte)WMSOrderStatusEnum.Draft && r.OrderStatusId > 0 &&
                                    (!track.FromDate.HasValue || r.CreatedOnUtc >= track.FromDate.Value) &&
                                    (!track.ToDate.HasValue || r.CreatedOnUtc <= track.ToDate.Value) &&
                                    (track.CustomerId == 0 || r.CustomerId == track.CustomerId) &&
                                    (d.Id == r.DelivieryMethodId) &&
                                    ((track.OrderNumber == "" || track.OrderNumber == null) || r.OrderNumber == track.OrderNumber) &&
                                     ((track.PONumber == "" || track.PONumber == null) || r.PONumber == track.PONumber)
                                  select new
                                  {
                                      OrderId = r.Id,
                                      PONumber = r.PONumber,
                                      CustomerId = r.CustomerId,
                                      Customer = u.CompanyName,
                                      OrderDate = r.CreatedOnUtc,
                                      OrderName = r.OrderName,
                                      OrderNumber = r.OrderNumber,
                                      OrderStatus = s.StatusDisplay,
                                      DeliveryType = r.OrderType,
                                      IsCartonLabel = carTemp == null ? false : true,
                                      DelivieryMethod = d.DelivieryNameDisplay,

                                      CustomerName = u.CompanyName,
                                      SalesCoordinatorId = r.SalesCordinator,
                                      Merchandiser = r.Mechandiser,
                                      WarehouseUser = r.WarehouseUserId,
                                      JobSheetFile = r.JobSheetFile,
                                      JobSheetShiipingFile = r.JobSheetShippingDetailFile,
                                      PickUpNoteFile = r.PickupNoteFile,
                                      UpdatedPickupNoteFile = r.UpdatedPickupNoteFile,
                                      DispatchLabelFile = r.DispatchNoteFile,
                                      OrderStatausId = r.OrderStatusId,

                                      SalesCoordinator = sc.ContactFirstName + " " + sc.ContactLastName,

                                      OrderDesignId = od.Id,
                                      DesignNumber = od.DesignNumber,
                                      ProductImage = od.ProductDesignImage,
                                      ProductImageURL = AppSettings.ProductPath + "Files/Orders/" + od.OrderId + "/" + od.Id + "/" + od.ProductDesignImage,
                                      OrderDesignStatusId = od.OrderDesignStatusId,
                                      DesignName = od.DesignName,
                                      ProductId = pm.Id,
                                      ProductName = pm.ProductName,
                                      ProductCode = pm.ProductCode,

                                      OrderDesignDetailId = odd.Id,
                                      PlayerName = odd.PlayerName,
                                      PlayerNumber = odd.PlayerNumber,
                                      Size = sd.size1,
                                      SizeId = sd.ID,
                                      ColorId = colorTemp == null ? 0 : colorTemp.ID,
                                      Color = colorTemp == null ? "" : colorTemp.color1,
                                      ColorCode = colorTemp == null ? "" : colorTemp.code,
                                      Quantity = odd.Quantity
                                  }).OrderByDescending(p => p.OrderId).ToList();

                if (loggedInUserDetail.RoleId == (byte)WMSUserRoleEnum.Customer)
                {
                    collection = collection.Where(p => p.CustomerId == track.UserId).ToList();
                }
                else if (loggedInUserDetail.RoleId == (byte)WMSUserRoleEnum.SalesCoOrdinator)
                {
                    collection = collection.Where(p => p.SalesCoordinatorId == track.UserId).ToList();
                }
                else if (loggedInUserDetail.RoleId == (byte)WMSUserRoleEnum.Merchandise)
                {
                    collection = collection.Where(p => p.Merchandiser == track.UserId).ToList();
                }
                else if (loggedInUserDetail.RoleId == (byte)WMSUserRoleEnum.Warehouse)
                {
                    collection = collection.Where(p => p.WarehouseUser == track.UserId).ToList();
                }
                else
                {

                }

                //if (!string.IsNullOrEmpty(track.Type))
                //{
                //    collection = collection.Where(p => p.OrderDesignStatusId == 12).ToList();
                //}

                if (loggedInUserDetail.RoleId == (byte)WMSUserRoleEnum.SalesCoOrdinator && !string.IsNullOrEmpty(track.Type))
                {
                    collection = collection.Where(p => p.OrderDesignStatusId == (int)WMSOrderDesignStatusEnum.DesignRejected).ToList();
                }
                else if (loggedInUserDetail.RoleId == (byte)WMSUserRoleEnum.Merchandise && !string.IsNullOrEmpty(track.Type))
                {
                    collection = collection.Where(p => p.OrderDesignStatusId == (int)WMSOrderDesignStatusEnum.DesignSampleRekjected).ToList();
                }

                List<DYOOrderGrid> list = collection.GroupBy(p => p.OrderId)
                                  .Select(group => new DYOOrderGrid
                                  {
                                      TotalRows = collection.Select(p => p.OrderId).Distinct().Count(),
                                      OrderId = group.FirstOrDefault().OrderId,
                                      Customer = group.FirstOrDefault().CustomerName,
                                      StatusImage = "",
                                      IsCartonLabel = group.FirstOrDefault().IsCartonLabel,
                                      SalesCoodinator = group.FirstOrDefault().SalesCoordinator,
                                      DeliveryType = group.FirstOrDefault().DeliveryType,
                                      DeliveryMethod = group.FirstOrDefault().DelivieryMethod,
                                      OrderDate = group.FirstOrDefault().OrderDate,
                                      OrderName = group.FirstOrDefault().OrderName,
                                      OrderNumber = group.FirstOrDefault().OrderNumber,
                                      PONumber = group.FirstOrDefault().PONumber,
                                      OrderStatus = group.FirstOrDefault().OrderStatus,
                                      IsCollapsed = false,
                                      jobSheetName = group.FirstOrDefault().JobSheetFile,
                                      jobSheetUrl = AppSettings.ProductPath + "Files/Orders/" + group.FirstOrDefault().OrderId + "/" + group.FirstOrDefault().JobSheetFile,
                                      jobSheetShiipingFileName = group.FirstOrDefault().JobSheetShiipingFile,
                                      jobSheetShippingFileUrl = AppSettings.ProductPath + "Files/Orders/" + group.FirstOrDefault().OrderId + "/" + group.FirstOrDefault().JobSheetShiipingFile,
                                      PickNoteName = group.FirstOrDefault().PickUpNoteFile,
                                      PickNoteUrl = AppSettings.ProductPath + "Files/Orders/" + group.FirstOrDefault().OrderId + "/" + group.FirstOrDefault().PickUpNoteFile,
                                      UpdatedPickNoteName = group.FirstOrDefault().UpdatedPickupNoteFile,
                                      UpdatedPickNoteUrl = AppSettings.ProductPath + "Files/Orders/" + group.FirstOrDefault().OrderId + "/" + group.FirstOrDefault().UpdatedPickupNoteFile,
                                      DispatchNoteName = group.FirstOrDefault().DispatchLabelFile,
                                      DispatchNoteUrl = AppSettings.ProductPath + "Files/Orders/" + group.FirstOrDefault().OrderId + "/" + group.FirstOrDefault().DispatchLabelFile,
                                      OrderStatusId = group.FirstOrDefault().OrderStatausId,

                                      OrderDesigns = group.GroupBy(zx => new { zx.OrderId, zx.OrderDesignId }).Select(p => new OrderDesignGrid
                                      {

                                          DesignName = p.FirstOrDefault().DesignName,
                                          OrderDesignId = p.FirstOrDefault().OrderDesignId,
                                          ProductCode = p.FirstOrDefault().ProductCode,
                                          ProductImageURL = p.FirstOrDefault().ProductImageURL,
                                          ProductId = p.FirstOrDefault().ProductId,
                                          ProductName = p.FirstOrDefault().ProductName,
                                          DesignNumber = p.FirstOrDefault().DesignNumber,
                                          IsCollapsed = false,
                                          OrderDesignDetails = p.GroupBy(xy => new { xy.OrderId, xy.OrderDesignId, xy.OrderDesignDetailId })
                                              .Select(q => new OrderDesignDetailGrid
                                              {
                                                  OrderDesignDetailId = q.FirstOrDefault().OrderDesignDetailId,
                                                  Color = q.FirstOrDefault().Color,
                                                  ColorId = (byte)q.FirstOrDefault().ColorId,
                                                  PlayerName = q.FirstOrDefault().PlayerName,
                                                  PlayerNumber = q.FirstOrDefault().PlayerNumber,
                                                  Quantity = q.FirstOrDefault().Quantity,
                                                  SizeId = q.FirstOrDefault().SizeId,
                                                  SizeName = q.FirstOrDefault().Size,
                                                  OrderDesignId = q.FirstOrDefault().OrderDesignId
                                              }).ToList()
                                      }).ToList()
                                  }).OrderByDescending(p => p.OrderId).ToList();

                foreach (var item in collection)
                {

                }

                foreach (var item in list)
                {
                    if (item.DeliveryType == WMSOrderDeliveryTypeEnum.StandardDelivery)
                    {
                        item.DeliveryType = WMSOrderDeliveryTypeEnum.StandardDeliveryDisplay;
                    }
                    if (item.DeliveryType == WMSOrderDeliveryTypeEnum.ExpressDelivery)
                    {
                        item.DeliveryType = WMSOrderDeliveryTypeEnum.ExpressDeliveryDisplay;
                    }
                    if (item.DeliveryType == WMSOrderDeliveryTypeEnum.UrgentDelivery)
                    {
                        item.DeliveryType = WMSOrderDeliveryTypeEnum.UrgentDeliveryDisplay;
                    }
                    if (item.OrderDesigns.Count == 1)
                    {
                        item.OrderName = item.OrderDesigns[0].DesignName;
                    }

                    if (item.OrderStatusId == (byte)WMSOrderStatusEnum.NewOrder)
                    {
                        item.StatusImage = "NewOrder.png";
                    }
                    else if (item.OrderStatusId == (byte)WMSOrderStatusEnum.Processed)
                    {
                        item.StatusImage = "Processed.png";
                    }
                    else if (item.OrderStatusId == (byte)WMSOrderStatusEnum.InProduction)
                    {
                        item.StatusImage = "InProduction.png";
                    }
                    else if (item.OrderStatusId == (byte)WMSOrderStatusEnum.Rejected)
                    {
                        item.StatusImage = "RejectedBy.png";
                    }
                    else if (item.OrderStatusId == (byte)WMSOrderStatusEnum.ReadyForDispatch)
                    {
                        item.StatusImage = "Readyfordispatch.png";
                    }
                    else if (item.OrderStatusId == (byte)WMSOrderStatusEnum.SampleCreation)
                    {
                        item.StatusImage = "SampleCreation.png";
                    }
                    else if (item.OrderStatusId == (byte)WMSOrderStatusEnum.Shipped)
                    {
                        item.StatusImage = "Shipped.png";
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                return null;
            }


        }
        #endregion

        #region Get order details
        public DYOOrderDetails getOrderDetails(int OrderId, int userId)
        {
            try
            {
                var userRole = (from r in dbContext.WMSUserRoles
                                where r.UserId == userId
                                select new { RoleId = r.RoleId }).FirstOrDefault();

                var orderDetails = (from a in dbContext.Orders

                                    join d in dbContext.DelivieryMethods on a.DelivieryMethodId equals d.Id
                                    join b in dbContext.WMSUsers on a.CustomerId equals b.Id
                                    join c in dbContext.OrderStatus on a.OrderStatusId equals c.Id
                                    where (a.Id == OrderId) // && (d.Id == a.DelivieryMethodId)
                                    select new DYOOrderDetails
                                    {
                                        OrderId = a.Id,
                                        PONumber = a.PONumber,
                                        OrderName = a.OrderName,
                                        OrderNo = a.OrderNumber,
                                        DeliveryType = a.OrderType,
                                        ARequestedDeliveryDate = a.RequestedDeliveryDate,
                                        ARequestedExFactoryDate = a.RequestedExFactoryDate,
                                        AConfirmedExFactoryDate = a.ConfirmedExFactoryDate,
                                        SalesCordinator = a.SalesCordinator.HasValue ? a.SalesCordinator.Value : 0,
                                        Merchandiser = a.Mechandiser.HasValue ? a.Mechandiser.Value : 0,
                                        CreatedBy = b.ContactFirstName,
                                        CreatedOnDate = a.CreatedOnUtc,
                                        CreatedOn = a.CreatedOnUtc.ToString(),
                                        Status = c.StatusDisplay,
                                        OrderStatusId = a.OrderStatusId,
                                        OrderNotes = a.OrderNote,
                                        ModifyDays = a.ModifyDays.HasValue ? a.ModifyDays.Value : 0,
                                        _OrderBillingAddress = (from e in dbContext.WMSUserAddresses
                                                                where e.UserId == b.Id
                                                                select new OrderBillingAddress
                                                                {
                                                                    CompanyName = b.CompanyName,
                                                                    ContactPerson = b.ContactFirstName + " " + b.ContactLastName,
                                                                    Address = e.Address + "," + e.Address2 + " " + e.Address3 + " " + e.City + ":- " + e.PostCode,
                                                                    Email = b.Email,
                                                                    PhoneNo = b.PhoneNumber,

                                                                }).FirstOrDefault(),

                                        _OrderDeliveryInformation = (from f in dbContext.OrderAddresses
                                                                     where f.Id == a.OrderAddressId
                                                                     select new OrderDeliveryInformation
                                                                     {
                                                                         CompanyName = f.CompanyName,
                                                                         ContactPerson = f.ContactFirstName + " " + f.ContactLastName,
                                                                         Address = f.Address + " " + f.Address2 + " " + f.Address3 + ", " + f.City + ":-" + f.PostCode,
                                                                         PhoneNo = f.PhoneNumber,
                                                                         Email = f.Email,
                                                                         DeliveryMethod = d.DelivieryNameDisplay,

                                                                     }).FirstOrDefault(),


                                        _ProductInformation = (from g in dbContext.OrderDesigns
                                                               join h in dbContext.Orders on g.OrderId equals h.Id
                                                               join i in dbContext.ProductMasters on g.ProductId equals i.Id
                                                               where h.Id == OrderId
                                                               select new ProductInformation
                                                               {
                                                                   OrderDesignId = g.Id,
                                                                   ProductCatagoryId = i.ProductCatagoryId.HasValue ? i.ProductCatagoryId.Value : 0,
                                                                   OrderDesignName = g.DesignName,
                                                                   ProductName = i.ProductName,
                                                                   DesignNumber = g.DesignNumber,
                                                                   ProductCode = i.ProductCode,
                                                                   OrderDesignStatusId = g.OrderDesignStatusId.HasValue ? g.OrderDesignStatusId.Value : 0,
                                                                   Image = AppSettings.ProductPath + "Files/Orders/" + g.OrderId + "/" + g.Id + "/" + g.ProductDesignImage,
                                                                   _ProductDetails = (from j in dbContext.OrderDesignDetails
                                                                                      join k in dbContext.ProductSKUs on j.ProductSKUId equals k.Id
                                                                                      join l in dbContext.Sizes on k.SizeId equals l.ID
                                                                                      where j.OrderDesignId == g.Id
                                                                                      select new ProductDetails
                                                                                      {
                                                                                          Name = j.PlayerName,
                                                                                          Number = j.PlayerNumber,
                                                                                          Quantity = j.Quantity.ToString(),
                                                                                          Size = l.size1,
                                                                                          totalqty = (from j in dbContext.OrderDesignDetails
                                                                                                      where j.OrderDesignId == g.Id
                                                                                                      select new { j.Quantity }).Sum(x => x.Quantity)
                                                                                      }).ToList()

                                                               }).ToList(),
                                    }).FirstOrDefault();


                if (orderDetails.CreatedOnDate.Day + orderDetails.ModifyDays > DateTime.UtcNow.Day)
                {
                    orderDetails.IsModifyShow = false;
                }
                else
                {
                    orderDetails.IsModifyShow = true;
                }
                if (orderDetails != null)
                {
                    orderDetails.CreatedOn = orderDetails.CreatedOnDate.ToString("dd-MMM-yyyy");
                }
                if (orderDetails._ProductInformation.Where(p => p.ProductCatagoryId == 2).Count() > 0)
                {
                    orderDetails.OrderTypeRange = StockTypeEnum.SockRange;
                }
                else
                {
                    orderDetails.OrderTypeRange = StockTypeEnum.StockRange;
                }

                if (orderDetails.ARequestedDeliveryDate.HasValue)
                {
                    orderDetails.RequestedDeliveryDate = orderDetails.ARequestedDeliveryDate.Value.ToString("dd-MMM-yyyy");
                }
                if (orderDetails.ARequestedExFactoryDate.HasValue)
                {
                    orderDetails.RequestedExFactoryDate = orderDetails.ARequestedExFactoryDate.Value.ToString("dd-MMM-yyyy");
                }
                if (orderDetails.AConfirmedExFactoryDate.HasValue)
                {
                    orderDetails.ConfirmedExFactoryDate = orderDetails.AConfirmedExFactoryDate.Value.ToString("dd-MMM-yyyy");
                }


                if (orderDetails._ProductInformation.Count() == 1)
                {
                    orderDetails.OrderName = orderDetails._ProductInformation[0].OrderDesignName;
                }
                if (orderDetails.DeliveryType == WMSOrderDeliveryTypeEnum.StandardDelivery)
                {
                    orderDetails.DeliveryType = WMSOrderDeliveryTypeEnum.StandardDeliveryDisplay;
                }
                if (orderDetails.DeliveryType == WMSOrderDeliveryTypeEnum.ExpressDelivery)
                {
                    orderDetails.DeliveryType = WMSOrderDeliveryTypeEnum.ExpressDeliveryDisplay;
                }
                if (orderDetails.DeliveryType == WMSOrderDeliveryTypeEnum.UrgentDelivery)
                {
                    orderDetails.DeliveryType = WMSOrderDeliveryTypeEnum.UrgentDeliveryDisplay;
                }
                foreach (var item in orderDetails._ProductInformation)
                {
                    if (userRole.RoleId == (int)WMSUserRoleEnum.SalesCoOrdinator || userRole.RoleId == (int)WMSUserRoleEnum.Merchandise)
                    {

                        if (item.OrderDesignStatusId == (int)WMSOrderDesignStatusEnum.NewDesign)
                        {
                            if (userRole.RoleId == (int)WMSUserRoleEnum.SalesCoOrdinator)
                            {
                                item.ShowAcceptReject = true;
                            }
                            else
                            {
                                item.ShowAcceptReject = true;
                            }
                            item.IsModify = true;
                        }
                        else if (item.OrderDesignStatusId == (int)WMSOrderDesignStatusEnum.DesignProcessed)
                        {
                            if (userRole.RoleId == (int)WMSUserRoleEnum.Merchandise)
                            {
                                item.ShowAcceptReject = true;
                            }
                            else
                            {
                                item.ShowAcceptReject = true;
                            }
                        }

                        else if (item.OrderDesignStatusId == (int)WMSOrderDesignStatusEnum.DesignSampleRekjected)
                        {
                            if (userRole.RoleId == (int)WMSUserRoleEnum.Merchandise)
                            {
                                item.ShowAcceptReject = true;
                            }
                            else
                            {
                                item.ShowAcceptReject = true;
                            }
                        }
                        else if (item.OrderDesignStatusId == (int)WMSOrderDesignStatusEnum.DesignCreation)
                        {
                            if (userRole.RoleId == (int)WMSUserRoleEnum.Merchandise)
                            {
                                item.ShowAcceptReject = true;
                            }
                            else
                            {
                                item.ShowAcceptReject = true;
                            }
                        }

                        else // (item.OrderDesignStatusId == (int)WMSOrderDesignStatusEnum.DesignRejected)
                        {
                            item.ShowAcceptReject = false;
                        }
                        if (item.OrderDesignStatusId == (int)WMSOrderDesignStatusEnum.DesignRejected)
                        {
                            item.ShowAcceptReject = true;
                            var commlog = dbContext.CommunicationLogs.Where(p => p.OrderDesignId == item.OrderDesignId && p.Type == "Reject" && p.UserID == orderDetails.SalesCordinator).FirstOrDefault();

                            if (commlog != null && commlog.Date.HasValue)
                            {
                                if (DateTime.UtcNow.Date <= commlog.Date.Value.AddDays(orderDetails.ModifyDays))
                                {
                                    item.IsModify = true;
                                }
                                else
                                {
                                    item.IsModify = false;
                                }
                            }
                            else
                            {
                                if (DateTime.UtcNow.Date <= orderDetails.CreatedOnDate.Date.AddDays(orderDetails.ModifyDays))
                                {
                                    item.IsModify = true;
                                }
                                else
                                {
                                    item.IsModify = false;
                                }
                            }
                        }
                        if (item.OrderDesignStatusId == (int)WMSOrderDesignStatusEnum.DesignSampleRekjected)
                        {
                            var commlog = dbContext.CommunicationLogs.Where(p => p.OrderDesignId == item.OrderDesignId && p.Type == "Reject" && p.UserID == orderDetails.Merchandiser).FirstOrDefault();
                            if (commlog != null && commlog.Date.HasValue)
                            {
                                if (DateTime.UtcNow <= commlog.Date.Value.AddDays(orderDetails.ModifyDays))
                                {
                                    item.IsModify = true;
                                }
                                else
                                {
                                    item.IsModify = false;
                                }
                            }
                            else
                            {
                                if (DateTime.UtcNow.Date <= orderDetails.CreatedOnDate.Date.AddDays(orderDetails.ModifyDays))
                                {
                                    item.IsModify = true;
                                }
                                else
                                {
                                    item.IsModify = false;
                                }
                            }
                        }
                    }
                    else if (userRole.RoleId == (int)WMSUserRoleEnum.Customer || userRole.RoleId == (int)WMSUserRoleEnum.SalesRepresentative)
                    {
                        if (item.OrderDesignStatusId == (int)WMSOrderDesignStatusEnum.DesignRejected)
                        {
                            var commlog = dbContext.CommunicationLogs.Where(p => p.OrderDesignId == item.OrderDesignId && p.Type == "Reject" && p.UserID == orderDetails.SalesCordinator).FirstOrDefault();

                            if (commlog != null && commlog.Date.HasValue)
                            {
                                if (DateTime.UtcNow.Date <= commlog.Date.Value.AddDays(orderDetails.ModifyDays))
                                {
                                    item.IsModify = true;
                                }
                                else
                                {
                                    item.IsModify = false;
                                }
                            }
                            else
                            {
                                if (DateTime.UtcNow.Date <= orderDetails.CreatedOnDate.Date.AddDays(orderDetails.ModifyDays))
                                {
                                    item.IsModify = true;
                                }
                                else
                                {
                                    item.IsModify = false;
                                }
                            }

                        }
                        if (item.OrderDesignStatusId == (int)WMSOrderDesignStatusEnum.DesignSampleRekjected)
                        {
                            var commlog = dbContext.CommunicationLogs.Where(p => p.OrderDesignId == item.OrderDesignId && p.Type == "Reject" && p.UserID == orderDetails.Merchandiser).FirstOrDefault();
                            if (commlog != null && commlog.Date.HasValue)
                            {
                                if (commlog.Date.Value.Date <= commlog.Date.Value.AddDays(orderDetails.ModifyDays))
                                {
                                    item.IsModify = true;
                                }
                                else
                                {
                                    item.IsModify = false;
                                }
                            }
                            else
                            {
                                if (DateTime.UtcNow.Date <= orderDetails.CreatedOnDate.Date.AddDays(orderDetails.ModifyDays))
                                {
                                    item.IsModify = true;
                                }
                                else
                                {
                                    item.IsModify = false;
                                }
                            }
                        }
                        item.ShowAcceptReject = false;
                    }
                }
                return orderDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region get strike off approval list
        public WebAPIResponse getStrikeApprovalList(int orderid, int roleid)
        {
            try
            {
                //var res = (from a in dbContext.OrderEmblishments
                //           join b in dbContext.OrderDesigns on a.OrderDesignId equals b.Id
                //           join c in dbContext.OrderRevisionHistories on a.Id equals c.OrderEmblishmentID
                //           where b.OrderId == orderid
                //           select new
                //           {
                //               RevisionNo = roleid == 4 ? c.CRevisionNo : c.MRevisionNo
                //           }).Distinct().Max(x => x.RevisionNo);
                //int revisionno = Convert.ToInt32(res);
                var TotalOrderDesign = (from a in dbContext.OrderDesigns
                                        join b in dbContext.OrderEmblishments on a.Id equals b.OrderDesignId
                                        where b.EmblishmentType != "PlayerName" && b.EmblishmentType != "PlayerNumber" && a.OrderId == orderid && b.LogoImage != ""
                                        select new
                                        {
                                            id = b.Id
                                        }).Count();


                var IsOrderCompleted = (from a in dbContext.OrderEmblishments
                                        join b in dbContext.OrderStrikeOffLogos on a.Id equals b.OrderEmblishmentID into temp
                                        from b in temp.DefaultIfEmpty()
                                        where a.EmblishmentType != "PlayerName" && a.EmblishmentType != "PlayerNumber" && b.OrderID == orderid && b.Status == 1 && a.LogoImage != ""
                                        select new
                                        {
                                            total = a.Id
                                        }).Count();


                var res = (from a in dbContext.OrderEmblishments
                           join b in dbContext.OrderDesigns on a.OrderDesignId equals b.Id
                           join c in dbContext.OrderStrikeOffLogos on a.Id equals c.OrderEmblishmentID
                           where b.OrderId == orderid
                           select new
                           {
                               RevisionNo = c.RevisionNo
                           }).Distinct().Max(x => x.RevisionNo);
                int revisionno = Convert.ToInt32(res);


                List<StrikeOffModel> li = new List<StrikeOffModel>();
                //   int j = 1;
                for (int i = revisionno; i <= revisionno; i--)
                {
                    StrikeOffModel model = new StrikeOffModel
                    {
                        IsOrderDesignComplete = (TotalOrderDesign == IsOrderCompleted) ? true : false,
                        RevisionNo = i == 0 ? revisionno + 1 : i,
                        OrderStatusId = dbContext.Orders.Where(x => x.Id == orderid).FirstOrDefault().OrderStatusId,

                        StrikeOfflist = (from a in dbContext.OrderEmblishments
                                         join b in dbContext.OrderDesigns on a.OrderDesignId equals b.Id
                                         //join c in dbContext.OrderRevisionHistories on a.Id equals c.OrderEmblishmentID into temp
                                         //from c in temp.DefaultIfEmpty()
                                         join d in dbContext.OrderStrikeOffLogos on a.Id equals d.OrderEmblishmentID into temp2
                                         from d in temp2.DefaultIfEmpty()
                                         join e in dbContext.Orders on b.OrderId equals e.Id
                                         where b.OrderId == orderid && a.EmblishmentType != "PlayerName" && a.EmblishmentType != "PlayerNumber"
                                         // && ((roleid == 4 && i == 0) ? c.CRevisionNo == null : roleid == 4 ? c.CRevisionNo == i : true)
                                         //  && ((roleid == 5 && i == 0) ? c.MRevisionNo == null : roleid == 5 ? c.MRevisionNo == i : true)


                                         && ((i == 0 && roleid == 5 && e.OrderStatusId >= 3) || (i == 0 && roleid == 4)) ? d.RevisionNo == null : d.OrderID == orderid && d.RevisionNo == i
                                         // && (i == 0 && roleid == 5 && e.OrderStatusId >= 3) ? d.RevisionNo == null : (roleid == 5 && e.OrderStatusId >= 3) ? d.RevisionNo == i : false
                                         //   && (i == 0 && roleid == 4) ? d.RevisionNo == null : roleid == 4 ? d.RevisionNo == i : false
                                         // && (roleid == 3) ? d.RevisionNo == i : false                                        
                                         select new StrikeOffListModel
                                         {
                                             EmblishmentType = a.EmblishmentType,
                                             //  ID = c.ID == null ? 0 : c.ID,
                                             SampleLogoID = d.ID == null ? 0 : d.ID,
                                             OrderID = b.OrderId,
                                             OrderEmblishmentID = a.Id,
                                             LogoImage = a.LogoImage != "" && a.LogoImage != null ? AppSettings.ProductPath + "Files/Orders/" + b.OrderId + "/" + b.Id + "/" + a.LogoImage : "",
                                             LogoImageName = a.LogoImage,
                                             PrintMethod = a.PrintMethod,
                                             OrderStatusId = e.OrderStatusId,
                                             //  Status = roleid == 4 ? c.CoordinatorStatus == 1 ? "Accept" : (c.CoordinatorStatus == 2 ? "Reject" : "") : roleid == 5 ? c.MechandiserStatus == 1 ? "Accept" : (c.MechandiserStatus == 2 ? "Reject" : "") : "",
                                             SampleLogo = d.Logo != "" && d.Logo != null ? AppSettings.ProductPath + "Files/Orders/" + b.OrderId + "/" + b.Id + "/" + d.Logo : "",
                                             SampleLogoName = d.Logo,
                                             SampleLogoStatus = d.Status == 1 ? "Accepted" : d.Status == 2 ? "Rejected" : "",
                                             IsFinished = d.Status == 2 ? (dbContext.OrderStrikeOffLogos.Where(x => x.ID > d.ID && x.OrderEmblishmentID == a.Id).FirstOrDefault() != null ? true : false) : false
                                         }).ToList()
                    };
                    if (model.StrikeOfflist.Count > 0)
                    {
                        li.Add(model);
                    }
                    //    j++;
                    if (i == 0) { break; }
                }

                foreach (var item in li)
                {
                    foreach (var sk in item.StrikeOfflist)
                    {
                        if (sk.EmblishmentType != EmblishmentOptionEnum.PlayerNameType && sk.EmblishmentType != EmblishmentOptionEnum.PlayerNumberType && sk.EmblishmentType != EmblishmentOptionEnum.InsertTextType)
                        {
                            var print = getPrintingMethods("PrintMethods").Where(p => p.Key == sk.PrintMethod).FirstOrDefault();
                            if (print != null)
                            {
                                sk.PrintMethod = print.Value;

                            }
                        }
                        else
                        {
                            var print = getPrintingMethods("").Where(p => p.Key == sk.PrintMethod).FirstOrDefault();
                            if (print != null)
                            {
                                sk.PrintMethod = print.Value;

                            }
                        }

                    }

                }

                if (li.Count > 0) { return new WebAPIResponse { Message = "Success", Result = li.OrderByDescending(x => x.RevisionNo) }; }
                else { return new WebAPIResponse { Message = "No data in Strike list" }; }
            }
            catch (Exception ex)
            {
                return new WebAPIResponse { Message = ex.Message };
            }
        }
        #endregion

        //#region accept/reject Order Emblishments
        //public WebAPIResponse setStrikeLogoStatus(StrikeApprovalModel model)
        //{
        //    try
        //    {
        //        List<OrderRevisionHistory> li = new List<OrderRevisionHistory>();
        //        foreach (var item in model.revHistory.oEmblishmentlist)
        //        {
        //            if (item.RevisionHistoryid > 0 && model.revHistory.roleid == 5)
        //            {
        //                var res = dbContext.OrderRevisionHistories.Where(x => x.ID == item.RevisionHistoryid).FirstOrDefault();
        //                if (res != null)
        //                {
        //                    var res2 = dbContext.OrderRevisionHistories.Where(x => x.OrderID == model.revHistory.orderid && x.CoordinatorStatus == 1).Max(x => x.MRevisionNo);
        //                    int? revNo = res2 == null ? 1 : res2 + 1;

        //                    res.MechandiserStatus = item.statusid;
        //                    res.MRevisionNo = Convert.ToByte(revNo);
        //                    dbContext.SaveChanges();
        //                }
        //            }
        //            else
        //            {
        //                if (item.oemblishmentid > 0)
        //                {
        //                    var res = dbContext.OrderRevisionHistories.Where(x => x.OrderID == model.revHistory.orderid).Max(x => x.CRevisionNo);
        //                    int? revNo = res == null ? 1 : res + 1;
        //                    OrderRevisionHistory obj = new OrderRevisionHistory
        //                    {
        //                        OrderID = model.revHistory.orderid,
        //                        OrderEmblishmentID = item.oemblishmentid,
        //                        CoordinatorStatus = item.statusid,
        //                        IsFinal = false,
        //                        CRevisionNo = Convert.ToByte(revNo)
        //                    };
        //                    li.Add(obj);
        //                }
        //            }
        //        }
        //        if (li.Count > 0)
        //        {
        //            dbContext.OrderRevisionHistories.AddRange(li);
        //            dbContext.SaveChanges();

        //            new EmailRepository().Email_E2_2(Convert.ToInt32(model.revHistory.orderid));
        //            new EmailRepository().Email_E2_4(Convert.ToInt32(model.revHistory.orderid));
        //            byte statusid = 3;
        //            if (model.revHistory.roleid == 5) { statusid = 5; }
        //            SetOrderStatus(model.revHistory.orderid, statusid);
        //        }
        //        //sample logo

        //        //var _res = setStrikeSampleLogoStatus(model.sampleLogo);
        //        //if (_res == "Success")
        //        //{
        //        //    new EmailRepository().Email_E2_5(Convert.ToInt32(model.revHistory.orderid));
        //        //}

        //        //comm log
        //        ManageCommunicationLog(model.commLogList);
        //        if (model.commLogList.Count > 0)
        //        {
        //            if (model.revHistory.roleid == 4)
        //            {
        //                new EmailRepository().Email_E2_3(Convert.ToInt32(model.revHistory.orderid));
        //            }
        //            if (model.revHistory.roleid == 5)
        //            {
        //                new EmailRepository().Email_E2_6(Convert.ToInt32(model.revHistory.orderid));
        //            }
        //            SetOrderStatus(model.revHistory.orderid, 4);
        //        }

        //        return new WebAPIResponse { Message = "Success" };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new WebAPIResponse
        //        {
        //            Message = ex.Message
        //        };
        //    }
        //}
        //#endregion

        #region manage communication log
        public WebAPIResponse ManageCommunicationLog(List<CommunicationLogModel> model)
        {
            try
            {
                List<CommunicationLog> cli = new List<CommunicationLog>();
                foreach (var item in model)
                {
                    CommunicationLog obj = new CommunicationLog
                    {
                        OrderID = item.OrderID,
                        OrderEmblishmentID = item.OrderEmblishmentID,
                        UserID = item.UserID,
                        Date = DateTime.Now,
                        IsPublic = item.IsPublic,
                        Reason = item.Reason
                    };
                    cli.Add(obj);
                }
                dbContext.CommunicationLogs.AddRange(cli);
                dbContext.SaveChanges();


                return new WebAPIResponse { Message = "Success" };
            }
            catch (Exception ex)
            {
                return new WebAPIResponse { Message = ex.Message };
            }
        }
        #endregion

        #region get communication log
        public WebAPIResponse GetCommunicationLogByOrderid(int orderid)
        {
            try
            {
                var data = (from a in dbContext.CommunicationLogs
                            join b in dbContext.WMSUsers on a.UserID equals b.Id
                            where a.OrderID == orderid
                            select new CommunicationLogModel
                            {
                                ID = a.ID,
                                OrderID = a.OrderID,
                                OrderEmblishmentID = a.OrderEmblishmentID,
                                UserID = a.UserID,
                                UserName = b.ContactFirstName + " " + b.ContactLastName,
                                Date = a.Date,
                                Reason = a.Reason
                            }).OrderByDescending(x => x.ID).ToList();

                if (data.Count > 0)
                {
                    return new WebAPIResponse { Message = "Success", Result = data };
                }
                else { return new WebAPIResponse { Message = "No data in communication log" }; }
            }
            catch (Exception ex)
            {
                return new WebAPIResponse { Message = ex.Message };
            }

        }
        #endregion

        #region Manage strike off sample logo
        public WebAPIResponse ManageStrikeOffSampleLogo(List<StrikeOffSampleLogo> model)
        {
            try
            {
                if (model.Count > 0)
                {
                    int oid = model[0].OrderID;
                    var order = dbContext.OrderStrikeOffLogos.Where(x => x.OrderID == oid).ToList();
                    //  maxRevisionNo == null ? (maxRevisionNo = 1) : (maxRevisionNo + 1);
                    int maxRevisionNo = 1;

                    if (order.Count > 0)
                    {
                        maxRevisionNo = Convert.ToInt32(order.Max(x => x.RevisionNo)) + 1;
                    }

                    List<OrderStrikeOffLogo> li = new List<OrderStrikeOffLogo>();

                    foreach (var item in model)
                    {
                        OrderStrikeOffLogo obj = new OrderStrikeOffLogo
                        {
                            OrderID = item.OrderID,
                            OrderEmblishmentID = item.OrderEmblishmentID,
                            Logo = item.FileName,
                            RevisionNo = maxRevisionNo
                        };
                        li.Add(obj);
                    }
                    dbContext.OrderStrikeOffLogos.AddRange(li);
                    dbContext.SaveChanges();
                    int? orderid = li.Count > 0 ? li[0].OrderID : 0;

                    //email to created by for accept and reject the logo
                    new EmailRepository().Email_E2_7(Convert.ToInt32(orderid), model);

                    return new WebAPIResponse { Message = "Success", Result = orderid };
                }
                else { return new WebAPIResponse { Message = "No data sent" }; }
            }
            catch (Exception ex)
            {
                return new WebAPIResponse { Message = ex.Message };
            }
        }
        #endregion

        #region get order design id
        public int getOrderDesignId(int orderEblishid)
        {
            try
            {
                var data = dbContext.OrderEmblishments.Where(x => x.Id == orderEblishid).FirstOrDefault();
                if (data != null)
                {
                    return data.OrderDesignId;
                }
                else { return 0; }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Set status for sample logo by customer
        public string setStrikeSampleLogoStatus(StrikeOffSampleLogoModel model)
        {
            try
            {
                //foreach (var item in model)
                //{
                if (model.SampleLogoID > 0)
                {
                    var data = dbContext.OrderStrikeOffLogos.Where(x => x.ID == model.SampleLogoID).FirstOrDefault();
                    if (data != null)
                    {
                        data.Status = model.statusid;
                        dbContext.SaveChanges();

                        //var res = dbContext.OrderRevisionHistories.Where(x => x.OrderID == data.OrderID && x.OrderEmblishmentID == data.OrderEmblishmentID).FirstOrDefault();
                        //if (res != null)
                        //{
                        //    res.IsFinal = true;
                        //    dbContext.SaveChanges();
                        //}
                    }
                }
                //}
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        #endregion

        #region Set order status
        public WebAPIResponse SetOrderStatus(int orederid, byte statusid)
        {
            try
            {
                var data = dbContext.Orders.Where(x => x.Id == orederid).FirstOrDefault();
                if (data != null)
                {
                    data.DispatchedOnUtc = DateTime.UtcNow;
                    data.OrderStatusId = statusid;
                    dbContext.SaveChanges();
                }
                return new WebAPIResponse { Message = "Success" };
            }
            catch (Exception ex)
            {
                return new WebAPIResponse { Message = ex.Message };
            }
        }

        #endregion



        public WebAPIResponse AcceptOrderDesign(OrderDesignModel obj)
        {
            var data = SetOrderStatus(obj.orderid, obj.statusid);
            if (data.Message == "Success")
            {
                //assign merchandiser/warehouse
                var custorder = dbContext.Orders.Where(x => x.Id == obj.orderid).FirstOrDefault();
                var user = (from a in dbContext.WMSUsers
                            join b in dbContext.WMSUserAdditionals on a.Id equals b.UserId
                            where a.Id == custorder.CustomerId
                            select new { b.MerchandiseUserId, b.WarehouseUserId }).FirstOrDefault();


                if (obj.roleid == 4)
                {
                    if (user != null)
                    {
                        custorder.Mechandiser = user.MerchandiseUserId;
                        dbContext.SaveChanges();
                    }
                    // add communication log
                    OrderStrickOffModel mode = new OrderStrickOffModel();
                    var SaleCordinatorName = dbContext.WMSUsers.Where(x => x.Id == obj.userid).FirstOrDefault().ContactFirstName;
                    var dbOrderDesigns = dbContext.OrderDesigns.Where(p => p.OrderId == obj.orderid).ToList();
                    if (dbOrderDesigns.Count > 0)
                    {
                        foreach (var item in dbOrderDesigns)
                        {
                            if (item.OrderDesignStatusId == (int)WMSOrderDesignStatusEnum.DesignRejected)
                            {
                                dbContext.OrderDesigns.Remove(item);
                                dbContext.SaveChanges();
                            }
                            else
                            {
                                if (item.OrderDesignStatusId == (int)WMSOrderDesignStatusEnum.NewDesign)
                                {
                                    item.UpdatedOn = DateTime.UtcNow;
                                    item.UpdatedBy = item.UpdatedBy;
                                    item.OrderDesignStatusId = (int)WMSOrderDesignStatusEnum.DesignProcessed;
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }

                    mode.ispublic = true;
                    mode.reason = "Sales coordinator " + SaleCordinatorName + " has accepted the order and order status changed from New Order to Processed";
                    mode.userid = obj.userid;
                    mode.orderid = obj.orderid;
                    AddCommLog(mode);
                    // email 
                    new EmailRepository().Email_E2_2(Convert.ToInt32(obj.orderid));
                    new EmailRepository().Email_E2_4(Convert.ToInt32(obj.orderid));

                }
                else if (obj.roleid == 5)
                {
                    if (user != null)
                    {
                        custorder.WarehouseUserId = user.WarehouseUserId;
                        dbContext.SaveChanges();
                    }

                    var dbOrderDesigns = dbContext.OrderDesigns.Where(p => p.OrderId == obj.orderid).ToList();
                    if (dbOrderDesigns.Count > 0)
                    {
                        foreach (var item in dbOrderDesigns)
                        {
                            if (item.OrderDesignStatusId == (int)WMSOrderDesignStatusEnum.DesignSampleRekjected)
                            {
                                dbContext.OrderDesigns.Remove(item);
                                dbContext.SaveChanges();
                            }
                            else
                            {
                                if (item.OrderDesignStatusId == (int)WMSOrderDesignStatusEnum.DesignProcessed)
                                {
                                    item.UpdatedOn = DateTime.UtcNow;
                                    item.UpdatedBy = item.UpdatedBy;
                                    item.OrderDesignStatusId = (int)WMSOrderDesignStatusEnum.DesignCreation;
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }

                    // add communication log
                    OrderStrickOffModel mode = new OrderStrickOffModel();
                    var Marchindiser = dbContext.WMSUsers.Where(x => x.Id == obj.userid).FirstOrDefault().ContactFirstName;
                    mode.ispublic = true;
                    mode.reason = "Marchindiser " + Marchindiser + " has accepted the order and order status changed from processed to sample creation";
                    mode.userid = obj.userid;
                    mode.orderid = obj.orderid;
                    AddCommLog(mode);

                    var order = dbContext.Orders.Where(x => x.Id == obj.orderid).FirstOrDefault();
                    if (order != null)
                    {
                        try
                        {
                            order.ExpectedDeliveryOnUtc = obj.delDate;
                            dbContext.SaveChanges();
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    // email to customer about sapmle creation process started  
                    new EmailRepository().Email_E2_5(Convert.ToInt32(obj.orderid));
                }
                return new WebAPIResponse { Message = "Success" };
            }
            else
            {
                return new WebAPIResponse { Message = "Error" };
            }
        }
        public WebAPIResponse RejectOrderDesign(OrderDesignModel obj)
        {
            try
            {
                var data = SetOrderStatus(obj.orderid, obj.statusid);
                if (data.Message == "Success")
                {

                    CommunicationLog comObj = new CommunicationLog();
                    comObj.OrderID = obj.orderid;
                    comObj.Reason = obj.reason;
                    comObj.UserID = obj.userid;
                    comObj.IsPublic = obj.ispublic;
                    comObj.Date = DateTime.UtcNow;
                    dbContext.CommunicationLogs.Add(comObj);
                    dbContext.SaveChanges();

                    // email 
                    if (obj.roleid == 4)
                    {
                        var dbOrderDesigns = dbContext.OrderDesigns.Where(p => p.OrderId == obj.orderid).ToList();
                        if (dbOrderDesigns.Count > 0)
                        {
                            foreach (var item in dbOrderDesigns)
                            {
                                item.UpdatedOn = DateTime.UtcNow;
                                item.UpdatedBy = item.UpdatedBy;
                                item.OrderDesignStatusId = (int)WMSOrderDesignStatusEnum.DesignRejected;
                                dbContext.SaveChanges();
                            }
                        }
                        new EmailRepository().Email_E2_3(Convert.ToInt32(obj.orderid), obj.reason);
                    }
                    else if (obj.roleid == 5)
                    {
                        var dbOrderDesigns = dbContext.OrderDesigns.Where(p => p.OrderId == obj.orderid).ToList();
                        if (dbOrderDesigns.Count > 0)
                        {
                            foreach (var item in dbOrderDesigns)
                            {
                                item.UpdatedOn = DateTime.UtcNow;
                                item.UpdatedBy = item.UpdatedBy;
                                item.OrderDesignStatusId = (int)WMSOrderDesignStatusEnum.DesignSampleRekjected;
                                dbContext.SaveChanges();
                            }
                        }
                        new EmailRepository().Email_E2_6(Convert.ToInt32(obj.orderid), obj.reason);
                    }
                    return new WebAPIResponse { Message = "Success", Result = obj.orderid };
                }
                else
                {
                    return new WebAPIResponse { Message = "Error" };
                }
            }
            catch (Exception ex)
            {
                return new WebAPIResponse { Message = ex.Message };
            }
        }

        public WebAPIResponse AcceptAllStrikeLogo(StrrikeModel obj)
        {
            try
            {
                var samples = dbContext.OrderStrikeOffLogos.Where(p => p.OrderID == obj.OrderId).ToList();

                foreach (var item in samples)
                {

                    StrikeOffSampleLogoModel mode = new StrikeOffSampleLogoModel();

                    mode.SampleLogoID = item.ID;
                    mode.statusid = 1;
                    mode.userid = obj.UserId;
                    mode.orderid = obj.OrderId;
                    AcceptAllStrikeOffSamples(mode);
                }

                return new WebAPIResponse { Message = "Success" };
            }
            catch (Exception ex)
            {
                return new WebAPIResponse { Message = "Error" };
            }
        }
        public void AcceptAllStrikeOffSamples(StrikeOffSampleLogoModel mode)
        {
            var data = setStrikeSampleLogoStatus(mode);
            if (data == "Success")
            {
                // add communication log
                OrderStrickOffModel model = new OrderStrickOffModel();
                var name = dbContext.WMSUsers.Where(x => x.Id == mode.userid).FirstOrDefault().ContactFirstName;
                model.ispublic = true;
                model.reason = "Customer " + name + " has accepted the sample logo.";
                model.userid = mode.userid;
                model.orderid = mode.orderid;
                AddCommLog(model);

                //
                var TotalOrderDesign = (from a in dbContext.OrderDesigns
                                        join b in dbContext.OrderEmblishments on a.Id equals b.OrderDesignId
                                        where b.EmblishmentType != "PlayerName" && b.EmblishmentType != "PlayerNumber" && a.OrderId == mode.orderid
                                        select new
                                        {
                                            id = b.Id
                                        }).Count();


                var IsOrderCompleted = (from a in dbContext.OrderEmblishments
                                        join b in dbContext.OrderStrikeOffLogos on a.Id equals b.OrderEmblishmentID into temp
                                        from b in temp.DefaultIfEmpty()
                                        where a.EmblishmentType != "PlayerName" && a.EmblishmentType != "PlayerNumber" && b.OrderID == mode.orderid && b.Status == 1
                                        select new
                                        {
                                            total = a.Id
                                        }).Count();
                if (TotalOrderDesign == IsOrderCompleted)
                {
                    SetOrderStatus(mode.orderid, 6);
                }
                // email 
                new EmailRepository().Email_E2_9(Convert.ToInt32(mode.orderid));
            }
        }
        public WebAPIResponse AcceptStrikeLogo(StrikeOffSampleLogoModel mode)
        {
            var data = setStrikeSampleLogoStatus(mode);
            if (data == "Success")
            {
                // add communication log
                OrderStrickOffModel model = new OrderStrickOffModel();
                var name = dbContext.WMSUsers.Where(x => x.Id == mode.userid).FirstOrDefault().ContactFirstName;
                model.ispublic = true;
                model.reason = "Customer " + name + " has accepted the sample logo.";
                model.userid = mode.userid;
                model.orderid = mode.orderid;
                AddCommLog(model);

                //
                var TotalOrderDesign = (from a in dbContext.OrderDesigns
                                        join b in dbContext.OrderEmblishments on a.Id equals b.OrderDesignId
                                        where b.EmblishmentType != "PlayerName" && b.EmblishmentType != "PlayerNumber" && a.OrderId == mode.orderid
                                        select new
                                        {
                                            id = b.Id
                                        }).Count();


                var IsOrderCompleted = (from a in dbContext.OrderEmblishments
                                        join b in dbContext.OrderStrikeOffLogos on a.Id equals b.OrderEmblishmentID into temp
                                        from b in temp.DefaultIfEmpty()
                                        where a.EmblishmentType != "PlayerName" && a.EmblishmentType != "PlayerNumber" && b.OrderID == mode.orderid && b.Status == 1
                                        select new
                                        {
                                            total = a.Id
                                        }).Count();
                if (TotalOrderDesign == IsOrderCompleted)
                {
                    SetOrderStatus(mode.orderid, 6);
                }
                // email 
                new EmailRepository().Email_E2_9(Convert.ToInt32(mode.orderid));


                return new WebAPIResponse { Message = "Success" };
            }
            else
            {
                return new WebAPIResponse { Message = "Error" };
            }
        }
        public WebAPIResponse RejectAllStrikeLogo(StrrikeModel obj)
        {
            try
            {
                var samples = dbContext.OrderStrikeOffLogos.Where(p => p.OrderID == obj.OrderId).ToList();

                foreach (var item in samples)
                {

                    OrderStrickOffModel mode = new OrderStrickOffModel();

                    mode.samplelogoid = item.ID;
                    mode.statusid = 2;
                    mode.userid = obj.UserId;
                    mode.orderid = obj.OrderId;
                    RejectAllStrikeLogo(mode);
                }

                return new WebAPIResponse { Message = "Success" };
            }
            catch (Exception ex)
            {
                return new WebAPIResponse { Message = "Error" };
            }
        }
        public void RejectAllStrikeLogo(OrderStrickOffModel mode)
        {
            try
            {
                var response = "";
                if (mode.samplelogoid != 0)
                {
                    StrikeOffSampleLogoModel strObj = new StrikeOffSampleLogoModel();
                    strObj.SampleLogoID = mode.samplelogoid;
                    strObj.statusid = mode.statusid;
                    response = setStrikeSampleLogoStatus(strObj);
                }

                if (response == "Success")
                {
                    CommunicationLog comObj = new CommunicationLog();
                    comObj.OrderID = mode.orderid;
                    comObj.Reason = mode.reason;
                    comObj.UserID = mode.userid;
                    comObj.IsPublic = mode.ispublic;
                    comObj.Date = DateTime.UtcNow;
                    comObj.SampleLogoID = mode.samplelogoid;
                    dbContext.CommunicationLogs.Add(comObj);
                    dbContext.SaveChanges();
                    // email 
                    new EmailRepository().Email_E2_8(Convert.ToInt32(mode.orderid), mode.reason);
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
            }
        }
        public WebAPIResponse RejectStrikeLogo(OrderStrickOffModel mode)
        {
            try
            {
                var response = "";
                if (mode.samplelogoid != 0)
                {
                    StrikeOffSampleLogoModel strObj = new StrikeOffSampleLogoModel();
                    strObj.SampleLogoID = mode.samplelogoid;
                    strObj.statusid = mode.statusid;
                    response = setStrikeSampleLogoStatus(strObj);
                }

                if (response == "Success")
                {
                    CommunicationLog comObj = new CommunicationLog();
                    comObj.OrderID = mode.orderid;
                    comObj.Reason = mode.reason;
                    comObj.UserID = mode.userid;
                    comObj.IsPublic = mode.ispublic;
                    comObj.Date = DateTime.UtcNow;
                    comObj.SampleLogoID = mode.samplelogoid;
                    dbContext.CommunicationLogs.Add(comObj);
                    dbContext.SaveChanges();

                    // email 
                    new EmailRepository().Email_E2_8(Convert.ToInt32(mode.orderid), mode.reason);

                    return new WebAPIResponse { Message = "Success", Result = mode.orderid };
                }
                else
                {
                    return new WebAPIResponse { Message = "Error" };
                }
            }
            catch (Exception ex)
            {
                return new WebAPIResponse { Message = ex.Message };
            }
        }


        public WebAPIResponse AddCommLog(OrderStrickOffModel mode)
        {
            try
            {
                if (mode.orderid != 0)
                {
                    CommunicationLog comObj = new CommunicationLog();
                    comObj.OrderID = mode.orderid;
                    comObj.Reason = mode.reason;
                    comObj.UserID = mode.userid;
                    comObj.IsPublic = mode.ispublic;
                    comObj.Date = DateTime.UtcNow;
                    dbContext.CommunicationLogs.Add(comObj);
                    dbContext.SaveChanges();
                    return new WebAPIResponse { Message = "Success", Result = mode.orderid };
                }
                else
                {
                    return new WebAPIResponse { Message = "Error" };
                }
            }
            catch (Exception ex)
            {
                return new WebAPIResponse { Message = ex.Message };
            }
        }

        #region "Scheduler"

        public List<SchedulerRejectDesign> GetAllRejectedDesign()
        {
            var justTime = DateTime.UtcNow.AddDays(-2);

            var DbRejectedDesigns = (from od in dbContext.OrderDesigns
                                         //where od.OrderDesignStatusId == (int)WMSOrderDesignStatusEnum.DesignRejected && od.UpdatedOn > DateTime.UtcNow
                                     where od.OrderDesignStatusId == 12 && od.UpdatedOn.Value < justTime
                                     select new SchedulerRejectDesign
                                     {
                                         OrderId = od.OrderId.Value,
                                         OrderDesignId = od.Id,
                                         OrderStatusId = od.OrderDesignStatusId.Value,
                                         DesignStatusId = od.OrderDesignStatusId.Value,
                                         CustomerId = od.CustomerId,
                                         ProductId = od.ProductId,
                                         CreatedBy = od.CreatedBy,
                                         UpdatedBy = od.UpdatedBy.Value,
                                         DesignName = od.DesignName,
                                         DesignNumber = od.DesignNumber
                                     }).ToList();
            return DbRejectedDesigns;
        }

        #endregion
    }
}
