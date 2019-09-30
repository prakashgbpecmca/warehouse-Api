//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace WMS.Model.Rack
//{
//    public class WareHouseModel
//    {
//        public int WareHouseId { get; set; }
//        public string WareHouseName { get; set; }
//        public string WareHouseCode { get; set; }
//        public string Description { get; set; }
//    }
//    public class BlockModel
//    {
//        public int BlockId { get; set; }
//        public string BlockName { get; set; }
//        public string Description { get; set; }
//        public int WareHouseId { get; set; }
//        public string BlockCode { get; set; }

//    }

//    public class RackModel
//    {
//        public int RackId { get; set; }
//        public string RackName { get; set; }
//        public string Description { get; set; }
//        public int BlockId { get; set; }
//        public string RackCode { get; set; }
//    }
//    public class RowModel
//    {
//        public int RackId { get; set; }
//        public int RowId { get; set; }
//        public string RowName { get; set; }
//        public string Description { get; set; }
//        public string RowCode { get; set; }
//    }



//    public class SectionModel
//    {
//        public int SectionId { get; set; }
//        public int RowId { get; set; }
//        public string SectionName { get; set; }
//        public string Description { get; set; }
//        public string SectionCode { get; set; }
//        public string SectionBarcode { get; set; }

//    }

//    public class WMSRelationsModel
//    {

//        public int ID { get; set; }
//        public int? WareHouseId { get; set; }
//        public int? BlockId { get; set; }
//        public int? RackId { get; set; }
//        public int? RowId { get; set; }
//        public int? SectionId { get; set; }
//        public string WareHouseName { get; set; }
//        public string BlockName { get; set; }
//        public string RackName { get; set; }
//        public string RowName { get; set; }
//        public string SectionName { get; set; }
//        public string SectionBarcode { get; set; }

//    }

//    public class RackMasterModel
//    {
//        public List<BlockModel> Blocks { get; set; }
//        public List<RackModel> Racks { get; set; }

//        public List<WareHouseModel> WareHouses { get; set; }
//        public List<RowModel> Rows { get; set; }
//        public List<SectionModel> Sections { get; set; }
//    }

//    public class DetailModel
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }
//        public string Description { get; set; }
//        public string Code { get; set; }
//        public string Barcode { get; set; }
//        public int ParentId { get; set; }
//    }
  
//}

