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
    
    public partial class tblCertificate
    {
        public int certificateID { get; set; }
        public int diamondID { get; set; }
        public string certificateNumber { get; set; }
        public System.DateTime issueDate { get; set; }
        public string certifyingAuthority { get; set; }
        public string cerImagePath { get; set; }
    
        public virtual tblDiamond tblDiamond { get; set; }
    }
}