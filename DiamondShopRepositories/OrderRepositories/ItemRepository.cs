using DiamondShopDAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopRepositories.OrderRepositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly ItemDAO itemDAO = null;
        public ItemRepository()
        {
            if (itemDAO == null)
            {
                itemDAO = new ItemDAO();
            }
        }
        public void CreateItem(string OrderId, int? settingID, int? accentStoneID, int? quantityAccent, int diamondID, decimal diamondPrice, decimal settingPrice, decimal accentPrice, int? settingSize)
        {
            itemDAO.CreateItem(OrderId, settingID, accentStoneID, quantityAccent, diamondID, diamondPrice, settingPrice, accentPrice, settingSize);
        }
    }
}
