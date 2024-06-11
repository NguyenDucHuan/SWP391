﻿using DiamondShopBOs;
using DiamondShopRepositories.OrderRepositories;
using System.Collections.Generic;

namespace DiamondShopServices.OrderServices
{
    public class OrderServices : IOrderServices
    {
        private readonly IOrderRepository orderRepository = null;

        public OrderServices()
        {
            orderRepository = new OrderRepository();
        }

        public tblOrder CreateOrder(string userID, decimal totalMoney, decimal paidAmount, decimal remainingAmont, string address, string phone, string status)
        {
            return orderRepository.CreateOrder(userID, totalMoney, paidAmount, remainingAmont, address, phone, status);
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
    }
}
