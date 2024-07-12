using DiamondShopBOs;
using System.Collections.Generic;

namespace DiamondShopServices.OrderServices
{
    public interface IOrderServices
    {
        tblOrder CreateOrder(string userID, string customerName, decimal totalMoney, decimal paidAmount, decimal remainingAmount, string address, string phone,string note, string status, int? voucherID);

        List<tblOrder> GetOrdersByStatus(string userID, string[] statuses, bool isHistory = false);
        tblOrder GetOrderById(string orderId);
        List<tblOrderItem> GetOrderItems(string orderId);
        void UpdateOrderStatus(string orderId, string status);
        List<tblOrderStatusUpdate> GetOrderStatusUpdates(string orderId);
        string GetDeliveryStaffID();
        List<tblVoucher> GetAvailableVouchers(string userID);
        tblVoucher ValidateVoucher(int voucherID, string userID);
        decimal GetVoucherDiscount(int? voucherID);
        void UpdateOrder(tblOrder order);
        tblOrder GetOrderByID(string orderID);
        tblOrderItem GetOrderItemByDiamondID(int diamondID);
        List<tblWarranty> GetWarrantiesByOrderID(string orderId);
    }
}
