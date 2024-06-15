using DiamondShopBOs;
using DiamondShopDAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopRepositories.StaffRepositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly StaffDAO _staffDAO;

        public StaffRepository()
        {
            _staffDAO = new StaffDAO();
        }

        public List<tblOrder> GetOrdersByStaffId(string staffId, int roleId, string searchOrderId)
        {
            return _staffDAO.GetOrdersByStaffId(staffId, roleId, searchOrderId);
        }

        public tblOrder GetOrderById(string orderId)
        {
            return _staffDAO.GetOrderById(orderId);
        }

        public void UpdateOrderStatus(string orderId, string status)
        {
            _staffDAO.UpdateOrderStatus(orderId, status);
        }
    }
}
