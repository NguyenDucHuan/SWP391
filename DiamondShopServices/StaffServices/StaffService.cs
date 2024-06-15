using DiamondShopBOs;
using DiamondShopRepositories.StaffRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopServices.StaffServices
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;

        public StaffService()
        {
            _staffRepository = new StaffRepository();
        }

        public List<tblOrder> GetOrdersByStaffId(string staffId, int roleId, string searchOrderId)
        {
            return _staffRepository.GetOrdersByStaffId(staffId, roleId, searchOrderId);
        }

        public tblOrder GetOrderById(string orderId)
        {
            return _staffRepository.GetOrderById(orderId);
        }

        public void UpdateOrderStatus(string orderId, string status)
        {
            _staffRepository.UpdateOrderStatus(orderId, status);
        }
    }
}
