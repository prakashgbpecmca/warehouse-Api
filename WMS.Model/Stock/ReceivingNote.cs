using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Model.Shelf;

namespace WMS.Model.Stock
{
    public class PickOrderModel
    {
        public int OrderID { get; set; }
        public string OrderNumber { get; set; }
        public int OrderDesignDetailId { get; set; }
        public int SectionDetailID { get; set; }
        public string line { get; set; }
        public string rack { get; set; }
        public string row { get; set; }
        public string section { get; set; }
        public string sectionbarcode { get; set; }

        public int ProductSKUID { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductCode { get; set; }
        public string SKU { get; set; }

        public int ColorId { get; set; }
        public string color1 { get; set; }
        public int SizeId { get; set; }
        public string size1 { get; set; }
        public int RequiredQty { get; set; }
        public int PickedQty { get; set; } 
    }
    public class ReceivingNoteResponse
    {
        public bool Status { get; set; }
        public int CustomerId { get; set; }
        public List<UserStockModel> Stocks { get; set; }
        public FilePathModel FileInfo { get; set; }
    }
}
