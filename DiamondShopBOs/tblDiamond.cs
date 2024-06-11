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
    
    public partial class tblDiamond
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblDiamond()
        {
            this.tblCertificates = new HashSet<tblCertificate>();
            this.tblComments = new HashSet<tblComment>();
            this.tblItems = new HashSet<tblItem>();
        }
    
        public int diamondID { get; set; }
        public string diamondName { get; set; }
        public decimal diamondPrice { get; set; }
        public string diamondDescription { get; set; }
        public double caratWeight { get; set; }
        public string clarityID { get; set; }
        public string cutID { get; set; }
        public string colorID { get; set; }
        public string shapeID { get; set; }
        public string diamondImagePath { get; set; }
        public Nullable<bool> status { get; set; }
        public int quantity { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblCertificate> tblCertificates { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblComment> tblComments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblItem> tblItems { get; set; }
    }
}
