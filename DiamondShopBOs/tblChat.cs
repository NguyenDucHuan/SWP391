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
    
    public partial class tblChat
    {
        public int chatID { get; set; }
        public string chatDetail { get; set; }
        public System.DateTime date { get; set; }
        public string senderID { get; set; }
        public string receiverID { get; set; }
    
        public virtual tblUser tblUser { get; set; }
        public virtual tblUser tblUser1 { get; set; }
    }
}
