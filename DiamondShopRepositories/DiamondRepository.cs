using DiamondShopBOs;
using DiamondShopDAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopRepositories
{
    public class DiamondRepository : IDiamondRepository
    {
        private readonly DiamondDAO diamondDAO = null;

        public DiamondRepository()
        {
            if (diamondDAO == null)
            {
                diamondDAO = new DiamondDAO();
            }
        }
        public void AddNewDiamond(tblDiamond newDiamond)
        {
            diamondDAO.AddNewDiamond(newDiamond);
        }

        public List<tblDiamond> Filter(string searchTerm, string clarity, string cut, string color, string shape, decimal? minPrice, decimal? maxPrice, float? minCaratWeight, float? maxCaratWeight, string sortBy, string type)
        {
            return diamondDAO.Filter(searchTerm, clarity, cut, color, shape, minPrice, maxPrice, minCaratWeight, maxCaratWeight, sortBy, type);
        }

        public List<tblDiamond> GetAllDiamond()
        {
            return diamondDAO.GetAllDiamonds();
        }

        public tblDiamond GetDiamondById(int id)
        {
            return diamondDAO.GetDiamondById(id);
        }

        public tblDiamond UpdateDiamond(int id, tblDiamond diamond)
        {
            return diamondDAO.UpdateDiamond(id, diamond);
        }


    }
}
