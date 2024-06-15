using DiamondShopBOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopDAOs.DAOs
{
    public class StaffDAO
    {
        private readonly DiamondShopManagementEntities _context;

        public StaffDAO()
        {
            _context = new DiamondShopManagementEntities();
        }

        public List<tblOrder> GetOrdersByStaffId(string staffId, int roleId, string searchOrderId)
        {
            IQueryable<tblOrder> ordersQuery;
            if (roleId == 4) // Delivery Staff
            {
                ordersQuery = _context.tblOrders.Where(o => o.deliveryStaffID == staffId);
            }
            else if (roleId == 5) // Sale Staff
            {
                ordersQuery = _context.tblOrders.Where(o => o.saleStaffID == staffId);
            }
            else
            {
                return new List<tblOrder>();
            }

            if (!string.IsNullOrEmpty(searchOrderId))
            {
                ordersQuery = ordersQuery.Where(o => o.orderID.Contains(searchOrderId));
            }

            return ordersQuery.ToList();
        }

        public tblOrder GetOrderById(string orderId)
        {
            return _context.tblOrders.SingleOrDefault(o => o.orderID == orderId);
        }

        public void UpdateOrderStatus(string orderId, string status)
        {
            var order = _context.tblOrders.SingleOrDefault(o => o.orderID == orderId);
            if (order != null)
            {
                order.status = status;

                var orderStatusUpdate = new tblOrderStatusUpdate
                {
                    orderID = orderId,
                    status = status,
                    updateTime = DateTime.Now
                };

                _context.tblOrderStatusUpdates.Add(orderStatusUpdate);
                _context.SaveChanges();
            }
        }
    }
}
