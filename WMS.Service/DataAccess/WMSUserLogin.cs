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
    
    public partial class WMSUserLogin
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public int UserId { get; set; }
        public Nullable<int> UserLoginId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public bool IsLocked { get; set; }
    }
}
