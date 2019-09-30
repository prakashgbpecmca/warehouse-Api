using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Model.ApiResponse;
using WMS.Model.Carton;
using WMS.Model.Common;
using WMS.Model.Product;
using WMS.Model.Shelf;
using WMS.Model.Stock;
using WMS.Service.DataAccess;

namespace WMS.Service.Repository
{
    public class ShelfRespository
    {
        WMSEntities DbContext = new WMSEntities();

        // prakash
        public class GetLocationStockModel
        {
            public string ProductName { get; set; }
            public string ProductCode { get; set; }
            public string ProductSKU { get; set; }
            public string Color { get; set; }
            public string Size { get; set; }
            public int Quantity { get; set; }
        }
        public List<GetLocationStockModel> GetLocationStock(int sectionDetailId)
        {

            var data = (from r in DbContext.WarehouseAllocationDetails
                        join psku in DbContext.ProductSKUs on r.ProductSKUID equals psku.Id
                        join p in DbContext.ProductMasters on psku.ProductId equals p.Id
                        join c in DbContext.Colors on psku.ColorId equals c.ID
                        join s in DbContext.Sizes on psku.SizeId equals s.ID
                        where r.SectionDetailID == sectionDetailId
                        select new GetLocationStockModel
                        {
                            ProductName = p.ProductName,
                            ProductCode = p.ProductCode,
                            ProductSKU = psku.SKU,
                            Color = c.color1,
                            Size = s.size1,
                            Quantity = r.Quantity.HasValue ? r.Quantity.Value : 0
                        }
                         ).ToList();

            return data;
        }
        // 

        public ShelfMasterModel Initials()
        {
            ShelfMasterModel model = new ShelfMasterModel();
            model.WareHouses = GetWarehouseList();
            model.Lines = GetLineList();
            model.Shelfs = GetShelfList();
            model.Rows = GetRowList();
            model.Sections = GetSectionList();
            return model;
        }
         
        #region  Get Warehouse List
        public List<ShelfDetailModel> GetWarehouseList()
        {
            List<ShelfDetailModel> list = (from a in DbContext.Warehouses
                                           select new ShelfDetailModel
                                           {
                                               ID = a.WarehouseId,
                                               IsActive = a.IsActive,
                                               Name = a.Name
                                           }).Where(p => p.IsActive == true).ToList();
            return list;
        }

        #endregion

        #region  Get Line List
        public List<ShelfDetailModel> GetLineList()
        {
            List<ShelfDetailModel> list = (from a in DbContext.LineMasters
                                           select new ShelfDetailModel
                                           {
                                               ID = a.ID,
                                               Name = a.Name
                                           }).ToList();
            return list;
        }

        #endregion

        #region  Get shelf List
        public List<ShelfDetailModel> GetShelfList()
        {
            List<ShelfDetailModel> list = (from a in DbContext.ShelfMasters
                                           select new ShelfDetailModel
                                           {
                                               ID = a.ID,
                                               Name = a.Name
                                           }).ToList();
            return list;
        }

