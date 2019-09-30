using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using WMS.Model.DYO;
using WMS.Model.Reports;
using WMS.Service.Repository;
using WMS.Services.Utility;

namespace WMS.Reports
{
    public class ReportPrinting
    {
        public ReportFile Report(int orderId, List<MainDataSource> _main)
        {
            ReportFile file = new ReportFile();
            Reports.Report.DesignSheet sheet = new Reports.Report.DesignSheet();
            sheet.DataSource = _main;

            string fileName = string.Empty;
            ReportResult result = new DYORepository().CollectionInfo(orderId);
            if (result != null)
            {
                if (_main[0].ReportType == "OrderGraphic")
                {
                    fileName = "Order Graphics - " + result.CustomerCompanyName + " - " + _main[0].OrderName + result.JobSheetNumber + ".pdf";
                }
                else
                {
                    fileName = "Job sheet - " + result.CustomerCompanyName + " - " + _main[0].OrderName + result.JobSheetNumber + ".pdf";
                }
            }
            string filePath = HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/");
            if (!System.IO.Directory.Exists(filePath))
            {
                System.IO.Directory.CreateDirectory(filePath);
            }

            if (System.IO.Directory.Exists(filePath))
            {
                sheet.ExportToPdf(filePath + "\\" + fileName);
            }
            file.FileName = fileName;
            file.FilePhysicalPath = filePath + "\\" + fileName;
            file.FilePath = AppSettings.ProductPath + "Files\\Orders\\" + orderId + "\\" + fileName;
            return file;
        }

        public ReportFile WarehouseDispatchLabel(int orderId, int userId)
        {
            ReportFile file = new ReportFile();

            List<WarehouseLabels> _labels = new ReportRepository().WarehouseDispatchLabel(orderId, userId);
            Reports.Report.WarehouseDispatch sheet = new Reports.Report.WarehouseDispatch();
            sheet.DataSource = _labels;

            string fileName = "DispatchLabel_" + _labels[0].warehouse[0].PickNumber + ".pdf";
            string filePath = HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/");
            if (!System.IO.Directory.Exists(filePath))
            {
                System.IO.Directory.CreateDirectory(filePath);
            }

            if (System.IO.Directory.Exists(filePath))
            {
                sheet.ExportToPdf(filePath + "\\" + fileName);
            }
            file.FileName = fileName;
            file.FilePhysicalPath = filePath + "\\" + fileName;
            file.FilePath = AppSettings.ProductPath + "Files\\Orders\\" + orderId + "\\" + fileName;

            return file;
        }

        public class ReportFile
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
            public string FilePhysicalPath { get; set; }
        }
    }
}
