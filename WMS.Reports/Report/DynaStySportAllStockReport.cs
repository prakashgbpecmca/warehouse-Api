namespace WMS.Reports.Report
{
    public partial class DynaStySportAllStockReport : DevExpress.XtraReports.UI.XtraReport
    {
        public DynaStySportAllStockReport(string ColorName)
        {
            InitializeComponent();

            switch (ColorName)
            {
                case "Green":
                    xrTableCell5.ForeColor = System.Drawing.Color.Green;
                    break;
                case "Orange":
                    xrTableCell5.ForeColor = System.Drawing.Color.Orange;
                    break;
                case "Red":
                    xrTableCell5.ForeColor = System.Drawing.Color.Red;
                    break;
            }
        }
    }
}
