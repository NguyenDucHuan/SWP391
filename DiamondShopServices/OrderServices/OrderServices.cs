using DiamondShopBOs;
using DiamondShopRepositories.OrderRepositories;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;

namespace DiamondShopServices.OrderServices
{
    public class OrderServices : IOrderServices
    {
        private readonly IOrderRepository orderRepository;
        public OrderServices()
        {
            orderRepository = new OrderRepository();
        }

        public tblOrder CreateOrder(string userID, string customerName, decimal totalMoney, decimal paidAmount, decimal remainingAmount, string address, string phone, string note, string status, int? voucherID)
        {
            return orderRepository.CreateOrder(userID, customerName, totalMoney, paidAmount, remainingAmount, address, phone, note,status, voucherID);
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


        public string GetDeliveryStaffID()
        {
            return orderRepository.GetDeliveryStaffID();
        }
        public List<tblVoucher> GetAvailableVouchers(string userID)
        {
            return orderRepository.GetAvailableVouchers(userID);
        }
        public tblVoucher ValidateVoucher(int voucherID, string userID)
        {
            return orderRepository.ValidateVoucher(voucherID, userID);
        }

        public decimal GetVoucherDiscount(int? voucherID)
        {
            return orderRepository.GetVoucherDiscount(voucherID);
        }
        public void UpdateOrder(tblOrder order)
        {
            orderRepository.UpdateOrder(order);
        }
        public tblOrder GetOrderByID(string orderID)
        {
            return orderRepository.GetOrderByID(orderID);
        }

        public tblOrderItem GetOrderItemByDiamondID(int diamondID)
        {
            return orderRepository.GetOrderItemByDiamondID(diamondID);
        }
        public List<tblWarranty> GetWarrantiesByOrderID(string orderId)
        {
            return orderRepository.GetWarrantiesByOrderID(orderId);
        }

    }
}
