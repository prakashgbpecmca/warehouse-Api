using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Model.Stock
{
    public class UserStockModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductSKUId { get; set; } 
        public System.DateTime UpdatedOnUtc { get; set; }
        public int UpdatedBy { get; set; }
        //

        public string UserName { get; set; }
        //
        public int ProductId { get; set; }
        public string SKU { get; set; }
        public byte SizeId { get; set; }
        public Nullable<int> ColorId { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> ExpectedQuantity { get; set; }
        public decimal Weight { get; set; }
        public string WeightUnit { get; set; }
        //
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        //

        public string SizeName { get; set; }
        public string ColorName { get; set; }

        public string CompanyName { get; set; }
    }
    public class productStock
    {
        public int productID { get; set; }
        public int LoginUserID { get; set; }
        public int customerID { get; set; }

        public List<ProductStockList> productStockList { get; set; }
    }
    public class ProductStockList
    {
        public int LoginUserID { get; set; }
        public int productSKUID { get; set; }
        public int ExpectedQuantity { get; set; }

    }
    public class UserStockList
    {
        public int UserId { get; set; }
        public int[] productSKUId { get; set; }
        public int LoginUserId { get; set; }

    }

}
