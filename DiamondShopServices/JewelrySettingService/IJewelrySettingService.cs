using DiamondShopBOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopServices.JewelrySettingService
{
    public interface IJewelrySettingService
    {
        tblSetting GetSettingByID(int settingID);
        List<tblSetting> GetSettingAllList();
    }
}
