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
    
    public partial class tblComment
    {
        public string commentID { get; set; }
        public string userID { get; set; }
        public int diamondID { get; set; }
        public string comment { get; set; }
        public Nullable<int> rating { get; set; }
    
        public virtual tblDiamond tblDiamond { get; set; }
        public virtual tblUser tblUser { get; set; }
    }
}
