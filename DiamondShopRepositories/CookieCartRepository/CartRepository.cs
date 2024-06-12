using DiamondShopDAOs.CookieCartDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopRepositories.CookieCartRepository
{
    public class CartRepository : ICartRepository
    {
        private readonly CartDao cart = null;
        public CartRepository()
        {
            cart = new CartDao();
        }

        public decimal ToatalCartMoney()
        {
            return cart.ToatalCartMoney();
        }
    }
}
