using DiamondShopBOs;
using DiamondShopRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopServices
{
    public class DiamondService : IDiamondService
    {
        private readonly IDiamondRepository diamondRepository = null;
        public DiamondService()
        {
            if (diamondRepository == null)
            {
                diamondRepository = new DiamondRepository();
            }
        }
        public void AddNewDiamond(tblDiamond newDiamond)
        {
            diamondRepository.AddNewDiamond(newDiamond);
        }

        public List<tblDiamond> Filter(string searchTerm, string clarity, string cut, string color, string shape, decimal? minPrice, decimal? maxPrice, float? minCaratWeight, float? maxCaratWeight, string sortBy)
        {
            return diamondRepository.Filter(searchTerm, clarity, cut, color, shape, minPrice, maxPrice, minCaratWeight, maxCaratWeight, sortBy);
        }

        public List<tblDiamond> GetAllDiamond()
        {
            return diamondRepository.GetAllDiamond();
        }

        public tblDiamond GetDiamondById(int id)
        {
            return diamondRepository.GetDiamondById(id);
        }

        public tblDiamond UpdateDiamond(int id, tblDiamond diamond)
        {
            return diamondRepository.UpdateDiamond(id, diamond);
        }
    }
}
