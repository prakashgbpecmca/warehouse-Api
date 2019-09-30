using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WMS.Model.Reports;
using WMS.Reports;
using WMS.Service.Repository;

namespace WMS.Api.Controllers
{
    public class ReportController : ApiController
    {
        [HttpPost]
        public IHttpActionResult GetReport(ReportTrackandTraceModel ReportDetail)
        {
            return Ok(new BarCodeReport().GetReport(ReportDetail));
        }
    }
}
