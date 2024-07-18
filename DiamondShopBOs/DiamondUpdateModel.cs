using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopBOs
{
    public class DiamondUpdateModel
    {
        public int diamondID { get; set; }
        public string diamondName { get; set; }
        public decimal diamondPrice { get; set; }
        public string diamondDescription { get; set; }
        public double caratWeight { get; set; }

    }
}
