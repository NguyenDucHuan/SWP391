using DiamondShopBOs;
using DiamondShopDAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
