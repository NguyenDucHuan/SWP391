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
    
    public partial class tblItem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblItem()
        {
            this.tblOrderItems = new HashSet<tblOrderItem>();
        }
    
        public int ItemID { get; set; }
        public Nullable<int> settingID { get; set; }
        public Nullable<int> accentStoneID { get; set; }
        public Nullable<int> quantityAccent { get; set; }
        public Nullable<int> diamondID { get; set; }
        public decimal diamondPrice { get; set; }
        public decimal settingPrice { get; set; }
        public Nullable<decimal> accentStonePrice { get; set; }
    
        public virtual tblAccentStone tblAccentStone { get; set; }
        public virtual tblDiamond tblDiamond { get; set; }
        public virtual tblSetting tblSetting { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblOrderItem> tblOrderItems { get; set; }
    }
}
