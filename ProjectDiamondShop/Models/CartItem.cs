using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectDiamondShop.Models
{
    public class CartItem
    {
        public int DiamondID { get; set; }
        public string DiamondName { get; set; }
        public decimal DiamondPrice { get; set; }
    }


}