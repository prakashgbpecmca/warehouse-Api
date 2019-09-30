using System;
using System.Collections.Generic;
using System.Web.Http;
using WMS.Model.ApiResponse;
using WMS.Model.Carton;
using WMS.Model.Product;
using WMS.Model.Shelf;
using WMS.Service.Repository;

namespace WMS.Api.Controllers
{
    public class MobileController : ApiController
    {
        [Route("Mobile/GetLocationStock")]
        [HttpGet]
        public IHttpActionResult GetLocationStock(int id)
        {
            var detail = new ShelfRespository().GetLocationStock(id);
            return Ok(detail);
        }

        public IHttpActionResult Get()
        {
            WebAPIResponse res = new WebAPIResponse { Message = "Success", Result = new ShelfRespository().GetCartonProducts(1) };
            return Ok(res);
        }

        #region Warehouse: item's rack allocation

        [Route("Mobile/ScanSection")]
        [HttpGet]
        public IHttpActionResult ScanSection(string barcode)
        {

            var result = new ShelfRespository().ScanSection(barcode);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));
            return Ok(result);
        }

        [Route("Mobile/GetProductBySKU")]
        [HttpGet]
        public IHttpActionResult GetProductBySKU(string sku)
        {
            var result = new ShelfRespository().GetProductBySKU(sku);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));
            return Ok(result);
        }

        [Route("Mobile/AllocateStocktoSection")]
        [HttpPost]
        public IHttpActionResult AllocateStocktoSection(allocateSectionStockModel model)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));

            var result = new ShelfRespository().AllocateStocktoSection(model);
            return Ok(result);
        }

        #endregion

        #region Warehouse: pickup process

        [Route("Mobile/PickupOrderStockList")]
        [HttpGet]
        public IHttpActionResult PickupOrderStockList(string barcode)
        {
            var result = new ShelfRespository().PickupOrderStockList(barcode);
            return Ok(result);
        }

        [Route("Mobile/ScanLocation")]
        [HttpGet]
        public IHttpActionResult ScanLocation(string barcode)
        {
            var result = new ShelfRespository().ScanLocation(barcode);
            return Ok(result);
        }

        [Route("Mobile/PickUpStock")]
        [HttpPost]
        public IHttpActionResult PickUpStock(List<ProductDetails> ProductDetails)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Request hit"));
            dynamic json = Newtonsoft.Json.JsonConvert.SerializeObject(ProductDetails);
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));
            var result = new ProductRepository().PickUpStock(ProductDetails);

            // Generate Updated Pick up note 
            var fileInfo = new WMS.Reports.BarCodeReport().UpdatedPickRequests(ProductDetails[0].OrderId);
            new DYORepository().SaveUpdatedPickupInformation(ProductDetails[0].OrderId, fileInfo.FileName);
            return Ok(result);
        }

        #endregion

        #region Stock: Receive stock

        [Route("Mobile/GetProductDetailByPDFno")]
        [HttpGet]
        public IHttpActionResult GetProductDetailByPDFno(string PDFno)
        {
            var result = new ProductRepository().GetProductDetailByPDFno(PDFno);
            return Ok(result);
        }

        [Route("Mobile/ReceiveStock")]
        [HttpPost]
        public IHttpActionResult ReceiveStock(List<ProductReceiveStockDetails> ProductDetails)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Request hit"));
            dynamic json = Newtonsoft.Json.JsonConvert.SerializeObject(ProductDetails);
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));
            var result = new ProductRepository().ReceiveStock(ProductDetails);
            return Ok(result);
        }

        #endregion

        #region Shipping: Dispatch process

        [Route("Mobile/DispatchOrderStockList")]
        [HttpGet]
        public IHttpActionResult DispatchOrderStockList(string barcode)
        {
            var result = new ShelfRespository().DispatchOrderStockList(barcode);
            return Ok(result);
        }

        [Route("Mobile/ManageCartonLabel")]
        [HttpPost]
        public IHttpActionResult ManageCartonLabel(OrderCartonModel details)
        {
            var result = new ShelfRespository().ManageCartonLabel(details);
            if (result.Message == "Success")
            {
                var cartons = new ShelfRespository().GetOrderCartons(details.OrderID);
                if (cartons != null && cartons.Count > 0)
                {
                    foreach (var item in cartons)
                    {
                        new WMS.Reports.BarCodeReport().CartonLabel(item.ID, details.OrderID.Value);
                    }
                }
            }
            return Ok(result);
        }

        [Route("Mobile/DispatchOrder")]
        [HttpGet]
        public IHttpActionResult DispatchOrder(int orderid)
        { 
            var result = new DYORepository().SetOrderStatus(orderid, 8);
            // Generate dispatch label for  warehouse user
            try
            {
                var file = new WMS.Reports.ReportPrinting().WarehouseDispatchLabel(orderid, 0);
                new DYORepository().SaveDispatchProcessInformation(orderid, file.FileName);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return Ok(result);
        }

        #endregion
    }
}
