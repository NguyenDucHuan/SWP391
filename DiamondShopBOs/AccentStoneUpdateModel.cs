using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopBOs
{
    public class AccentStoneUpdateModel
    {
        public int accentStoneID { get; set; }
        public string accentStonesName { get; set; }
        public double caratWeight { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
    }
}
