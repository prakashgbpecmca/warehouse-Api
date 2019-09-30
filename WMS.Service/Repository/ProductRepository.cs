using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Model.ApiResponse;
using WMS.Model.Carton;
using WMS.Model.Product;
using WMS.Service.DataAccess;
using WMS.Service.GenericRepository;
using static WMS.Model.Product.StockRangeModel;

namespace WMS.Service.Repository
{
    public class ProductRepository
    {
        private readonly WMSEntities DbContext = new WMSEntities();

        public string SaveProductDetail(ProductSKUModel productDetail)
        {
            try
            {
                ProductSKU tbProductSKU = new ProductSKU();


                if (productDetail.Id <= 0)
                {
                    // tbProductSKU.StockRangeId = Convert.ToByte(productDetail.StockRangeId);
                    tbProductSKU.ProductId = productDetail.ProductId;
                    tbProductSKU.ColorId = Convert.ToInt32(productDetail.ColorId);
                    tbProductSKU.SizeId = Convert.ToByte(productDetail.SizeId);
                    tbProductSKU.SKU = productDetail.SKU;
                    tbProductSKU.Weight = Convert.ToDecimal(productDetail.Weight);
                    tbProductSKU.WeightUnit = productDetail.WeightUnit;
                    tbProductSKU.ActualQuantity = Convert.ToInt32(productDetail.Quantity);
                    tbProductSKU.Style = productDetail.Style;
                    tbProductSKU.WarningLevel = productDetail.WarningLevel;
                    tbProductSKU.OriginalJob = productDetail.OriginalJob;
                    tbProductSKU.CreatedBy = Convert.ToInt32(productDetail.UserId);
                    tbProductSKU.CreatedOnUtc = DateTime.UtcNow;
                    tbProductSKU.FebricNo = productDetail.FebricNo;
                    tbProductSKU.FebricDescription = productDetail.FebricDescription;
                    tbProductSKU.ProductDescription = productDetail.ProductDescription;
                    tbProductSKU.HSCode = productDetail.HSCode;
                    tbProductSKU.LanguageDescription = productDetail.LanguageDescription;
                    DbContext.ProductSKUs.Add(tbProductSKU);
                    DbContext.SaveChanges();
                }
                else
                {
                    var data = DbContext.ProductSKUs.Where(x => x.Id == productDetail.Id).FirstOrDefault();
                    // data.StockRangeId = Convert.ToByte(productDetail.StockRangeId);
                    data.ProductId = productDetail.ProductId;
                    data.ColorId = Convert.ToInt32(productDetail.ColorId);
                    data.SizeId = Convert.ToByte(productDetail.SizeId);
                    data.SKU = productDetail.SKU;
                    data.Weight = Convert.ToDecimal(productDetail.Weight);
                    data.WeightUnit = productDetail.WeightUnit.ToString();
                    data.Style = productDetail.Style;
                    data.ActualQuantity = Convert.ToInt32(productDetail.Quantity);
                    data.WarningLevel = productDetail.WarningLevel;
                    data.OriginalJob = productDetail.OriginalJob;
                    data.FebricNo = productDetail.FebricNo;
                    data.FebricDescription = productDetail.FebricDescription;
                    data.ProductDescription = productDetail.ProductDescription;
                    data.HSCode = productDetail.HSCode;
                    data.UpdatedBy = Convert.ToInt32(productDetail.UserId);
                    data.UpdatedOnUtc = DateTime.UtcNow;
                    data.LanguageDescription = productDetail.LanguageDescription;
                    DbContext.SaveChanges();
                }
                return "success";

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public List<ProductSKUModel> GetProductList(int id, string productColor, byte productSize, int pageIndex, int pageSize)
        {
            try
            {
                var list = (from a in DbContext.ProductSKUs
                            join b in DbContext.Colors on a.ColorId equals b.ID into temp
                            from b in temp.DefaultIfEmpty()
                            join s in DbContext.Sizes on a.SizeId equals s.ID into TemSize
                            from s in TemSize.DefaultIfEmpty()
                            join pm in DbContext.ProductMasters on a.ProductId equals pm.Id
                            //   join sr in DbContext.StockRanges on pm.StockRangeId equals sr.Id into tempstock
                            //   from sr in tempstock.DefaultIfEmpty()
                            join pc in DbContext.ProductCatagories on pm.ProductCatagoryId equals pc.Id into tempcat
                            from pc in tempcat.DefaultIfEmpty()
                            join u in DbContext.WMSUsers on pc.CustomerId equals u.Id into temppcust
                            from u in temppcust.DefaultIfEmpty()
                            select new ProductSKUModel
                            {
                                Id = a.Id,
                                // StockRangeId = a.StockRangeId,

                                StockRange = pc.CatagoryDisplay,
                                CustomerCompanyName = u.CompanyName,
                                //  StockRange = sr.Name,
                                // CustomerCompanyName = u.CompanyName,
                                ProductId = a.ProductId,
                                ProductName = pm.ProductName,
                                ProductCode = pm.ProductCode,
                                StockRangeId = pm.StockRangeId,
                                ColorId = a.ColorId.ToString(),
                                ColorName = b.color1,
                                Size = s.size1,
                                SizeId = a.SizeId,
                                SKU = a.SKU,
                                Weight = a.Weight,
                                WeightUnit = a.WeightUnit,
                                Quantity = a.ActualQuantity,
                                WarningLevel = a.WarningLevel,
                                // ProductDescription = pm.ProductDescription,
                                Style = a.Style,
                                OriginalJob = a.OriginalJob,
                                FebricNo = a.FebricNo,
                                FebricDescription = a.FebricDescription,
                                ProductDescription = a.ProductDescription,
                                LanguageDescription = a.LanguageDescription,
                                HSCode = a.HSCode
                            });



                //if (!canPage) // do what you wish if you can page no further
                //    return;
                //if (keyword != "" && keyword != null)
                //{
                //    list = list.Where(x => x.ProductName.ToLower().Contains(keyword.ToLower())
                //    //|| x.ProductName.ToLower().Contains(keyword.ToLower())
                //    || x.ColorName.ToLower().Contains(keyword.ToLower())
                //    || x.Size.ToLower().Contains(keyword.ToLower())
                //    // ||x.Quantity.Contains(keyword.ToLower())
                //    || x.SKU.ToLower().Contains(keyword.ToLower())
                //    // || x.Weight.Contains(keyword.ToLower())
                //    || x.ProductCode.ToLower().Contains(keyword.ToLower())
                //    || x.ProductDescription.ToLower().Contains(keyword.ToLower())
                //    );
                //}
                if (id > 0)
                {
                    list = list.Where(x => x.ProductId == id);
                }
                if (productColor != "0" && productColor != null)
                {
                    list = list.Where(x => x.ColorId.ToLower().Contains(productColor.ToLower()));
                }
                if (productSize != 0)
                {
                    list = list.Where(x => x.SizeId == productSize);
                }

                var total = list.OrderByDescending(x => x.Id).Count();
                var skip = pageSize * (pageIndex - 1);

                var canPage = skip < total;

                var list2 = list.OrderByDescending(x => x.Id).Skip(skip).Take(pageSize).ToArray().ToList();

                foreach (var item in list2)
                {
                    item.TotalRows = total;
                }
                return list2;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<ProductSKUModel> GetStockProductList(ProductTrackAndTrace track)
        {
            try
            {
                var list = (from a in DbContext.ProductSKUs
                            join b in DbContext.Colors on a.ColorId equals b.ID into temp
                            from b in temp.DefaultIfEmpty()
                            join s in DbContext.Sizes on a.SizeId equals s.ID into TemSize
                            from s in TemSize.DefaultIfEmpty()
                            join pm in DbContext.ProductMasters on a.ProductId equals pm.Id
                            select new ProductSKUModel
                            {
                                TotalRows = (from r in DbContext.ProductSKUs
                                             join f in DbContext.Colors on r.ColorId equals f.ID into temp
                                             from f in temp.DefaultIfEmpty()
                                             join g in DbContext.Sizes on r.SizeId equals g.ID into TemSize
                                             from g in TemSize.DefaultIfEmpty()
                                             where (track.ProductId == 0 || r.ProductId == track.ProductId) &&
                                                    (track.ProductSize == 0 || g.ID == track.ProductSize) &&
                                                      (track.ProductColor == 0 || f.ID == track.ProductColor)
                                             select r).Count(),
                                Id = a.Id,
                                ProductId = a.ProductId,
                                ProductName = pm.ProductName,
                                ProductCode = pm.ProductCode,
                                StockRangeId = pm.StockRangeId,
                                ColorId = a.ColorId.ToString(),
                                ColorName = b.color1,
                                Size = s.size1,
                                SizeId = a.SizeId,
                                SKU = a.SKU,
                                Weight = a.Weight,
                                WeightUnit = a.WeightUnit,
                                Quantity = a.ActualQuantity,
                                WarningLevel = a.WarningLevel,
                                Style = a.Style,
                                OriginalJob = a.OriginalJob,
                                FebricNo = a.FebricNo,
                                FebricDescription = a.FebricDescription,
                                ProductDescription = a.ProductDescription,
                                HSCode = a.HSCode

                            }).Where(p => p.ProductCode != "DSS1234" && p.ProductCode != "DSS1235");


                if (track.ProductId > 0)
                {
                    list = list.Where(x => x.ProductId == track.ProductId);
                }
                if (track.ProductColor != 0)
                {
                    list = list.Where(x => x.ColorId.ToLower().Contains(track.ProductColor.ToString().ToLower()));
                }
                if (track.ProductSize != 0)
                {
                    list = list.Where(x => x.SizeId == track.ProductSize);
                }
                return list.OrderByDescending(x => x.Id).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ProductSKUModel> GetProductList(ProductTrackAndTrace track)
        {
            try
            {
                var list = (from a in DbContext.ProductSKUs
                            join b in DbContext.Colors on a.ColorId equals b.ID into temp
                            from b in temp.DefaultIfEmpty()
                            join s in DbContext.Sizes on a.SizeId equals s.ID into TemSize
                            from s in TemSize.DefaultIfEmpty()
                            join pm in DbContext.ProductMasters on a.ProductId equals pm.Id
                            select new ProductSKUModel
                            {
                                TotalRows = (from r in DbContext.ProductSKUs
                                             join f in DbContext.Colors on r.ColorId equals f.ID into temp
                                             from f in temp.DefaultIfEmpty()
                                             join g in DbContext.Sizes on r.SizeId equals g.ID into TemSize
                                             from g in TemSize.DefaultIfEmpty()
                                             where (track.ProductId == 0 || r.ProductId == track.ProductId) &&
                                                    (track.ProductSize == 0 || g.ID == track.ProductSize) &&
                                                      (track.ProductColor == 0 || f.ID == track.ProductColor)
                                             select r).Count(),
                                Id = a.Id,
                                ProductId = a.ProductId,
                                ProductName = pm.ProductName,
                                ProductCode = pm.ProductCode,
                                StockRangeId = pm.StockRangeId,
                                ColorId = a.ColorId.ToString(),
                                ColorName = b.color1,
                                Size = s.size1,
                                SizeId = a.SizeId,
                                SKU = a.SKU,
                                Weight = a.Weight,
                                WeightUnit = a.WeightUnit,
                                Quantity = a.ActualQuantity,
                                WarningLevel = a.WarningLevel,
                                Style = a.Style,
                                OriginalJob = a.OriginalJob,
                                FebricNo = a.FebricNo,
                                FebricDescription = a.FebricDescription,
                                ProductDescription = a.ProductDescription,
                                HSCode = a.HSCode

                            });


                if (track.ProductId > 0)
                {
                    list = list.Where(x => x.ProductId == track.ProductId);
                }
                if (track.ProductColor != 0)
                {
                    list = list.Where(x => x.ColorId.ToLower().Contains(track.ProductColor.ToString().ToLower()));
                }
                if (track.ProductSize != 0)
                {
                    list = list.Where(x => x.SizeId == track.ProductSize);
                }
                return list.OrderByDescending(x => x.Id).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ProductMasterModel Initials()
        {
            try
            {
                ProductMasterModel model = new ProductMasterModel();
                model.Color = GetColorList();
                model.Size = GetSizeList();
                model.StockRange = null; // GetStockRangeList();
                                         //model.Product = GetProductList(id, keyword);
                model.ProductMaster = GetProductMaster();
                return model;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private List<ColorModel> GetColorList()
        {
            try
            {
                var interfaceObj = new AllRepository<Color>();
                List<ColorModel> list = new List<ColorModel>();
                var Color = interfaceObj.GetAll();
                foreach (var item in Color)
                {
                    var colr = new ColorModel();
                    colr.ID = item.ID;
                    colr.color = item.color1;
                    colr.description = item.description;
                    list.Add(colr);
                }
                return list;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private List<SizeModel> GetSizeList()
        {
            try
            {
                var interfaceObj = new AllRepository<Size>();
                List<SizeModel> list = new List<SizeModel>();
                var Size = interfaceObj.GetAll().Where(p => p.isActive == true).OrderBy(p => p.OrderNumber);
                foreach (var item in Size)
                {
                    var siz = new SizeModel();
                    siz.ID = item.ID;
                    siz.size = item.size1;
                    siz.description = item.description;
                    siz.type = item.Type;
                    list.Add(siz);
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool DeleteProductDetails(int id)
        {
            try
            {
                var data = DbContext.ProductSKUs.Where(x => x.Id == id).FirstOrDefault();
                DbContext.ProductSKUs.Remove(data);
                DbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<ProductDetailsModel> BindProductNameDDL(int id)
        {
            var data = (from a in DbContext.ProductMasters
                        where a.StockRangeId == id
                        select new ProductDetailsModel
                        {
                            Id = a.Id,
                            ProductName = a.ProductName,
                            ProductCode = a.ProductCode,
                        }).ToList();
            return data;
        }
        public List<ProductDetailsModel> GetStockProductMaster()
        {
            try
            {
                var data = (from a in DbContext.ProductMasters
                            select new ProductDetailsModel
                            {
                                Id = a.Id,
                                ProductCode = a.ProductCode,
                                ProductName = a.ProductName,
                                StockRangeId = a.StockRangeId
                            }).Where(p => p.ProductCode != "DSS1234" && p.ProductCode != "DSS1235").ToList();
                return data;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<ProductDetailsModel> GetProductMaster()
        {
            try
            {
                var data = (from a in DbContext.ProductMasters
                            select new ProductDetailsModel
                            {
                                Id = a.Id,
                                ProductCode = a.ProductCode == "DSS1235" ? "LSSO003" : a.ProductCode,
                                ProductName = a.ProductName,
                                StockRangeId = a.StockRangeId
                            }).ToList();
                return data;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #region Get product sku's bu Product ID
        public List<ProductSKUModel> GetProductSKUByPID(int productid)
        {
            try
            {
                var data = (from a in DbContext.ProductSKUs
                            join b in DbContext.ProductMasters on a.ProductId equals b.Id
                            join c in DbContext.Sizes on a.SizeId equals c.ID
                            join d in DbContext.Colors on a.ColorId equals d.ID
                            where a.ProductId == productid
                            select new ProductSKUModel
                            {
                                Id = a.Id,
                                OrderNumner = c.OrderNumber.HasValue ? c.OrderNumber.Value : 0,
                                ProductId = a.ProductId,
                                ProductName = b.ProductName,
                                ProductCode = b.ProductCode,
                                SKU = a.SKU,
                                SizeId = a.SizeId,
                                ClrId = a.ColorId,
                                Quantity = a.ActualQuantity,
                                ExpectedQuantity = a.ExpectedQuantity,
                                Size = c.size1,
                                ColorName = d.color1
                            }).OrderBy(p => new { p.ColorName, p.OrderNumner }).ToList();


                return data;
            }
            catch (Exception)
            {
                throw;
            }

        }
        #endregion

        #region get product sku by id
        public ProductSKUModel GetProductSKUByID(int id)
        {
            try
            {
                var data = (from a in DbContext.ProductSKUs
                            join b in DbContext.ProductMasters on a.ProductId equals b.Id
                            join c in DbContext.Sizes on a.SizeId equals c.ID
                            join d in DbContext.Colors on a.ColorId equals d.ID
                            where a.Id == id
                            select new ProductSKUModel
                            {
                                Id = a.Id,
                                ProductId = a.ProductId,
                                ProductName = b.ProductName,
                                ProductCode = b.ProductCode,
                                SKU = a.SKU,
                                SizeId = a.SizeId,
                                ClrId = a.ColorId,
                                Quantity = a.ActualQuantity,
                                ExpectedQuantity = a.ExpectedQuantity,
                                Size = c.size1,
                                ColorName = d.color1
                            }).FirstOrDefault();
                return data;
            }
            catch (Exception)
            {
                throw;
            }

        }
        #endregion

        #region get allocated product details using PDF no./accountNo

        private string getAccountNumbrFromBarcode(string barcode)
        {
            if (!string.IsNullOrEmpty(barcode))
            {
                var arra = barcode.Split('-');

                if (arra.Length > 0)
                {
                    return arra[0];
                }
            }

            return string.Empty;

        }
        public WebAPIResponse GetProductDetailByPDFno(string PDFno)
        {
            try
            {
                var rev = DbContext.UserReceivingNotes.Where(p => p.IsClosed == false && p.Barcode == PDFno).FirstOrDefault();

                if (rev == null)
                {
                    return new WebAPIResponse { Message = "PDF no. does not exist" };
                }

                string ac = getAccountNumbrFromBarcode(PDFno);

                var checkPDFNo = DbContext.WMSUserAdditionals.Where(x => x.AccountNumber == ac).FirstOrDefault();
                if (checkPDFNo != null)
                {
                    if (checkPDFNo.AccountNumber != null && checkPDFNo.AccountNumber != "")
                    {
                        var data = //(from a in DbContext.UserStocks
                                   //join b in DbContext.WMSUsers on a.UserId equals b.Id
                                   //join c in DbContext.WMSUserAdditionals on b.Id equals c.UserId
                                   //join d in DbContext.ProductSKUs on a.ProductSKUId equals d.Id
                                   //join e in DbContext.ProductMasters on d.ProductId equals e.Id
                                    (from b in DbContext.WMSUsers
                                     join c in DbContext.WMSUserAdditionals on b.Id equals c.UserId
                                     join d in DbContext.UserReceivingNotes on b.Id equals d.UserId into temp
                                     from d in temp.DefaultIfEmpty()
                                     join e in DbContext.UserReceivingNoteDetails on d.Id equals e.UserReceivingNoteId into temp2
                                     from e in temp2.DefaultIfEmpty()
                                     join f in DbContext.ProductSKUs on e.ProductSKUId equals f.Id
                                     join g in DbContext.ProductMasters on f.ProductId equals g.Id
                                     where c.AccountNumber == ac && d.IsClosed == false //&& e.ExpectedQuantity > 0
                                     select new ProductDetails
                                     {
                                         ProductSKUId = f.Id,
                                         ProductName = g.ProductName,
                                         Quantity = e.ExpectedQuantity,
                                         SKU = f.SKU,
                                         ReceivingNoteId = d.Id
                                     }).ToList();

                        if (data.Count != 0)
                        {
                            return new WebAPIResponse { Message = "Success", Result = data };
                        }
                        else
                        {
                            return new WebAPIResponse { Message = "No Record found" };
                        }
                    }
                    else
                    {
                        return new WebAPIResponse { Message = "PDF no. does not exist" };
                    }
                }
                else
                {
                    return new WebAPIResponse { Message = "PDF no. does not exist" };
                }
            }
            catch (Exception ex)
            {

                return new WebAPIResponse { Message = ex.Message };
            }

        }
        #endregion

        #region Update Product Quantity
        public WebAPIResponse ReceiveStock(List<ProductReceiveStockDetails> ProductDetails)
        {
            try
            {

                if (ProductDetails.Count > 0)
                {
                    foreach (var item in ProductDetails)
                    {
                        var data = DbContext.ProductSKUs.Where(x => x.Id == item.ProductSKUId).FirstOrDefault();
                        data.ActualQuantity = data.ActualQuantity + item.Quantity;
                        data.ExpectedQuantity = 0;
                        DbContext.SaveChanges();

                        var note = DbContext.UserReceivingNoteDetails.Where(x => x.UserReceivingNoteId == item.ReceivingNoteId && x.ProductSKUId == item.ProductSKUId).FirstOrDefault();
                        if (note != null)
                        {
                            note.ActualQuantity = Convert.ToInt32(item.Quantity);
                            DbContext.SaveChanges();
                        }
                    }
                    var dbReceivingNote = DbContext.UserReceivingNotes.Find(ProductDetails[0].ReceivingNoteId);
                    if (dbReceivingNote != null)
                    {
                        dbReceivingNote.IsClosed = true;
                        DbContext.SaveChanges();
                    }
                    return new WebAPIResponse { Message = "Success" };
                }
                else
                {
                    return new WebAPIResponse { Message = "No Record found" };
                }
            }
            catch (Exception ex)
            {
                return new WebAPIResponse { Message = ex.Message };
            }


        }
        #endregion

        #region 
        public WebAPIResponse PickUpStock(List<ProductDetails> ProductDetails)
        {
            try
            {
                if (ProductDetails.Count > 0)
                {
                    foreach (var item in ProductDetails)
                    {
                        var data = DbContext.ProductSKUs.Where(x => x.Id == item.ProductSKUId).FirstOrDefault();
                        // data.ActualQuantity = data.ActualQuantity - item.Quantity;

                        item.Quantity = item.Quantity != null ? item.Quantity:0;

                        var sectionData = DbContext.WarehouseAllocationDetails.Where(x => x.SectionDetailID == item.SectionId && x.ProductSKUID == item.ProductSKUId).FirstOrDefault();
                        sectionData.Quantity = sectionData.Quantity - item.Quantity.Value;
                        var pickOrder = DbContext.OrderPickupDetails.Where(x => x.SectionDetailID == item.SectionId && x.OrderID == item.OrderId && x.ProductSKUID == item.ProductSKUId && x.OrderDesignDetailId == item.OrderDesignDetailId).FirstOrDefault();
                        if (pickOrder != null)
                        {
                            pickOrder.PickedQty = item.Quantity;

                            #region "Update the actual quantity of the product sku"

                            if (item.Quantity < pickOrder.RequiredQty)
                            {
                                var pkObj = DbContext.ProductSKUs.Where(p => p.Id == item.ProductSKUId).FirstOrDefault();
                                if (pkObj != null)
                                {
                                    pkObj.ActualQuantity = pkObj.ActualQuantity + (pickOrder.RequiredQty - item.Quantity);
                                    //DbContext.Entry(pkObj).State = System.Data.Entity.EntityState.Modified;
                                    //DbContext.SaveChanges();
                                }
                            }
                            #endregion
                        }

                        DbContext.SaveChanges();

                        if (sectionData.Quantity < 1)
                        {
                            DbContext.WarehouseAllocationDetails.Remove(sectionData);
                            DbContext.SaveChanges();
                        }
                    }
                    return new WebAPIResponse { Message = "Success" };
                }
                else
                {
                    return new WebAPIResponse { Message = "No Record found" };
                }
            }
            catch (Exception ex)
            {
                return new WebAPIResponse { Message = ex.Message };
            }
        }
        #endregion

        #region Check is sku exists or not
        public bool CheckIsSKUExist(string sku)
        {
            try
            {
                //var data = DbContext.ProductSKUs.Where(x => x.Id == item.ProductSKUId).FirstOrDefault();
                //data.ActualQuantity = data.ActualQuantity - item.Quantity;

                //var sectionData = DbContext.WarehouseAllocationDetails.Where(x => x.SectionDetailID == item.SectionId && x.ProductSKUID == item.ProductSKUId).FirstOrDefault();
                //sectionData.Quantity = sectionData.Quantity - item.Quantity;
                //var pickOrder = DbContext.OrderPickupDetails.Where(x => x.OrderID == item.OrderId && x.ProductSKUID == item.ProductSKUId && x.OrderDesignDetailId == item.OrderDesignDetailId).FirstOrDefault();
                //if (pickOrder != null)

                var data = DbContext.ProductSKUs.Where(x => x.SKU == sku).FirstOrDefault();
                if (data != null)

                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

    }
}
