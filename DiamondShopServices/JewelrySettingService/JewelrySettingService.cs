using DiamondShopBOs;
using DiamondShopRepositories.JewelrySettingRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopServices.JewelrySettingService
{
    public class JewelrySettingService : IJewelrySettingService
    {
        private readonly IJewelrySettingRepository jewelrySetting = null;
        public JewelrySettingService()
        {
            jewelrySetting = new JewelrySettingRepository();
        }
        public List<tblSetting> GetSettingAllList()
        {
            return jewelrySetting.GetSettingAllList();
        }

        public tblSetting GetSettingByID(int settingID)
        {
            return jewelrySetting.GetSettingByID(settingID);
        }
    }
}
