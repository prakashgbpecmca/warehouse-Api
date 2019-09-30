using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WMS.Model.Product
{
    public class ProductTrackAndTrace
    {
        public int UserId { get; set; }
        public int CurrentPage { get; set; }
        public int TakeRows { get; set; }
        public int ProductId { get; set; }
        public int ProductColor { get; set; }
        public int ProductSize { get; set; }
    }

    public class ProductModel
    {
        public int Id { get; set; }
        public string WeightUnit { get; set; }
        public int CategoryId { get; set; }
        public string UserId { get; set; }
        public string Category { get; set; }
        public string StockRange { get; set; }
        public string StockRangeID { get; set; }
        public string Weight { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string SKUCode { get; set; }
        public byte? Color { get; set; }
        public string ColorID { get; set; }

        public string ColorName { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string SizeID { get; set; }
        public string Quantity { get; set; }
        public string ProductDescription { get; set; }
        public byte? Unit { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOnUtc { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOnUtc { get; set; }
    }

    public class ColorModel
    {
        public byte ID { get; set; }
        public string color { get; set; }
        public string description { get; set; }
        public Nullable<bool> isActive { get; set; }

    }
    public class SizeModel
    {
        public byte ID { get; set; }
        public string size { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public Nullable<bool> isActive { get; set; }
    }
    public class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
    public partial class StockRangeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive
        {
            get; set;
        }

        public class ProductMasterModel
        {
            public List<CategoryModel> Category { get; set; }
            public List<ColorModel> Color { get; set; }
            public List<SizeModel> Size { get; set; }
            public List<ProductSKUModel> Product { get; set; }
            public List<StockRangeModel> StockRange { get; set; }

            public List<ProductDetailsModel> ProductMaster { get; set; }
        }

        public class ProductDetailsModel
        {
            public int Id { get; set; }
            public int StockRangeId { get; set; }
            public string ProductName { get; set; }
            public string ProductCode { get; set; }
        }
        public class ProductMasterM
        {
            public int Id { get; set; }
            public string ProductName { get; set; }
            public string ProductCode { get; set; }
            public string ProductDescription { get; set; }
        }

       
    }
}
