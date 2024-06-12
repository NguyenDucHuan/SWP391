using DiamondShopBOs;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopDAOs.DAOs
{
    public class OrderItemDAO
    {
        private readonly DiamondShopManagementEntities diamondShopManagementEntities = null;
        public OrderItemDAO()
        {
            if (diamondShopManagementEntities == null)
            {
                diamondShopManagementEntities = new DiamondShopManagementEntities();
            }
        }
        public void CreateOrderItem(string orderID, int itemID, decimal salePrice)
        {
            try
            {
                using (var context = new DiamondShopManagementEntities())
                {
                    var item = context.tblItems.FirstOrDefault(i => i.ItemID == itemID);
                    var order = context.tblOrders.FirstOrDefault(o => o.orderID == orderID);

                    if (item == null)
                    {
                        throw new Exception($"Item with ID {itemID} not found.");
                    }
                    if (order == null)
                    {
                        throw new Exception($"Order with ID {orderID} not found.");
                    }

                    // Create a new tblOrderItem
                    var tblOrderItem = new tblOrderItem
                    {
                        ItemID = itemID,
                        orderID = orderID,
                        salePriceItem = salePrice,
                        tblItem = item,
                        tblOrder = order
                    };

                    // Add the new tblOrderItem to the context
                    context.tblOrderItems.Add(tblOrderItem);
                    context.SaveChanges();
                }
            }
            catch (DbUpdateException dbEx)
            {
                var errorMessage = "Failed to add order item due to database update issue: " + dbEx.Message;
                if (dbEx.InnerException != null)
                {
                    errorMessage += " Inner Exception: " + dbEx.InnerException.Message;
                }
                throw new Exception(errorMessage, dbEx);
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                throw new Exception("Failed to add order item: " + ex.Message, ex);
            }
        }
    }
}
