using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WMS.Model.DYO;
using WMS.Model.Reports;
using WMS.Model.Shelf;
using WMS.Reports.Report;
using WMS.Service.Repository;
using WMS.Services.Utility;

namespace WMS.Reports
{
    public class BarCodeReport
    {
        public FilePathModel Barcode(int WarehouseId, int LineId, int ShelfId)
        {
            FilePathModel FPM = new FilePathModel();
            var data = new ShelfRespository().GetShelfBarCodeDetail(WarehouseId, LineId, ShelfId);

            BarcodeNo code = new BarcodeNo();
            code.DataSource = data;
            FPM.FileName = "WMSBarcode_Shelf_" + ShelfId + ".pdf";
            FPM.FilePath = AppSettings.ProductPath + "/BarcodeFiles/" + ShelfId + "/" + FPM.FileName;
            string filePhysicalPath = HttpContext.Current.Server.MapPath("~/BarcodeFiles/" + ShelfId + "/" + FPM.FileName);

            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/BarcodeFiles/" + ShelfId + "/")))
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/BarcodeFiles/" + ShelfId + "/"));

            if (File.Exists(filePhysicalPath))
            {
                File.Delete(filePhysicalPath);
            }
            code.ExportToPdf(filePhysicalPath);
            FPM.FilePhysicalPath = filePhysicalPath;
            return FPM;
        }
        public FilePathModel ReceivingNote(int userId, int customerId)
        {
            FilePathModel fileInfo = new FilePathModel();
            var data = new StockRespository().GenerateReceivingNote(userId, customerId);

            ReceivingNote code = new ReceivingNote();
            code.DataSource = data;
            fileInfo.FileName = "Receiving Note - " + data[0].CustomerCompany + " - " + data[0].Barcode + ".pdf";
            fileInfo.FilePath = AppSettings.ProductPath + "/BarcodeFiles/" + "/" + fileInfo.FileName;
            string filePhysicalPath = HttpContext.Current.Server.MapPath("~/BarcodeFiles/" + "/" + fileInfo.FileName);

            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/BarcodeFiles/" + "/")))
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/BarcodeFiles/" + "/"));

            if (File.Exists(filePhysicalPath))
            {
                File.Delete(filePhysicalPath);
            }
            code.ExportToPdf(filePhysicalPath);
            fileInfo.FilePhysicalPath = filePhysicalPath;
            return fileInfo;
        }