        public WebAPIResponse DispatchOrderStockList(string barcode)
        {
            try
            {
                var dbOrder = DbContext.Orders.Where(p => p.OrderNumber == barcode).FirstOrDefault();

                if (dbOrder != null)
                {
                    if (dbOrder.OrderStatusId == (byte)WMSOrderStatusEnum.Shipped)
                    {
                        return new WebAPIResponse { Message = "Dispatch process is done for this order." };
                    }
                }

                var data = (from a in DbContext.Orders
                            join b in DbContext.OrderPickupDetails on a.Id equals b.OrderID
                            join p in DbContext.ProductSKUs on b.ProductSKUID equals p.Id
                            join c in DbContext.Colors on p.ColorId equals c.ID
                            join s in DbContext.Sizes on p.SizeId equals s.ID
                            join pm in DbContext.ProductMasters on p.ProductId equals pm.Id
                            join sd in DbContext.SectionDetails on b.SectionDetailID equals sd.ID
                            join rr in DbContext.ShelfRowDetails on sd.ShelfRowID equals rr.ID
                            where a.OrderNumber == barcode
                            select new PickOrderModel
                            {
                                OrderNumber = a.OrderNumber,
                                OrderDesignDetailId = b.OrderDesignDetailId.HasValue ? b.OrderDesignDetailId.Value : 0,
                                OrderID = b.OrderID.HasValue ? b.OrderID.Value : 0,
                                SectionDetailID = b.SectionDetailID.HasValue ? b.SectionDetailID.Value : 0,
                                line = DbContext.LineMasters.Where(x => x.ID == rr.LineID).FirstOrDefault().Name,
                                rack = DbContext.ShelfMasters.Where(x => x.ID == rr.ShelfID).FirstOrDefault().Name,
                                row = DbContext.RowMasters.Where(x => x.ID == rr.RowID).FirstOrDefault().Name,
                                section = DbContext.SectionMasters.Where(x => x.ID == sd.SectionID).FirstOrDefault().Name,
                                sectionbarcode = sd.Barcode,
                                ProductSKUID = b.ProductSKUID.HasValue ? b.ProductSKUID.Value : 0,
                                ProductName = pm.ProductName,
                                ProductDescription = pm.ProductDescription,
                                ProductCode = pm.ProductCode,
                                SKU = p.SKU,
                                ColorId = p.ColorId.HasValue ? p.ColorId.Value : 0,
                                color1 = c.color1,
                                SizeId = p.SizeId,
                                size1 = s.size1,
                                RequiredQty = b.RequiredQty.HasValue ? b.RequiredQty.Value : 0,
                                PickedQty = b.PickedQty.HasValue ? b.PickedQty.Value : 0
                            }).ToList();
                if (data.Count > 0)
                {
                    return new WebAPIResponse { Message = "Success", Result = data };
                }
                else
                {
                    return new WebAPIResponse { Message = "No data" };
                }
            }
            catch (Exception ex)
            {
                return new WebAPIResponse { Message = ex.Message };
            }
        }

