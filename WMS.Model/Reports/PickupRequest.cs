using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Model.Reports
{
    public class PickupRequestReportModel
    {
        public string PrintedBy { get; set; }
        public string PrintedOn { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string Barcode { get; set; }
        public string ShippingAddress { get; set; }
        public string InvoiceAddress { get; set; }
        public List<PickupRequestReportModelDetail> Stocks { get; set; }
    }

    public class PickupRequestReportModelDetail
    {
        public int SNo { get; set; }
        public string Line { get; set; }
        public string Rack { get; set; }
        public string Row { get; set; }
        public string PickLocation { get; set; }
        public string SKU { get; set; }
        public string DesignNumber { get; set; }
        public string ProductColor { get; set; }
        public string Description { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public int QuantityToPick { get; set; }
        public int PickedQuantity { get; set; }
        public int SectionDetailId { get; set; }
    }
}
