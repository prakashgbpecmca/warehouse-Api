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
    
    public partial class UserReceivingNote
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Barcode { get; set; }
        public string ReceivingNote { get; set; }
        public System.DateTime CreatedOnUtc { get; set; }
        public int CreatedBy { get; set; }
        public bool IsClosed { get; set; }
    }
}
