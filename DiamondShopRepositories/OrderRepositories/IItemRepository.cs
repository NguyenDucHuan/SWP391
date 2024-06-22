using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopRepositories.OrderRepositories
{
    public interface IItemRepository
    {
        void CreateItem(string OrderId, int? settingID, int? accentStoneID, int? quantityAccent, int diamondID, decimal diamondPrice, decimal settingPrice, decimal accentPrice, int? settingSize);
    }
}
