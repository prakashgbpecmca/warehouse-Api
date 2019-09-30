using Frayte.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Model.ApiResponse;
using WMS.Model.DYO;
using WMS.Model.Product;
using WMS.Model.Reports;
using WMS.Model.Shelf;
using WMS.Model.Stock;
using WMS.Model.User;
using WMS.Service.DataAccess;

namespace WMS.Service.Repository
{
    public class StockRespository
    {
        WMSEntities DbContext = new WMSEntities();

        public int CommonCOnversion { get; private set; }

        public List<WMSCustomreGrid> GetStockRangeCustomers(int userId)
        {
            var data = (
                        from r in DbContext.WMSUsers
                        join sr in DbContext.ProductCatagories on r.Id equals sr.CustomerId
                        join ua in DbContext.WMSUserAddresses on r.Id equals ua.UserId into add
                        from ua in add.DefaultIfEmpty()
                        join c in DbContext.Countries on ua.CountryId equals c.CountryId into cntry
                        from c in cntry.DefaultIfEmpty()
                        join uadd in DbContext.WMSUserAdditionals on r.Id equals uadd.UserId into temp
                        from uadd in temp.DefaultIfEmpty()
                        join ur in DbContext.WMSUserRoles on r.Id equals ur.UserId
                        where r.IsActive == true && ur.RoleId == 3
                        select new
                        {
                            AccountNo = uadd.AccountNumber,
                            CompanyName = r.CompanyName,
                            ContactName = r.ContactFirstName + " " + r.ContactLastName,
                            Country = c.CountryName,
                            Email = r.Email,
                            TelephoneNo = "(+ " + c.CountryPhoneCode + " )" + r.TelephoneNo,
                            UserId = r.Id
                        }).ToList();

            var list = data.GroupBy(group => group.UserId)
                            .Select(p => new WMSCustomreGrid
                            {
                                AccountNo = p.FirstOrDefault().AccountNo,
                                CompanyName = p.FirstOrDefault().CompanyName,
                                ContactName = p.FirstOrDefault().ContactName,
                                Country = p.FirstOrDefault().Country,
                                Email = p.FirstOrDefault().Email,
                                TelephoneNo = p.FirstOrDefault().TelephoneNo,
                                UserId = p.FirstOrDefault().UserId
                            }).ToList();
            return list;
        }

