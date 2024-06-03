using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectDiamondShop.Models
{
    public class OrderUpdateModel
    {
        public string OrderID { get; set; }
        public string SaleStaffID { get; set; }
        public string DeliveryStaffID { get; set; }
        public string Status { get; set; }
    }

}