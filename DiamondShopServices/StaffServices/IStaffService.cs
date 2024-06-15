using DiamondShopBOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopServices.StaffServices
{
    public interface IStaffService
    {
        List<tblOrder> GetOrdersByStaffId(string staffId, int roleId, string searchOrderId);
        tblOrder GetOrderById(string orderId);
        void UpdateOrderStatus(string orderId, string status);
    }
}
