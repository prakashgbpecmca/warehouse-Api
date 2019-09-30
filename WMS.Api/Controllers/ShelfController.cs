using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WMS.Model.Shelf;
using WMS.Service.Repository;
// using WMS.Reports;
using System.IO;
using System.Net.Http.Headers;

namespace WMS.Api.Controllers
{
    public class ShelfController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get(HttpRequestMessage request)
        {
            var queryStrings = request.GetQueryNameValuePairs();
            var wid = queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, "wid", true) == 0);
            var id = queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, "id", true) == 0);

            var lineid = queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, "lineid", true) == 0);
            var shelfid = queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, "shelfid", true) == 0);

            if (wid.Value != "null" && wid.Value != "" && wid.Value != null)
            {
                var result = new ShelfRespository().GetShelfDetail(Convert.ToInt32(wid.Value), Convert.ToInt32(lineid.Value), Convert.ToInt32(shelfid.Value));
                return Ok(result);
            }
            if (id.Value != "null" && id.Value != "" && id.Value != null)
            {
                var result = new ShelfRespository().GetShelfDetailById(Convert.ToInt32(id.Value));
                return Ok(result);
            }
            return null;
        }
        public IHttpActionResult Post(List<ShelfList> model)
        {
            var result = new ShelfRespository().ManageShelfDetail(model);
            return Ok(result);
        }
        [Route("Shelf/Initials")]
        [HttpGet]
        public IHttpActionResult Initials()
        {
            var result = new ShelfRespository().Initials();
            return Ok(result);
        }
        [Route("Shelf/getBoxDataCount")]
        [HttpGet]
        public IHttpActionResult getBoxDataCount(int wid, int lineid, int shelfid)
        {
            var result = new ShelfRespository().getBoxDataCount(wid, lineid, shelfid);
            return Ok(result);
        }
        [Route("Shelf/IsExistShelfData")]
        [HttpGet]
        public IHttpActionResult IsExistShelfData(int wid, int lineid, int shelfid, int rowid)
        {
            var result = new ShelfRespository().IsExistShelfData(wid, lineid, shelfid, rowid);
            return Ok(result);
        }

        [Route("Shelf/GetShelfBarCodeDetail")]
        [HttpGet]
        public FilePathModel GetShelfBarCodeDetail(int WarehouseId, int LineId, int ShelfId)
        {

            var result = new WMS.Reports.BarCodeReport().Barcode(WarehouseId, LineId, ShelfId);

            //using (MemoryStream ms = new MemoryStream())
            //{
            //    using (FileStream file = new FileStream(result.FilePhysicalPath, FileMode.Open, FileAccess.Read))
            //    {
            //        byte[] bytes = new byte[file.Length];
            //        file.Read(bytes, 0, (int)file.Length);
            //        ms.Write(bytes, 0, (int)file.Length);
            //        HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            //        httpResponseMessage.Content = new ByteArrayContent(bytes);
            //        httpResponseMessage.Content.Headers.Add("download-status", "downloaded");
            //        httpResponseMessage.Content.Headers.Add("x-filename", result.FileName);
            //        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            //        httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            //        httpResponseMessage.Content.Headers.ContentDisposition.FileName = result.FileName;
            //        httpResponseMessage.StatusCode = HttpStatusCode.OK;
            //      //  return httpResponseMessage;
            //    }
            //} 
            return result;
        }

        [Route("Shelf/IsShelfOrderWise")]
        [HttpPost]
        public IHttpActionResult IsShelfOrderWise(ShelfModel model)
        {
            var result = new ShelfRespository().IsShelfOrderWise(model);
            return Ok(result);
        }
        //[Route("Shelf/GetShelfBarCodeDetail")]
        //[HttpGet]
        //public IHttpActionResult GetShelfBarCodeDetail(int WarehouseId, int LineId, int ShelfId)
        //{
        //    var result = new ShelfRespository().GetShelfBarCodeDetail(WarehouseId, LineId, ShelfId);
        //    return Ok(result);
        //}

    }
}

