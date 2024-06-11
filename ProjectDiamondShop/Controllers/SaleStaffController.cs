using DiamondShopBOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ProjectDiamondShop.Controllers
{
    public class SaleStaffController : Controller
    {
        private readonly DiamondShopManagementEntities db = new DiamondShopManagementEntities(); // Entity Framework DbContext

        // GET: SaleStaff
        public ActionResult Index(string searchOrderId)
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 5)
            {
                return RedirectToAction("Index", "Home");
            }

            List<tblOrder> orders = GetOrders(searchOrderId);
            return View("SaleStaff", orders); // Ensure your view expects IEnumerable<tblOrder>
        }

        private List<tblOrder> GetOrders(string searchOrderId)
        {
            string saleStaffID = Session["UserID"].ToString();
            var ordersQuery = db.tblOrders.Where(o => o.saleStaffID == saleStaffID);

            if (!string.IsNullOrEmpty(searchOrderId))
            {
                ordersQuery = ordersQuery.Where(o => o.orderID.Contains(searchOrderId));
            }

            return ordersQuery.ToList();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Process(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                TempData["UpdateMessage"] = "Order ID is required.";
                return RedirectToAction("Index");
            }

            var order = db.tblOrders.SingleOrDefault(o => o.orderID == orderId);
            if (order != null)
            {
                order.status = "Preparing Goods";

                var orderStatusUpdate = new tblOrderStatusUpdate
                {
                    orderID = orderId,
                    status = "Preparing Goods",
                    updateTime = DateTime.Now
                };

                db.tblOrderStatusUpdates.Add(orderStatusUpdate);
                db.SaveChanges();

                TempData["UpdateMessage"] = "Order updated successfully.";
            }
            else
            {
                TempData["UpdateMessage"] = "Order not found.";
            }

            return RedirectToAction("Index");
        }
    }
}
