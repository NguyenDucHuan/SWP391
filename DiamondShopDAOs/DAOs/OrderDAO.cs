using DiamondShopBOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiamondShopDAOs
{
    public class OrderDAO
    {

        private readonly DiamondShopManagementEntities entities = null;
        public OrderDAO()
        {
            if (entities == null)
            {
                entities = new DiamondShopManagementEntities();
            }
        }

        public tblOrder CreateOrder(string userID, decimal totalMoney, decimal paidAmount, decimal remainingAmont, string address, string phone, string status)
        {
            tblOrder tblOrder = new tblOrder
            {
                orderID = GenerateNextOrderId(),
                customerID = userID,
                saleStaffID = null,
                deliveryStaffID = null,
                totalMoney = (double)totalMoney,
                paidAmount = (double)paidAmount,
                remainingAmount = (double)remainingAmont,
                address = address,
                phone = phone,
                saleDate = DateTime.Now,
                status = status,
                paymentStatus = "Pending"
            };

            try
            {
                entities.tblOrders.Add(tblOrder);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("create Oreder error");
            }
            return tblOrder;
        }
        public string GenerateNextOrderId()
        {
            string currentOId = GetTheLastestOrderID();
            if (string.IsNullOrEmpty(currentOId))
            {
                return "OID0000001";
            }
            string numericPart = currentOId.Substring(3);
            if (!int.TryParse(numericPart, out int numericValue))
            {
                throw new ArgumentException("Invalid numeric part in ID");
            }
            numericValue++;
            string newNumericPart = numericValue.ToString().PadLeft(numericPart.Length, '0');
            return "OID" + newNumericPart;
        }

        public String GetTheLastestOrderID()
        {
            var allOrder = entities.tblOrders.ToList();
            var usersWithFormattedId = allOrder.Where(d => Regex.IsMatch(d.orderID, @"^OID\d+$")).ToList();
            if (!usersWithFormattedId.Any())
            {
                return null;
            }
            var latestOrder = usersWithFormattedId
                .OrderByDescending(d => int.Parse(Regex.Match(d.orderID, @"\d+$").Value))
                .FirstOrDefault();
            return latestOrder?.orderID;
        }
    }
}
