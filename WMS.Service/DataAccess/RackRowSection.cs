//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WMS.Service.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class RackRowSection
    {
        public int Id { get; set; }
        public int WareHouseId { get; set; }
        public int BockId { get; set; }
        public int RackId { get; set; }
        public int RowRackId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BarCode { get; set; }
    }
}