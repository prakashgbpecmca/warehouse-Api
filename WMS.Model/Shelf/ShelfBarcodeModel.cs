using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Model.Shelf
{
    public class ShelfBarcodeModel
    {
        public int SectionId { get; set; }
        public string ShelfBarcode { get; set; }
        public string ShelfBarcodeDisplay { get; set; }
    }

    public class ShelfBarcodeReportModel
    {
        public string ShelfBarcode1 { get; set; }
        public string ShelfBarcodeDisplay1 { get; set; }
    }

    public class ShelfBarcodeRoportFinalModel
    {
        public List<ShelfBarcodeReportModel> ShelfBarcodeFinalObj { get; set; }
    }

    public class FilePathModel
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FilePhysicalPath { get; set; } 
    }
}
