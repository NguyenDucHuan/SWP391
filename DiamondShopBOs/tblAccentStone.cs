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
    
    public partial class tblAccentStone
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblAccentStone()
        {
            this.tblItems = new HashSet<tblItem>();
        }
    
        public int accentStoneID { get; set; }
        public string accentStonesName { get; set; }
        public string shape { get; set; }
        public double caratWeight { get; set; }
        public string clarity { get; set; }
        public string color { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
        public string imagePath { get; set; }
        public Nullable<bool> status { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblItem> tblItems { get; set; }
    }
}
