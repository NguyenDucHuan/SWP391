﻿using System.Collections.Generic;

namespace ProjectDiamondShop.Models
{
    public class ViewOrderViewModel
    {
        public List<Order> CurrentOrders { get; set; }
        public List<Order> HistoryOrders { get; set; }
        public int RoleID { get; set; }
        public Order Order { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public string VoucherID { get; set; }
    }
}