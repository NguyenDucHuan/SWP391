using DiamondShopBOs;
using DiamondShopDAOs;
using System.Collections.Generic;

namespace DiamondShopRepositories.OrderRepositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDAO orderDAO;

        public OrderRepository()
        {
            orderDAO = new OrderDAO();
        }

        public tblOrder CreateOrder(string userID, string customerName, decimal totalMoney, decimal paidAmount, decimal remainingAmount, string address, string phone, string status, int? voucherID)
        {
            return orderDAO.CreateOrder(userID, customerName, totalMoney, paidAmount, remainingAmount, address, phone, status, voucherID);
        }


        public List<tblOrder> GetOrdersByStatus(string userID, string[] statuses, bool isHistory = false)
        {
            return orderDAO.GetOrdersByStatus(userID, statuses, isHistory);
        }

        public tblOrder GetOrderById(string orderId)
        {
            return orderDAO.GetOrderById(orderId);
        }

        public List<tblOrderItem> GetOrderItems(string orderId)
        {
            return orderDAO.GetOrderItems(orderId);
        }

        public void UpdateOrderStatus(string orderId, string status)
        {
            orderDAO.UpdateOrderStatus(orderId, status);
        }

        public List<tblOrderStatusUpdate> GetOrderStatusUpdates(string orderId)
        {
            return orderDAO.GetOrderStatusUpdates(orderId);
        }

        public string GetDeliveryStaffID()
        {
            return orderDAO.GetDeliveryStaffID();
        }
        public List<tblVoucher> GetAvailableVouchers(string userID)
        {
            return orderDAO.GetAvailableVouchers(userID);
        }
        public tblVoucher ValidateVoucher(int voucherID, string userID)
        {
            return orderDAO.ValidateVoucher(voucherID, userID);
        }

        public decimal GetVoucherDiscount(int? voucherID)
        {
            return orderDAO.GetVoucherDiscount(voucherID);
        }
    }
}
