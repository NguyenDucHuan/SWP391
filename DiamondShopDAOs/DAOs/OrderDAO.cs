using DiamondShopBOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiamondShopDAOs
{
    public class OrderDAO
    {
        private readonly DiamondShopManagementEntities _context;

        public OrderDAO()
        {
            _context = new DiamondShopManagementEntities();
        }

   


        public tblOrder CreateOrder(string userID, decimal totalMoney, decimal paidAmount, decimal remainingAmount, string address, string phone, string status, string deliveryStaffID)
        {
            var order = new tblOrder
            {
                orderID = GenerateNextOrderId(),
                customerID = userID,
                saleStaffID = null,
                deliveryStaffID = deliveryStaffID, // Set delivery staff ID here
                totalMoney = (double)totalMoney,
                paidAmount = (double)paidAmount,
                remainingAmount = (double)remainingAmount,
                address = address,
                phone = phone,
                saleDate = System.DateTime.Now,
                status = status,
                paymentStatus = "Pending"
            };

            _context.tblOrders.Add(order);
            _context.SaveChanges();
            return order;
        }

        public string GenerateNextOrderId()
        {
            string currentOId = GetTheLastestOrderID();
            if (string.IsNullOrEmpty(currentOId))
            {
                return "OID0000001";
            }
            string numericPart = currentOId.Substring(3);
            if (!int.TryParse(numericPart, out int numericValue))
            {
                throw new ArgumentException("Invalid numeric part in ID");
            }
            numericValue++;
            string newNumericPart = numericValue.ToString().PadLeft(numericPart.Length, '0');
            return "OID" + newNumericPart;
        }

        public string GetTheLastestOrderID()
        {
            var latestOrderId = _context.tblOrders
                .OrderByDescending(o => o.orderID)
                .FirstOrDefault()?.orderID;
            return latestOrderId;
        }

        public tblOrder GetOrderById(string orderId)
        {
            return _context.tblOrders.FirstOrDefault(o => o.orderID == orderId);
        }

        public List<tblOrderItem> GetOrderItems(string orderId)
        {
            return _context.tblOrderItems.Where(oi => oi.orderID == orderId).ToList();
        }

        public void UpdateOrderStatus(string orderId, string status)
        {
            var order = _context.tblOrders.FirstOrDefault(o => o.orderID == orderId);
            if (order != null)
            {
                order.status = status;
                _context.SaveChanges();

                var statusUpdate = new tblOrderStatusUpdate
                {
                    orderID = orderId,
                    status = status,
                    updateTime = DateTime.Now
                };
                _context.tblOrderStatusUpdates.Add(statusUpdate);
                _context.SaveChanges();
            }
        }

        public List<tblOrderStatusUpdate> GetOrderStatusUpdates(string orderId)
        {
            return _context.tblOrderStatusUpdates.Where(su => su.orderID == orderId).OrderBy(su => su.updateTime).ToList();
        }

        public List<tblOrder> GetOrdersByStatus(string userID, string[] statuses, bool isHistory = false)
        {
            var orders = _context.tblOrders
                .Where(o => o.customerID == userID && statuses.Contains(o.status))
                .ToList();

            if (!isHistory)
            {
                orders.AddRange(_context.tblOrders
                    .Where(o => o.customerID == userID && (o.deliveryStaffID == null || o.saleStaffID == null))
                    .ToList());
            }

            return orders;
        }

        public string GetDeliveryStaffID()
        {
            var deliveryStaff = _context.tblUsers.FirstOrDefault(u => u.roleID == 4);
            return deliveryStaff?.userID;
        }

        public List<tblOrder> GetOrdersByPaymentStatus(string userID, string paymentStatus)
        {
            return _context.tblOrders
                           .Where(o => o.customerID == userID && o.paymentStatus == paymentStatus)
                           .ToList();
        }


    }
}
