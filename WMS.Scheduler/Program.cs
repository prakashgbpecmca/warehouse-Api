using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace WMS.Scheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"C:\SchedulerErrorLog\Error.txt";

            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("-----------------------------------------------------------------------------");
                    writer.WriteLine("Scheduler started at " + DateTime.Now.ToString());
                    writer.WriteLine();
                }

                //string apiUrl = "https://lionstarstock.com/API/DYO";
                string apiUrl = "http://localhost:56269/DYO";

                WebClient client = new WebClient();
                client.Headers["Content-type"] = "application/json";
                client.Encoding = Encoding.UTF8;
                string json = client.UploadString(apiUrl + "/RemoveRejectedDesign", "");
               // List<ProductDetailsModel> customers = JsonConvert.DeserializeObject<List<ProductDetailsModel>>(json);

                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("-----------------------------------------------------------------------------");
                    writer.WriteLine("Scheduler ended successfully at " + DateTime.Now.ToString());
                    writer.WriteLine();
                }
            }
            catch (Exception ex) {

                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("-----------------------------------------------------------------------------");
                    writer.WriteLine("Date : " + DateTime.Now.ToString());
                    writer.WriteLine();

                    while (ex != null)
                    {
                        writer.WriteLine(ex.GetType().FullName);
                        writer.WriteLine("Message : " + ex.Message);
                        writer.WriteLine("StackTrace : " + ex.StackTrace);

                        ex = ex.InnerException;
                    }

                    writer.WriteLine("-----------------------------------------------------------------------------");
                    writer.WriteLine("Scheduler ended at " + DateTime.Now.ToString());
                    writer.WriteLine();
                }
            }
        }      
    }

    public class ProductDetailsModel
    {
        public int Id { get; set; }
        public int StockRangeId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
    }
}
