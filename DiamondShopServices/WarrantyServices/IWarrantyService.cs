using DiamondShopBOs;

namespace DiamondShopServices.WarrantyServices
{
    public interface IWarrantyService
    {
        WarrantyDetailsViewModel GetWarrantyByCode(string warrantyCode);
        void UpdateWarrantyDetails(string warrantyCode, string details);
    }
}
