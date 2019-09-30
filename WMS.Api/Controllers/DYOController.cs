using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WMS.Model.ApiResponse;
using WMS.Model.Common;
using WMS.Model.DYO;
using WMS.Model.Email;
using WMS.Model.Reports;
using WMS.Service.Repository;
using WMS.Services.Utility;
//using WMS.Reports;

namespace WMS.Api.Controllers
{
    public class DYOController : ApiController
    {
        [HttpGet]
        [Route("DYO/GetOrderDesignSheetReportObj")]
        public IHttpActionResult GetOrderDesignSheetReportObj(int orderId)
        {
            //orderId = 967;
            List<MainDataSource> list = new ReportRepository().GetJobSheetImage(orderId, "");
            var fileInfo = new Reports.ReportPrinting().Report(orderId, list);
            return Ok();
        }

        #region  my collection 
        [HttpPost]
        [Route("DYO/SaveCollection")]
        public IHttpActionResult SaveCollection(CollectionSaveModel collection)
        {
            bool status = new DYORepository().SaveCollection(collection);
            return Ok(status);


        }
        [HttpGet]
        [Route("DYO/AddTOCollection")]
        public IHttpActionResult AddTOCollection(int oderId)
        {
            bool status = new DYORepository().AddTOCollection(oderId);
            return Ok(status);

        }
        [HttpGet]
        [Route("DYO/DeleteOrderDesign")]
        public IHttpActionResult DeleteOrderDesign(int orderDesignId)
        {
            new DYORepository().sum(10);
            int MyOrderID = 0;
            bool status = new DYORepository().DeleteOrderDesign(orderDesignId, ref MyOrderID);

            int TotalOrderDesigns = new DYORepository().TotalOrderDesigns(MyOrderID);

            if (TotalOrderDesigns > 0)
            {
                var fileInfo2 = new WMS.Reports.BarCodeReport().JobSheet(MyOrderID);
                bool status4 = new DYORepository().SaveJobsheetFile(MyOrderID, fileInfo2.FileName);

                if (new DYORepository().IsSendToEmailToMerchandiser(MyOrderID))
                {
                    new EmailRepository().Email_E2_4(MyOrderID);     // Send email to merchandiser user     
                }
            }

            return Ok(status);
        }
        [HttpPost]
        [Route("DYO/ViewCollection")]
        public List<CollectionDYOOrder> ViewCollection(MyOrderTrackAndTrace trackObj)

        {
            List<CollectionDYOOrder> list = new DYORepository().ViewCollection(trackObj);
            return list;
        }

        [HttpGet]
        [Route("DYO/ViewCollectionDetail")]
        public CollectionDYOOrder ViewCollectionDetail(int collectionId)
        {
            CollectionDYOOrder collectionDetail = new DYORepository().CollectionDetail(collectionId);
            return collectionDetail;

        }
        [HttpGet]
        [Route("DYO/DeleteCollection")]
        public bool DeleteCollection(int collectionId)
        {
            bool status = new DYORepository().DeleteCollection(collectionId);
            return status;
        }
        #endregion

