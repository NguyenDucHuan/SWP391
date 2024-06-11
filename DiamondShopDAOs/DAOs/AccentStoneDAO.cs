using DiamondShopBOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopDAOs
{
    public class AccentStoneDAO
    {
        private readonly DiamondShopManagementEntities diamondShopManagementEntities = null;
        public AccentStoneDAO()
        {
            if (diamondShopManagementEntities == null)
            {
                diamondShopManagementEntities = new DiamondShopManagementEntities();
            }
        }
        public tblAccentStone GetAccentStone(int id)
        {
            return diamondShopManagementEntities.tblAccentStones.Where(d => d.accentStoneID == id).FirstOrDefault();
        }
        public List<tblAccentStone> GetAllStones()
        {
            return diamondShopManagementEntities.tblAccentStones.ToList();
        }


    }
}