        public FilePathModel UpdatedPickRequests(int orderId)
        {
            FilePathModel fileInfo = new FilePathModel();
            var data = new ReportRepository().GetPickRequestObj(orderId);

            string reportHeading = "Order Dispatch Note";

            PickRequestReport code = new PickRequestReport();
            code.DataSource = data;
            code.Parameters["TopReportHeader"].Value = reportHeading;

            fileInfo.FileName = "UpdatedPickRequest_" + data[0].OrderNumber + ".pdf";
            fileInfo.FilePath = AppSettings.ProductPath + "/BarcodeFiles/" + "/" + fileInfo.FileName;
            string filePhysicalPath = HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/" + fileInfo.FileName);

            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/")))
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/"));

            if (File.Exists(filePhysicalPath))
            {
                File.Delete(filePhysicalPath);
            }
            code.ExportToPdf(filePhysicalPath);

            fileInfo.FilePhysicalPath = filePhysicalPath;
            return fileInfo;
        }
        public FilePathModel PickRequests(int orderId)
        {
            FilePathModel fileInfo = new FilePathModel();
            var data = new ReportRepository().GetPickRequestObj(orderId);

            string reportHeading = "Pick Request Note";

            PickRequestReport code = new PickRequestReport();
            code.DataSource = data;
            code.Parameters["TopReportHeader"].Value = reportHeading;

            fileInfo.FileName = "PickRequest_" + data[0].OrderNumber + ".pdf";
            fileInfo.FilePath = AppSettings.ProductPath + "/BarcodeFiles/" + "/" + fileInfo.FileName;
            string filePhysicalPath = HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/" + fileInfo.FileName);

            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/")))
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/"));

            if (File.Exists(filePhysicalPath))
            {
                File.Delete(filePhysicalPath);
            }
            code.ExportToPdf(filePhysicalPath);

            fileInfo.FilePhysicalPath = filePhysicalPath;
            return fileInfo;
        }
        public FilePathModel GenerateDispatchLabel(int orderId)
        {
            FilePathModel fileInfo = new FilePathModel();
            List<PickupRequestReportModel> data = new ReportRepository().GetDispatchProcessObj(orderId);
            PickRequestReport code = new PickRequestReport();
            code.DataSource = data;
            fileInfo.FileName = "DispatchLabel_" + data[0].OrderNumber + ".pdf";
            fileInfo.FilePath = AppSettings.ProductPath + "/BarcodeFiles/" + "/" + fileInfo.FileName;
            string filePhysicalPath = HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/" + fileInfo.FileName);

            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/")))
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/"));

            if (File.Exists(filePhysicalPath))
            {
                File.Delete(filePhysicalPath);
            }
            code.ExportToPdf(filePhysicalPath);

            fileInfo.FilePhysicalPath = filePhysicalPath;
            return fileInfo;
        }
        public FilePathModel JobSheet(int orderId)
        {
            FilePathModel file = new FilePathModel();

            List<MainDataSource> list = new ReportRepository().GetJobSheetImage(orderId, "Type");
            Reports.Report.DesignJobSheet sheet = new Reports.Report.DesignJobSheet();
            sheet.DataSource = list;
            string fileName = string.Empty;

            ReportResult result = new DYORepository().CollectionInfo(orderId);
            if (result != null)
            {
                fileName = !string.IsNullOrEmpty(result.CustomerCompanyName) ? result.CustomerCompanyName + "_" + result.JobSheetNumber + ".pdf" : result.CustomerName + "_" + result.JobSheetNumber + ".pdf";
            }
            string filePath = HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/");
            if (!System.IO.Directory.Exists(filePath))
            {
                System.IO.Directory.CreateDirectory(filePath);
            }

            if (System.IO.Directory.Exists(filePath))
            {
                if (File.Exists(filePath + "\\" + fileName))
                {
                    File.Delete(filePath + "\\" + fileName);
                }

                sheet.ExportToPdf(filePath + "\\" + fileName);
            }

            file.FileName = fileName;
            file.FilePhysicalPath = filePath + "\\" + fileName;
            file.FilePath = AppSettings.ProductPath + "Files\\Orders\\" + orderId + "\\" + fileName;
            return file;
        }
        public FilePathModel JobSheetShippingFile(int orderId)
        {

            FilePathModel file = new FilePathModel();
            List<MainDataSourceHeader> list = new ReportRepository().GetJobSheetShiipingImage(orderId);
            Reports.Report.JobSheetShippingDetail sheet = new Reports.Report.JobSheetShippingDetail();
            sheet.DataSource = list;
            string fileName = string.Empty;

            string result = new DYORepository().OrderNumber(orderId);
            if (result != null)
            {
                fileName = "Shipping Detail" + "_" + result + ".pdf";
            }
            string filePath = HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/");
            if (!System.IO.Directory.Exists(filePath))
            {
                System.IO.Directory.CreateDirectory(filePath);
            }

            if (System.IO.Directory.Exists(filePath))
            {
                if (File.Exists(filePath + "\\" + fileName))
                {
                    File.Delete(filePath + "\\" + fileName);
                }
            }
            sheet.ExportToPdf(filePath + "\\" + fileName);
            file.FileName = fileName;
            file.FilePhysicalPath = filePath + "\\" + fileName;
            file.FilePath = AppSettings.ProductPath + "Files\\Orders\\" + orderId + "\\" + fileName;
            return file;
        }
        public FilePathModel GetReport(ReportTrackandTraceModel ReportDetail)
        {
            FilePathModel FPM = new FilePathModel();
            if (ReportDetail.ReportType == "AllSKU")
            {
                var data = new ReportRepository().GetAllSKU(ReportDetail);
                if (data != null && data.Count > 0)
                {
                    AllSkuReport code = new AllSkuReport();
                    code.DataSource = data;
                    FPM.FileName = "All_SKU" + ".xlsx";
                    FPM.FilePath = AppSettings.ProductPath + "/Files/Reports/" + FPM.FileName;
                    string filePhysicalPath = HttpContext.Current.Server.MapPath("~/Files/Reports/" + FPM.FileName);

                    if (File.Exists(filePhysicalPath))
                    {
                        File.Delete(filePhysicalPath);
                    }
                    code.ExportToXlsx(filePhysicalPath);
                    FPM.FilePhysicalPath = filePhysicalPath;
                }
            }
            else if (ReportDetail.ReportType == "CustomerReport")
            {
                var data = new ReportRepository().GetCustomer(ReportDetail);
                if (data != null && data.Count > 0)
                {
                    CustomerReport code = new CustomerReport();
                    code.DataSource = data;
                    FPM.FileName = "All_Customers" + ".xlsx";
                    FPM.FilePath = AppSettings.ProductPath + "/Files/Reports/" + FPM.FileName;
                    string filePhysicalPath = HttpContext.Current.Server.MapPath("~/Files/Reports/" + FPM.FileName);

                    if (File.Exists(filePhysicalPath))
                    {
                        File.Delete(filePhysicalPath);
                    }
                    code.ExportToXlsx(filePhysicalPath);
                    FPM.FilePhysicalPath = filePhysicalPath;
                }
            }
            else if (ReportDetail.ReportType == "OrderReport")
            {
                var data = new ReportRepository().GetOrders(ReportDetail);
                if (data.Item1.Count > 0)
                {
                    string fileName = new ReportRepository().GetReportFileName(ReportDetail);
                    DynaStySportOrdersReport code = new DynaStySportOrdersReport();
                    code.DataSource = data.Item1;
                    FPM.FileName = fileName + ".xlsx";
                    FPM.FilePath = AppSettings.ProductPath + "/Files/Reports/" + FPM.FileName;
                    string filePhysicalPath = HttpContext.Current.Server.MapPath("~/Files/Reports/" + FPM.FileName);

                    if (File.Exists(filePhysicalPath))
                    {
                        File.Delete(filePhysicalPath);
                    }
                    code.ExportToXlsx(filePhysicalPath);
                    FPM.FilePhysicalPath = filePhysicalPath;
                }
                else if (data.Item2.Count > 0)
                {
                    string fileName = new ReportRepository().GetReportFileName(ReportDetail);
                    OrdersReport code = new OrdersReport();
                    code.DataSource = data.Item2;
                    FPM.FileName = fileName + ".xlsx";
                    FPM.FilePath = AppSettings.ProductPath + "/Files/Reports/" + FPM.FileName;
                    string filePhysicalPath = HttpContext.Current.Server.MapPath("~/Files/Reports/" + FPM.FileName);
                    if (File.Exists(filePhysicalPath))
                    {
                        File.Delete(filePhysicalPath);
                    }
                    code.ExportToXlsx(filePhysicalPath);
                    FPM.FilePhysicalPath = filePhysicalPath;
                }

            }
            else if (ReportDetail.ReportType == "StockStatusReport")
            {
                string color = string.Empty;
                var data = new ReportRepository().GetStockOrder(ReportDetail);
                if (data != null && data.Count > 0)
                {
                    if (ReportDetail.ReportStatus == "Available")
                    {
                        color = "Green";
                    }
                    else if (ReportDetail.ReportStatus == "RunningOutOfStock")
                    {
                        color = "Orange";
                    }
                    else if (ReportDetail.ReportStatus == "OutOfStock")
                    {
                        color = "Red";
                    }
                    DynaStySportAllStockReport code = new DynaStySportAllStockReport(color);
                    code.DataSource = data;
                    FPM.FileName = ReportDetail.ReportType + " " + "Report" + ".xlsx";
                    FPM.FilePath = AppSettings.ProductPath + "/Files/Reports/" + FPM.FileName;
                    string filePhysicalPath = HttpContext.Current.Server.MapPath("~/Files/Reports/" + FPM.FileName);
                    if (File.Exists(filePhysicalPath))
                    {
                        File.Delete(filePhysicalPath);
                    }
                    code.ExportToXlsx(filePhysicalPath);
                    FPM.FilePhysicalPath = filePhysicalPath;
                }
            }
            else if (ReportDetail.ReportType == "StockReport")
            {
                var custlist = new ReportRepository().GetStockReport(ReportDetail);

                FPM.FileName = ReportDetail.ReportType + " " + "Report" + ".xls";
                FPM.FilePath = AppSettings.ProductPath + "/Files/Reports/" + FPM.FileName;
                string filePhysicalPath = HttpContext.Current.Server.MapPath("~/Files/Reports/" + FPM.FileName);
                string FilePath = @"D:\WMSAPIProject\WMS.Api\Files\Reports\" + FPM.FileName;
                if (File.Exists(filePhysicalPath))
                {
                    File.Delete(filePhysicalPath);
                }

                ReportRepository rp = new ReportRepository();
                string aa = rp.GenerateExcel(custlist.Item1, custlist.Item2, custlist.Item3, filePhysicalPath);

                FPM.FilePhysicalPath = filePhysicalPath;
            }
            //DailyDispatchReport
            else if (ReportDetail.ReportType == "DailyDispatchReport")
            {
                var data = new ReportRepository().GetDailyDispatchReport(ReportDetail);
                if (data != null && data.Count > 0)
                {
                    DailyDispatchReport code = new DailyDispatchReport();
                    code.DataSource = data;
                    FPM.FileName = "Daily_Dispatch_Orders" + ReportDetail.FromDate.Value.ToString("dd-MMM-yyyy") + ".xlsx";
                    FPM.FilePath = AppSettings.ProductPath + "/Files/Reports/" + FPM.FileName;
                    string filePhysicalPath = HttpContext.Current.Server.MapPath("~/Files/Reports/" + FPM.FileName);

                    if (File.Exists(filePhysicalPath))
                    {
                        File.Delete(filePhysicalPath);
                    }
                    code.ExportToXlsx(filePhysicalPath);
                    FPM.FilePhysicalPath = filePhysicalPath;
                }
            }

            //OrderPendingDispatchReport
            else if (ReportDetail.ReportType == "OrderPendingDispatchReport")
            {
                var data = new ReportRepository().GetOrdersPendingDispatchReport(ReportDetail);
                if (data != null && data.Count > 0)
                {
                    OrderPendingDispatchReport code = new OrderPendingDispatchReport();
                    code.DataSource = data;
                    FPM.FileName = "Order_Pending Dispatch" + ".xlsx";
                    FPM.FilePath = AppSettings.ProductPath + "/Files/Reports/" + FPM.FileName;
                    string filePhysicalPath = HttpContext.Current.Server.MapPath("~/Files/Reports/" + FPM.FileName);

                    if (File.Exists(filePhysicalPath))
                    {
                        File.Delete(filePhysicalPath);
                    }
                    code.ExportToXlsx(filePhysicalPath);
                    FPM.FilePhysicalPath = filePhysicalPath;
                }
            }

            //TotalDailyDispatchReport
            else if (ReportDetail.ReportType == "TotalDailyDispatchReport")
            {
                var data = new ReportRepository().GetTotalDailyDispatchReport(ReportDetail);
                if (data != null && data.Count > 0)
                {
                    TotalDailyDispatchReport code = new TotalDailyDispatchReport();
                    code.DataSource = data;
                    FPM.FileName = "Total_DailyDispatch_Orders" + ReportDetail.FromDate.Value.ToString("dd-MMM-yyyy") + "" + ".xlsx";
                    FPM.FilePath = AppSettings.ProductPath + "/Files/Reports/" + FPM.FileName;
                    string filePhysicalPath = HttpContext.Current.Server.MapPath("~/Files/Reports/" + FPM.FileName);
                    if (File.Exists(filePhysicalPath))
                    {
                        File.Delete(filePhysicalPath);
                    }
                    code.ExportToXlsx(filePhysicalPath);
                    FPM.FilePhysicalPath = filePhysicalPath;
                }
            }

            //PendingPickRequestDispatch
            else if (ReportDetail.ReportType == "PendingPickRequestDispatchReport")
            {
                var data = new ReportRepository().GetPendingPickRequestDispatchReport(ReportDetail);
                if (data != null && data.Count > 0)
                {
                    PendingPickRequestDispatchReport code = new PendingPickRequestDispatchReport();
                    code.DataSource = data;
                    FPM.FileName = "Pending Pick Request" + DateTime.UtcNow.ToString("dd-MMM-yyyy") + ".xlsx";
                    FPM.FilePath = AppSettings.ProductPath + "/Files/Reports/" + FPM.FileName;
                    string filePhysicalPath = HttpContext.Current.Server.MapPath("~/Files/Reports/" + FPM.FileName);
                    if (File.Exists(filePhysicalPath))
                    {
                        File.Delete(filePhysicalPath);
                    }
                    code.ExportToXlsx(filePhysicalPath);
                    FPM.FilePhysicalPath = filePhysicalPath;
                }
            }

            return FPM;
        }
        public FilePathModel CartonLabel(int cartonId, int orderId)
        {
            FilePathModel fileInfo = new FilePathModel();

            List<CartonLabelReportModel> dataSouce = new RackRespository().GetCartonReportObj(cartonId);

            CartonNumber report = new CartonNumber();

            report.DataSource = dataSouce;

            fileInfo.FileName = dataSouce[0].Barcode + ".pdf";
            fileInfo.FilePath = AppSettings.ProductPath + "/Files/Orders/" + orderId + " /Cartons/ " + "/" + fileInfo.FileName;
            string filePhysicalPath = HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/Cartons/" + fileInfo.FileName);

            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/Cartons/")))
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/Cartons/"));

            if (File.Exists(filePhysicalPath))
            {
                File.Delete(filePhysicalPath);
            }
            report.ExportToPdf(filePhysicalPath);
            return fileInfo;
        }
    }
}
