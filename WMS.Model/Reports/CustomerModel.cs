using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Model.Reports
{
    public class CustomerModel
    {
        public HeaderModel HeaderName { get; set; }
        public List<ValuesModel> valuesCollection { get; set; }
    }

    public class CustomerColor
    {
        public int ColorId { get; set; }
        public string ColorName { get; set; }
    }

    public class HeaderSize
    {
        public int SizeId { get; set; }
        public string SizeHeader { get; set; }
    }

    public class HeaderModel
    {
        public string Customer { get; set; }
        public string Range { get; set; }
        public string StyleNo { get; set; }
        public string Color { get; set; }
        public List<string> Headers { get; set; }
        public string Total { get; set; }
        public string DataType { get; set; }
    }

    public class ValuesModel
    {
        public string Customer { get; set; }
        public string Range { get; set; }
        public string StyleNo { get; set; }
        public string Color { get; set; }
        public List<decimal> HeadersValue { get; set; }
        public decimal Total { get; set; }
        public string DataType { get; set; }
    }

    public class CustomerRawModel
    {
        public string CompanyName { get; set; }
        public string Range { get; set; }
        public string StyleNo { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
       
    }
}
