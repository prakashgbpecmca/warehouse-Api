using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WMS.Reports;
using static WMS.Reports.ReportPrinting;

namespace WMS.Api.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

        [HttpGet]
        [Route("Values/DispatchLabel")]
        public IHttpActionResult DispatchLabel(int orderId, int userId)
        {
            ReportFile file = new ReportPrinting().WarehouseDispatchLabel(orderId, userId);
            return Ok(file);
        }
    }
}
