using DiamondShopBOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopRepositories.AccentStoneRepository
{
    public interface IAccentStoneRepository
    {
        List<tblAccentStone> GetAllStones();
        tblAccentStone GetAccentStone(int id);
    }
}
