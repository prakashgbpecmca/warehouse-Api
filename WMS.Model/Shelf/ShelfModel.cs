using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Model.Shelf
{
    public class ShelfModel
    {
        public int warehouseId { get; set; }
        public int lineId { get; set; }
        public int shelfId { get; set; }
        public int rowId { get; set; }
        public int sectionId { get; set; }
    }
    public class ShelfMasterModel
    {
        public List<ShelfDetailModel> WareHouses { get; set; }
        public List<ShelfDetailModel> Lines { get; set; }
        public List<ShelfDetailModel> Shelfs { get; set; }
        public List<ShelfDetailModel> Rows { get; set; }
        public List<ShelfDetailModel> Sections { get; set; }
    }
    public class ShelfDetailModel
    {
        public bool IsActive { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }
    public class ShelfList
    {
        public int ID { get; set; }
        public int warehouseId { get; set; }
        public int lineId { get; set; }
        public int shelfId { get; set; }
        public int rowId { get; set; }
        public int sectionId { get; set; }
        public string shelfrowname { get; set; }
        public List<SectionList> section { get; set; }
        public int ResSectionID { get; set; }
    }
    public class SectionList
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class boxData
    {
        public int id { get; set; }
        public string name { get; set; }
        public int number { get; set; }
        public bool active { get; set; }
    }

    public class allocateStockModel
    {
        public int sectionid { get; set; }
        public string productSKU { get; set; }
        public int productSKUID { get; set; }

    }
    public class CartonModel
    {
        public int CartonID { get; set; }
        public string ProductSKU { get; set; }
        public int OrderID { get; set; }

    }
    public class allocateSectionStockModel
    {
        public int sectionid { get; set; }
        public List<StockProductList> products { get; set; }

    }
    public class StockProductList
    {
        public int productSKUID { get; set; }
        public int Qty { get; set; }
    }
}
