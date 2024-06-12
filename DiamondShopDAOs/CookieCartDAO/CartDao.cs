using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopDAOs.CookieCartDAO
{
    public class CartDao
    {
        public List<ItemCartDAO> Items;
        public CartDao()
        {
            if (Items == null) { Items = new List<ItemCartDAO>(); }

        }
        public decimal ToatalCartMoney()
        {
            decimal total = 0;
            foreach (var item in Items)
            {
                total += (decimal)item.diamondPrice + ((decimal)item.accentStonePrice * (decimal)item.quantityAccent) + (decimal)item.settingPrice;
            }
            return total;
        }
    }
}