using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using DevExpress.XtraRichEdit;

namespace WMS.Reports.Report
{
    public partial class JobSheetShippingDetail : DevExpress.XtraReports.UI.XtraReport
    {
        public JobSheetShippingDetail()
        {
            InitializeComponent();
        }

        private void xrRichText1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            using (RichEditDocumentServer docServer = new RichEditDocumentServer())
            {
                docServer.RtfText = xrRichText1.Rtf;
                docServer.Document.DefaultCharacterProperties.FontName = "Arial";
                docServer.Document.DefaultCharacterProperties.FontSize = 7;
                xrRichText1.Rtf = docServer.RtfText;
            }
        }
    }
}
