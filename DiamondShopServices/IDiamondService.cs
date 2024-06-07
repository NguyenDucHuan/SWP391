using DiamondShopBOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopServices
{
    public interface IDiamondService
    {
        List<tblDiamond> GetAllDiamond();
        tblDiamond GetDiamondById(int id);
        tblDiamond UpdateDiamond(int id, tblDiamond diamond);
        void AddNewDiamond(tblDiamond newDiamond);
        List<tblDiamond> Filter(string searchTerm, string clarity, string cut, string color, string shape, decimal? minPrice, decimal? maxPrice, float? minCaratWeight, float? maxCaratWeight, string sortBy, string type);
    }
}
