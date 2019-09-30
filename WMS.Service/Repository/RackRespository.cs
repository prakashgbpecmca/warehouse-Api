using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Model.Common;
// using WMS.Model.Rack;
using WMS.Model.Reports;
using WMS.Service.DataAccess;
using WMS.Service.GenericRepository;

namespace WMS.Service.Repository
{
    public class RackRespository
    {
        WMSEntities _db = new WMSEntities();
        public bool SaveRockDetail()
        {
            return true;
        }


        public List<CartonLabelReportModel> GetCartonReportObj(int? orderID)
        {
            List<CartonLabelReportModel> dataSource = new List<CartonLabelReportModel>();
            CartonLabelReportModel obj = new CartonLabelReportModel();
            var orderCartons = _db.OrderCartons.Find(orderID.Value);
            if (orderCartons != null)
            {
                obj.Barcode = orderCartons.CartonNo;
            }
            dataSource.Add(obj);
            return dataSource;
        }


    }
}
