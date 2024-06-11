using DiamondShopRepositories.OrderRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopServices.OrderServices
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository repository = null;
        public ItemService()
        {
            if (repository == null)
            {
                repository = new ItemRepository();
            }
        }
        public void CreateItem(string OrderId, int? settingID, int? accentStoneID, int? quantityAccent, int diamondID, decimal diamondPrice, decimal settingPrice, decimal accentPrice)
        {
            repository.CreateItem(OrderId, settingID, accentStoneID, quantityAccent, diamondID, diamondPrice, settingPrice, accentPrice);
        }
    }
}
