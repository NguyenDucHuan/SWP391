using DiamondShopBOs;
using System.Collections.Generic;

namespace DiamondShopServices.WarrantyServices
{
    public interface IWarrantyService
    {
        WarrantyDetailsViewModel GetWarrantyByCode(string warrantyCode);
        void UpdateWarrantyDetails(string warrantyCode, string details);
        void CreateWarranty(tblWarranty warranty);
        List<tblWarranty> GetNonValidWarrantiesByCustomer(string customerId);
        WarrantyDetailsViewModel GetWarrantyByID(int warrantyID);
        bool ProcessWarranty(int warrantyID);
        void SubmitWarranty(tblWarranty warranty);
        List<tblWarranty> GetNonValidWarranties();
    }
}
