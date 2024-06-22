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

        public ItemCartDAO(int settingID, int accentStoneID, int diamondID, int? settingSize) : this()
        {
            this.diamondID = diamondID;
            var diamond = diamondDAO.GetDiamondById(diamondID);
            if (diamond != null)
            {
                this.diamondPrice = diamond.diamondPrice;
                this.DiamondName = diamond.diamondName;
            }
            else
            {
                throw new Exception("Diamond not found");
            }

            if (settingID != 0 && accentStoneID != 0)
            {
                this.settingID = settingID;
                var tblSetting = jewelrySettingDAO.GetSettingByID(settingID);
                if (tblSetting != null)
                {
                    this.settingPrice = tblSetting.priceTax;
                    this.quantityAccent = tblSetting.quantityStones;
                    this.imagePath = tblSetting.imagePath + "|" + diamond.diamondImagePath;
                    this.decription = tblSetting.description;
                    this.settingSize = settingSize;
                }
                else
                {
                    throw new Exception("Setting not found");
                }

                this.accentStoneID = accentStoneID;
                var accentStone = accentStoneDAO.GetAccentStone(accentStoneID);
                if (accentStone != null)
                {
                    this.accentStonePrice = accentStone.price;
                }
                else
                {
                    throw new Exception("Accent stone not found");
                }

            }
            else
            {
                this.settingID = 0;
                this.settingPrice = 0;
                this.accentStoneID = 0;
                this.accentStonePrice = 0;
                imagePath = diamond.diamondImagePath;
                decription = diamond.diamondDescription;
                this.settingSize = settingSize;
            }


        }
    }
}
