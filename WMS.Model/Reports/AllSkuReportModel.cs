namespace WMS.Model.Reports
{
    public class AllSkuReportModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string StyleCode { get; set; }
        public string Colour { get; set; }
        public string Size { get; set; }
        public int SizeId { get; set; }
        public decimal NetWeight { get; set; }
        public int Quantity { get; set; }
        public string WeightUnit { get; set; }
    }
}
