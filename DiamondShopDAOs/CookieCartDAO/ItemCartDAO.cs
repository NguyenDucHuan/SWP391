using DiamondShopBOs;
using DiamondShopDAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopDAOs.CookieCartDAO
{
    public class ItemCartDAO
    {
        private readonly DiamondDAO diamondDAO = null;
        private readonly JewelrySettingDAO jewelrySettingDAO = null;
        private readonly AccentStoneDAO accentStoneDAO = null;
        public int? settingID { get; set; }
        public int? accentStoneID { get; set; }
        public int? quantityAccent { get; set; }
        public int diamondID { get; set; }
        public decimal diamondPrice { get; set; }
        public decimal? settingPrice { get; set; }
        public decimal? accentStonePrice { get; set; }

        public ItemCartDAO()
        {
            if (diamondDAO == null)
            {
                diamondDAO = new DiamondDAO();
            }
            if (jewelrySettingDAO == null)
            {
                jewelrySettingDAO = new JewelrySettingDAO();
            }
            if (accentStoneDAO == null)
            {
                accentStoneDAO = new AccentStoneDAO();
            }
        }

        public ItemCartDAO(int settingID, int accentStoneID, int diamondID)
        {
            this.diamondID = diamondID;
            this.diamondPrice = diamondDAO.GetDiamondById(diamondID).diamondPrice;
            if (settingID != 0 && accentStoneID != 0)
            {
                this.settingID = settingID;
                tblSetting tblSetting = jewelrySettingDAO.GetSettingByID(settingID);
                this.settingPrice = tblSetting.priceTax;
                this.quantityAccent = tblSetting.quantityStones;
                this.accentStoneID = accentStoneID;
                this.accentStonePrice = accentStoneDAO.GetAccentStone(accentStoneID).price;
            }
            else
            {
                this.settingID = null;
                this.settingPrice = null;
                this.accentStoneID = null;
                this.accentStonePrice = null;
            }
        }
    }
}