        public bool UpdateStock(UserStockModel item)
        {

            var prodSKU = DbContext.ProductSKUs.Find(item.ProductSKUId);
            if (prodSKU != null)
            {
                prodSKU.ExpectedQuantity = item.ExpectedQuantity;
                DbContext.SaveChanges();
                return true;
            }

            return false;

        }
        #region Get customers Stock list
        /// <summary>
        /// Get customers Stock list
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<UserStockModel> getCustomerStockList(int userid, string keyword)
        {
            try
            {

                var stockRangeCustomer = DbContext.ProductCatagories.Where(p => p.CustomerId == userid && p.CatagoryName != "SockRange").FirstOrDefault();

                var dbUser = DbContext.WMSUsers.Find(userid);

                if (stockRangeCustomer != null && dbUser != null)
                {
                    var data = (from r in DbContext.ProductMasters
                                join psku in DbContext.ProductSKUs on r.Id equals psku.ProductId
                                join e in DbContext.Sizes on psku.SizeId equals e.ID into temp
                                from e in temp.DefaultIfEmpty()
                                join f in DbContext.Colors on psku.ColorId equals f.ID into temp2
                                from f in temp2.DefaultIfEmpty()
                                select new UserStockModel
                                {
                                    Id = 0,
                                    UserId = dbUser.Id,
                                    ProductSKUId = psku.Id,
                                    UserName = dbUser.ContactFirstName + " " + dbUser.ContactLastName,
                                    CompanyName = dbUser.CompanyName,
                                    ProductId = r.Id,
                                    ProductName = r.ProductName,
                                    ProductCode = r.ProductCode,
                                    SKU = psku.SKU,
                                    SizeId = psku.SizeId,
                                    ColorId = psku.ColorId,
                                    Quantity = psku.ActualQuantity,
                                    ExpectedQuantity = psku.ExpectedQuantity,
                                    SizeName = e.size1,
                                    ColorName = f.color1,
                                    Weight = psku.Weight,
                                    WeightUnit = psku.WeightUnit
                                });
                    if (userid != null && userid > 0)
                    {
                        data = data.Where(x => x.UserId == userid);
                    }
                    if (keyword != null && keyword != "")
                    {
                        data = data.Where(x => x.UserName.ToLower().Contains(keyword.ToLower())
                          || x.ProductName.ToLower().Contains(keyword.ToLower())
                          || x.ProductCode.ToLower().Contains(keyword.ToLower())
                          || x.SKU.ToLower().Contains(keyword.ToLower())
                          || x.SizeName.ToLower().Contains(keyword.ToLower())
                          || x.ColorName.ToLower().Contains(keyword.ToLower()));
                    }
                    return data.OrderByDescending(x => x.Id).ToList();
                }
                else
                {
                    var data = (from a in DbContext.UserStocks
                                join b in DbContext.WMSUsers on a.UserId equals b.Id
                                join c in DbContext.ProductSKUs on a.ProductSKUId equals c.Id
                                join d in DbContext.ProductMasters on c.ProductId equals d.Id
                                join e in DbContext.Sizes on c.SizeId equals e.ID into temp
                                from e in temp.DefaultIfEmpty()
                                join f in DbContext.Colors on c.ColorId equals f.ID into temp2
                                from f in temp2.DefaultIfEmpty()
                                select new UserStockModel
                                {
                                    Id = a.Id,
                                    UserId = a.UserId,
                                    ProductSKUId = a.ProductSKUId,
                                    UserName = b.ContactFirstName + " " + b.ContactLastName,
                                    CompanyName = b.CompanyName,
                                    ProductId = c.ProductId,
                                    ProductName = d.ProductName,
                                    ProductCode = d.ProductCode,
                                    SKU = c.SKU,
                                    SizeId = c.SizeId,
                                    ColorId = c.ColorId,
                                    Quantity = c.ActualQuantity,
                                    ExpectedQuantity = c.ExpectedQuantity,
                                    SizeName = e.size1,
                                    ColorName = f.color1,
                                    Weight = c.Weight,
                                    WeightUnit = c.WeightUnit
                                });
                    if (userid != null && userid > 0)
                    {
                        data = data.Where(x => x.UserId == userid);
                    }
                    if (keyword != null && keyword != "")
                    {
                        data = data.Where(x => x.UserName.ToLower().Contains(keyword.ToLower())
                          || x.ProductName.ToLower().Contains(keyword.ToLower())
                          || x.ProductCode.ToLower().Contains(keyword.ToLower())
                          || x.SKU.ToLower().Contains(keyword.ToLower())
                          || x.SizeName.ToLower().Contains(keyword.ToLower())
                          || x.ColorName.ToLower().Contains(keyword.ToLower()));
                    }
                    return data.OrderByDescending(x => x.Id).ToList();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }



        public static int ConvertToInt(string stringValue)
        {
            int intValue = 0;
            int.TryParse(stringValue, out intValue);
            return intValue;
        }

        private int getBarcodeRev(string barcode)
        {
            int rev = 0;
            if (!string.IsNullOrEmpty(barcode))
            {
                var arr = barcode.Split('-');
                if (arr.Length > 1)
                {
                    rev = CommonConversion.ConvertToInt(arr[1]);
                    return rev;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }

        }
        public void SaveReceivingNoteFile(int userId, int customerId, ReceivingNoteResponse response)
        {
            var dbUserreceivingNote = DbContext.UserReceivingNotes.Where(p => p.UserId == response.CustomerId && p.IsClosed == false).FirstOrDefault();
            if (dbUserreceivingNote != null)
            {
                dbUserreceivingNote.ReceivingNote = response.FileInfo.FileName;
                DbContext.SaveChanges();

            }
        }
        public void SaveReceivingNote(int userId, int customerId, ReceivingNoteResponse obj)
        {
            if (obj.Stocks.Count > 0)
            {
                var userAddional = DbContext.WMSUserAdditionals.Where(p => p.UserId == customerId).FirstOrDefault();

                var dbUserreceivingNote = DbContext.UserReceivingNotes.Where(p => p.UserId == obj.CustomerId && p.IsClosed == false).FirstOrDefault();
                if (dbUserreceivingNote != null)
                {
                    dbUserreceivingNote.CreatedOnUtc = DateTime.UtcNow;
                    dbUserreceivingNote.CreatedBy = userId;
                    //  dbUserreceivingNote.ReceivingNote = obj.FileInfo.FileName;
                    dbUserreceivingNote.IsClosed = false;
                    dbUserreceivingNote.UserId = customerId;

                    int rev = getBarcodeRev(dbUserreceivingNote.Barcode);

                    var arr = dbUserreceivingNote.Barcode.Split('-');
                    dbUserreceivingNote.Barcode = arr[0] + "-" + (rev + 1);
                    DbContext.SaveChanges();

                    UserReceivingNoteDetail detail;
                    foreach (var item in obj.Stocks)
                    {
                        var dbSKU = DbContext.UserReceivingNoteDetails.Where(p => p.UserReceivingNoteId == dbUserreceivingNote.Id && p.ProductSKUId == item.ProductSKUId).FirstOrDefault();
                        if (dbSKU != null)
                        {
                            dbSKU.ExpectedQuantity = item.ExpectedQuantity.HasValue ? item.ExpectedQuantity.Value : 0;
                            dbSKU.ActualQuantity = 0;
                            DbContext.SaveChanges();
                        }
                        else
                        {
                            detail = new UserReceivingNoteDetail();
                            detail.UserReceivingNoteId = dbUserreceivingNote.Id;
                            detail.ProductSKUId = item.ProductSKUId;
                            detail.ExpectedQuantity = item.ExpectedQuantity.HasValue ? item.ExpectedQuantity.Value : 0;
                            detail.ActualQuantity = 0;
                            DbContext.UserReceivingNoteDetails.Add(detail);
                            DbContext.SaveChanges();
                        }

                    }

                }
                else
                {
                    UserReceivingNote dbUserReceivingNote = new UserReceivingNote();
                    dbUserReceivingNote.CreatedOnUtc = DateTime.UtcNow;
                    dbUserReceivingNote.CreatedBy = userId;
                    //     dbUserReceivingNote.ReceivingNote = obj.FileInfo.FileName;
                    dbUserReceivingNote.IsClosed = false;
                    dbUserReceivingNote.UserId = customerId;
                    int rev = getBarcodeRev(userAddional.AccountNumber);

                    dbUserReceivingNote.Barcode = userAddional.AccountNumber + "-" + (rev + 1);
                    DbContext.UserReceivingNotes.Add(dbUserReceivingNote);
                    DbContext.SaveChanges();
                    UserReceivingNoteDetail detail;
                    foreach (var item in obj.Stocks)
                    {
                        detail = new UserReceivingNoteDetail();
                        detail.UserReceivingNoteId = dbUserReceivingNote.Id;
                        detail.ProductSKUId = item.ProductSKUId;
                        detail.ExpectedQuantity = item.ExpectedQuantity.HasValue ? item.ExpectedQuantity.Value : 0;
                        detail.ActualQuantity = 0;
                        DbContext.UserReceivingNoteDetails.Add(detail);
                        DbContext.SaveChanges();
                    }
                }
            }
        }

        public List<UserStockModel> GetReceivingNoteStock(int customerId)
        {
            var data = (from a in DbContext.UserStocks
                        join b in DbContext.WMSUsers on a.UserId equals b.Id
                        join c in DbContext.ProductSKUs on a.ProductSKUId equals c.Id
                        join d in DbContext.ProductMasters on c.ProductId equals d.Id
                        join e in DbContext.Sizes on c.SizeId equals e.ID into temp
                        from e in temp.DefaultIfEmpty()
                        join f in DbContext.Colors on c.ColorId equals f.ID into temp2
                        from f in temp2.DefaultIfEmpty()
                        where c.ExpectedQuantity > 0 && a.UserId == customerId
                        select new UserStockModel
                        {
                            Id = a.Id,
                            UserId = a.UserId,
                            ProductSKUId = a.ProductSKUId,
                            UserName = b.ContactFirstName + " " + b.ContactLastName,
                            CompanyName = b.CompanyName,
                            ProductId = c.ProductId,
                            ProductName = d.ProductName,
                            ProductCode = d.ProductCode,
                            SKU = c.SKU,
                            SizeId = c.SizeId,
                            ColorId = c.ColorId,
                            Quantity = c.ActualQuantity,
                            ExpectedQuantity = c.ExpectedQuantity,
                            SizeName = e.size1,
                            ColorName = f.color1,
                            Weight = c.Weight,
                            WeightUnit = c.WeightUnit
                        }).ToList();

            return data;
        }
        public List<WMSCustomreGrid> GetCustomers(TrackUser model) // 4th in stock allocation
        {
            List<WMSCustomreGrid> list = new List<WMSCustomreGrid>();
            List<WMSCustomreGrid> list1 = new List<WMSCustomreGrid>();
            var stockrange = DbContext.ProductCatagories.Distinct().Select(p => new { CustomerId = p.CustomerId }).Distinct().ToList();
            var data = (from r in DbContext.WMSUsers
                        join ua in DbContext.WMSUserAddresses on r.Id equals ua.UserId into add
                        from ua in add.DefaultIfEmpty()
                        join c in DbContext.Countries on ua.CountryId equals c.CountryId into cntry
                        from c in cntry.DefaultIfEmpty()
                        join uadd in DbContext.WMSUserAdditionals on r.Id equals uadd.UserId into temp
                        from uadd in temp.DefaultIfEmpty()
                        join u in DbContext.WMSUsers on uadd.MerchandiseUserId equals u.Id into tempCordi
                        from u in tempCordi.DefaultIfEmpty()
                        join ur in DbContext.WMSUserRoles on r.Id equals ur.UserId
                        where r.IsActive == true && ur.RoleId == 3
                        select new WMSCustomreGrid
                        {
                            AccountNo = "",
                            CompanyName = r.CompanyName,
                            ContactName = r.ContactFirstName + " " + r.ContactLastName,
                            Country = c.CountryName,
                            Email = r.Email,
                            TelephoneNo = "(+ " + c.CountryPhoneCode + " )" + r.TelephoneNo,
                            UserId = r.Id,
                            Merchandiser = u.ContactFirstName + " " + u.ContactLastName
                        }).ToList();

            if (stockrange.Count > 0)
            {
                foreach (var item in stockrange)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        if (item.CustomerId != data[i].UserId)
                        {
                            list1.Add(data[i]);
                        }
                    }
                }
            }
            list = list1.OrderByDescending(x => x.UserId).ToList();
            return list;
        }
        public List<ReceivingNoteReport> GenerateReceivingNote(int userId, int customerId)
        {
            List<ReceivingNoteReport> dataSource = new List<ReceivingNoteReport>();
            var createdBy = DbContext.WMSUsers.Find(userId);
            ReceivingNoteReport reportModel = new ReceivingNoteReport();
            var customreAddress = (from r in DbContext.WMSUsers
                                   join ua in DbContext.WMSUserAddresses on r.Id equals ua.UserId
                                   join c in DbContext.Countries on ua.CountryId equals c.CountryId
                                   where r.Id == customerId
                                   select new DYOOrderAddress
                                   {
                                       Address = ua.Address,
                                       Address2 = ua.Address2,
                                       Area = ua.Suburb,
                                       State = ua.State,
                                       City = ua.City,
                                       FirstName = r.ContactFirstName,
                                       LastName = r.ContactLastName,
                                       CompanyName = r.CompanyName,
                                       Email = r.Email,
                                       Phone = "(+" + c.CountryPhoneCode + ") " + r.TelephoneNo,
                                       PostCode = ua.PostCode,
                                       Country = new Model.User.WMSCountry
                                       {
                                           Code = c.CountryCode,
                                           Code2 = c.CountryCode2,
                                           CountryPhoneCode = c.CountryPhoneCode,
                                           Name = c.CountryName
                                       }
                                   }).FirstOrDefault();

            reportModel.CustomerCompany = customreAddress.CompanyName;

            var customreDetail = DbContext.WMSUserAdditionals.Where(p => p.UserId == customerId).FirstOrDefault();
            if (customreDetail != null)
            {
                string Address = "Company: ";
                if (!string.IsNullOrEmpty(customreAddress.CompanyName))
                {
                    Address = Address + customreAddress.CompanyName + Environment.NewLine;
                }
                else
                {
                    Address = Address + customreAddress.FirstName + " " + customreAddress.LastName + Environment.NewLine;
                }
                Address += "Address: ";
                if (!string.IsNullOrEmpty(customreAddress.Address))
                {
                    Address = Address + customreAddress.Address + Environment.NewLine;
                }
                if (!string.IsNullOrEmpty(customreAddress.Address2))
                {
                    Address = Address + customreAddress.Address2 + Environment.NewLine;
                }
                if (!string.IsNullOrEmpty(customreAddress.City))
                {
                    Address = Address + customreAddress.City;
                }
                if (!string.IsNullOrEmpty(customreAddress.PostCode))
                {
                    Address = Address + " - " + customreAddress.PostCode + Environment.NewLine;
                }
                if (!string.IsNullOrEmpty(customreAddress.State))
                {
                    Address = Address + customreAddress.State + Environment.NewLine; ;
                }
                if (customreAddress.Country != null && !string.IsNullOrEmpty(customreAddress.Country.Name))
                {
                    Address = Address + customreAddress.Country.Name + Environment.NewLine;
                }
                Address += "Phone: ";
                if (customreAddress.Country != null && !string.IsNullOrEmpty(customreAddress.Country.CountryPhoneCode) && !string.IsNullOrEmpty(customreAddress.Phone))
                {
                    Address += customreAddress.Phone + Environment.NewLine;
                }
                Address += "Email: ";

                if (!string.IsNullOrEmpty(customreAddress.Email))
                {
                    Address += customreAddress.Email + Environment.NewLine;
                }
                reportModel.CustomerInfo = Address;

                #region
                var rece = DbContext.UserReceivingNotes.Where(p => p.UserId == customerId && p.IsClosed == false).FirstOrDefault();

                if (rece != null)
                {
                    reportModel.Barcode = rece.Barcode;
                }
                else
                {
                    reportModel.Barcode = customreDetail.AccountNumber;
                }

                #endregion


                reportModel.PrintedBy = createdBy.ContactFirstName + " " + createdBy.ContactLastName;


                var TimeZone = new ReportRepository().UserTimeZoneDetail(customerId);

                if (TimeZone != null)
                {
                    var TimeZoneInformation = TimeZoneInfo.FindSystemTimeZoneById(TimeZone.Name);
                    var date = UtilityRepository.UtcDateToOtherTimezone(DateTime.UtcNow, DateTime.UtcNow.TimeOfDay, TimeZoneInformation);
                    reportModel.PrintedOn = date.Item1.ToString("dd-MMM-yy") + " " + UtilityRepository.GetFormattedTimeString(date.Item2) + " " + TimeZone.OffsetShort;   // "03-May-19 17:34 GMT +5:30";
                }
                else
                {

                }


                var data = (from a in DbContext.UserReceivingNotes
                            join rd in DbContext.UserReceivingNoteDetails on a.Id equals rd.UserReceivingNoteId
                            join b in DbContext.WMSUsers on a.UserId equals b.Id
                            join c in DbContext.ProductSKUs on rd.ProductSKUId equals c.Id
                            join d in DbContext.ProductMasters on c.ProductId equals d.Id
                            join e in DbContext.Sizes on c.SizeId equals e.ID into temp
                            from e in temp.DefaultIfEmpty()
                            join f in DbContext.Colors on c.ColorId equals f.ID into temp2
                            from f in temp2.DefaultIfEmpty()
                            where c.ExpectedQuantity > 0 && a.UserId == customerId && a.IsClosed == false
                            select new UserStockModel
                            {
                                Id = a.Id,
                                UserId = a.UserId,
                                ProductSKUId = rd.ProductSKUId,
                                UserName = b.ContactFirstName + " " + b.ContactLastName,
                                CompanyName = b.CompanyName,
                                ProductId = c.ProductId,
                                ProductName = d.ProductName,
                                ProductCode = d.ProductCode,
                                SKU = c.SKU,
                                SizeId = c.SizeId,
                                ColorId = c.ColorId,
                                Quantity = c.ActualQuantity,
                                ExpectedQuantity = c.ExpectedQuantity,
                                SizeName = e.size1,
                                ColorName = f.color1,
                                Weight = c.Weight,
                                WeightUnit = c.WeightUnit
                            }).ToList();


                reportModel.ReceivingNote = "Order No#: " + Environment.NewLine;
                reportModel.ReceivingNote += "Order Type: " + Environment.NewLine;
                reportModel.ReceivingNote += "Total Qty: " + data.Sum(p => p.ExpectedQuantity) + Environment.NewLine;
                reportModel.ReceivingNote += "Created On: " + DateTime.UtcNow.ToString("") + Environment.NewLine; ;
                reportModel.ReceivingNote += "Craeted By: " + createdBy.ContactFirstName + " " + createdBy.ContactLastName + Environment.NewLine; ;

                if (data.Count > 0)
                {
                    reportModel.Stocks = new List<ReceivingNoteReportDetail>();
                    ReceivingNoteReportDetail stock;
                    int i = 0;
                    foreach (var item in data)
                    {
                        stock = new ReceivingNoteReportDetail();
                        stock.SNo = ++i;
                        stock.ProductColor = item.ColorName;
                        stock.ProductSize = item.SizeName;
                        stock.GrossWeight = item.Weight;
                        stock.NetWeight = item.Weight;
                        stock.ProductName = item.ProductName;
                        stock.SKU = item.SKU;
                        stock.TotalQuantity = item.ExpectedQuantity.HasValue ? item.ExpectedQuantity.Value : 0;
                        reportModel.Stocks.Add(stock);
                    }
                }
            }

            dataSource.Add(reportModel);
            return dataSource;
        }
        #endregion

        #region add stock

        public string addStock(productStock model)
        {
            try
            {
                List<int> pSKUidArray = new List<int>();
                foreach (var item in model.productStockList)
                {
                    var data = DbContext.ProductSKUs.Where(x => x.Id == item.productSKUID).FirstOrDefault();
                    if (data != null)
                    {
                        data.ExpectedQuantity = item.ExpectedQuantity;
                        data.UpdatedBy = model.LoginUserID;
                        data.UpdatedOnUtc = DateTime.UtcNow;
                        DbContext.SaveChanges();
                    }
                    pSKUidArray.Add(item.productSKUID);
                }
                if (model.customerID > 0)
                {
                    UserStockList obj = new UserStockList();
                    obj.UserId = model.customerID;
                    obj.LoginUserId = model.LoginUserID;
                    obj.productSKUId = pSKUidArray.ToArray();
                    AllocateStockToUser(obj);
                }
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        public string AllocateStockToUser(UserStockList model)
        {
            try
            {
                foreach (var item in model.productSKUId)
                {
                    int productSKUId = Convert.ToInt32(item);
                    var data = DbContext.UserStocks.Where(x => x.UserId == model.UserId && x.ProductSKUId == productSKUId).FirstOrDefault();
                    if (data == null)
                    {
                        UserStock tb = new UserStock();
                        tb.UserId = model.UserId;
                        tb.ProductSKUId = item;
                        tb.UpdatedOnUtc = DateTime.UtcNow;
                        tb.UpdatedBy = Convert.ToInt32(model.LoginUserId);
                        DbContext.UserStocks.Add(tb);
                        DbContext.SaveChanges();
                    }

                }
                return "Success";
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<ProductSKUModel> GetProductSKUList(int UserId, string keyword, int colorid, int sizeid) // 3rd in stock allocation
        {
            try
            {
                var list = (from a in DbContext.ProductSKUs
                            join b in DbContext.Colors on a.ColorId equals b.ID into temp
                            from b in temp.DefaultIfEmpty()
                            join s in DbContext.Sizes on a.SizeId equals s.ID into TemSize
                            from s in TemSize.DefaultIfEmpty()
                                // join st in DbContext.StockRanges on a.StockRangeId equals st.Id into tempstock
                                // from st in tempstock.DefaultIfEmpty()
                            join pm in DbContext.ProductMasters on a.ProductId equals pm.Id
                            select new ProductSKUModel
                            {
                                Id = a.Id,
                                // StockRangeId = a.StockRangeId,
                                //   StockRange = st.Name,
                                ProductId = a.ProductId,
                                ProductName = pm.ProductName,
                                ProductCode = pm.ProductCode,
                                StockRangeId = pm.StockRangeId,
                                ColorId = a.ColorId.ToString(),
                                ClrId = a.ColorId,
                                ColorName = b.color1,
                                Size = s.size1,
                                SizeId = a.SizeId,
                                SKU = a.SKU,
                                Weight = a.Weight,
                                WeightUnit = a.WeightUnit,
                                Quantity = a.ActualQuantity,
                                WarningLevel = a.WarningLevel,
                                ProductDescription = pm.ProductDescription,
                                Style = a.Style,
                                IsChecked = (DbContext.UserStocks.Where(x => x.ProductSKUId == a.Id && x.UserId == UserId).FirstOrDefault() != null ? true : false)

                            });

                if (keyword != null && keyword != "")
                {
                    list = list.Where(x => x.ProductCode.ToLower().Contains(keyword.ToLower()));
                }

                if (colorid > 0)
                {
                    list = list.Where(x => x.ClrId == colorid);
                }

                if (sizeid > 0)
                {
                    list = list.Where(x => x.SizeId == sizeid);
                }
                //  list = list.OrderByDescending(x => x.Id).ToList();
                return list.ToList();


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}