        #region Place Order
        [HttpGet]
        [Route("DYO/SKuQuantity")]
        public IHttpActionResult SKuQuantity(int orderDesignId, int sizeId)
        {
            int quantity = new DYORepository().SKUQuantity(orderDesignId, sizeId);
            return Ok(quantity);
        }
        [HttpGet]
        [Route("DYO/SendEmail")]
        public IHttpActionResult SendEmail(string emailNumber)
        {
            //bool status = new EmailRepository().Email_E2(34);
            bool status = new EmailRepository().Email_E2_1(34);
            return Ok(status);
        }
        [HttpPost]
        [Route("DYO/PlaceOrder")]
        public IHttpActionResult PlaceOrder(DYOOrder order)
        {
            ApiResponse response = new DYORepository().PlaceOrder(order);
            // Send order email 
            if (response.Status)
            {
                // var isJobSheet = new DYORepository().IsJobSheetCreate(order.OrderId);
                // if (isJobSheet)
                {
                    var fileInfo = new WMS.Reports.BarCodeReport().JobSheet(order.OrderId);
                    bool status3 = new DYORepository().SaveJobsheetFile(order.OrderId, fileInfo.FileName);
                }

                {
                    var fileInfo2 = new WMS.Reports.BarCodeReport().JobSheetShippingFile(order.OrderId);
                    bool status4 = new DYORepository().SaveJobsheetShippingFile(order.OrderId, fileInfo2.FileName);
                }

                // Email to Customer 
                bool status = new EmailRepository().Email_E2(order.OrderId);


                // Email to sales cordinator 
                bool status1 = new EmailRepository().Email_E2_1(order.OrderId);
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("DYO/UpdateDeliveryDate")]
        public IHttpActionResult UpdateDeliveryDate(DeliveryDateModel obj)
        {
            try
            {
                var Status = new DYORepository().UpdateDeliveryDate(obj);
                var fileInfo2 = new WMS.Reports.BarCodeReport().JobSheetShippingFile(obj.OrderId);
                bool status4 = new DYORepository().SaveJobsheetShippingFile(obj.OrderId, fileInfo2.FileName);
                return Ok(Status);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpGet]
        [Route("DYO/GetDeliveryDate")]
        public DeliveryDateModel GetDeliveryDate(int userId, int orderId)
        {
            try
            {
                var Status = new DYORepository().GetDeliveryDate(userId, orderId);
                return Status;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }
        [HttpGet]
        [Route("DYO/JobShhet")]
        public IHttpActionResult JobShhet(int orderId)
        {
            //var fileInfo = new WMS.Reports.BarCodeReport().JobSheet(orderId);
            //return Ok(fileInfo);
            return Ok();
        }

        [HttpGet]
        [Route("DYO/OrderInitials")]
        public DYOOrder OrderInitials(int orderId)
        {
            DYOOrder orderInitials = new DYORepository().OrderInitials(orderId);
            return orderInitials;
        }
        [HttpGet]
        [Route("DYO/OrderCollectionInitials")]
        public DYOPrderCollectionInitials OrderCollectionInitials(int orderId)
        {
            DYOPrderCollectionInitials initials = new DYORepository().OrderCollectionInitials(orderId);
            return initials;
        }
        public DYOPrderCollectionInitials OrderDetailInitials(int orderDesignId)
        {
            DYOPrderCollectionInitials initials = new DYORepository().OrderCollectionInitials(orderDesignId);
            return initials;
        }

        [HttpGet]
        [Route("DYO/GeneratePickRequest")]
        public IHttpActionResult GeneratePickRequest(int orderId)
        {
            try
            {
                // Generate pick up note 
                new DYORepository().SaveOrderPickupDetail(orderId);

                var filerInfo = new WMS.Reports.BarCodeReport().PickRequests(orderId);
                new DYORepository().SavePickupInformation(orderId, filerInfo.FileName);

                // email to warehouse user with pick up note
                new EmailRepository().SendEmailE3(orderId);

                return Ok();
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        #endregion

        #region My Orders

        [HttpGet]
        public DYOOrderDetail OrderDetail(int orderId)
        {
            DYOOrderDetail orderDetail = new DYORepository().OrderDetail(orderId);

            return orderDetail;
        }

        [HttpPost]
        [Route("DYO/TrackAndTraceOrders")]
        public IHttpActionResult TrackAndTraceOrders(MyOrderTrackAndTrace trackObj)
        {
            List<DYOOrderGrid> dyoOrders = new DYORepository().TrackAndTraceOrders(trackObj);
            return Ok(dyoOrders);
        }

        [HttpGet]
        [Route("DYO/OrderTRackAndTraceInitials")]
        public OrderTrackAndTraceInitials OrderTRackAndTraceInitials(int userId)
        {
            OrderTrackAndTraceInitials initials = new DYORepository().OrderTRackAndTraceInitials(userId);
            return initials;

        }

        [HttpGet]
        [Route("DYO/OrderCartons")]
        public IHttpActionResult OrderCartons(int orderId)
        {
            List<WMSFile> list = new DYORepository().OrderCartons(orderId);
            return Ok(list);
        }
        #endregion 

        #region Design  Order
        [HttpPost]
        [Route("DYO/DesignProduct")]
        public IHttpActionResult DesignProduct(DYOOrderCollection productDesign)
        {
            DYOOrderCollection response = new DYORepository().DesignProduct(productDesign);
            //  Create Design Sheet

            //if (!productDesign.OrderDesigns[0].IsRawGarment)
            // {
            List<MainDataSource> list = new ReportRepository().GetJobSheetImage(response.OrderId, "");
            var fileInfo = new Reports.ReportPrinting().Report(response.OrderId, list);
            response.JobsheetPath = fileInfo.FilePath;
            response.JobSheetName = fileInfo.FileName;
            var data = new DYORepository().getDesignNamePath(response.OrderDesigns[0].OrderDesignId);
            response.DesignImageName = data.FileName;
            response.DesignImagePath = data.FilePath;
            bool status = new DYORepository().SaveJobsheetFile(response.OrderId, fileInfo.FileName);

            if (productDesign.OrderDesigns[0].IsMailSendToMerchandiser)
            {
                // email to merchandiser that design is modified  
                bool s = new EmailRepository().Email_E41(productDesign.OrderId, productDesign.OrderDesigns[0].OrderDesignId);
            }
            if (productDesign.OrderDesigns[0].IsMailSendToSalesCordinator)
            {
                //  email to sales cordinator that design is modified  
                bool s = new EmailRepository().Email_E4(productDesign.OrderId, productDesign.OrderDesigns[0].OrderDesignId);
            }
            //else
            //{
            //    var data = new DYORepository().getDesignNamePath(response.OrderDesigns[0].OrderDesignId);
            //    response.DesignImageName = data.FileName;
            //    response.DesignImagePath = data.FilePath;
            //}
            return Ok(response);
        }
        #endregion

        #region  Initials    
        [HttpGet]
        [Route("DYO/GetAvailableStock")]
        public IHttpActionResult GetAvailableStock(int userId, int orderId)
        {
            List<DYODesignCatagory> list = new DYORepository().GetAvailableStock(userId, orderId);
            return Ok(list);
        }

        [HttpGet]
        [Route("DYO/ProductDesignInitials")]
        public ProductDesignInitials ProductDesignInitials(int productId, int customerId)
        {
            try
            {
                ProductDesignInitials initials = new DYORepository().DesignInitials(productId, customerId);
                return initials;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        public IHttpActionResult GetStockProducts()
        {

            return Ok();
        }

        [HttpGet]
        [Route("DYO/GetDesignDetail")]
        public DYOProductDesign GetDesignDetail(int designId)
        {
            try
            {
                DYOProductDesign design = new DYORepository().GetDesignDetail(designId);
                return design;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }

        }
        #endregion

        #region Order Details
        [HttpGet]
        [Route("DYO/getOrderDetails")]
        public IHttpActionResult getOrderDetails(int OrderId, int userId)
        {
            try
            {
                var Result = new DYORepository().getOrderDetails(OrderId, userId);
                return Ok(Result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Design process 

        [HttpPost]
        [Route("DYO/AcceptrejectDesign")]
        public IHttpActionResult AcceptRejectDesign(AcceptRejectDesign obj)
        {
            try
            {
                AcceptRejectDesignResultModel status = new DYORepository().AcceptRejectDesign(obj);
                return Ok(status);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest();
            }
        }

        #endregion

        #region Get strike approval data
        [HttpGet]
        [Route("DYO/getStrikeApprovalList")]
        public IHttpActionResult getStrikeApprovalList(int orderid, int roleid)
        {
            var Result = new DYORepository().getStrikeApprovalList(orderid, roleid);
            return Ok(Result);
        }
        #endregion

        //#region Get strike approval data
        //[HttpPost]
        //[Route("DYO/setStrikeLogoStatus")]
        //public IHttpActionResult setStrikeLogoStatus(StrikeApprovalModel model)
        //{
        //    var Result = new DYORepository().setStrikeLogoStatus(model);
        //    return Ok(Result);
        //}
        //#endregion

        #region get communication log
        [HttpPost]
        [Route("DYO/AcceptOrder")]
        public IHttpActionResult AcceptOrder(OrderDesignModel obj)
        {

            OrderDesignModel obj1 = new DYORepository().GetAcceptOrderDYO(obj);
            var Result = new DYORepository().AcceptOrderDesign(obj1);
            if (obj.roleid == (byte)WMSUserRoleEnum.Merchandise)
            {
                if (obj.statusid != (byte)WMSOrderStatusEnum.ReadyForDispatch)
                {
                    // Generate pick up note 
                    new DYORepository().SaveOrderPickupDetail(obj.orderid);

                    var filerInfo = new WMS.Reports.BarCodeReport().PickRequests(obj.orderid);
                    new DYORepository().SavePickupInformation(obj.orderid, filerInfo.FileName);

                    // email to warehouse user with pick up note
                    new EmailRepository().SendEmailE3(obj.orderid);
                }
            }
            if (Result.Message == "Success")
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }


        }
        [HttpPost]
        [Route("DYO/RejectOrder")]
        public IHttpActionResult RejectOrder(OrderDesignModel obj)
        {

            OrderDesignModel obj1 = new DYORepository().GetAcceptOrderDYO(obj);
            var Result = new DYORepository().RejectOrderDesign(obj1);

            if (Result.Message == "Success")
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }


        }

        [HttpPost]
        [Route("DYO/AddCommunicationLog")]
        public IHttpActionResult AddCommunicationLog(DYOOrderComunicationLog comm)
        {
            WebAPIResponse result = new DYORepository().AddCommunicationLog(comm);
            return Ok(result);
        }

        [HttpGet]
        [Route("DYO/GetCommunicationLogByOrderid")]
        public IHttpActionResult GetCommunicationLogByOrderid(int orderid)
        {
            var Result = new DYORepository().GetCommunicationLogByOrderid(orderid);
            return Ok(Result);
        }
        #endregion

        #region "Other Action"
        [HttpPost]
        public IHttpActionResult ManageStrikeOffSampleLogo()
        {
            try
            {
                string filename = "";

                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    List<StrikeOffSampleLogo> li = new List<StrikeOffSampleLogo>();
                    foreach (string file in httpRequest.Files)
                    {
                        string myfile = file;
                        int orderid = Convert.ToInt32(myfile.Split('_')[0]);
                        int orderemblishid = Convert.ToInt32(myfile.Split('_')[1]);
                        string orderdesignid = Convert.ToString(new DYORepository().getOrderDesignId(orderemblishid));
                        var postedFile = httpRequest.Files[file];
                        if (postedFile != null && postedFile.ContentLength > 0)
                        {
                            var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                            filename = Guid.NewGuid() + ext.ToLower();

                            string dirPath = HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderid + "/" + orderdesignid);
                            // Check for Directory, If not exist, then create it  
                            if (!Directory.Exists(dirPath))
                            {
                                Directory.CreateDirectory(dirPath);
                            }
                            var filePath = dirPath + "/" + filename;

                            postedFile.SaveAs(filePath);
                        }
                        li.Add(new StrikeOffSampleLogo
                        {
                            OrderID = orderid,
                            OrderEmblishmentID = orderemblishid,
                            FileName = filename
                        });
                    }
                    var result = new DYORepository().ManageStrikeOffSampleLogo(li);
                    return Ok(result);
                }
                else { return Ok("Please upload File(s)!"); }
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpGet]
        [Route("DYO/LogoStatus")]
        public IHttpActionResult LogoStatus(int orderId, int userId)
        {
            int StatusId = new DYORepository().GetLogoStatus(orderId, userId);
            return Ok(StatusId);
        }

        [HttpPost]
        [Route("DYO/AcceptOrderDesign")]
        public IHttpActionResult AcceptOrderDesign(OrderDesignModel obj)
        {
            try
            {
                var Result = new DYORepository().AcceptOrderDesign(obj);
                var fileInfo = new WMS.Reports.BarCodeReport().JobSheet(obj.orderid);
                bool status3 = new DYORepository().SaveJobsheetFile(obj.orderid, fileInfo.FileName);
                if (obj.roleid == (byte)WMSUserRoleEnum.Merchandise)
                {
                    if (obj.statusid != (byte)WMSOrderStatusEnum.ReadyForDispatch)
                    {
                        // Generate pick up note 
                        new DYORepository().SaveOrderPickupDetail(obj.orderid);

                        var filerInfo = new WMS.Reports.BarCodeReport().PickRequests(obj.orderid);
                        new DYORepository().SavePickupInformation(obj.orderid, filerInfo.FileName);

                        // email to warehouse user with pick up note
                        new EmailRepository().SendEmailE3(obj.orderid);
                    }
                }
                return Ok(Result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost]
        [Route("DYO/RejectOrderDesign")]
        public IHttpActionResult RejectOrderDesign(OrderDesignModel obj)
        {
            try
            {
                var Result = new DYORepository().RejectOrderDesign(obj);
                return Ok(Result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }


        [HttpPost]
        [Route("DYO/AcceptAllStrikeLogo")]
        public IHttpActionResult AcceptAllStrikeLogo(StrrikeModel obj)
        {
            try
            {
                var Result = new DYORepository().AcceptAllStrikeLogo(obj);
                return Ok();
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpPost]
        [Route("DYO/RejectAllStrikeLogo")]
        public IHttpActionResult RejectAllStrikeLogo(StrrikeModel obj)
        {
            try
            {
                var Result = new DYORepository().RejectAllStrikeLogo(obj);
                return Ok();
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpPost]
        [Route("DYO/AcceptStrikeLogo")]
        public IHttpActionResult AcceptStrikeLogo(StrikeOffSampleLogoModel obj)
        {
            try
            {
                var Result = new DYORepository().AcceptStrikeLogo(obj);
                return Ok();
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }



        [HttpPost]
        [Route("DYO/RejectStrikeLogo")]
        public IHttpActionResult RejectStrikeLogo(OrderStrickOffModel obj)
        {
            try
            {
                var Result = new DYORepository().RejectStrikeLogo(obj);
                return Ok(Result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost]
        [Route("DYO/AddCommLog")]
        public IHttpActionResult AddCommLog(OrderStrickOffModel obj)
        {
            try
            {
                var Result = new DYORepository().AddCommLog(obj);
                return Ok(Result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        #endregion

        #region "Scheduler Action"

        [HttpPost]
        [Route("DYO/RemoveRejectedDesign")]
        public IHttpActionResult GetAllRejectedDesign()
        {
            try
            {
                var RejectedDesigns=new DYORepository().GetAllRejectedDesign();

                foreach(var objRejectedDesign in RejectedDesigns)
                {   
                    int MyOrderID = 0;
                    bool status = new DYORepository().DeleteOrderDesign(objRejectedDesign.OrderDesignId, ref MyOrderID);

                    int TotalOrderDesigns = new DYORepository().TotalOrderDesigns(MyOrderID);

                    if (TotalOrderDesigns > 0)
                    {
                        var fileInfo2 = new WMS.Reports.BarCodeReport().JobSheet(MyOrderID);
                        bool status4 = new DYORepository().SaveJobsheetFile(MyOrderID, fileInfo2.FileName);

                        if (new DYORepository().IsSendToEmailToMerchandiser(MyOrderID))
                        {
                            new EmailRepository().Email_E2_4(MyOrderID);     // Send email to merchandiser user     
                        }
                    }
                }
                return Ok(true);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        [HttpPost]
        [Route("DYO/ReCreateJobSheet")]
        public IHttpActionResult ReCreateJobSheet(int OrderID)
        {
            try
            {
                var fileInfo2 = new WMS.Reports.BarCodeReport().JobSheet(OrderID);
                bool status4 = new DYORepository().SaveJobsheetFile(OrderID, fileInfo2.FileName);

                return Ok(true);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        #endregion
    }
}
