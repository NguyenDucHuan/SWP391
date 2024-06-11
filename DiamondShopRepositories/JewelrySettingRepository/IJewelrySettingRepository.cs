using DiamondShopBOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopRepositories.JewelrySettingRepository
{
    public interface IJewelrySettingRepository
    {
        tblSetting GetSettingByID(int settingID);
        List<tblSetting> GetSettingAllList();
    }
}
