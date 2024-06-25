using DiamondShopBOs;
using DiamondShopDAOs.DAOs;
using System;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace DiamondShopDAOs
{
    public class ItemDAO
    {
        private readonly DiamondShopManagementEntities entities = null;
        private readonly WarrantyDAO _warrantyDAO;

        public ItemDAO()
        {
            if (entities == null)
            {
                entities = new DiamondShopManagementEntities();
            }

            _warrantyDAO = new WarrantyDAO();
        }

        public void CreateItem(string orderId, int? settingID, int? accentStoneID, int? quantityAccent, int diamondID, decimal diamondPrice, decimal settingPrice, decimal accentPrice, int? settingSize)
        {
            tblItem tblItem = new tblItem();
            if (settingID == 0)
            {
                tblItem = new tblItem
                {
                    accentStoneID = null,
                    settingID = null,
                    quantityAccent = null,
                    diamondID = diamondID,
                    diamondPrice = diamondPrice,
                    accentStonePrice = 0,
                    settingPrice = 0,
                    settingSize = null
                };
            }
            else
            {
                tblItem = new tblItem
                {
                    accentStoneID = accentStoneID,
                    settingID = settingID,
                    quantityAccent = quantityAccent,
                    diamondID = diamondID,
                    diamondPrice = diamondPrice,
                    accentStonePrice = accentPrice,
                    settingPrice = settingPrice,
                    settingSize = settingSize
                };

            }
            try
            {
                using (var diamondShop = new DiamondShopManagementEntities())
                {
                    diamondShop.tblItems.Add(tblItem);
                    diamondShop.SaveChanges();
                }
                AccentStoneDAO accentStoneDAO = new AccentStoneDAO();
                accentStoneDAO.UpdateAccentStoneQuatity(quantityAccent, accentStoneID);
                string warrantyCode = _warrantyDAO.GenerateWarrantyCode();
                OrderItemDAO orderItemDAO = new OrderItemDAO();
                orderItemDAO.CreateOrderItem(orderId, tblItem.ItemID, ((decimal)diamondPrice + (decimal)accentPrice * (decimal)quantityAccent + (decimal)settingPrice), warrantyCode);

                _warrantyDAO.CreateWarranty(orderId, tblItem.ItemID, warrantyCode, "Valid");
                SendWarrantyEmail(orderId, warrantyCode);
            }
            catch (Exception ex)
            {
                {
                    // Log chi tiết lỗi
                    Console.WriteLine("Error occurred: " + ex.ToString());
                    throw new Exception("Add Item ERROR: " + ex.Message);
                }
            }
        }
        private void SendWarrantyEmail(string orderId, string warrantyCode)
        {
            // Lấy thông tin khách hàng từ orderId
            var order = entities.tblOrders.FirstOrDefault(o => o.orderID == orderId);
            if (order == null) return;

            string customerName = order.tblUser.fullName;
            string customerEmail = order.tblUser.email;

            var message = new MailMessage();
            message.To.Add(new MailAddress(customerEmail));
            message.From = new MailAddress(ConfigurationManager.AppSettings["smtp:from"]);
            message.Subject = "Your Warranty Code from Diamond Shop";

            message.Body = $@"
<html>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 10px;'>
        <h2 style='color: #4CAF50; text-align: center;'>Thank You for Your Purchase!</h2>
        <p>Dear <strong>{customerName}</strong>,</p>
        <p>We sincerely thank you for purchasing our product. At <strong>Diamond Shop</strong>, we are committed to providing you with the highest quality products and services. Your trust and satisfaction are our top priorities, and we hope our product meets all your expectations.</p>
        <p>Your warranty code is: <strong style='color: #4CAF50; font-size: 18px;'>{warrantyCode}</strong>. Please keep this code safe as it is very important for any warranty services.</p>
        <p style='color: #d9534f; font-weight: bold;'>Important:</p>
        <ul style='color: #d9534f;'>
            <li>Keep your warranty code safe. It is crucial for any warranty claims.</li>
            <li>Without the warranty code, you will need to bring the GIA certificate to the store for warranty services.</li>
        </ul>
        <p>We truly appreciate your business and are here to assist you with any questions or concerns you may have. Please do not hesitate to contact us.</p>
        <p>Best regards,</p>
        <p><strong>The Diamond Shop Team</strong></p>
        <p style='font-size: 12px; color: #777; text-align: center;'>This is an automated message, please do not reply.</p>
    </div>
</body>
</html>";

            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = ConfigurationManager.AppSettings["smtp:username"],
                    Password = ConfigurationManager.AppSettings["smtp:password"]
                };
                smtp.Credentials = credential;
                smtp.Host = ConfigurationManager.AppSettings["smtp:host"];
                smtp.Port = int.Parse(ConfigurationManager.AppSettings["smtp:port"]);
                smtp.EnableSsl = true;
                smtp.Send(message);
            }
        }
    }
}
