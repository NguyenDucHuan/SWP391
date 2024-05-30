using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectDiamondShop.Models
{
    public class Cart
    {
        public List<CartItem> Items { get; set; }
        public Cart()
        {
            Items = new List<CartItem>();
        }
    }

}