using System;

namespace ProjectDiamondShop.Models
{
    public class Order
    {
        public string OrderID { get; set; }
        public string CustomerID { get; set; }
        public string DeliveryStaffID { get; set; }
        public string SaleStaffID { get; set; }  // New Property
        public double TotalMoney { get; set; }
        public string Status { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateTime SaleDate { get; set; }
        public string ShopOrderID { get; set; }
        public string DeliveryService { get; set; }
    }
}
