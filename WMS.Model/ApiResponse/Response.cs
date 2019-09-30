using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Model.ApiResponse
{
    public class ApiResponse
    {
        public bool Status { get; set; }

        public string Email { get; set; }
    }

    public class WebAPIResponse
    {
        public string Message { get; set; }
        public object Result { get; set; }
    }
}
