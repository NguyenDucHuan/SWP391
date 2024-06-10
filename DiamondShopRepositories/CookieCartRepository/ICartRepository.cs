using DiamondShopDAOs.CookieCartDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopRepositories.CookieCartRepository
{
    public interface ICartRepository
    {
        decimal ToatalCartMoney();
    }
}
