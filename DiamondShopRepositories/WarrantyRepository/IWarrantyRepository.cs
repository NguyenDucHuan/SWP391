using DiamondShopBOs;
using System.Collections.Generic;

namespace DiamondShopRepositories.WarrantyRepository
{
    public interface IWarrantyRepository
    {
        tblWarranty GetWarrantyByCode(string warrantyCode);
        void UpdateWarrantyDetails(string warrantyCode, string details);
        void CreateWarranty(tblWarranty warranty);
        List<tblWarranty> GetNonValidWarrantiesByCustomer(string customerId);
        tblWarranty GetWarrantyByID(int warrantyID);
        void ProcessWarranty(int warrantyId);
        void SubmitWarranty(tblWarranty warranty);
    }
}
