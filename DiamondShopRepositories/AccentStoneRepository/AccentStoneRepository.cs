using DiamondShopBOs;
using DiamondShopDAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopRepositories.AccentStoneRepository
{
    public class AccentStoneRepository : IAccentStoneRepository
    {
        private readonly AccentStoneDAO accentStoneDAO = null;

        public AccentStoneRepository()
        {
            accentStoneDAO = new AccentStoneDAO();
        }
        public tblAccentStone GetAccentStone(int id)
        {
            return accentStoneDAO.GetAccentStone(id);
        }

        public List<tblAccentStone> GetAllStones()
        {
            return accentStoneDAO.GetAllStones();
        }
    }
}
