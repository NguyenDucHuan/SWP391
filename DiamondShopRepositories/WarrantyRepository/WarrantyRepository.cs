using DiamondShopBOs;
using DiamondShopDAOs;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;

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
        public void CreateWarranty(tblWarranty warranty)
        {
            _warrantyDAO.CreateWarranty(warranty.orderID, warranty.ItemID, warranty.warrantyCode, warranty.warrantyDetails);
        }
        public List<tblWarranty> GetNonValidWarrantiesByCustomer(string customerId)
        {
            return _warrantyDAO.GetNonValidWarrantiesByCustomer(customerId);
        }
        public tblWarranty GetWarrantyByID(int warrantyID)
        {
            return _warrantyDAO.GetWarrantyByID(warrantyID);
        }
    }
}
