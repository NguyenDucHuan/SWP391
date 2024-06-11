using DiamondShopBOs;
using DiamondShopDAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopRepositories.JewelrySettingRepository
{
    public class JewelrySettingRepository : IJewelrySettingRepository
    {
        private readonly JewelrySettingDAO jewelrySettingDAO = null;
        public JewelrySettingRepository()
        {
            jewelrySettingDAO = new JewelrySettingDAO();
        }
        public List<tblSetting> GetSettingAllList()
        {
            return jewelrySettingDAO.GetSettingAllList();
        }

        public tblSetting GetSettingByID(int settingID)
        {
            return jewelrySettingDAO.GetSettingByID(settingID);
        }
    }
}
