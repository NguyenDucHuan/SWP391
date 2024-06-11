using DiamondShopBOs;
using DiamondShopRepositories.OrderRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
