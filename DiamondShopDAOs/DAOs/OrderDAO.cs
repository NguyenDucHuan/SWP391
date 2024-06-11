using DiamondShopBOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiamondShopDAOs
{
    public class OrderDAO
    {
        private readonly DiamondShopManagementEntities entities = null;

        public OrderDAO()
        {
            if (entities == null)
            {
                entities = new DiamondShopManagementEntities();
            }
        }

        public tblOrder CreateOrder(string userID, decimal totalMoney, decimal paidAmount, decimal remainingAmont, string address, string phone, string status)
        {
            tblOrder tblOrder = new tblOrder
            {
                orderID = GenerateNextOrderId(),
                customerID = userID,
                saleStaffID = null,
                deliveryStaffID = null,
                totalMoney = (double)totalMoney,
                paidAmount = (double)paidAmount,
                remainingAmount = (double)remainingAmont,
                address = address,
                phone = phone,
                saleDate = DateTime.Now,
                status = status,
                paymentStatus = "Pending"
            };

            try
            {
                entities.tblOrders.Add(tblOrder);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("create Oreder error");
            }
            return tblOrder;
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
            var allOrder = entities.tblOrders.ToList();
            var usersWithFormattedId = allOrder.Where(d => Regex.IsMatch(d.orderID, @"^OID\d+$")).ToList();
            if (!usersWithFormattedId.Any())
            {
                return null;
            }
            var latestOrder = usersWithFormattedId
                .OrderByDescending(d => int.Parse(Regex.Match(d.orderID, @"\d+$").Value))
                .FirstOrDefault();
            return latestOrder?.orderID;
        }

        public List<tblOrder> GetOrdersByStatus(string userID, string[] statuses, bool isHistory = false)
        {
            var orders = entities.tblOrders
                .Where(o => o.customerID == userID && statuses.Contains(o.status))
                .ToList();

            if (!isHistory)
            {
                orders.AddRange(entities.tblOrders
                    .Where(o => o.customerID == userID && (o.deliveryStaffID == null || o.saleStaffID == null))
                    .ToList());
            }

            return orders;
        }

        public tblOrder GetOrderById(string orderId)
        {
            return entities.tblOrders.FirstOrDefault(o => o.orderID == orderId);
        }

        public List<tblOrderItem> GetOrderItems(string orderId)
        {
            return entities.tblOrderItems.Where(oi => oi.orderID == orderId).ToList();
        }

        public void UpdateOrderStatus(string orderId, string status)
        {
            var order = entities.tblOrders.FirstOrDefault(o => o.orderID == orderId);
            if (order != null)
            {
                order.status = status;
                entities.SaveChanges();

                var statusUpdate = new tblOrderStatusUpdate
                {
                    orderID = orderId,
                    status = status,
                    updateTime = DateTime.Now
                };
                entities.tblOrderStatusUpdates.Add(statusUpdate);
                entities.SaveChanges();
            }
        }

        public List<tblOrderStatusUpdate> GetOrderStatusUpdates(string orderId)
        {
            return entities.tblOrderStatusUpdates.Where(su => su.orderID == orderId).OrderBy(su => su.updateTime).ToList();
        }
    }
}
