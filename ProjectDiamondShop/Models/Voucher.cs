using System;

namespace ProjectDiamondShop.Models
{
    public class Voucher
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Discount { get; set; }
        public int Quantity { get; set; }
    }
}