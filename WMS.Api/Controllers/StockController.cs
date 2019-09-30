using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WMS.Model.Product;
using WMS.Model.Reports;
using WMS.Model.Shelf;
using WMS.Model.Stock;
using WMS.Model.User;
using WMS.Service.Repository;

namespace WMS.Api.Controllers
{
    public class StockController : ApiController
    {
        // GET api/<controller>
        [HttpGet]
        [Route("Stock/GetStockRangeCustomer")]
        public IHttpActionResult GetStockRangeCustomer(int userId)
        {
            try
            {
                List<WMSCustomreGrid> list = new StockRespository().GetStockRangeCustomers(userId);
                return Ok(list);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }

        }

        // GET api/<controller>
        public IHttpActionResult Get(HttpRequestMessage request)
        {
            int userid = 0;
            var queryStrings = request.GetQueryNameValuePairs();
            var id = queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, "userid", true) == 0);
            var keyword = queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, "keyword", true) == 0);

            if (id.Value != "null" && id.Value != "" && id.Value != null)
            {
                userid = Convert.ToInt32(id.Value);
                var result = new StockRespository().getCustomerStockList(userid, keyword.Value);
                return Ok(result);
            }
            return null;
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public IHttpActionResult Post(productStock model)
        {
            var result = new StockRespository().addStock(model);
            return Ok(result);
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {

        }
        [Route("Stock/AllocateStockToUser")]
        [HttpPost]
        public IHttpActionResult AllocateStockToUser(UserStockList model)
        {
            var result = new StockRespository().AllocateStockToUser(model);
            return Ok(result);

        }
        [Route("Stock/GetProductSKUList")]
        [HttpGet]
        public IHttpActionResult getProductSKUList(int UserId, string keyword, int colorid, int sizeid)
        {
            var result = new StockRespository().GetProductSKUList(UserId, keyword, colorid, sizeid);
            return Ok(result);
        }
        [Route("Stock/GetStockProductList")]
        [HttpGet]
        public IHttpActionResult GetStockProductList()
        {
            var result = new ProductRepository().GetStockProductMaster();
            return Ok(result);
        }
        [Route("Stock/GetCustomers")]
        [HttpPost]
        public List<WMSCustomreGrid> GetCustomers(TrackUser model)
        {
            List<WMSCustomreGrid> list = new StockRespository().GetCustomers(model);
            return list;
        }
        [HttpPost]
        [Route("Stock/EditStock")]
        public IHttpActionResult EditStock(UserStockModel item)
        {
            try
            {
                bool status = new StockRespository().UpdateStock(item);
                return Ok(status);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest();
            }
        }

        [Route("Stock/GenerateReceivingNote")]
        [HttpGet]
        public IHttpActionResult GenerateReceivingNote(int userId, int customerId)
        {
            ReceivingNoteResponse response = new ReceivingNoteResponse();
            try
            {
                if (customerId > 0)
                {
                    response.CustomerId = customerId;
                    response.Stocks = new StockRespository().GetReceivingNoteStock(customerId);
                    if (response.Stocks.Count > 0)
                    {
                        // Save Receiving stock 
                        new StockRespository().SaveReceivingNote(userId, customerId, response);

                        // Generate receiving note 
                        response.FileInfo = new WMS.Reports.BarCodeReport().ReceivingNote(userId, customerId);

                        // Save receiving note file 
                        new StockRespository().SaveReceivingNoteFile(userId, customerId, response);

                        response.Status = true;
                    }
                    else
                    {
                        response.Status = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                response.Status = false;
            }
            return Ok(response);

        }



    }
}