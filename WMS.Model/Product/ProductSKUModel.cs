using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Model.Product
{
    public class ProductSKUModel
    {
        public int TotalRows { get; set; }
        public int Id { get; set; }
        public int StockRangeId { get; set; }
        public string StockRange { get; set; }
        public string  CustomerCompanyName { get; set; }
        public string OriginalJob { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string SKU { get; set; }
        public byte SizeId { get; set; }
        public string ColorId { get; set; }
        public string Style { get; set; }
        public decimal Weight { get; set; }
        public string WeightUnit { get; set; }
        public int OrderNumner { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> WarningLevel { get; set; }
        public int UserId { get; set; }
        public int keyword { get; set; }
        public int Colorid { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOnUtc { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOnUtc { get; set; }
        public string ProductName { get; set; }
        public string ColorName { get; set; }
        public string Size { get; set; }
        public string ProductDescription { get; set; }
        public string HSCode { get; set; }
        public string FebricNo { get; set; }
        public string FebricDescription { get; set; }
        public int? ClrId { get; set; }
        public Boolean IsChecked { get; set; }
        public Nullable<int> ExpectedQuantity { get; set; }
        public string LanguageDescription { get; set; }

    }
    public class ProductSKUList
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string SKU { get; set; }
        public byte SizeId { get; set; }
        public Nullable<int> ColorId { get; set; }
        public string Style { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> ActualQuantity { get; set; }
        public Nullable<int> ExpectedQuantity { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
        public decimal Weight { get; set; }
        public string WeightUnit { get; set; }
    }

    public class ProductReceiveStockDetails
    {
        public int ProductSKUId { get; set; }  
        public string SKU { get; set; }
        public Nullable<int> Quantity { get; set; }
        public int ReceivingNoteId { get; set; }
        
    }

    public class ProductDetails
    {
        public int ProductSKUId { get; set; }
        public int SectionId { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public Nullable<int> Quantity { get; set; }
        public int ReceivingNoteId { get; set; }
        public int OrderId { get; set; }
        public int OrderDesignDetailId { get; set; }
    }
}
