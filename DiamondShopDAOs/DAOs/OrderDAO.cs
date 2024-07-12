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
        public tblOrder CreateOrder(string userID, string customerName, decimal totalMoney, decimal paidAmount, decimal remainingAmount, string address, string phone, string note, string status, int? voucherID)
        {
            var order = new tblOrder
            {
                orderID = GenerateNextOrderId(),
                customerID = userID,
                customerName = customerName, // Thêm dòng này
                saleStaffID = null,
                totalMoney = (double)totalMoney,
                paidAmount = (double)paidAmount,
                remainingAmount = (double)remainingAmount,
                address = address,
                phone = phone,
                Note = note,
                saleDate = DateTime.Now,
                status = status,
                paymentStatus = "Pending",
                voucherID = voucherID
            };

            _context.tblOrders.Add(order);

            if (voucherID.HasValue)
            {
                var voucher = _context.tblVouchers.SingleOrDefault(v => v.voucherID == voucherID.Value);
                if (voucher != null)
                {
                    voucher.quantity -= 1;
                    if (voucher.quantity < 0)
                    {
                        voucher.quantity = 0;
                    }
                }
            }

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
                var additionalOrders = _context.tblOrders
                    .Where(o => o.customerID == userID && (o.deliveryStaffID == null || o.saleStaffID == null))
                    .ToList();

                orders.AddRange(additionalOrders);
            }


            var uniqueOrders = orders.GroupBy(o => o.orderID).Select(g => g.First()).ToList();

            return uniqueOrders;
        }


        public string GetDeliveryStaffID()
        {
            var deliveryStaff = _context.tblUsers.FirstOrDefault(u => u.roleID == 4);
            return deliveryStaff?.userID;
        }
        public List<tblVoucher> GetAvailableVouchers(string userID)
        {
            return _context.tblVouchers
                .Where(v => v.status == true && (v.targetUserID == "All" || v.targetUserID == userID))
                .ToList();
        }
        public tblVoucher ValidateVoucher(int voucherID, string userID)
        {
            return _context.tblVouchers.FirstOrDefault(v => v.voucherID == voucherID && v.status == true && (v.targetUserID == "All" || v.targetUserID == userID));
        }
        public decimal GetVoucherDiscount(int? voucherID)
        {
            if (voucherID == null)
            {
                return 0;
            }

            var voucher = _context.tblVouchers.SingleOrDefault(v => v.voucherID == voucherID);
            if (voucher != null && voucher.quantity > 0)
            {
                return (decimal)voucher.discount / 100;
            }

            return 0;
        }
        public void Update(tblOrder order)
        {
            var existingOrder = _context.tblOrders.Find(order.orderID);
            if (existingOrder != null)
            {
                existingOrder.status = order.status;
                existingOrder.totalMoney = order.totalMoney;
                existingOrder.paidAmount = order.paidAmount;
                if (order.paidAmount == order.totalMoney)
                {
                    order.paymentStatus = "Complete";
                }
                // Cập nhật các thuộc tính khác của đơn hàng
            }
        }
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        public tblOrder GetOrderByID(string orderID)
        {
            return _context.tblOrders.SingleOrDefault(o => o.orderID == orderID);
        }

        public tblOrderItem GetOrderItemByDiamondID(int diamondID)
        {
            return _context.tblOrderItems.SingleOrDefault(oi => oi.ItemID == diamondID);
        }
        public List<tblWarranty> GetWarrantiesByOrderID(string orderId)
        {
            return _context.tblWarranties.Where(w => w.orderID == orderId).ToList();
        }

    }
}
