using DiamondShopBOs;
using System.Collections.Generic;

namespace DiamondShopRepositories.OrderRepositories
{
    public interface IOrderRepository
    {
        tblOrder CreateOrder(string userID, decimal totalMoney, decimal paidAmount, decimal remainingAmount, string address, string phone, string status, int? voucherID);
        List<tblOrder> GetOrdersByStatus(string userID, string[] statuses, bool isHistory = false);
        tblOrder GetOrderById(string orderId);
        List<tblOrderItem> GetOrderItems(string orderId);
        void UpdateOrderStatus(string orderId, string status);
        List<tblOrderStatusUpdate> GetOrderStatusUpdates(string orderId);
   
        string GetDeliveryStaffID();
        List<tblVoucher> GetAvailableVouchers(string userID);
        tblVoucher ValidateVoucher(int voucherID, string userID);
    }

}
