using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Model.Reports
{
    public class WarehouseLabels
    {
        public List<WarehouseDispatch> warehouse { get; set; }
    }

    public class WarehouseDispatch
    {
        public string PickNumber { get; set; }
        public string PONumber { get; set; }
        public DateTime Date { get; set; }
        public string TimeZone { get; set; }
        public int Pices { get; set; }
        public string WeightUnit { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public string CustomerId { get; set; }
        public string Carton { get; set; }
    }
}
