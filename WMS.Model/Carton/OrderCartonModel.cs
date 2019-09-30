using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Model.Carton
{
  public  class OrderCartonModel
    { 
        public Nullable<int> OrderID { get; set; } 
        public List<OrderCartonDetailModel> OrderCartonDetailModel { get; set; }
    }
    public class OrderCartonDetailModel
    { 
        public Nullable<int> CartonID { get; set; }
        public Nullable<int> ProductSKUID { get; set; }
        public Nullable<int> Quantity { get; set; }
          
    }
}
