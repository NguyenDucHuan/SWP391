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
        public void ProcessWarranty(int warrantyId)
        {
            var warranty = _warrantyDAO.GetWarrantyById(warrantyId);
            if (warranty != null)
            {
                _warrantyDAO.ProcessWarranty(warranty);
            }
        }

        public void SubmitWarranty(tblWarranty warranty)
        {
            _warrantyDAO.SubmitWarranty(warranty);
        }
        public List<tblWarranty> GetNonValidWarranties()
        {
            return _warrantyDAO.GetNonValidWarranties();
        }
        public void UpdateWarranty(tblWarranty warranty) // Thêm phương thức này
        {
            _warrantyDAO.UpdateWarranty(warranty);
        }
    }
}
