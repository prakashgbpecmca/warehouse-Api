using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Model.Reports
{
    public class ReportTrackandTraceModel
    {
        public int UserId { get; set; }
        public int CustomerId { get; set; }
        public string ReportType { get; set; }
        public string ReportStatus { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