        public List<OrderCarton> GetOrderCartons(int? orderID)
        {
            if (orderID.HasValue)
            {
                return DbContext.OrderCartons.Where(p => p.OrderID == orderID.Value).ToList();
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region  Get Row List
        public List<ShelfDetailModel> GetRowList()
        {
            List<ShelfDetailModel> list = (from a in DbContext.RowMasters
                                           select new ShelfDetailModel
                                           {
                                               ID = a.ID,
                                               Name = a.Name
                                           }).ToList();
            return list;
        }

        #endregion

        #region  Get Section List
        public List<ShelfDetailModel> GetSectionList()
        {
            List<ShelfDetailModel> list = (from a in DbContext.SectionMasters
                                           select new ShelfDetailModel
                                           {
                                               ID = a.ID,
                                               Name = a.Name
                                           }).ToList();
            return list;
        }

        #endregion

        #region get Box data
        public List<boxData> getBoxDataCount(int wid, int lineID, int shelfid)
        {
            try
            {
                List<boxData> list = new List<boxData>();
                list.Add(new boxData
                {
                    id = 1,
                    name = "Total Line",
                    active = true,
                    number = (from a in DbContext.ShelfRowDetails
                              where (wid == 0 || a.WarehouseID == wid) && (lineID == 0 || a.LineID == lineID)
                              select new
                              {
                                  a.WarehouseID,
                                  a.LineID
                              }).Distinct().Count()
                });
                list.Add(new boxData
                {
                    id = 2,
                    name = "Total Rack",
                    active = true,
                    number = (from a in DbContext.ShelfRowDetails
                              where (wid == 0 || a.WarehouseID == wid) && (lineID == 0 || a.LineID == lineID) && (shelfid == 0 || a.ShelfID == shelfid)
                              select new
                              {
                                  a.WarehouseID,
                                  a.LineID,
                                  a.ShelfID
                              }).Distinct().Count()
                });
                list.Add(new boxData
                {
                    id = 3,
                    name = "Total Rows",
                    active = true,
                    number = (from a in DbContext.ShelfRowDetails
                              where (wid == 0 || a.WarehouseID == wid) && (lineID == 0 || a.LineID == lineID) && (shelfid == 0 || a.ShelfID == shelfid)
                              select new
                              {
                                  a.WarehouseID,
                                  a.LineID,
                                  a.ShelfID,
                                  a.RowID
                              }).Distinct().Count()
                });
                list.Add(new boxData
                {
                    id = 4,
                    name = "Total Sections",
                    active = true,
                    number =
                    (from a in DbContext.SectionDetails
                     join b in DbContext.ShelfRowDetails on a.ShelfRowID equals b.ID
                     where (wid == 0 || b.WarehouseID == wid) && (lineID == 0 || b.LineID == lineID) && (shelfid == 0 || b.ShelfID == shelfid)
                     select new
                     {
                         a.ID
                     }).Distinct().Count()
                });

                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region add shelf detail
        public string ManageShelfDetail(List<ShelfList> model)
        {
            try
            {
                foreach (var item in model)
                {
                    if (item.ID > 0)
                    {
                        var data = DbContext.ShelfRowDetails.Where(x => x.WarehouseID == item.warehouseId && x.LineID == item.lineId && x.ShelfID == item.shelfId && x.RowID == item.rowId).FirstOrDefault();
                        if (data == null)
                        {
                            data.WarehouseID = item.warehouseId;
                            data.LineID = item.lineId;
                            data.ShelfID = item.shelfId;
                            data.RowID = item.rowId;
                            DbContext.SaveChanges();
                        }
                        var secData = DbContext.SectionDetails.Where(x => x.ShelfRowID == data.ID).ToList();
                        if (secData.Count < item.section.Count)
                        {
                            for (int i = secData.Count; i < item.section.Count; i++)
                            {
                                string barcode = IsExistBarcode();
                                SectionDetail sec = new SectionDetail();
                                sec.ShelfRowID = data.ID;
                                sec.SectionID = item.section[i].id;
                                sec.Barcode = barcode;
                                DbContext.SectionDetails.Add(sec);
                                DbContext.SaveChanges();
                            }
                        }
                        else if (secData.Count > item.section.Count)
                        {
                            DbContext.SectionDetails.RemoveRange(secData.Where(x => x.SectionID > item.section.Count));
                            DbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        ShelfRowDetail obj = new ShelfRowDetail();
                        obj.WarehouseID = item.warehouseId;
                        obj.LineID = item.lineId;
                        obj.ShelfID = item.shelfId;
                        obj.RowID = item.rowId;
                        DbContext.ShelfRowDetails.Add(obj);
                        DbContext.SaveChanges();

                        foreach (var row in item.section)
                        {
                            string barcode = IsExistBarcode();
                            SectionDetail sec = new SectionDetail();
                            sec.ShelfRowID = obj.ID;
                            sec.SectionID = row.id;
                            sec.Barcode = barcode;
                            DbContext.SectionDetails.Add(sec);
                            DbContext.SaveChanges();
                        }
                    }
                }
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        #region get shelf detail
        public List<ShelfList> GetShelfDetail(int wID, int lineID, int shelfid)
        {
            try
            {
                var data = (from a in DbContext.ShelfRowDetails
                            join l in DbContext.LineMasters on a.LineID equals l.ID
                            join b in DbContext.ShelfMasters on a.ShelfID equals b.ID
                            join c in DbContext.RowMasters on a.RowID equals c.ID
                            where (wID == 0 || a.WarehouseID == wID) && (lineID == 0 || a.LineID == lineID) && (shelfid == 0 || a.ShelfID == shelfid)
                            select new ShelfList
                            {
                                ID = a.ID,
                                warehouseId = a.WarehouseID,
                                lineId = a.LineID,
                                shelfId = a.ShelfID,
                                rowId = a.RowID,
                                shelfrowname = l.Name + " " + b.Name + " " + c.Name,
                                section = (from p in DbContext.SectionDetails
                                           join q in DbContext.SectionMasters on p.SectionID equals q.ID
                                           where p.ShelfRowID == a.ID
                                           select new SectionList
                                           {
                                               id = p.ID,
                                               name = l.Name + " " + b.Name + " " + c.Name + " " + q.Name
                                           }).ToList()
                            });
                if (wID > 0 && wID != null)
                {
                    data = data.Where(x => x.warehouseId == wID);
                }

                return data.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Is shelf detail exist
        public bool IsExistShelfData(int wid, int lineid, int shelfid, int rowid)
        {
            try
            {
                var data = DbContext.ShelfRowDetails.Where(x => x.WarehouseID == wid && x.LineID == lineid && x.ShelfID == shelfid && x.RowID == rowid).FirstOrDefault();
                if (data != null)
                { return false; }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region is barcode exist
        public string IsExistBarcode()
        {
            Random rnd = new Random();
            string barcode = rnd.Next(100000000, 999999999).ToString();
            var data = DbContext.SectionDetails.Where(x => x.Barcode == barcode).FirstOrDefault();
            if (data != null)
            {
                return IsExistBarcode();
            }
            else { return barcode; }
        }
        #endregion

        #region get shelf detail by id
        public ShelfList GetShelfDetailById(int ID)
        {
            try
            {
                var data = (from a in DbContext.ShelfRowDetails
                            join l in DbContext.LineMasters on a.LineID equals l.ID
                            join b in DbContext.ShelfMasters on a.ShelfID equals b.ID
                            join c in DbContext.RowMasters on a.RowID equals c.ID
                            where a.ID == ID
                            select new ShelfList
                            {
                                ID = a.ID,
                                warehouseId = a.WarehouseID,
                                lineId = a.LineID,
                                shelfId = a.ShelfID,
                                rowId = a.RowID,
                                shelfrowname = l.Name + " " + b.Name + " " + c.Name,
                                section = (from p in DbContext.SectionDetails
                                           join q in DbContext.SectionMasters on p.SectionID equals q.ID
                                           where p.ShelfRowID == a.ID
                                           select new SectionList
                                           {
                                               id = p.ID,
                                               name = l.Name + " " + b.Name + " " + c.Name + " " + q.Name
                                           }).ToList(),
                                ResSectionID = (from p in DbContext.SectionDetails
                                                join wa in DbContext.WarehouseAllocationDetails on p.ID equals wa.SectionDetailID
                                                where p.ShelfRowID == a.ID && wa.ID != null
                                                select new
                                                {
                                                    wa.ID
                                                }).Count()
                            }).FirstOrDefault();
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region 
        public WebAPIResponse IsShelfOrderWise(ShelfModel model)
        {
            try
            {
                string msg = ""; bool id = false;
                if (model.lineId > 0 && model.shelfId > 0 && model.rowId > 0)
                {
                    msg = "Row";
                    var data = (from a in DbContext.ShelfRowDetails
                                where a.WarehouseID == model.warehouseId && a.LineID == model.lineId && a.ShelfID == model.shelfId
                                select new
                                {
                                    a.RowID
                                }).ToList();
                    if (data.Count > 0)
                    {
                        int maxid = data.Max(y => y.RowID);
                        var res = DbContext.RowMasters.Where(x => x.ID > maxid).Take(1).FirstOrDefault();
                        if (res != null)
                        {
                            id = res.ID == model.rowId ? true : false;
                        }
                    }
                }
                else if (model.lineId > 0 && model.shelfId > 0)
                {
                    msg = "Rack";
                    var data = (from a in DbContext.ShelfRowDetails
                                where a.WarehouseID == model.warehouseId && a.LineID == model.lineId
                                select new
                                {
                                    a.ShelfID
                                }).ToList();
                    if (data.Count > 0)
                    {
                        int maxid = data.Max(y => y.ShelfID);
                        var res = DbContext.ShelfMasters.Where(x => x.ID > maxid).Take(1).FirstOrDefault();
                        if (res != null) { id = res.ID == model.shelfId ? true : false; }
                    }
                }
                else if (model.lineId > 0)
                {
                    msg = "Line";
                    var data = (from a in DbContext.ShelfRowDetails
                                where a.WarehouseID == model.warehouseId
                                select new
                                {
                                    a.LineID
                                }).ToList();
                    if (data.Count > 0)
                    {
                        int maxid = data.Max(y => y.LineID);
                        var res = DbContext.LineMasters.Where(x => x.ID > maxid).Take(1).FirstOrDefault();
                        if (res != null) { id = res.ID == model.lineId ? true : false; }
                    }
                }
                return new WebAPIResponse { Message = msg, Result = id };
            }
            catch (Exception ex)
            {
                return new WebAPIResponse { Message = ex.Message };
            }
        }
        #endregion

        //For Mobile app

        #region For section scan
        public WebAPIResponse ScanSection(string barcode)
        {
            try
            {
                var data = (from sd in DbContext.SectionDetails
                            join rr in DbContext.ShelfRowDetails on sd.ShelfRowID equals rr.ID
                            join l in DbContext.LineMasters on rr.LineID equals l.ID
                            join b in DbContext.ShelfMasters on rr.ShelfID equals b.ID
                            join r in DbContext.RowMasters on rr.RowID equals r.ID
                            join s in DbContext.SectionMasters on sd.SectionID equals s.ID
                            where sd.Barcode == barcode
                            select new ShelfBarcodeModel
                            {
                                SectionId = sd.ID,
                                ShelfBarcode = sd.Barcode,
                                ShelfBarcodeDisplay = l.Name + " " + b.Name + " " + r.Name + " " + s.Name
                            }).FirstOrDefault();

                if (data != null)
                {
                    return new WebAPIResponse { Message = "Success", Result = data };
                }
                else { return new WebAPIResponse { Message = "No section found!" }; }
            }
            catch (Exception ex)
            {
                return new WebAPIResponse { Message = ex.Message };
            }
        }
        #endregion

        #region Get product by sku
        public WebAPIResponse GetProductBySKU(string sku)
        {
            try
            {
                var data = (from a in DbContext.ProductSKUs
                            join c in DbContext.Colors on a.ColorId equals c.ID
                            join s in DbContext.Sizes on a.SizeId equals s.ID
                            join pm in DbContext.ProductMasters on a.ProductId equals pm.Id
                            where a.SKU == sku
                            select new ProductSKUList
                            {
                                Id = a.Id,
                                ProductId = a.ProductId,
                                ProductName = pm.ProductName,
                                ProductCode = pm.ProductCode,
                                ColorId = a.ColorId,
                                ColorName = c.color1,
                                SizeName = s.size1,
                                SizeId = a.SizeId,
                                SKU = a.SKU
                            }).FirstOrDefault();

                if (data != null)
                {
                    return new WebAPIResponse { Message = "Success", Result = data };
                }
                else
                {
                    return new WebAPIResponse { Message = "No product found" };
                }
            }
            catch (Exception ex)
            {
                return new WebAPIResponse { Message = ex.Message };
            }
        }
        #endregion

        #region  Warehouse allocation
        public WebAPIResponse AllocateStocktoSection(allocateSectionStockModel model)
        {
            try
            {
                string msg = "";
                if (model.products.Count > 0)
                {
                    foreach (var item in model.products)
                    {
                        var data = DbContext.WarehouseAllocationDetails.Where(x => x.SectionDetailID == model.sectionid && x.ProductSKUID == item.productSKUID).FirstOrDefault();
                        if (data != null)
                        {
                            data.Quantity = data.Quantity + item.Qty;
                            DbContext.SaveChanges();
                            msg = "Success";
                        }
                        else
                        {
                            WarehouseAllocationDetail obj = new WarehouseAllocationDetail();
                            obj.SectionDetailID = model.sectionid;
                            obj.ProductSKUID = item.productSKUID;
                            obj.Quantity = item.Qty;
                            DbContext.WarehouseAllocationDetails.Add(obj);
                            DbContext.SaveChanges();
                            msg = "Success";
                        }
                    }
                }
                else { msg = "No data sent!"; }
                return new WebAPIResponse { Message = msg, Result = GetProductSKUBySection(model.sectionid) };
            }
            catch (Exception ex)
            {
                return new WebAPIResponse { Message = ex.Message };
            }
        }

        #endregion

        //#region Warehouse allocation
        //public WebAPIResponse AllocateStocktoSection(int sectionid, string productSKU)
        //{
        //    try
        //    {
        //        string msg = "";
        //        var product = DbContext.ProductSKUs.Where(x => x.SKU == productSKU).FirstOrDefault();
        //        if (product == null)
        //        {
        //            msg = "Product not found!";
        //        }
        //        else
        //        {
        //            var data = DbContext.WarehouseAllocationDetails.Where(x => x.SectionDetailID == sectionid && x.ProductSKUID == product.Id).FirstOrDefault();
        //            if (data != null)
        //            {
        //                data.Quantity = data.Quantity + 1;
        //                DbContext.SaveChanges();
        //                msg = "Success";
        //            }
        //            else
        //            {
        //                WarehouseAllocationDetail obj = new WarehouseAllocationDetail();
        //                obj.SectionDetailID = sectionid;
        //                obj.ProductSKUID = product.Id;
        //                obj.Quantity = 1;
        //                DbContext.WarehouseAllocationDetails.Add(obj);
        //                DbContext.SaveChanges();
        //                msg = "Success";
        //            }
        //        }
        //        var result = GetProductSKUBySection(sectionid);
        //        return new WebAPIResponse { Message = msg, Result = result };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new WebAPIResponse { Message = ex.Message };
        //    }
        //}
        //#endregion

        #region Get productSKU list by section
        public List<ProductSKUList> GetProductSKUBySection(int sectionid)
        {
            try
            {
                var data = (from w in DbContext.WarehouseAllocationDetails
                            join a in DbContext.ProductSKUs on w.ProductSKUID equals a.Id
                            join c in DbContext.Colors on a.ColorId equals c.ID
                            join s in DbContext.Sizes on a.SizeId equals s.ID
                            join pm in DbContext.ProductMasters on a.ProductId equals pm.Id
                            where w.SectionDetailID == sectionid
                            select new ProductSKUList
                            {
                                Id = a.Id,
                                ProductId = a.ProductId,
                                ProductName = pm.ProductName,
                                ProductCode = pm.ProductCode,
                                ColorId = a.ColorId,
                                ColorName = c.color1,
                                SizeName = s.size1,
                                SizeId = a.SizeId,
                                SKU = a.SKU,
                                Quantity = w.Quantity
                            }).ToList();

                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region pickup Order  stock
        public WebAPIResponse PickupOrderStockList(string barcode)
        {
            try
            {
                var data = (from a in DbContext.Orders
                            join b in DbContext.OrderPickupDetails on a.Id equals b.OrderID
                            join p in DbContext.ProductSKUs on b.ProductSKUID equals p.Id
                            join c in DbContext.Colors on p.ColorId equals c.ID
                            join s in DbContext.Sizes on p.SizeId equals s.ID
                            join pm in DbContext.ProductMasters on p.ProductId equals pm.Id
                            join sd in DbContext.SectionDetails on b.SectionDetailID equals sd.ID
                            join rr in DbContext.ShelfRowDetails on sd.ShelfRowID equals rr.ID
                            where a.OrderNumber == barcode
                            select new PickOrderModel
                            {
                                OrderNumber = a.OrderNumber,
                                OrderDesignDetailId = b.OrderDesignDetailId.HasValue ? b.OrderDesignDetailId.Value : 0,
                                OrderID = b.OrderID.HasValue ? b.OrderID.Value : 0,
                                SectionDetailID = b.SectionDetailID.HasValue ? b.SectionDetailID.Value : 0,
                                line = DbContext.LineMasters.Where(x => x.ID == rr.LineID).FirstOrDefault().Name,
                                rack = DbContext.ShelfMasters.Where(x => x.ID == rr.ShelfID).FirstOrDefault().Name,
                                row = DbContext.RowMasters.Where(x => x.ID == rr.RowID).FirstOrDefault().Name,
                                section = DbContext.SectionMasters.Where(x => x.ID == sd.SectionID).FirstOrDefault().Name,
                                sectionbarcode = sd.Barcode,
                                ProductSKUID = b.ProductSKUID.HasValue ? b.ProductSKUID.Value : 0,
                                ProductName = pm.ProductName,
                                ProductDescription = pm.ProductDescription,
                                ProductCode = pm.ProductCode,
                                SKU = p.SKU,
                                ColorId = p.ColorId.HasValue ? p.ColorId.Value : 0,
                                color1 = c.color1,
                                SizeId = p.SizeId,
                                size1 = s.size1,
                                RequiredQty = b.RequiredQty.HasValue ? b.RequiredQty.Value : 0,
                                PickedQty = b.PickedQty.HasValue ? b.PickedQty.Value : 0
                            }).ToList();

                if (data.Where(p => p.PickedQty > 0).ToList().Count > 0)
                {
                    return new WebAPIResponse { Message = "Pick up process is already done." };
                }
                if (data.Count > 0)
                {
                    return new WebAPIResponse { Message = "Success", Result = data };
                }
                else
                {
                    return new WebAPIResponse { Message = "No data" };
                }
            }
            catch (Exception ex)
            {
                return new WebAPIResponse { Message = ex.Message };
            }

        }
        #endregion
        #region scan location
        public WebAPIResponse ScanLocation(string barcode)
        {
            try
            {
                var data = (from a in DbContext.SectionDetails
                            join b in DbContext.ShelfRowDetails on a.ShelfRowID equals b.ID
                            where a.Barcode == barcode
                            select new
                            {
                                line = DbContext.LineMasters.Where(x => x.ID == b.LineID).FirstOrDefault().Name,
                                rack = DbContext.ShelfMasters.Where(x => x.ID == b.ShelfID).FirstOrDefault().Name,
                                row = DbContext.RowMasters.Where(x => x.ID == b.RowID).FirstOrDefault().Name,
                                section = a.SectionID,
                                sectionbarcode = a.Barcode
                            }).FirstOrDefault();
                if (data != null)
                {
                    return new WebAPIResponse { Message = "Success", Result = data };
                }
                else { return new WebAPIResponse { Message = "No data" }; }
            }
            catch (Exception ex)
            {
                return new WebAPIResponse { Message = ex.Message };
            }

        }
        #endregion
        public WebAPIResponse ManageCartonLabel(OrderCartonModel details)
        {
            try
            {
                if (details.OrderCartonDetailModel.Count > 0)
                {
                    var data = DbContext.Orders.Where(x => x.Id == details.OrderID).FirstOrDefault();
                    if (data != null)
                    {
                        string cartonno = IsExistCartonNo(details.OrderID.HasValue ? details.OrderID.Value : 0);
                        OrderCarton obj = new OrderCarton
                        {
                            OrderID = details.OrderID,
                            CartonNo = cartonno,
                            CartonDisplayName = cartonno,
                            IsClosed = true,
                        };
                        DbContext.OrderCartons.Add(obj);
                        DbContext.SaveChanges();
                        foreach (var item in details.OrderCartonDetailModel)
                        {
                            OrderCartonDetail detail = new OrderCartonDetail
                            {
                                CartonID = obj.ID,
                                ProductSKUID = item.ProductSKUID,
                                Quantity = item.Quantity
                            };
                            DbContext.OrderCartonDetails.Add(detail);
                            DbContext.SaveChanges();
                        }
                        return new WebAPIResponse { Message = "Success" };
                    }
                    else
                    {
                        return new WebAPIResponse { Message = "Order does not exist" };
                    }
                }
                else
                {
                    return new WebAPIResponse { Message = "No records in obj" };
                }
            }
            catch (Exception ex)
            {
                return new WebAPIResponse { Message = ex.Message };
            }
        }


        #region get carton's product
        public List<ProductSKUList> GetCartonProducts(int cartonid)
        {
            try
            {
                var data = (from o in DbContext.OrderCartonDetails
                            join a in DbContext.ProductSKUs on o.ProductSKUID equals a.Id
                            join c in DbContext.Colors on a.ColorId equals c.ID
                            join s in DbContext.Sizes on a.SizeId equals s.ID
                            join pm in DbContext.ProductMasters on a.ProductId equals pm.Id
                            where o.CartonID == cartonid
                            select new ProductSKUList
                            {
                                Id = o.ID,
                                ProductId = a.ProductId,
                                ProductName = pm.ProductName,
                                ProductCode = pm.ProductCode,
                                ColorId = a.ColorId,
                                ColorName = c.color1,
                                SizeName = s.size1,
                                SizeId = a.SizeId,
                                SKU = a.SKU,
                                Quantity = o.Quantity
                            }).ToList();
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region is carton no exist
        public string IsExistCartonNo(int orderId)
        {
            if (orderId > 0)
            {
                var cartons = DbContext.OrderCartons.Where(p => p.OrderID == orderId).ToList();
                if (cartons.Count > 0)
                {
                    int count = 1;
                    foreach (var item in cartons)
                    {
                        count++;
                    }
                    return count.ToString();
                }
                else
                {
                    return "1";
                }
            }
            else
            {
                return "";
            }
        }
        #endregion


        #region get shelf barcode detail
        public List<ShelfBarcodeRoportFinalModel> GetShelfBarCodeDetail(int wID, int lineID, int shelfid)
        {
            List<ShelfBarcodeReportModel> SBRMList = new List<ShelfBarcodeReportModel>();
            List<ShelfBarcodeRoportFinalModel> FnlObjList = new List<ShelfBarcodeRoportFinalModel>();
            ShelfBarcodeRoportFinalModel FnlObj = new ShelfBarcodeRoportFinalModel();
            try
            {
                if (wID > 0 && lineID > 0 && shelfid > 0)
                {
                    var data = (from w in DbContext.SectionDetails
                                join a in DbContext.ShelfRowDetails on w.ShelfRowID equals a.ID
                                join l in DbContext.LineMasters on a.LineID equals l.ID
                                join b in DbContext.ShelfMasters on a.ShelfID equals b.ID
                                join c in DbContext.RowMasters on a.RowID equals c.ID
                                join q in DbContext.SectionMasters on w.SectionID equals q.ID
                                where (wID == 0 || a.WarehouseID == wID) && (lineID == 0 || a.LineID == lineID) && (shelfid == 0 || a.ShelfID == shelfid)
                                select new ShelfBarcodeModel
                                {
                                    ShelfBarcode = w.Barcode,
                                    ShelfBarcodeDisplay = l.Name + " " + b.Name + " " + c.Name + " " + q.Name
                                }).Distinct().OrderBy(a => a.ShelfBarcodeDisplay).ToList();


                    ShelfBarcodeReportModel SBRM;

                    if (data != null && data.Count > 0)
                    {
                        foreach (var r in data)
                        {
                            SBRM = new ShelfBarcodeReportModel();
                            SBRM.ShelfBarcode1 = r.ShelfBarcode;
                            SBRM.ShelfBarcodeDisplay1 = r.ShelfBarcodeDisplay;
                            SBRMList.Add(SBRM);
                        }
                    }
                }

                FnlObj.ShelfBarcodeFinalObj = new List<ShelfBarcodeReportModel>();
                FnlObj.ShelfBarcodeFinalObj.AddRange(SBRMList);
                FnlObjList.Add(FnlObj);
                return FnlObjList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion


    }
}
