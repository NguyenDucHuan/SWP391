using DiamondShopBOs;
using DiamondShopDAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DiamondShopDAOs
{
    public class ItemDAO
    {
        private readonly DiamondShopManagementEntities entities = null;
        public ItemDAO()
        {
            if (entities == null)
            {
                entities = new DiamondShopManagementEntities();
            }
        }
        public void CreateItem(string OrderId, int? settingID, int? accentStoneID, int? quantityAccent, int diamondID, decimal diamondPrice, decimal settingPrice, decimal accentPrice)
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
            }
            catch (Exception ex)
            {
                throw new Exception("Add Item ERROR");
            }
            OrderItemDAO orderItemDAO = new OrderItemDAO();
            orderItemDAO.CreateOrderItem(OrderId, tblItem.ItemID, ((decimal)diamondPrice + (decimal)accentPrice * (decimal)quantityAccent + (decimal)settingPrice));
        }
    }
}
