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
    
    public partial class OrderCarton
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderCarton()
        {
            this.OrderCartonDetails = new HashSet<OrderCartonDetail>();
        }
    
        public int ID { get; set; }
        public Nullable<int> OrderID { get; set; }
        public string CartonNo { get; set; }
        public string CartonDisplayName { get; set; }
        public bool IsClosed { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderCartonDetail> OrderCartonDetails { get; set; }
        public virtual Order Order { get; set; }
    }
}
