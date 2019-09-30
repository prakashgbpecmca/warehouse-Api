using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WMS.Model.Product;
using WMS.Service.Repository;

namespace WMS.Api.Controllers
{
    public class ProductController : ApiController
    {



        [Route("Product/SaveProductDetail")]
        [HttpPost]
        public IHttpActionResult SaveProductDetail(ProductSKUModel obj)
        {
            var result = new ProductRepository().SaveProductDetail(obj);
            return Ok();
        }


        [Route("Product/Initials")]
        [HttpGet]
        public IHttpActionResult Initials()
        {
            var result = new ProductRepository().Initials();
            return Ok(result);
        }

        [Route("Product/GetProductList")]
        [HttpGet]
        public IHttpActionResult GetProductList(int id, string productColor, byte productSize,int pageIndex, int pageSize)
        {
            var result = new ProductRepository().GetProductList(id, productColor, productSize, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("Product/GetProductList")]
        [HttpPost]
        public IHttpActionResult GetProductList(ProductTrackAndTrace track) 
        {
            var result = new ProductRepository().GetProductList(track);

            return Ok(result);
        }

        [Route("Product/DeleteProductDetails")]
        [HttpDelete]
        public IHttpActionResult DeleteProductDetails(int id)
        {
            var result = new ProductRepository().DeleteProductDetails(id);
            return Ok(result);
        }
        
        [Route("Product/BindProductNameDDL")]
        [HttpGet]
        public IHttpActionResult BindProductNameDDL(int id)
        {
            var result = new ProductRepository().BindProductNameDDL(id);
            return Ok(result);
        }
        [Route("Product/GetProductList")]
        [HttpGet]
        public IHttpActionResult GetProductList()
        {
            var result = new ProductRepository().GetProductMaster();
            return Ok(result);
        }
        [Route("Product/GetProductSKUByPID")]
        [HttpGet]
        public IHttpActionResult GetProductSKUByPID(int pID)
        {
            var result = new ProductRepository().GetProductSKUByPID(pID);
            return Ok(result);
        }

        [Route("Product/CheckIsSKUExist")]
        [HttpGet]
        public IHttpActionResult CheckIsSKUExist(string SKU)
        {
            var result = new ProductRepository().CheckIsSKUExist(SKU);
            return Ok(result);
        }
    }
}
