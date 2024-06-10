using DiamondShopBOs;
using DiamondShopRepositories.AccentStoneRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopServices.AccentStoneService
{
    public class AccentStoneService : IAccentStoneService
    {
        private readonly IAccentStoneRepository AccentStoneRepository = null;

        public AccentStoneService()
        {
            AccentStoneRepository = new AccentStoneRepository();
        }
        public tblAccentStone GetAccentStone(int id)
        {
            return AccentStoneRepository.GetAccentStone(id);
        }

        public List<tblAccentStone> GetAllStones()
        {
            return AccentStoneRepository.GetAllStones();
        }
    }
}
