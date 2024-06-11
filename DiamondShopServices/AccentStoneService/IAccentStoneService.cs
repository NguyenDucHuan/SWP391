using DiamondShopBOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopServices.AccentStoneService
{
    public interface IAccentStoneService
    {
        List<tblAccentStone> GetAllStones();
        tblAccentStone GetAccentStone(int id);
    }
}
