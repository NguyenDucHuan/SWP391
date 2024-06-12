using DiamondShopBOs;
using DiamondShopRepositories.OrderRepositories;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;

namespace DiamondShopServices.OrderServices
{
    public class OrderServices : IOrderServices
    {
        private readonly IOrderRepository orderRepository;
        private readonly DiamondShopManagementEntities _context;
        public OrderServices()
        {
            orderRepository = new OrderRepository();
        }

        public tblOrder CreateOrder(string userID, decimal totalMoney, decimal paidAmount, decimal remainingAmount, string address, string phone, string status, string deliveryStaffID)
        {
            return orderRepository.CreateOrder(userID, totalMoney, paidAmount, remainingAmount, address, phone, status, deliveryStaffID);
        }

        public List<tblOrder> GetOrdersByStatus(string userID, string[] statuses, bool isHistory = false)
        {
            return orderRepository.GetOrdersByStatus(userID, statuses, isHistory);
        }

        public tblOrder GetOrderById(string orderId)
        {
            return orderRepository.GetOrderById(orderId);
        }

        public List<tblOrderItem> GetOrderItems(string orderId)
        {
            return orderRepository.GetOrderItems(orderId);
        }

        public void UpdateOrderStatus(string orderId, string status)
        {
            orderRepository.UpdateOrderStatus(orderId, status);
        }

        public List<tblOrderStatusUpdate> GetOrderStatusUpdates(string orderId)
        {
            return orderRepository.GetOrderStatusUpdates(orderId);
        }

        public List<tblOrder> GetOrdersByPaymentStatus(string userID, string paymentStatus)
        {
            return orderRepository.GetOrdersByPaymentStatus(userID, paymentStatus);
        }


        public string GetDeliveryStaffID()
        {
            return orderRepository.GetDeliveryStaffID();
        }

    }
}
