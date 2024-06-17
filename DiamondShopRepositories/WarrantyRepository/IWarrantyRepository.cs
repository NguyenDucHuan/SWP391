using DiamondShopBOs;

namespace DiamondShopRepositories.WarrantyRepository
{
    public interface IWarrantyRepository
    {
        tblWarranty GetWarrantyByCode(string warrantyCode);
        void UpdateWarrantyDetails(string warrantyCode, string details);
    }
}
