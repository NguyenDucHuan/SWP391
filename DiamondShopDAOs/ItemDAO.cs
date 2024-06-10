using DiamondShopBOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopDAOs
{
    public class ItemDAO
    {
        private readonly DiamondShopManagementEntities entities = null;
        public ItemDAO()
        {
            if (entities == null)
            {
                entities = new DiamondShopManagementEntities();
            }
        }

    }
}
