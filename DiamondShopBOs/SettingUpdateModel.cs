using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopBOs
{
    public class SettingUpdateModel
    {
        public int settingID { get; set; }
        public string settingType { get; set; }
        public string material { get; set; }
        public decimal priceTax { get; set; }
        public int quantityStones { get; set; }
        public string description { get; set; }
    }
}
