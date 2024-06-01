using System.Collections.Generic;

namespace ProjectDiamondShop.Models
{
    public class ManagerViewModel
    {
        public List<Order> Orders { get; set; }
        public List<Diamond> Diamonds { get; set; }
        public List<User> SaleStaff { get; set; }
        public List<User> DeliveryStaff { get; set; }
    }
}
