using DiamondShopBOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopDAOs
{
    public class JewelrySettingDAO
    {
        private readonly DiamondShopManagementEntities diamondShopManagementEntities = null;
        public JewelrySettingDAO()
        {
            if (diamondShopManagementEntities == null)
            {
                diamondShopManagementEntities = new DiamondShopManagementEntities();
            }
        }
        public tblSetting GetSettingByID(int settingID)
        {
            return diamondShopManagementEntities.tblSettings.FirstOrDefault(d => d.settingID == settingID);
        }
        public List<tblSetting> GetSettingAllList()
        {
            return diamondShopManagementEntities.tblSettings.ToList();
        }

    }
}
