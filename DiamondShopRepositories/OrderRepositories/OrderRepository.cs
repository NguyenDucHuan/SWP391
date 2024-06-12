using DiamondShopBOs;
using DiamondShopDAOs;
using System.Collections.Generic;

namespace DiamondShopRepositories.OrderRepositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDAO orderDAO = null;

        public OrderRepository()
        {
            if (orderDAO == null)
            {
                orderDAO = new OrderDAO();
            }
        }

        public tblOrder CreateOrder(string userID, decimal totalMoney, decimal paidAmount, decimal remainingAmont, string address, string phone, string status)
        {
            return orderDAO.CreateOrder(userID, totalMoney, paidAmount, remainingAmont, address, phone, status);
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

    }
}
