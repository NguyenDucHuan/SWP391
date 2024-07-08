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
        public void UpdateDiamondStatus(tblDiamond diamond, bool status)
        {
            diamond.status = status;
            diamondRepository.UpdateDiamond(diamond.diamondID, diamond);
        }
        public void AddNewDiamond(tblDiamond newDiamond, tblCertificate newCertificate)
        {
            diamondRepository.AddNewDiamond(newDiamond, newCertificate);
        }
        public List<tblCertificate> GetCertificatesByDiamondId(int diamondId)
        {
            return diamondRepository.GetCertificatesByDiamondId(diamondId);
        }
        public void UpdateDiamond(tblDiamond diamond)
        {
            diamondRepository.UpdateDiamond(diamond);
        }

        public tblDiamond GetDiamondByID(int diamondID, bool status, string detailStatus)
        {
            return diamondRepository.GetDiamondByID(diamondID, status, detailStatus);
        }

        public tblDiamond GetDiamondBySearchTerm(string searchTerm)
        {
            if (int.TryParse(searchTerm, out int id))
            {
                return diamondRepository.GetDiamondByID(id, false, "Sold");
            }
            return diamondRepository.GetDiamondByWarrantyCode(searchTerm, false, "Sold");
        }
    }
}
