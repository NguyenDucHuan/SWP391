//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DiamondShopBOs
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblVoucher
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblVoucher()
        {
            this.tblUsers = new HashSet<tblUser>();
        }
    
        public int voucherID { get; set; }
        public System.DateTime startTime { get; set; }
        public System.DateTime endTime { get; set; }
        public Nullable<int> discount { get; set; }
        public Nullable<int> quantity { get; set; }
        public Nullable<bool> status { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblUser> tblUsers { get; set; }
    }
}