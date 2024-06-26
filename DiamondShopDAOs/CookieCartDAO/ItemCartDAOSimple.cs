using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopDAOs.CookieCartDAO
{
    public class ItemCartDAOSimple
    {
        public int settingID { get; set; }
        public int accentStoneID { get; set; }
        public int quantityAccent { get; set; }
        public int diamondID { get; set; }
        public decimal diamondPrice { get; set; }
        public decimal settingPrice { get; set; }
        public decimal accentStonePrice { get; set; }
        public string DiamondName { get; set; }
        public string imagePath { get; set; }
        public string decription { get; set; }
        public int? settingSize { get; set; }
    }
}
