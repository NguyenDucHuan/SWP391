using DiamondShopBOs;
using DiamondShopDAOs.DAOs;
using System;

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

        public void CreateItem(string orderId, int? settingID, int? accentStoneID, int? quantityAccent, int diamondID, decimal diamondPrice, decimal settingPrice, decimal accentPrice)
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
                    settingPrice = 0
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
                    settingPrice = settingPrice
                };
            }
            try
            {
                entities.tblItems.Add(tblItem);
                entities.SaveChanges();

                string warrantyCode = _warrantyDAO.GenerateWarrantyCode();
                OrderItemDAO orderItemDAO = new OrderItemDAO();
                orderItemDAO.CreateOrderItem(orderId, tblItem.ItemID, ((decimal)diamondPrice + (decimal)accentPrice * (decimal)quantityAccent + (decimal)settingPrice), warrantyCode);

                _warrantyDAO.CreateWarranty(orderId, tblItem.ItemID, warrantyCode, "Valid");
            }
            catch (Exception ex)
            {
                throw new Exception("Add Item ERROR");
            }
        }
    }
}
