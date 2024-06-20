using DiamondShopBOs;
using DiamondShopDAOs;

namespace DiamondShopRepositories.WarrantyRepository
{
    public class WarrantyRepository : IWarrantyRepository
    {
        private readonly WarrantyDAO _warrantyDAO;

        public WarrantyRepository()
        {
            _warrantyDAO = new WarrantyDAO();
        }

        public tblWarranty GetWarrantyByCode(string warrantyCode)
        {
            return _warrantyDAO.GetWarrantyByCode(warrantyCode);
        }

        public void UpdateWarrantyDetails(string warrantyCode, string details)
        {
            _warrantyDAO.UpdateWarrantyDetails(warrantyCode, details);
        }
    }
}
