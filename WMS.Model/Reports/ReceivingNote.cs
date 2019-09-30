using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Model.Reports
{
    public class ReceivingNoteReport
    {
        public string CustomerCompany { get; set; }
        public string PrintedBy { get; set; }
        public string PrintedOn { get; set; }
        public string Barcode { get; set; }
        public string ReceivingNote { get; set; }
        public string CustomerInfo { get; set; }
        public List<ReceivingNoteReportDetail> Stocks { get; set; }
    }

   public class ReceivingNoteReportDetail
    {
        public int SNo { get; set; }
        public string ProductName { get; set; }
        public string ProductColor { get; set; }
        public string ProductSize { get; set; }
        public string SKU { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight{ get; set; }
        public int TotalQuantity { get; set; }
    }
}
