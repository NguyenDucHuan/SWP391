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
        public void UpdateAccentStoneQuatity(int? inOrder, int? Id)
        {
            tblAccentStone accentStone = diamondShopManagementEntities.tblAccentStones.Where(i => i.accentStoneID == Id).FirstOrDefault();
            try
            {
                if (accentStone != null)
                {
                    accentStone.quantity = accentStone.quantity - (int)inOrder;
                    diamondShopManagementEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error at Update accent stone quantity");
            }
        }

    }
}