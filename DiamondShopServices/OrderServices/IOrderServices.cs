using DiamondShopBOs;
using System.Collections.Generic;

namespace DiamondShopServices.OrderServices
{
    public interface IOrderServices
    {
        tblOrder CreateOrder(string userID, decimal totalMoney, decimal paidAmount, decimal remainingAmount, string address, string phone, string status);
        List<tblOrder> GetOrdersByStatus(string userID, string[] statuses, bool isHistory = false);
        tblOrder GetOrderById(string orderId);
        List<tblOrderItem> GetOrderItems(string orderId);
        void UpdateOrderStatus(string orderId, string status);
        List<tblOrderStatusUpdate> GetOrderStatusUpdates(string orderId);
 

        string GetDeliveryStaffID();
    }
}
